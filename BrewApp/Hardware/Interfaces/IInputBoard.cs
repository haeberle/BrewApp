using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace BrewApp.Hardware.Interfaces
{
    public interface IInputBoard
    {
        bool GetIO(int pin, GpioPinDriveMode mode);
        GpioPin GetGpio(int pin, GpioPinDriveMode mode);
    }
}
