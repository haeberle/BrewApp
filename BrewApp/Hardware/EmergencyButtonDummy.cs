using BrewApp.Hardware.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Hardware
{
    class EmergencyButtonDummy : IEmergencyButton
    {
        public event ButtonAction ButtonPressed;
        public event ButtonAction ButtonReleased;
    }
}
