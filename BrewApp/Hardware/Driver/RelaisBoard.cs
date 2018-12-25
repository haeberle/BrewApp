using BrewApp.Hardware.Interfaces;
using System.Collections.Generic;
using Windows.Devices.Gpio;

namespace BrewApp.Hardware.Driver
{
    public class RelaisBoard : IRelaisBoard
    {        
        public RelaisBoard()
        {

        }

        ~RelaisBoard()
        {
#if (!SIMULATOR)
            foreach (var g in _gpios.Values)
            {
                g?.Dispose();
            }
#endif
        }

#if (!SIMULATOR)
        Dictionary<int, GpioPin> _gpios = new Dictionary<int, GpioPin>();
#else
        Dictionary<int, bool> _gpios = new Dictionary<int, bool>();
#endif
        public void SetIO(int pin, bool on)
        {
#if (!SIMULATOR)
            if (!_gpios.ContainsKey(pin))
            {
                var g = GpioController.GetDefault().OpenPin(pin);
                // Latch HIGH value first. This ensures a default value when the pin is set as output
                g.Write(GpioPinValue.High);
                // Set the IO direction as output
                g.SetDriveMode(GpioPinDriveMode.Output);
                _gpios.Add(pin, g);
            }
            _gpios[pin].Write(on ? GpioPinValue.Low : GpioPinValue.High);
#else
            if (!_gpios.ContainsKey(pin))
            {
                _gpios.Add(pin, on);
            }
#endif
        }

        public bool GetIO(int pin)
        {
#if (!SIMULATOR)
            if (!_gpios.ContainsKey(pin))
            {
                var g = GpioController.GetDefault().OpenPin(pin);
                // Latch HIGH value first. This ensures a default value when the pin is set as output
                g.Write(GpioPinValue.High);
                // Set the IO direction as output
                g.SetDriveMode(GpioPinDriveMode.Output);
                _gpios.Add(pin, g);
            }

            return _gpios[pin].Read() == GpioPinValue.Low;
#else
            if (!_gpios.ContainsKey(pin))
            {
                _gpios.Add(pin, false);
            }
            return _gpios[pin];
#endif
        }


        public void ToggleIO(int pin)
        {
            var b = GetIO(pin);
            SetIO(pin, !b);
        }

    }
}
