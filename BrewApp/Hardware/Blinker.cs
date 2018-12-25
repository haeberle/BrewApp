using BrewApp.Hardware.BK500;
using BrewApp.Hardware.Interfaces;
using System.Diagnostics;
using Windows.Devices.Gpio;

namespace BrewApp.Hardware
{
    public class Blinker : IBlinker
    {
        ~Blinker()
        {
//#if (!SIMULATOR)
//            _blinkPin?.Dispose();
//#endif
        }

        void InitPinAsOutPut(GpioPin pin)
        {
            // Latch HIGH value first. This ensures a default value when the pin is set as output
            pin.Write(GpioPinValue.High);

            // Set the IO direction as output
            pin.SetDriveMode(GpioPinDriveMode.Output);

        }
        System.Timers.Timer _blinkTimer = null;// new System.Timers.Timer(2000);
        public void EnableBlinker(bool enable)
        {
            
            if (enable)
            {
                if (_blinkTimer == null)
                {
                    _blinkTimer = new System.Timers.Timer(2000);
                    _blinkTimer.Elapsed += _blinkTimer_Elapsed;                    
                }
                _blinkTimer.Start();
            }
            else
            {
                _blinkTimer?.Stop();
                HardwareController.GetDefault().GetRelaisBoard().SetIO(HardwareDefinition.BlinkerOut, false);
                //_blinkPin?.Write(GpioPinValue.Low);
                //Debug.WriteLine($"Blink: {_blinkPin.Read()}");
                _toggle = false;
            }
        }
//#if (!SIMULATOR)
//        //GpioPin _blinkPin = null;
//#endif
        bool _toggle = false;
        private void _blinkTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_blinkTimer.Enabled)
            {
                if (_toggle)
                {
                    _toggle = false;
                }
                else
                {
                    _toggle = true;
                }
                //#if (!SIMULATOR)
                //if (_blinkPin == null)
                //{
                //    _blinkPin = GpioController.GetDefault().OpenPin(HardwareDefinition.BlinkerOut);
                //    InitPinAsOutPut(_blinkPin);
                //}
                HardwareController.GetDefault().GetRelaisBoard().SetIO(HardwareDefinition.BlinkerOut, _toggle);
                //_blinkPin.Write(_toggle ? GpioPinValue.High : GpioPinValue.Low);
                //Debug.WriteLine($"Blink: {_blinkPin.Read()}");
            }
//#endif
        }
    }
}
