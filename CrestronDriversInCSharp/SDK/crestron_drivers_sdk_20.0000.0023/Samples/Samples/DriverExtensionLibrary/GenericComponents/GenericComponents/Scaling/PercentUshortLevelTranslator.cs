using System;

namespace Crestron.RAD.Ext.Util.Scaling
{
    /// <summary>
    /// Extension of a <seealso cref="ScalingLevelTranslator"/> that is built to expose a
    /// percent-range level as well as a 0-65535 ushort range. This is useful
    /// for certain RAD use cases like Volume, etc.
    /// </summary>
    public class PercentUshortLevelTranslator : ScalingLevelTranslator
    {
		/// <summary>
		/// The level as a percentage
		/// </summary>
		private readonly IIntegerLevel _percentLevel;

		/// <summary>
		/// The level in range 0-65535
		/// </summary>
		private readonly IIntegerLevel _ushortLevel;

		/// <summary>
        /// Factory function to create IIntegerLevels
        /// </summary>
        /// <param name="scale">The scale to initialize the level with</param>
        /// <param name="changeLevel">The delegate for IIntegerLevel to change the physical level</param>
        /// <returns>The IIntegerLevel instance</returns>
        public delegate IIntegerLevel CreateIntegerLevel(Scale scale, IntegerLevel.LevelChangeDelegate changeLevel);

        /// <summary>
        /// Default constructor which creates IntegerLevels for percent and ushort
        /// </summary>
        /// <param name="changeLevel">Delegate function to change the actual device level</param>
        /// <param name="defaultDeviceScale">The device's scale (default, can be updated)</param>
        public PercentUshortLevelTranslator(LevelChangeDelegate changeLevel, Scale defaultDeviceScale)
            : this(changeLevel, defaultDeviceScale, DefaultIntegerLevelFactory)
        {
        }

        /// <summary>
        /// Fully-featured constructor which allows specifying a factory
        /// function to create custom IIntegerLevels.
        /// </summary>
        /// <param name="changeLevel">Delegate function to change the actual device level</param>
        /// <param name="defaultDeviceScale">The device's scale (default, can be updated)</param>
        /// <param name="levelFactory">Function to create IIntegerLevel instances for percent and ushort</param>
        public PercentUshortLevelTranslator(LevelChangeDelegate changeLevel, Scale defaultDeviceScale, CreateIntegerLevel levelFactory)
            : base(changeLevel, defaultDeviceScale)
        {
            _percentLevel = levelFactory(Scale.PercentScale, SetLevel);
            _percentLevel.IntegerLevelChanged += PercentChangedHandler;
            LevelChanged += _percentLevel.LevelChangedHandler;

            _ushortLevel = levelFactory(Scale.UInt16Scale, SetLevel);
            _ushortLevel.IntegerLevelChanged += UshortChangedHandler;
            LevelChanged += _ushortLevel.LevelChangedHandler;
        }

		/// <summary>
		/// Event fired when the percentage value changes. Args include new value.
		/// </summary>
		public event EventHandler<LevelChangedEventArgs<uint>> PercentChanged;

		/// <summary>
		/// Event fired when the ushort value changes. Args include new value.
		/// </summary>
		public event EventHandler<LevelChangedEventArgs<ushort>> UshortLevelChanged;

		/// <summary>
		/// The level as a percentage. Setting this level will update the
		/// level and may trigger a device level change if it corresponds
		/// to a new device step. It will update other synchronized levels
		/// either way. <seealso cref="UshortLevel"/>
		/// </summary>
		public uint Percent
		{
			get
			{
				return (uint)_percentLevel.Level;
			}
			set
			{
				_percentLevel.Level = value;
			}
		}

		/// <summary>
		/// The level in range 0-65535. Setting this level will update the
		/// level and may trigger a device level change if it corresponds
		/// to a new device step. It will update other synchronized levels
		/// either way. <seealso cref="Percent"/>
		/// </summary>
		public ushort UshortLevel
		{
			get
			{
				return (ushort)_ushortLevel.Level;
			}
			set
			{
				_ushortLevel.Level = value;
			}
		}

        /// <summary>
        /// Default factory function to create IntegerLevel objects
        /// </summary>
        /// <param name="scale">The scale to initialize the level with</param>
        /// <param name="changeLevel">The delegate for IIntegerLevel to change the physical level</param>
        /// <returns>The IIntegerLevel instance</returns>
        private static IIntegerLevel DefaultIntegerLevelFactory(Scale scale, IntegerLevel.LevelChangeDelegate changeLevel)
        {
            return new IntegerLevel(scale, changeLevel);
        }

        /// <summary>
        /// Event handler for the percentage level. This simply forwards the
        /// event along but as the expected uint type.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event args with new level</param>
        private void PercentChangedHandler(object sender, LevelChangedEventArgs<long> e)
        {
            var handler = PercentChanged;
            if (handler != null)
            {
                handler(this, new LevelChangedEventArgs<uint>((uint)e.Level));
            }
        }

        /// <summary>
        /// Event handler for the ushort level. This simply forwards the
        /// event along but as the expected ushort type.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event args with new level</param>
        private void UshortChangedHandler(object sender, LevelChangedEventArgs<long> e)
        {
            var handler = UshortLevelChanged;
            if (handler != null)
            {
                handler(this, new LevelChangedEventArgs<ushort>((ushort)e.Level));
            }
        }
    }
}