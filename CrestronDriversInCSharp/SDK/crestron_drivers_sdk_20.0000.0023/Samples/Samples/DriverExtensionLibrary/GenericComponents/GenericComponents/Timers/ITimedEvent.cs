using System;

namespace Crestron.RAD.Ext.Util.Timers
{
    /// <summary>
    /// Raises an event periodically on a separate thread. The events can be
    /// started and stopped. Control over the frequency of the timer is outside
    /// the scope of this interface.
    /// </summary>
    public interface IEventTimer
    {
        /// <summary>
        /// Start the timer if it is not already running.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the timer and block until any active TimerTick events complete
        /// </summary>
        /// <remarks>DO NOT CALL THIS FROM A <see cref="TimerTick"/> HANDLER!</remarks>
        void Stop();

        /// <summary>
        /// Cancel the timer, stopping it without blocking waiting for TimerTick
        /// events to complete. Safe to call from a <see cref="TimerTick"/>
        /// event handler.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Event fired for each tick of the timer.
        /// Do not call Stop() from the handler or the thread may hang.
        /// </summary>
        event EventHandler<EventArgs> TimerTick;
    }
}
