using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Hardware.PID
{
    class ArduinoPID
    {
        #region Constructor
        ArduinoPID(double input, double output, double setpoint,
            double Kp, double Ki, double Kd, int POn, int ControllerDirection)

        {

            myOutput = Output;

            myInput = Input;

            mySetpoint = Setpoint;

            inAuto = false;



            PID::SetOutputLimits(0, 255);               //default output limit corresponds to

            //the arduino pwm limits



            SampleTime = 100;                           //default Controller Sample Time is 0.1 seconds



            PID::SetControllerDirection(ControllerDirection);

            PID::SetTunings(Kp, Ki, Kd, POn);



            lastTime = millis() - SampleTime;

        }
    }
}
