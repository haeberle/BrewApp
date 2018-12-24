using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Hardware.Interfaces
{
    public interface IValues
    {

    }

    public interface IHardwareTest
    {
        void SetValues(IValues values);
    }
}
