using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Hardware.Interfaces
{
    public interface IRelaisBoard
    {
        void SetIO(int pin, bool on);
        bool GetIO(int pin);
        void ToggleIO(int pin);
    }
}
