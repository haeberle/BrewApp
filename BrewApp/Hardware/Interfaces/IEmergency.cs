﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Hardware.Interfaces
{
    public interface IEmergency
    {
        void SetEmergencyStop(object sender, DateTime time);
        void ResetEmergencyStop(object sender, DateTime time);
    }
}
