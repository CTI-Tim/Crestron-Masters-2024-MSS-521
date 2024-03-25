using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.RAD.Ext.Util.Timers;

namespace DriverExtensionLibrary.Helpers
{
    /// <summary>
    /// IEventTimer that follows a provided "schedule" of delay times. This
    /// can be used for things like volume ramping by providing increasingly
    /// shorter delays for each tick. Note that there is a delay before the
    /// first callback, so if the desired behavior is to raise the event
    /// immediately and then delay after that, you need to make the first
    /// value in the schedule 0. In this case, the event will be fired
    /// immediately on the thread calling Start().
    /// </summary>
    public class ScheduledEventTimer : IEventTimer, IDisposable
    {
        /// <summary>
        /// Delegate to provide the schedule. The schedule is reloaded every
        /// time the timer is started. The schedule is simply a sequence of
        /// delay times, in order. A delay of 0 will be called immediately
        /// without restarting the timer for that entry. A delay of less than
        /// 0 or Timeout.Infinite will stop the timer early. Otherwise, the
        /// timer will repeat the last delay in the schedule when it runs out
        /// of entries, except if the last entry is zero since that'd be an
        /// infinite loop.
        /// </summary>
        /// <returns>Enumerable of delay times.</returns>
        public delegate IEnumerable<long> TimerScheduleProvider();

        /// <summary>
        /// Instance of <see cref="TimerScheduleProvider"/> to reload timings
        /// </summary>
        private TimerScheduleProvider RestartSchedule;

        /// <summary>
        /// The schedule to fire events on
        /// </summary>
        private IEnumerator<long> _schedule;

        /// <summary>
        /// Holds the previous delay so that when we run out of entries, we
        /// know the delay to use
        /// </summary>
        private long _lastDelay = Timeout.Infinite;

        /// <summary>
        /// Underlying CTimer object for this timer
        /// </summary>
        private CTimer _timer;

        /// <summary>
        /// Critical section used to synchronize Start()/Stop() with the
        /// callbacks. This allows Stop() to know that it's sucessfully
        /// stopped the timer and that event handlers have finished.
        /// </summary>
        private CCriticalSection _timerLock;

        /// <summary>
        /// The event fired for each scheduled interval. Timer is restarted
        /// after all handlers execute, so doing long processing here will
        /// result in timing skew.
        /// </summary>
        /// <remarks>DO NOT CALL Stop() FROM A HANDLER OF THIS EVENT</remarks>
        public event EventHandler<EventArgs> TimerTick;

        /// <summary>
        /// Describes the present state of the timer
        /// </summary>
        private enum TimerState
        {
            /// <summary>
            /// Timer is stopped
            /// </summary>
            Stopped = 0,

            /// <summary>
            /// Timer is running but not actively processing events
            /// </summary>
            Running = 1,

            /// <summary>
            /// Timer is processing event callbacks
            /// </summary>
            Callback = 2
        }

        /// <summary>
        /// Flag that stopping has been requested
        /// </summary>
        private bool _stopRequested;

        /// <summary>
        /// The present state of the timer
        /// </summary>
        private TimerState _state = TimerState.Stopped;

        /// <summary>
        /// Initialize the timer with the scheduled delays in the array
        /// </summary>
        /// <param name="schedule"></param>
        public ScheduledEventTimer(long[] schedule)
            : this(schedule.AsEnumerable)
        {
        }

        /// <summary>
        /// Initialize the timer with the provided TimerScheduleProvider
        /// </summary>
        /// <param name="scheduleProvider">Delegate that will provide the schedule each time the timer is started</param>
        public ScheduledEventTimer(TimerScheduleProvider scheduleProvider)
        {
            RestartSchedule = scheduleProvider;
            _timerLock = new CCriticalSection();
            _timer = new CTimer(TimerTickHandler, Timeout.Infinite);
        }

