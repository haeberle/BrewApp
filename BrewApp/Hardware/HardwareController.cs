using BrewApp.Hardware.Driver;
using BrewApp.Hardware.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Hardware
{
    public class HardwareController
    {
        static HardwareController _hwController = new HardwareController();
        public static HardwareController GetDefault()
        {
            return _hwController;
        }

        IRelaisBoard _relaisBoard = new RelaisBoard();
        public IRelaisBoard GetRelaisBoard()
        {
            return _relaisBoard;
        }

        IInputBoard _inputBoard = new InputBoard();
        public IInputBoard GetInputBoard()
        {
            return _inputBoard;
        }

    }
}
