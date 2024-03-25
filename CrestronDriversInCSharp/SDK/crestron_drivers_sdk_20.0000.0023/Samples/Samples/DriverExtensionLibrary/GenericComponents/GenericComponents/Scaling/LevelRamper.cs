using System;
using Crestron.RAD.Ext.Util.Timers;

namespace Crestron.RAD.Ext.Util.Scaling
{
    public class LevelRamper : IDisposable
    {
	    private readonly IEventTimer _timer;

        public LevelRamper(IAdjustableLevel level, IEventTimer timer)
        {
            Level = level;
			_timer = timer;
			_timer.TimerTick += TimerTickEventHandler;
			StepsPerTick = 1.0;
		}

		protected IAdjustableLevel Level { get; set; }
		protected double Step { get; set; }

		public double StepsPerTick { get; set; }

        public virtual void Start(bool up)
        {
			_timer.Stop();
            Step = up ? 1 : -1;
			_timer.Start();
        }

        public void Stop()
        {
			_timer.Stop();
        }

        protected virtual void TimerTickEventHandler(object sender, EventArgs e)
        {
			Level.AdjustSteps(Step * StepsPerTick);
        }

        public virtual void Dispose()
        {
            _timer.TimerTick -= TimerTickEventHandler;
        }
    }
}