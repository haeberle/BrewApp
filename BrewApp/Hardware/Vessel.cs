using BrewApp.Hardware.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace BrewApp.Hardware
{
    // Stirrer
    // start, left, right
    // speed

    // Heater
    // None, Half, Full
    // Pump

    // MashTemp
    // VesselTemp

    public enum StirrerDirection
    {
        None,
        Left,
        Right
    }

    public class VesselValues
    {
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

    public delegate void TemperatureEvent(object sender, double temperature);
    public delegate void VesselEvent(object sender, VesselValues vesselValues);
    public class Vessel : IEmergency, IDisposable
    {
        #region Constants
        const int SLEEPTIME = 500;
        const double HYSTERESYSUP = 0.1;
        const double HYSTERESYSDOWN = 0.2;
        const double VESSELOVERRUN = 10.0;
        #endregion
        #region Events
        //public event TemperatureEvent VesselIsTemperatureChange;
        //public event TemperatureEvent VesselBeTemperatureChange;
        //public event TemperatureEvent HeaterTemperatureChange;
        public event TemperatureEvent TargetTemperaturReached;
        public event VesselEvent VesselEvent;
        #endregion

        #region Variables
        CancellationTokenSource _cancellationToken = new CancellationTokenSource();
#if (!SIMULATOR)
        GpioPin _stirrerLeftPin = null;
        GpioPin _stirrerRightPin = null;
        GpioPin _heaterPumpPin = null;
        GpioPin _heater1Pin = null;
        GpioPin _heater2Pin = null;
#endif
        TemperaturReader _vesselTemperature = new TemperaturReader("VesselTemperatureProbe");
        TemperaturReader _mashTemperature = new TemperaturReader("MashTemperatureProbe");

        #endregion

        #region Constructor
        public Vessel()
        {
#if (!SIMULATOR)
            _stirrerLeftPin = GpioController.GetDefault().OpenPin(HardwareDefinition.StirrerLeftOut);
            _stirrerRightPin = GpioController.GetDefault().OpenPin(HardwareDefinition.StirrerRightOut);
            _heaterPumpPin = GpioController.GetDefault().OpenPin(HardwareDefinition.HeaterPumpOut);
            _heater1Pin = GpioController.GetDefault().OpenPin(HardwareDefinition.Heater1Out);
            _heater2Pin = GpioController.GetDefault().OpenPin(HardwareDefinition.Heater2Out);
#endif
            ProcessStart();
        }

        ~Vessel()
        {
            Dispose(false);
        }
        #endregion

        #region Emergency
        bool _emergencyOn = false;
        public void SetEmergencyStop(object sender, DateTime time)
        {
            _emergencyOn = true;
        }

        public void ResetEmergencyStop(object sender, DateTime time)
        {
            _emergencyOn = false;
        }
        #endregion

        #region Temperatures

        public double GetVesselTemperature()
        {
            return _vesselTemperature.GetTemperature();
        }

        public double GetMashTemperature()
        {
            return _mashTemperature.GetTemperature();
        }

        #endregion

        #region Parameters
        double _targetTemperature = 0.0;
        public void SetTargetTemperature(double temperature)
        {
            _targetTemperature = temperature;
        }

        StirrerDirection _stirrerDirection = StirrerDirection.None;
        public void SetStirrer(StirrerDirection direction)
        {
            _stirrerDirection = direction;
        }

        int _stirrerSpeed = 0;
        public void SetStirrerSpeed(int speed)
        {
            if (speed > 100)
            {
                _stirrerSpeed = 100;
            }
            else if (speed < 0)
            {
                _stirrerSpeed = 0;
            }
            else
            {
                _stirrerSpeed = speed;
            }
        }
        #endregion

        #region Start/Stop/Hold
        bool _run = false;
        public void Start()
        {
            _run = true;
        }

        private void ProcessStart()
        {

            //StirrerDirection tempDirection = StirrerDirection.None;
            //int tempStirrerSpeed = 0;
            ////bool sendVesselStateState = false;
            //double vesselTemperature = 0.0;
            ////double tempVesselTemperature = 0.0;
            //double mashTemperature = 0.0;
            ////double tempMashTemperature = 0.0;
            //bool heater1On = false;
            //bool heater2On = false;
            //bool pump = false;
            //bool tempHeater1On = false;
            //bool tempHeater2On = false;
            //bool tempPump = false;

            var consumerTask = Task.Factory.StartNew(
                () =>
                {
                    while (!_cancellationToken.IsCancellationRequested)
                    {
                        StirrerDirection tempDirection = StirrerDirection.None;
                        int tempStirrerSpeed = 0;
                        //bool sendVesselStateState = false;
                        double vesselTemperature = 0.0;
                        //double tempVesselTemperature = 0.0;
                        double mashTemperature = 0.0;
                        //double tempMashTemperature = 0.0;
                        bool heater1On = false;
                        bool heater2On = false;
                        bool pump = false;
                        bool tempHeater1On = false;
                        bool tempHeater2On = false;
                        bool tempPump = false;

                        #region Temperature Reader

                        #region Read Vessel Temp
                        vesselTemperature = GetVesselTemperature();
                        #endregion

                        #region Read Mash Temp
                        mashTemperature = GetMashTemperature();
                        #endregion

                        #endregion

                        if (_run)
                        {
                            #region Controller

                            pump = true;

                            if (mashTemperature + HYSTERESYSUP >= _targetTemperature) // temp reached
                            {
                                // heater off
                                heater1On = heater2On = false;
                                TargetTemperaturReached?.Invoke(this, mashTemperature);
                            }
                            else if (mashTemperature < _targetTemperature - HYSTERESYSDOWN)
                            {
                                if (vesselTemperature < mashTemperature + VESSELOVERRUN)
                                {
                                    // heater on
                                    heater1On = heater2On = true;
                                }
                                else
                                {
                                    // heater off
                                    heater1On = heater2On = false;
                                }
                            }

                            if (tempHeater1On != heater1On || tempHeater2On != heater2On || tempPump != pump)
                            {
                                //sendVesselStateState = true;
                                tempHeater1On = heater1On;
                                tempHeater2On = heater2On;
                                tempPump = pump;
                            }

                            #endregion

                            #region Stirrer
                            if (tempDirection != _stirrerDirection)
                            {
                                //if (_stirrerDirection == StirrerDirection.None)
                                //{
                                //    // switch off
                                //}
                                //else if (_stirrerDirection == StirrerDirection.Left)
                                //{
                                //    // left on
                                //}
                                //else
                                //{
                                //    // right on
                                //}
                                tempDirection = _stirrerDirection;
                                //sendVesselStateState = true;
                            }

                            if (tempStirrerSpeed != _stirrerSpeed)
                            {
                                // set Speed

                                tempStirrerSpeed = _stirrerSpeed;
                                //sendVesselStateState = true;
                            }
                            #endregion

                            #region Set IO's
                            // set io's
                            if (!_emergencyOn)
                            {
                                // set io's as defined
#if (!SIMULATOR)
                                _stirrerLeftPin.Write(_stirrerDirection == StirrerDirection.Left ? GpioPinValue.High:GpioPinValue.Low);
                                _stirrerRightPin.Write(_stirrerDirection == StirrerDirection.Right ? GpioPinValue.High : GpioPinValue.Low);
                                _heaterPumpPin.Write(pump ? GpioPinValue.High : GpioPinValue.Low);
                                _heater1Pin.Write(heater1On ? GpioPinValue.High : GpioPinValue.Low);
                                _heater2Pin.Write(heater2On ? GpioPinValue.High : GpioPinValue.Low);
#endif
                            }
                            else
                            {
#if (!SIMULATOR)
                                _stirrerLeftPin.Write(GpioPinValue.Low);
                                _stirrerRightPin.Write(GpioPinValue.Low);
                                _heaterPumpPin.Write(GpioPinValue.Low);
                                _heater1Pin.Write(GpioPinValue.Low);
                                _heater2Pin.Write(GpioPinValue.Low);
#endif
                            }
                            #endregion
                        }

                        //if (sendVesselStateState)
                        //{
                        #region Send Vessel State
                        var vesselValues = new VesselValues()
                        {
                            StirrerDirection = tempDirection,
                            StirrerSpeed = tempStirrerSpeed,
                            PumpOn = pump,
                            HeaterLevel1On = heater1On,
                            HeaterLevel2On = heater2On,
                            VesselTemperature = vesselTemperature,
                            MashCurrentTemperature = mashTemperature,
                            MashTargetTemperature = _targetTemperature,
                            EmergencyOn = _emergencyOn
                        };

                        VesselEvent?.Invoke(this, vesselValues);
                        #endregion
                        //sendVesselStateState = false;
                        //}
                        #endregion
                        // speep for x seconds
                        Thread.Sleep(SLEEPTIME);
                    }
                }, _cancellationToken.Token);
        }


        public void Stop()
        {
            if (_run)
            {
                //_cancellationToken.Cancel();
                _run = false;
#if (!SIMULATOR)
                _stirrerLeftPin.Write(GpioPinValue.Low);
                _stirrerRightPin.Write(GpioPinValue.Low);
                _heaterPumpPin.Write(GpioPinValue.Low);
                _heater1Pin.Write(GpioPinValue.Low);
                _heater2Pin.Write(GpioPinValue.Low);
#endif
            }
        }
        //public void Hold()
        //{
        //    if (_run)
        //    {

        //    }
        //}
        //public void Resume()
        //{
        //    if (_run)
        //    {

        //    }
        //}


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
#if (!SIMULATOR)
            _stirrerLeftPin?.Dispose();
            _stirrerRightPin?.Dispose();
            _heaterPumpPin?.Dispose();
            _heater1Pin?.Dispose();
            _heater2Pin?.Dispose();
#endif
            _cancellationToken.Cancel();
            disposed = true;
        }
        #endregion
    }
}
