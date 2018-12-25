using BrewApp.Hardware.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace BrewApp.Hardware.Driver
{
    public class InputBoard : IInputBoard
    {
        public GpioPin GetGpio(int pin, GpioPinDriveMode mode = GpioPinDriveMode.Input)
        {
            if (!_gpios.ContainsKey(pin))
            {
                var g = GpioController.GetDefault().OpenPin(pin);
                // Set the IO direction as input
                g.SetDriveMode(mode);
                _gpios.Add(pin, g);
            }

            return _gpios[pin];
        }
        public bool GetIO(int pin, GpioPinDriveMode mode = GpioPinDriveMode.Input)
        {
#if (!SIMULATOR)
            if (!_gpios.ContainsKey(pin))
            {
                var g = GpioController.GetDefault().OpenPin(pin);
                // Set the IO direction as input
                g.SetDriveMode(mode);
                _gpios.Add(pin, g);
            }

            return _gpios[pin].Read() == GpioPinValue.High;
#else
            if (!_gpios.ContainsKey(pin))
            {
                _gpios.Add(pin, false);
            }
            return _gpios[pin];
#endif
        }

#if (!SIMULATOR)
        Dictionary<int, GpioPin> _gpios = new Dictionary<int, GpioPin>();
#else
        Dictionary<int, bool> _gpios = new Dictionary<int, bool>();
#endif

        ~InputBoard()
        {
#if (!SIMULATOR)
            foreach (var g in _gpios.Values)
            {
                g?.Dispose();
            }
#endif
        }
    }
}
