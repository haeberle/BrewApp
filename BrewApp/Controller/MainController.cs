using BrewApp.Controls.Brewery.Logger;
using BrewApp.Hardware;
using BrewApp.Hardware.BK500;
using BrewApp.Hardware.Interfaces;
using BrewApp.Logic;
using BrewApp.Logic.Recipes;
using BrewApp.ViewModel;
using System;
using System.Globalization;
using System.IO;

namespace BrewApp.Controller
{
    public delegate void ProcessStopEvent(object sender, int stepNumber);

    public class MainController : IDisposable
    {
        #region Members      
        IEmergencyButton _emergencyButton = null;
        IVessel _vessel = null;
        int _totalDuration = 0;
        StepController _stepController = null;
        //BrewingViewModel _brewingViewModel = null;
        System.Timers.Timer _logUpdateTimer = new System.Timers.Timer(10000);
        Recipe _recipe = null;
        #endregion

        #region Constructor
        public MainController()
        {
            _emergencyButton = new EmergencyButtonDummy();// EmergencyButton();
            _vessel = ((App)App.Current).Vessel;
            _vessel.VesselEvent += _vessel_VesselEvent;
            _stepController = new StepController(_vessel, _emergencyButton);
            _stepController.StepEvent += _stepController_StepEvent;
            _stepController.StepReached += _stepController_StepReached;
            _logUpdateTimer.Elapsed += _logUpdateTimer_Elapsed;
            //_brewingViewModel = brewingViewModel;
        }

        private void _logUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _logItem.TimeStamp = DateTime.Now;
            Logger.Log(_logfilePath, _logItem);
        }

        ~MainController()
        {
            Dispose(false);
        }
        #endregion

        #region Next Step

        public int StepCount
        {
            get
            {
                return _recipe != null ? _recipe.Steps.Count : 0;
            }
        }

        public void NextStep()
        {
            _stepController.StopStep();

            if (_stepCount < _recipe.Steps.Count)
            {
                var step = _recipe.Steps[_stepCount];
                _stepController.StartStep(step);
                _stepCount++;
                Logger.Log(_logfilePathHeader, $"Manual next step:{step.Name},StepNo:{step.SortOrder},Time:{DateTime.Now}");
            }
        }
        #endregion

        #region Step Reached
        public event ProcessStopEvent ProcessStopEvent;

        int _stepCount = 0;
        private void _stepController_StepReached(object sender, int stepNo)
        {
            _stepController.StopStep();

            if (_stepCount >= _recipe.Steps.Count)
            {
                // done....
                ProcessStopEvent?.Invoke(sender, stepNo);
            }
            else
            {               
                var step = _recipe.Steps[_stepCount];
                _stepController.StartStep(step);
                Logger.Log(_logfilePathHeader, $"Auto next step:{step.Name},StepNo:{step.SortOrder},Time:{DateTime.Now}");               
                _stepCount++;
            }
        }
        #endregion

        public event StepEvent StepEvent;
        LogItem _logItem = new LogItem();
        private void _stepController_StepEvent(object sender, StepEvents stepEvents)
        {
            stepEvents.EndTime = stepEvents.StartTime.AddMinutes(_totalDuration);
            _logItem.LogItemStepEvents(stepEvents);
            StepEvent?.Invoke(sender, stepEvents);
        }

        public event VesselEvent VesselEvent;
        private void _vessel_VesselEvent(object sender, VesselValues vesselValues)
        {
            _logItem.LogItemVesselValues(vesselValues);
            VesselEvent?.Invoke(sender, vesselValues);
        }       

        //public void Init(BrewingViewModel brewingViewModel)
        //{
        //    _brewingViewModel = brewingViewModel;

        //}
        public void LoadRecipe(Recipe recipe)
        {
            _stepController?.Reset();
            _recipe = recipe;
            //_brewingViewModel.RecipeLoaded = true;
            _stepCount = 0;

            if (_recipe.MashIn != null)
            {
                //_brewingViewModel.MashIn = true;
            }

            _totalDuration = 0;
            foreach (var step in recipe.Steps)
            {
                _totalDuration += step.Duration;
            }

        }

        #region Start/Stop
        bool _run = false;
        bool _mashInDone = false;
        string _logfilePath = "";
        string _logfilePathHeader = "";
        public void Start(string brewerName)
        {
            if (!_run)
            {
                var id = Guid.NewGuid();
                _logfilePath = $"{id}_brewlog.csv";
                _logfilePath = Path.Combine(Constants.ApplicationLogPath, _logfilePath);
                _logfilePathHeader = $"{id}_brewlog_header.txt";
                _logfilePathHeader = Path.Combine(Constants.ApplicationLogPath, _logfilePathHeader);
                Logger.Header<LogItem>(_logfilePath);
                _logUpdateTimer.Start();

                Logger.Log(_logfilePathHeader, $"Start:{DateTime.Now}");
                Logger.Log(_logfilePathHeader, $"Programm:{_recipe.Name}");
                Logger.Log(_logfilePathHeader, $"Brewer:{brewerName}");

                if (_recipe.MashIn != null && !_mashInDone)
                {

                }
                else
                {
                    _stepCount = 0;
                    _stepController?.Reset();
                    _stepController_StepReached(this, 0);
                }
                _run = true;
               
            }
        }
        public void Stop()
        {
            if (_run)
            {
                _stepController.StopStep();
                _run = false;
                _logUpdateTimer.Stop();
                Logger.Log(_logfilePathHeader, $"Stop : {DateTime.Now}");
            }
        }
        #endregion

        #region Dispose
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

            _stepController?.Dispose();
            //_emergencyButton?.Dispose();
            //_vessel?.Dispose();
            _vessel.VesselEvent -= _vessel_VesselEvent;
            _logUpdateTimer.Dispose();

            disposed = true;
        }
        #endregion 
    }
}
