using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Hardware.Interfaces
{

    public delegate void Alert(object sender, string description);

    public interface IAlert
    {
        event Alert Alert;
    }
}
