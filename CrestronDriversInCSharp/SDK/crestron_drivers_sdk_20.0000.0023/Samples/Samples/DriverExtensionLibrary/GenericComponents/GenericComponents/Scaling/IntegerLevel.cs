using System;

namespace Crestron.RAD.Ext.Util.Scaling
{
    /// <summary>
    /// Integer-based level for use with <see cref="ScalingLevelTranslator"/>
    /// </summary>
    public class IntegerLevel : IIntegerLevel
    {
        /// <summary>
        /// Delegate function to be called when this level is written. Unless
        /// the new level is blocked, the function should call <see cref="IntegerLevel.UpdateLevel"/>
        /// with the new level.
        /// </summary>
        /// <param name="level">The new level in range 0 to 1.0</param>
        public delegate void LevelChangeDelegate(double level);

        /// <summary>
        /// Event fired whenever this level changes values. EventArgs contains
        /// the new level.
        /// </summary>
        public event EventHandler<LevelChangedEventArgs<long>> IntegerLevelChanged;

        /// <summary>
        /// The presently set level
        /// </summary>
        private long _targetLevel;

        /// <summary>
        /// The scale for this level.
        /// </summary>
        private readonly Scale _scale;

        /// <summary>
        /// The function to call to set a new level.
        /// </summary>
        private readonly LevelChangeDelegate _setLevel;

        /// <summary>
        /// Initialize the level with the provided scale and level setter
        /// </summary>
        /// <param name="scale">The scale for the integer level. Step size must be a positive integral value.</param>
        /// <param name="levelChanger">Delegate function to call to set the level. <see cref="LevelChangeDelegate"/></param>
        public IntegerLevel(Scale scale, LevelChangeDelegate levelChanger)
        {
            _scale = scale;
            _setLevel = levelChanger;
        }

        /// <summary>
        /// The present level value in scaled integer units. Reading reports
        /// the most recently set level (updated with feedback UpdateLevel()).
        /// Writing this property will set the new level. This should only be
        /// set to change the level. Updates to the level based on feedback or
        /// control via another mechanism should not use this setter.
        /// </summary>
        public long Level
        {
            get
            {
                return _targetLevel;
            }
            set
            {
                // Always call this even if the level is the same because
                // we might need to update a level with a different scale
                // to this new value.
                _setLevel(_scale.ToUnityScale(value));
            }
        }

	    /// <summary>
	    /// Informs this level of a change in value. Should be called by the
	    /// <see cref="LevelChangeDelegate"/> to inform this object that its
	    /// update was accepted. It should also be called when the underlying
	    /// value changes for other reasons.
	    /// </summary>
	    /// <param name="sender"></param>
		/// <param name="e">The new value in a 0 to 1.0 scale</param>
	    public void LevelChangedHandler(object sender, LevelChangedEventArgs<double> e)
        {
            // Convert level to this integer scale
            var level = (long)_scale.FromUnityScale(e.Level).Value;

            // Update and call event handler if value changed
            if (level != _targetLevel)
            {
                OnLevelChanged(level);
            }
        }

        /// <summary>
        /// Called whenever the level changes. Sets the new level and fires
        /// events.
        /// </summary>
        /// <param name="level">The new level</param>
        private void OnLevelChanged(long level)
        {
            _targetLevel = level;
            var listener = IntegerLevelChanged;
            if (listener != null)
            {
                listener(this, new LevelChangedEventArgs<long>(level));
            }
        }
    }
}