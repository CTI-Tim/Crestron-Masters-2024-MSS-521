using System;

namespace Crestron.RAD.Ext.Util.Scaling
{
    /// <summary>
    /// Event argument for when a scaled level changes
    /// </summary>
    /// <typeparam name="T">The type of the level</typeparam>
    public class LevelChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The new level value
        /// </summary>
        public T Level { get; private set; }

        /// <summary>
        /// Initialize with the provided level
        /// </summary>
        /// <param name="level">The new level</param>
        public LevelChangedEventArgs(T level)
        {
            Level = level;
        }

        public override string ToString()
        {
            return Level.ToString();
        }
    }

    /// <summary>
    /// Represents an integer level that can be synchronized with other scaled
    /// levels
    /// </summary>
    public interface IIntegerLevel
    {
        /// <summary>
        /// Fired whenever the level changes values, providing the new level
        /// value in the event arguments.
        /// </summary>
        event EventHandler<LevelChangedEventArgs<long>> IntegerLevelChanged;

        /// <summary>
        /// The present level value in scaled integer units. Reading reports
        /// the most recently set level (updated with feedback UpdateLevel()).
        /// Writing this property will set the new level. This should only be
        /// set to change the level. Updates to the level based on feedback or
        /// control via another mechanism should not use this setter.
        /// </summary>
        long Level { get; set; }

        /// <summary>
        /// Informs this object of an update to the level from outside its own
        /// control. This does not take any action to change the level, it is
        /// just a notification to keep it in sync with the newest value.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Holds the new level in a 0 to 1.0 scale</param>
        void LevelChangedHandler(object sender, LevelChangedEventArgs<double> e);
    }

    public interface IAdjustableLevel
    {
        /// <summary>
        /// Adjusts the present level up or down (as given by the sign of
        /// "direction") by the specified number of steps (as given by the
        /// magnitude of "direction"). If the step size is not set on the
        /// device scale, 1 is used.
        /// </summary>
        /// <param name="direction">How many steps to adjust by</param>
        void AdjustSteps(double direction);
    }

    public static class ScalingExtensions
    {
        /// <summary>
        /// Adjusts the level by one step in the positive direction
        /// </summary>
        /// <param name="level">Level to adjust</param>
        public static void Increment(this IAdjustableLevel level)
        {
            level.AdjustSteps(1);
        }

        /// <summary>
        /// Adjusts the level by one step in the negative direction
        /// </summary>
        /// <param name="level">Level to adjust</param>
        public static void Decrement(this IAdjustableLevel level)
        {
            level.AdjustSteps(-1);
        }
    }
}