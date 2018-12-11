using BrewApp.Hardware.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace BrewApp.Hardwares
{
    public delegate void TemperatureEvent(object sender, double temperature);

    class Heater : IDisposable, IEmergency
    {
        SpiDevice _spiDevice = null;
        GpioPin _stage1Heater, _stage2Heater, _pump;
        double _temperature = 0.0;
        CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        public Heater(SpiDevice device, GpioPin stage1Heater, GpioPin stage2Heater, GpioPin pump, bool autoTuneMode = false)
        {
            _spiDevice = device;
            _stage1Heater = stage1Heater;
            _stage2Heater = stage2Heater;
            _pump = pump;
        }

        ~Heater()
        {
            Dispose(false);
        }

        public event TemperatureEvent VesselIsTemperatureChange;
        public event TemperatureEvent VesselBeTemperatureChange;
        public event TemperatureEvent HeaterTemperatureChange;
        public event TemperatureEvent TemperaturReached;

        public void SetTemperature(double temperature)
        {
            _temperature = temperature;
        }

        bool _run = false;
        public void Run()
        {
            // load PID settings

            if (!_run)
            {
                var consumerTask = Task.Factory.StartNew(
                    () =>
                    {
                        while (true)
                        {
                        }
                    }, _cancellationToken.Token);
            }
        }

        public void Stop()
        {

        }

        #region Emergency
        public void ResetEmergencyStop(object sender, DateTime time)
        {
            Run();
        }

        public void SetEmergencyStop(object sender, DateTime time)
        {
            Stop();
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

            //_stirrer?.Dispose();
            //_emergencyButton?.Dispose();
            //_heater?.Dispose();

            disposed = true;
        }
        #endregion 
    }
}
