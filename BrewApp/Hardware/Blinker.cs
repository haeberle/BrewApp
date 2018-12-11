using BrewApp.Hardware.Interfaces;
using Windows.Devices.Gpio;

namespace BrewApp.Hardware
{
    public class Blinker : IBlinker
    {
        ~Blinker()
        {
#if (!SIMULATOR)
            _blinkPin?.Dispose();
#endif
        }
        System.Timers.Timer _blinkTimer = null;// new System.Timers.Timer(2000);
        public void EnbableBlinker(bool enable)
        {
            
            if (enable)
            {
                if (_blinkTimer == null)
                {
                    _blinkTimer = new System.Timers.Timer(2000);
                    _blinkTimer.Elapsed += _blinkTimer_Elapsed;
                    _blinkTimer.Start();
                }
            }
            else
            {
                _blinkTimer?.Stop();
            }
        }
#if (!SIMULATOR)
        GpioPin _blinkPin = null;
#endif
        bool _toggle = false;
        private void _blinkTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_toggle)
            {
                _toggle = false;
            }
            else
            {
                _toggle = true;
            }
#if (!SIMULATOR)
            if (_blinkPin == null)
            {
                _blinkPin = GpioController.GetDefault().OpenPin(HardwareDefinition.BlinkerOut);
            }
            _blinkPin.Write(_toggle ? GpioPinValue.High : GpioPinValue.Low);
#endif
        }
    }
}
