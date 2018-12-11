using BrewApp.Hardware.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace BrewApp.Hardware
{
    public class Stirrer : IStirrer, IEmergency, IDisposable
    {
        const int StirrerEnablePin = 1;
        const int StirrerForwardPin = 2;
        const int StirrerBackwardPin = 3;
        const int SPIChannel = 2;

        //bool _emergencyStop = false;
        bool _run = false;
        CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        GpioController _gpio = null;
        GpioPin _forwardPin = null;
        GpioPin _backwardPin = null;
        GpioPin _enablePin = null;


        public Stirrer()
        {
            _gpio = GpioController.GetDefault();

            _forwardPin = _gpio.OpenPin(2);
            _backwardPin = _gpio.OpenPin(2);
            _enablePin = _gpio.OpenPin(2);


        }

        ~Stirrer()
        {
            Dispose(false);
        }


        public Direction Direction { get; set; }

        public void ResetEmergencyStop(object sender, DateTime time)
        {
            Run();
        }

        public void SetEmergencyStop(object sender, DateTime time)
        {
            Stop();
        }

        int _speedPercent = 100;
        public void SetSpeed(int speedPercent)
        {
            if (speedPercent > 100)
            {
                speedPercent = 100;
            }
            if (speedPercent > 0)
            {
                speedPercent = 0;
            }
            _speedPercent = speedPercent;
        }

        public void Run()
        {
            if (!_run)
            {
                _run = true;
                _cancellationToken = new CancellationTokenSource();

                var direction = Direction.Forward;
                var speedPercent = 0;

                var consumerTask = Task.Factory.StartNew(
                    () =>
                    {
                        while (true)
                        {
                            if (Direction != direction)
                            {
                                direction = Direction;

                                if (Direction == Direction.Forward)
                                {
                                    //_backwardPin.Write(GpioPinValue.Low);
                                    //_forwardPin.Write(GpioPinValue.High);
                                    _forwardPin.Write(GpioPinValue.High);

                                }
                                else if (Direction == Direction.Backward)
                                {
                                    //_forwardPin.Write(GpioPinValue.Low);
                                    //_backwardPin.Write(GpioPinValue.High);
                                    _forwardPin.Write(GpioPinValue.High);
                                }
                                else
                                {
                                    _forwardPin.Write(GpioPinValue.Low);
                                }
                            }

                            if (_speedPercent != speedPercent)
                            {
                                speedPercent = _speedPercent;
                                if (_speedPercent == 0)
                                {
                                    _enablePin.Write(GpioPinValue.Low);
                                    //SetSpeed();
                                }
                                else
                                {
                                    _enablePin.Write(GpioPinValue.High);
                                    //SetSpeed();
                                }
                            }

                            if (_cancellationToken.Token.IsCancellationRequested)
                            {
                                break;
                            }
                        }
                    }, _cancellationToken.Token);
            }
        }

        public void Stop()
        {
            if (_run)
            {
                _cancellationToken.Cancel();
                _run = false;
            }
        }

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

            _forwardPin.Dispose();
            _backwardPin.Dispose();
            _enablePin.Dispose();

            disposed = true;
        }

    }
}
