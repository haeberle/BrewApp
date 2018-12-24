using BrewApp.Hardware.BK500;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Hardware.Interfaces
{
     public interface IVessel
    {
        event TemperatureEvent TargetTemperaturReached;
        event VesselEvent VesselEvent;

        Task<bool> InitVessel();

        void SetTargetTemperature(double temperature);

        void Start();

        void Stop();
        
    }
}