        /// <summary>
        /// Gets the next delay from the schedule
        /// </summary>
        /// <returns>The next delay, or Timeout.Infinite if the timer should stop</returns>
        private long GetNextDelay()
        {
            // Check for more entries
            if (_schedule.MoveNext())
            {
                _lastDelay = _schedule.Current;
            }
            else if (_lastDelay == 0)
            {
                // Avoid an infinite loop if the final delay was zero
                _lastDelay = Timeout.Infinite;
            }
            
            // Note that if neither above condition is satisfied, we will
            // repeat the final delay in the schedule until the timer is
            // stopped.
            return _lastDelay;
        }

        /// <summary>
        /// Prepare for the next tick, restarting the timer
        /// </summary>
        private void StartNextTick()
        {
            // Use a loop to avoid recursion when 0 delay is used
            while (true)
            {
                // Break if timer stops
                if (_stopRequested)
                {
                    _state = TimerState.Stopped;
                    break;
                }

                // Get the next delay. Call immediately if the delay is zero.
                long nextDelay = GetNextDelay();
                if (nextDelay == 0)
                {
                    OnTimerTick();
                }
                // Check for infinite will be optimized out, but don't want to
                // assume anything about its value in this code so include it
                else if (nextDelay > 0 && nextDelay != Timeout.Infinite)
                {
                    // Non-zero delay, so start the timer
                    _state = TimerState.Running;
                    _timer.Reset(nextDelay);
                    break;
                }
                else
                {
                    // Invalid delay, stop the timer
                    _stopRequested = true;
                }
            }
        }

        /// <summary>
        /// Start the timer if it is not already started
        /// </summary>
        public void Start()
        {
            if (_timerLock.TryEnter())
            {
                try
                {
                    if (_state == TimerState.Stopped)
                    {
                        _stopRequested = false;
                        _schedule = RestartSchedule().GetEnumerator();
                        StartNextTick();
                    }
                }
                finally
                {
                    _timerLock.Leave();
                }
            }
        }

        /// <summary>
        /// Stop the timer, optionally blocking until that happens
        /// </summary>
        /// <param name="block">If true, will wait until Events have finished firing before returning.</param>
        public void Stop(bool block)
        {
            // Stop timer right away
            _stopRequested = true;
            _timer.Stop();

            if (block)
            {
                // If blocking, wait
                _timerLock.Enter();
            }
            else
            {
                // Try to grab the lock if we can anyway
                block = _timerLock.TryEnter();
            }

            try
            {
                _state = TimerState.Stopped;
            }
            finally
            {
                if (block)
                {
                    _timerLock.Leave();
                }
            }
        }

        /// <summary>
        /// Stops the timer, blocking until event processing is completed.
        /// DO NOT CALL THIS FROM AN EVENT HANDLER!
        /// </summary>
        public void Stop()
        {
            Stop(true);
        }

        /// <summary>
        /// Stops the timer and return without waiting for event processing to complete
        /// </summary>
        public void Cancel()
        {
            Stop(false);
        }

        /// <summary>
        /// Clean up used resources
        /// </summary>
        public void Dispose()
        {
            // This disposed flag is used to handle all others.
            if (!_timer.Disposed)
            {
                _timer.Stop(); // use Stop() instead?
                _timer.Dispose();
                _timerLock.Dispose();
            }
        }

        /// <summary>
        /// Callback for the CTimer() for each timer tick.
        /// Processes events in a critical section to keep timer control
        /// in sync with the events it processes.
        /// </summary>
        /// <param name="unused">Incoming object from CTimer callback</param>
        private void TimerTickHandler(object unused)
        {
            try
            {
                _timerLock.Enter();
                if (_state == TimerState.Stopped)
                {
                    // Check state and do nothing if we are stopping
                    return;
                }

                _state = TimerState.Callback;
                OnTimerTick();
                StartNextTick();
            }
            finally
            {
                try
                {
                    _timerLock.Leave();
                }
                catch (ObjectDisposedException)
                {
                    // Lock disposed, so no need to unlock it anymore
                }
            }
        }

        /// <summary>
        /// Fire TimerTick() event
        /// </summary>
        private void OnTimerTick()
        {
            var tick = TimerTick;
            if (tick != null)
            {
                tick(this, EventArgs.Empty);
            }
        }
    }
}