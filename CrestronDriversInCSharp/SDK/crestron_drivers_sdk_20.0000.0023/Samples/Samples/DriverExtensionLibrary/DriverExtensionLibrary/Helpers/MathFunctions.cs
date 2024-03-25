using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace DriverExtensionLibrary.Helpers
{
    public static class MathFunctions
    {
        public static float Scale(float raw, float rawMin, float rawMax, float scaledMin, float scaledMax)
        {
            float rawDelta = rawMax - rawMin;
            float rawScalar = raw - rawMin;

            float scaledDelta = scaledMax - scaledMin;

            return (raw - rawMin) * (scaledDelta / rawDelta) + scaledMin;
        }

        public static float ScaleNearestMultiple(float raw, float rawMin, float rawMax, float scaledMin, float scaledMax, float stepSize)
        {
            float rawDelta = rawMax - rawMin;
            float rawScalar = raw - rawMin;

            float scaledDelta = scaledMax - scaledMin;

            return (raw - rawMin) * (scaledDelta / rawDelta) + scaledMin;
        }

        public static float ScaleRangeToPercent(float raw, float rawMin, float rawMax)
        {
            return ScaleNearestMultiple(raw, rawMin, rawMax, 0, 100, 1);
        }

        public static float ScalePercentToRange(float raw, float scaledMin, float scaledMax)
        {
            return ScaleNearestMultiple(raw, 0, 100, scaledMin, scaledMax, 1);
        }

        public static float GetPercentIncrement(float rawMin, float rawMax)
        {
            float range = Math.Abs(rawMin) + Math.Abs(rawMax);
            float increment = range / 100;
            return increment;
        }
    }
}