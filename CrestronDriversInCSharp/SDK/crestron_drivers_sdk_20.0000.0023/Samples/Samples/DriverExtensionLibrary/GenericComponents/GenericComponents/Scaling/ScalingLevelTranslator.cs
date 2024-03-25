using System;

namespace Crestron.RAD.Ext.Util.Scaling
{
    /// <summary>
    /// Synchronizes multiple consumers of a level in device units to allow
    /// manipulation of the same underlying value from different scales. The
    /// intended use case is for when a device may have its own scale and we
    /// would like to control it as a percentage or perhaps in other units.
    /// Because of this, names and descriptions refer to a "device scale"
    /// but it's really just the underlying scale of the value being
    /// controlled.
    /// </summary>
    /// <remarks>
    /// This class is not thread-safe. Do not interact with it or the
    /// subscribers of its LevelChanged event directly without being careful
    /// to avoid race conditions when events are firing.
    /// </remarks>
    public class ScalingLevelTranslator : IAdjustableLevel
    {
        /// <summary>
        /// Delegate function to be called to change the actual underlying
        /// level.
        /// </summary>
        /// <param name="level">The new level in native device units</param>
        public delegate void LevelChangeDelegate(double level);

        /// <summary>
        /// Controls the behavior when the native device scale is changed.
        /// When set to true, the present relative value is preserved. If
        /// we were at 47% before the change, we'll set a new level to remain
        /// at 47%.
        /// When set to false, the present value in device's units is
        /// preserved. If we were at 40% and the device changed from a 0-100
        /// scale to 0-50, then we would report 80% after the change.
        /// </summary>
        public bool PreservePercentWithScaleChange { get; set; }

        /// <summary>
        /// The present relative level in range 0 to 1.0
        /// </summary>
        private double _currentLevel;

        /// <summary>
        /// The present level in device native units
        /// </summary>
        private Scale.ScaledValue _currentDeviceLevel;

        /// <summary>
        /// The presently active scale on the device
        /// </summary>
        private Scale _deviceScale;

        /// <summary>
        /// Delegate function to be called whenever we need to change the
        /// underlying device's level. Will not be called if a new level is
        /// set that is equivalent to the previous one.
        /// </summary>
        private readonly LevelChangeDelegate _changeLevel;

        /// <summary>
        /// Event fired whenever the relative level is changed (0 to 1.0 scale)
        /// Objects like <see cref="IIntegerLevel"/> can subscribe to this to
        /// be kept in sync with other controls. The consuming classes of this
        /// translator may also use it if they care to track the present level
        /// in a 0 - 1.0 scale.
        /// </summary>
        public event EventHandler<LevelChangedEventArgs<double>> LevelChanged; 

        /// <summary>
        /// Initialize the level translator
        /// </summary>
        /// <param name="changeLevel">Delegate that will be called to set new levels on the device</param>
        /// <param name="defaultDeviceScale">Initial scale for the device's range</param>
        public ScalingLevelTranslator(LevelChangeDelegate changeLevel, Scale defaultDeviceScale)
        {
            _changeLevel = changeLevel;
            _deviceScale = defaultDeviceScale;
        }

        /// <summary>
        /// Changes the default native scale. This will likely result in either
        /// changes to the <see cref="IScalingLevel"/> objects or a call to the
        /// <see cref="LevelChangeDelegate"/>, depending on the value of
        /// <see cref="PreservePercentWithScaleChange"/>.
        /// </summary>
        /// <param name="newScale">The new device scale</param>
        public void UpdateDeviceScale(Scale newScale)
        {
            _deviceScale = newScale;
            if (PreservePercentWithScaleChange)
            {
                // Write new level after scale was updated to maintain present
                // relative value
                SetLevel(_currentLevel);
            }
            else
            {
                // Update relative values with new value after the scale
                // changed but the value remained constant.
                UpdateLevel(newScale.ToUnityScale(_currentDeviceLevel));
            }
        }

        /// <summary>
        /// Adjusts the present level up or down (as given by the sign of
        /// "direction") by the specified number of steps (as given by the
        /// magnitude of "direction"). If the step size is not set on the
        /// device scale, 1 is used.
        /// </summary>
        /// <param name="direction">How many steps to adjust by</param>
        public void AdjustSteps(double direction)
        {
            double step = _deviceScale.Step == null ? 1 : (double)_deviceScale.Step;
            step *= direction;
            SetLevel(_deviceScale.ToUnityScale(_currentDeviceLevel.Value + step));
        }

        /// <summary>
        /// Sets the new relative level in range 0 to 1.0. This will
        /// synchronize the levels and, if the new level corresponds to a
        /// new step on the device, call the <see cref="LevelChangeDelegate"/>
        /// </summary>
        /// <param name="level">The new relative level in range 0 to 1.0</param>
        public void SetLevel(double level)
        {
            // Get the new level in device units
            Scale.ScaledValue newLevel = _deviceScale.FromUnityScale(level);

            // Check if it is the same as the old level
            if (!newLevel.Equals(_currentDeviceLevel))
            {
                // Remember level in device units
                _currentDeviceLevel = newLevel;

                // Set actual device level
                _changeLevel(newLevel.Value);
            }

            // Notify listening levels
            UpdateLevel(level);
        }

        /// <summary>
        /// Updates the current level and notifies listeners
        /// </summary>
        /// <param name="level"></param>
        private void UpdateLevel(double level)
        {
            // Remember new level
            _currentLevel = level;

            // Synchronize levels across all scales
            var handler = LevelChanged;
            if (handler != null)
            {
                handler(this, new LevelChangedEventArgs<double>(level));
            }
        }

        /// <summary>
        /// Processes feedback from the device. This method is intended to be
        /// called in instances where we have determined the presently active
        /// level and want to only update the listening levels if this new
        /// level is different than the old one. For example, if the device
        /// has steps 0 - 10 but the user set 0.48, the device will be at 5.
        /// This method will know that we intended to set 5 and will not change
        /// he 0.48 to 0.50. However, if the device reports it is at 6, that's no
        /// longer equivalent to the present value, so levels will be
        /// synchronized to the new relative value at 0.60
        /// </summary>
        /// <param name="deviceLevel">The new level in the device's units (should match the active scale)</param>
        public void ProcessDeviceFeedback(double deviceLevel)
        {
            // Convert to 0 - 1.0 scale
            double newLevel = _deviceScale.ToUnityScale(deviceLevel);
            
            // Don't trust the input, use normalized level in the comparison
            Scale.ScaledValue normalizedDeviceLevel = _deviceScale.FromUnityScale(newLevel);

            // Only update levels if we had an actual change on the device
            if (!normalizedDeviceLevel.Equals(_currentDeviceLevel))
            {
                _currentDeviceLevel = normalizedDeviceLevel;
                UpdateLevel(newLevel);
            }
        }
    }
}