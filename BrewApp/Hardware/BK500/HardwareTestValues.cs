using BrewApp.Hardware.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Hardware.BK500
{
    public class HardwareTestValues : IValues
    {
        public bool Heater1On { get; set; }
        public bool Heater2On { get; set; }
        public bool PumpOn { get; set; }
        public StirrerDirection StirrerDirection { get; set; }
        //public int StirrerSpeed { get; set; }
    }
}
