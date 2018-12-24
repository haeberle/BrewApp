using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Hardware.Interfaces
{
    public delegate void ButtonAction(object sender, DateTime time);
    public interface IEmergencyButton
    {
        event ButtonAction ButtonPressed;
        event ButtonAction ButtonReleased;
    }
}
