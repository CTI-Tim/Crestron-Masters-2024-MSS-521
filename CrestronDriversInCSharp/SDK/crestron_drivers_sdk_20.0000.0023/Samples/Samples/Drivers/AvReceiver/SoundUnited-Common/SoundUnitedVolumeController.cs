using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.RAD.Ext.Util.Scaling;
using DriverExtensionLibrary.Helpers;

namespace Crestron.RAD.Drivers.AVReceivers.SoundUnited
{
    // This class isn't super clean but is meant to reduce copy/paste in
    // creating each of these objects for different zones. Time was not
    // invested to make an API for this class, so controllers are just
    // public properties.
    internal class SoundUnitedVolumeController 
    {
        // Delay times for volume ramping. First entry is zero so
        // that button presses result in one immediate step.
        // Units are ms
        private readonly long[] _rampingSchedule = new long[] { 0, 500, 250, 100 };

        private Scale _defaultVolumeScale;

        // Handler for mute and volume sequencing logic
        public SoundUnitedMuteVolController MuteVol { get; private set; }

        // Handler for volume scaling
        public PercentUshortLevelTranslator VolumeLevel { get; private set; } 

        // Timer for volume ramping
        private ScheduledEventTimer _rampingTimer;

        // Controller that connects the ramping timer to the volume level
        public LevelRamper VolumeRamper { get; private set; }
        
        public SoundUnitedVolumeController(Func<bool> getMuteState, ScalingLevelTranslator.LevelChangeDelegate changeVolume, double volumeStepSize, long timeBetweenCommands)
        {
            _defaultVolumeScale = new Scale(0, 98, volumeStepSize);
                
            MuteVol = new SoundUnitedMuteVolController(getMuteState);
            VolumeLevel = new PercentUshortLevelTranslator(changeVolume, _defaultVolumeScale);

            for (var i = 0; i < _rampingSchedule.Length; i++)
            {
                var t = _rampingSchedule[i];
                // Skip 0 since that's supposed to be immediate
                if (t != 0 && t < timeBetweenCommands)
                {
                    _rampingSchedule[i] = timeBetweenCommands;
                }
            }
            _rampingTimer = new ScheduledEventTimer(_rampingSchedule);
            VolumeRamper = new LevelRamper(VolumeLevel, _rampingTimer)
            {
                // Increment enough steps to get 1%
                StepsPerTick = 1 / (double)_defaultVolumeScale.Step
            };
        }

        public void UpdateScale(double max)
        {
            var scale = new Scale(_defaultVolumeScale.Min, max, _defaultVolumeScale.Step);
            VolumeLevel.UpdateDeviceScale(scale);
        }

        public void Dispose()
        {
            VolumeRamper.Dispose();
            _rampingTimer.Dispose();
        }
    }
}