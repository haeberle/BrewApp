using BrewApp.Controller;
using BrewApp.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Controls.Brewery.Logger
{
    public class LogItem
    {
        public LogItem()
        {

        }
        public LogItem(StepEvents stepevents, VesselValues vesselValues)
        {
            StartTime = stepevents.StartTime;
            EndTime = stepevents.EndTime;
            TimeLeft = stepevents.TimeLeft;
            Duration = stepevents.Duration;
            StepName = stepevents.StepName;
            StepNo = stepevents.StepNo;
            PumpOn = vesselValues.PumpOn;
            HeaterLevel1On = vesselValues.HeaterLevel1On;
            HeaterLevel2On = vesselValues.HeaterLevel2On;
            StirrerDirection = vesselValues.StirrerDirection;
            StirrerSpeed = vesselValues.StirrerSpeed;
            VesselTemperature = vesselValues.VesselTemperature;
            MashTargetTemperature = vesselValues.MashTargetTemperature;
            MashCurrentTemperature = vesselValues.MashCurrentTemperature;
            EmergencyOn = vesselValues.EmergencyOn;
        }

        public void LogItemStepEvents(StepEvents stepevents)
        {
            StartTime = stepevents.StartTime;
            EndTime = stepevents.EndTime;
            TimeLeft = stepevents.TimeLeft;
            Duration = stepevents.Duration;
            StepName = stepevents.StepName;
            StepNo = stepevents.StepNo;            
        }

        public void LogItemVesselValues(VesselValues vesselValues)
        {            
            PumpOn = vesselValues.PumpOn;
            HeaterLevel1On = vesselValues.HeaterLevel1On;
            HeaterLevel2On = vesselValues.HeaterLevel2On;
            StirrerDirection = vesselValues.StirrerDirection;
            StirrerSpeed = vesselValues.StirrerSpeed;
            VesselTemperature = vesselValues.VesselTemperature;
            MashTargetTemperature = vesselValues.MashTargetTemperature;
            MashCurrentTemperature = vesselValues.MashCurrentTemperature;
            EmergencyOn = vesselValues.EmergencyOn;
        }

        public DateTime TimeStamp { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan TimeLeft { get; set; }
        public int Duration { get; set; }
        public string StepName { get; set; }
        public int StepNo { get; set; }
        public bool PumpOn { get; set; }
        public bool HeaterLevel1On { get; set; }
        public bool HeaterLevel2On { get; set; }
        public StirrerDirection StirrerDirection { get; set; }
        public int StirrerSpeed { get; set; }
        public double VesselTemperature { get; set; }
        public double MashTargetTemperature { get; set; }
        public double MashCurrentTemperature { get; set; }
        public bool EmergencyOn { get; set; }
    }
}
