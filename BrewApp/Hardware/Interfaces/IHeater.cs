using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Hardware.Interfaces
{
    

    interface IHeater: IDisposable
    {        
        void SetTemperature(double temperature);
        double GetTemperature(string source);
        void Init(string jsonConfig);
    }
}
