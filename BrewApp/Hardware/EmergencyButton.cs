using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace BrewApp.Hardware
{
    public delegate void ButtonAction(object sender, DateTime time);
    //public delegate void ButtonReleased(object sender, DateTime time);
    public class EmergencyButton : IDisposable
    {
        public event ButtonAction ButtonPressed;
        public event ButtonAction ButtonReleased;
#if (SIMULATOR)
#else
        GpioPin _buttonPin = null;
#endif
        public EmergencyButton()
        {
#if (SIMULATOR)
#else
            _buttonPin = GpioController.GetDefault().OpenPin(HardwareDefinition.EmergencyButtonIn);
            _buttonPin.ValueChanged += _buttonPin_ValueChanged;
#endif
        }
        ~EmergencyButton()
        {
            Dispose(false);
        }
        
        private void _buttonPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.FallingEdge)
            {
                ButtonPressed?.Invoke(this, DateTime.Now);
            }
            else
            {
                ButtonReleased?.Invoke(this, DateTime.Now);
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
#if (SIMULATOR)
#else
            _buttonPin.Dispose();
#endif
            disposed = true;
        }
    }
}
