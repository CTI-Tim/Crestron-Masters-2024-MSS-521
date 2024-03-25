using System;

namespace Crestron.RAD.Ext.Util.Scaling
{
    // Should the public methods and properties here be virtual?
    // Should this have an interface?
    public class Scale
    {
        /// <summary>
        /// Wrapper around a double for values in scaled units. Provided to
        /// help identify which values are in which units and allow comparison
        /// between values that are known to be sanitized such that that is
        /// safe.
        /// DO NOT MANUALLY CREATE INSTANCES OF THIS OUTSIDE OF
        /// <see cref="Scale"/>! This is not enforced via an interface to
        /// avoid the performance impact of boxing/unboxing.
        /// </summary>
        public struct ScaledValue
        {
            public readonly double Value;
            public readonly Scale Scale;

            public ScaledValue(Scale scale, double v)
            {
                Scale = scale;
                Value = v;
            }

            public bool Equals(ScaledValue o)
            {
                if (o.Scale != Scale)
                {
                    return false;
                }

                // This comparison is only safe because we explicitly expect that
                // both numbers went through Math.Round() and had the same
                // operations performed since then. This is the reason we have
                // the "ScaledValue" struct.
				// ReSharper disable once CompareOfFloatsByEqualityOperator
                return o.Value == Value;
            }
        }

        private static readonly Scale _percentScale = new Scale(0, 100, 1);
        private static readonly Scale _uint16Scale = new Scale(UInt16.MinValue, UInt16.MaxValue, 1);

        /// <summary>
        /// Scale with 0 to 100 range and a step size of 1
        /// </summary>
        public static Scale PercentScale
        {
            get { return _percentScale; }
        }

        /// <summary>
        /// Scale with 0 to 65535 range and a step size of 1
        /// </summary>
        public static Scale UInt16Scale
        {
            get { return _uint16Scale; }
        }

        private readonly double _min;
        private readonly double _max;
        private readonly double? _step;

        /// <summary>
        /// Minimum value for the range (inclusive)
        /// </summary>
        public double Min
        {
            get { return _min; }
        }

        /// <summary>
        /// Maximum value for the range (inclusive)
        /// </summary>
        public double Max
        {
            get { return _max; }
        }

        /// <summary>
        /// Step size for the range, or null for double-precision ranges.
        /// </summary>
        public double? Step
        {
            get { return _step; }
        }

        /// <summary>
        /// The span of the range between max and min values
        /// </summary>
        private double Magnitude
        {
            get
            {
                return Max - Min;
            }
        }

        /// <summary>
        /// Initialize the scale with the given range
        /// </summary>
        /// <param name="min">Minimum value for the range</param>
        /// <param name="max">Maximum value for the range</param>
        /// <param name="step">Step size for the range. Set to null for a double-precision range.</param>
        public Scale(double min, double max, double? step)
        {
            _min = min;
            _max = max;
            _step = step;
        }

        /// <summary>
        /// Converts a floating point value between 0 and 1.0 to its
        /// corresponding value in this scale. Result will be constrained to
        /// fit in the range (inclusive). If <see cref="Step"/> is not null,
        /// the result will be a rounded to the nearest valid step.
        /// </summary>
        /// <param name="raw">The raw floating point value in range 0 to 1.0</param>
        /// <returns>The corresponding value in this scale</returns>
        public ScaledValue FromUnityScale(double raw)
        {
            double offset = raw * Magnitude;
            if (Step != null)
            {
                var step = (double)Step;
                offset = Math.Round(offset / step) * step;
            }
            return new ScaledValue(this, Constrain(Min + offset));
        }

        /// <summary>
        /// Converts a value in this level to the corresponding floating
        /// point value between 0 and 1.0 (inclusive)
        /// </summary>
        /// <param name="level">The level to convert</param>
        /// <returns>The value scaled to range 0 to 1.0</returns>
        public double ToUnityScale(double level)
        {
            return (Constrain(level) - Min) / Magnitude;
        }

        /// <summary>
        /// Converts a value in this level to the corresponding floating
        /// point value between 0 and 1.0 (inclusive)
        /// </summary>
        /// <param name="level">The level to convert</param>
        /// <returns>The value scaled to range 0 to 1.0</returns>
        public double ToUnityScale(ScaledValue level)
        {
            return ToUnityScale(level.Value);
        }

        /// <summary>
        /// Constrains the input value to be between <see cref="Max"/> and
        /// <see cref="Min"/>, inclusive.
        /// </summary>
        /// <param name="value">The value to constrain</param>
        /// <returns>The constrained value</returns>
        public double Constrain(double value)
        {
            if (value < Min)
            {
                value = Min;
            }
            if (value > Max)
            {
                value = Max;
            }
            return value;
        }
    }
}