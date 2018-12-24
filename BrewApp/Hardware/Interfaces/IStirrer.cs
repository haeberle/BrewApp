using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Hardware.Interfaces
{
    public enum StirrerDirection
    {
        None,
        Left,
        Right
    }
    interface IStirrer
    {
        void SetStirrer(StirrerDirection direction);
        void SetStirrerSpeed(int speed);
    }
}
