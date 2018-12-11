using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Hardware.Interfaces
{
    public enum Direction
    {
        Forward,
        Stop,
        Backward
    }
    interface IStirrer: IDisposable
    {
        Direction Direction { get; set; }
        void SetSpeed(int speed);
    }
}
