using BrewApp.Hardware;
using BrewApp.Hardware.Interfaces;
using BrewApp.Logic.Recipes;
using System;

namespace BrewApp.Controller
{
    public class StepEvents
    {
        //public VesselValues VesselValues { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan TimeLeft { get; set; }
        public int Duration { get; set; }
        public string StepName { get; set; }
        public int StepNo { get; set; }
        public StepType StepType { get; set; }

        public void ResetValues()
        {
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue;
            TimeLeft = TimeSpan.MinValue;
            Duration = 0;
            StepName = "";
            StepNo = 0;
        }
    }

    public delegate void StepReached(object sender, int stepNo);
    public delegate void StepEvent(object sender, StepEvents stepEvents);

    public class StepController : IDisposable
    {
        #region Events
        public event StepReached StepReached;
        public event StepEvent StepEvent;
        #endregion

        #region Variables
        IVessel _vessel = null;
        IEmergencyButton _emergencyButton = null;
        StepEvents _stepEvent = new StepEvents();
        System.Timers.Timer _updateTimer = new System.Timers.Timer(1000);
        System.Timers.Timer _stepTimer = new System.Timers.Timer();
        #endregion

        #region Constructor
        public StepController(IVessel vessel, IEmergencyButton emergencyButton)
        {
            _vessel = vessel;
            //_vessel.VesselEvent += _vessel_VesselEvent;
            _vessel.TargetTemperaturReached += _vessel_TargetTemperaturReached;
            _emergencyButton = emergencyButton;

            if (_vessel is IEmergency)
            {
                _emergencyButton.ButtonPressed += (_vessel as IEmergency).SetEmergencyStop;
                _emergencyButton.ButtonReleased += (_vessel as IEmergency).ResetEmergencyStop;
            }
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _updateTimer.AutoReset = true;
            _stepTimer.Elapsed += Timer_Elapsed;
            _stepTimer.AutoReset = false;
        }


        ~StepController()
        {
            Dispose(false);
        }
        #endregion

        #region Rise up events
        private void _updateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _stepEvent.TimeLeft = _stepEvent.StartTime.AddMinutes(_stepEvent.Duration) - DateTime.Now;
            StepEvent?.Invoke(this, _stepEvent);
        }

        //private void _vessel_VesselEvent(object sender, VesselValues vesselValues)
        //{
        //    _stepEvent.Duration = _stepEvent.StartTime - DateTime.Now;
        //    _stepEvent.VesselValues = vesselValues;
        //    StepEvent?.Invoke(this, _stepEvent);
        //}
        #endregion

        #region Start / Stop
        bool _run = false;
        StepType _stepType = StepType.HoldAuto;

        int _stepNo = 0;
        public void StartStep(Step step)
        {
            if (!_run)
            {
                _stepType = step.StepType;
                _stepNo = step.SortOrder;
                _stepEvent.StartTime = DateTime.Now;
                _stepEvent.StepName = step.Name;
                _stepEvent.StepNo = _stepNo;
                _stepEvent.Duration = step.Duration;
                _stepEvent.TimeLeft = new TimeSpan(0, step.Duration, 0);
                _stepEvent.StepType = step.StepType;

                _vessel.SetTargetTemperature(step.Temperature);
                (_vessel as IStirrer)?.SetStirrer(StirrerDirection.Left);
                (_vessel as IStirrer)?.SetStirrerSpeed(step.StirrerSpeed);
                _vessel.Start();
                _updateTimer.Start();
                _updateTimer_Elapsed(this, null);
                //if (step.StepType == StepType.Heating)
                //{
                //    _vessel.SetTargetTemperature(step.Temperature);
                //    _vessel.SetStirrer(StirrerDirection.Left);
                //    _vessel.SetStirrerSpeed(step.StirrerSpeed);

                //}
                //else 
                if (step.StepType == StepType.HoldAuto)
                {
                    // hold
                    //_vessel.SetTargetTemperature(step.Temperature);
                    //_vessel.SetStirrer(StirrerDirection.Left);
                    //_vessel.SetStirrerSpeed(step.StirrerSpeed);

                    //var timer = new System.Timers.Timer(step.Duration * 60000);

                    _stepTimer.Interval = step.Duration * 60000;
                    _stepTimer.Enabled = true;

                    // Hook up the event handler for the Elapsed event.
                    //timer.Elapsed += Timer_Elapsed;

                    // Only raise the event the first time Interval elapses.
                    //timer.AutoReset = false;

                }
                //else
                //{
                //    // MashIn
                //    _vessel.SetTargetTemperature(step.Temperature);
                //    _vessel.SetStirrer(StirrerDirection.Left);
                //    _vessel.SetStirrerSpeed(step.StirrerSpeed);
                //}
                _run = true;
            }
        }
        public void StopStep()
        {
            if (_run)
            {
                _run = false;
                _vessel?.Stop();
                _updateTimer.Stop();
                _updateTimer_Elapsed(this, null);
                _stepTimer.Stop();
            }
        }

        #endregion

        #region Reset Controller

        public void Reset()
        {
            StopStep();
            _stepEvent.ResetValues();
        }
        #endregion

        #region Step Reached Events
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            StepReached?.Invoke(this, _stepNo);
        }

        private void _vessel_TargetTemperaturReached(object sender, double temperature)
        {
            if (_stepType == StepType.Heating)
            {
                StepReached?.Invoke(this, _stepNo);
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
        }

        bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }
            if (_vessel is IEmergency)
            {
                _emergencyButton.ButtonPressed -= (_vessel as IEmergency).SetEmergencyStop;
                _emergencyButton.ButtonReleased -= (_vessel as IEmergency).ResetEmergencyStop;
            }
            _vessel.TargetTemperaturReached -= _vessel_TargetTemperaturReached;
            //_vessel?.Dispose();
            //_emergencyButton?.Dispose();
            disposed = true;
        }
        #endregion
    }
}
