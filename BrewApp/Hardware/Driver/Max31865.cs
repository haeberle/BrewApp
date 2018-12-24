using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;
using Windows.System.Threading;

namespace BrewApp.Hardware.Driver
{
    /// <summary>
    /// Based on a MAX31865 module for Microsoft .NET Gadgeteer
    /// https://csharp.hotexamples.com/site/file?hash=0x3fb08f4bcc801c35ff37614640b32db98aabe2dae64f4e81d42a9a070bb81b16&fullName=Gadgeteer+Driver/MAX31865/MAX31865/MAX31865/MAX31865_43.cs&project=ianlee74/RTD01-Module
    /// </summary>
    public class Max31865 : IDisposable
    {
        #region Constants
        // The value of the Rref resistor. Use 430.0 for PT100 and 4300.0 for PT1000
        const int RREF = 430;
        // The 'nominal' 0-degrees-C resistance of the sensor
        // 100.0 for PT100, 1000.0 for PT1000
        const int RNOMINAL = 100;

        const double RTD_A = 3.9083e-3;
        const double RTD_B = -5.775e-7;
        #endregion
        #region Fields

        SpiDevice _max31865 = null;
        int _rRef = RREF;
        double _calibration = 0.0;

        //private GT.Timer FaultScanner;
        private byte _config;
        //private GTI.DigitalOutput _csPin;
        private bool _initialized;
        //private GTI.InterruptInput _irqPin;
        //private GT.Socket _socket;
        //private GTI.Spi _spi;
        //private GTI.SpiConfiguration _spiConfig;

        #endregion Fields

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
        }

        bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            //Stop();
            //_max31865?.Dispose();
            disposed = true;
        }
        #endregion

        #region Constructors
        public Max31865(double calibration = 0.0, int rRef = RREF)
        {
            _rRef = rRef;
            _calibration = calibration;
        }

        ~Max31865()
        {
            Dispose(false);
        }

        #endregion Constructors

        #region Enumerations

        public enum ConfigSettings
        {
            VBIAS = 0x80,
            CONV_MODE = 0x40,
            ONE_SHOT = 0x20,
            WIRE_TYPE = 0x10,
            FLT_DETECT = 0x0C,
            FAULT_CLR = 0x02,
            FILTER = 0x01
        }

        ///<Summary>
        /// Config Bits
        ///</Summary>
        public enum ConfigValues
        {
            VBIAS_ON = 0x80,
            VBIAS_OFF = 0x00,
            CONV_MODE_AUTO = 0x40,
            CONV_MODE_OFF = 0x00,
            ONE_SHOT_ON = 0x20,
            ONE_SHOT_OFF = 0x00,
            THREE_WIRE = 0x10,
            FLT_DETECT_AUTO_DLY = 0x04,
            FLT_DETECT_RUN_MAN = 0x08,
            FLT_DETECT_FINISH_MAN = 0x0C,
            TWO_WIRE = 0x00,
            FOUR_WIRE = 0x00,
            FAULT_CLR = 0x02,
            FILTER_50Hz = 0x01,
            FILTER_60Hz = 0x00
        }

        public enum Register
        {
            CONFIG = 0x00,
            RTD_MSB = 0x01,
            RTD_LSB = 0x02,
            HI_FLT_THRESH_MSB = 0x03,
            HI_FLT_THRESH_LSB = 0x04,
            LO_FLT_THRESH_MSB = 0x05,
            LO_FLT_THRESH_LSB = 0x06,
            FLT_STATUS = 0x07
        }

        enum Command
        {
            READ = 0x00,
            WRITE = 0x80
        }

        enum FaultBits
        {
            RTD_HI_THRESH = 0x80,
            RTD_LO_THRESH = 0x40,
            REF_IN_HI = 0x20,
            FORCE_OPEN_REFIN = 0x10,
            FORCE_OPEN_RTDIN = 0x08,
            UNDERVOLT = 0x04
        }

        #endregion Enumerations

        #region Delegates

        public delegate void DataReadyEventHandler(Max31865 sender, double Data);

        public delegate void FaultEventHandler(Max31865 sender, byte DataByte);

        #endregion Delegates

        #region Events

        public event DataReadyEventHandler DataReadyCelEvent;

        public event DataReadyEventHandler DataReadyFarEvent;

        public event FaultEventHandler FaultEvent;

        #endregion Events

        #region Methods

        /// <summary>
        /// Clear faults
        /// After reset, it sets the mode back to previous state (Auto/Manual)
        /// </summary>
        public void ClearFaults()
        {
            Debug.Print("Clear Faults");
            byte OldValue = GetRegister(0x00);
            byte NewValue = (byte)((OldValue & 0xD3) | 0x02); //Everything by D5,D3 and D2...plus the falut clear bit
            Debug.Print("Clear Faults: Old:" + OldValue.ToString("X") + " New:" + NewValue.ToString("X"));
            SetRegister(0x00, NewValue);
            if ((OldValue & 0x40) > 0)
            {
                SetConvToAuto();
            }
        }

        ThreadPoolTimer _tempTimer = null;
        bool _tempTimerRun = false;
        public void StartReading(int interval)
        {
            //_tempTimer = new Timer(tempTimerCallback, null, (int)TimeSpan.FromMilliseconds(intervall).TotalMilliseconds, Timeout.Infinite);
            if (_tempTimerRun) return;
            _tempTimerRun = true;
            SetConvToAuto();
            _tempTimer = ThreadPoolTimer.CreatePeriodicTimer((t) =>
            {
                DataReadyFarEvent?.Invoke(this, GetTempF());
                DataReadyCelEvent?.Invoke(this, GetTempC());

            }, TimeSpan.FromMilliseconds(interval));

        }

        public void StopReading()
        {
            _tempTimer.Cancel();
            _tempTimerRun = false;
            SetConvToManual();
        }

        public void DisableFaultScanner()
        {
            _faultTimer.Cancel();
            _faultTimerRun = false;
        }

        //CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        bool _faultTimerRun = false;
        ThreadPoolTimer _faultTimer = null;
        public void EnableFaultScanner(int interval)
        {
            if (_faultTimerRun) return;
            _faultTimerRun = true;
            _faultTimer = ThreadPoolTimer.CreatePeriodicTimer((t) =>
            {
                RunAutoFltScan();

            }, TimeSpan.FromMilliseconds(interval));
        }

        /// <summary>
        ///   Executes a command (for details see module datasheet)
        /// </summary>
        /// <param name = "command">Command</param>
        /// <param name = "address">Register to write to</param>
        /// <param name = "data">Data to write</param>
        /// <returns>Response byte array. First byte is the status register</returns>
        public byte[] Execute(byte command, byte address, byte[] data)
        {
            CheckIsInitialized();

            //_csPin.Write(false);

            // Create SPI Buffers with Size of Data + 1 (For Command)
            //var writeBuffer = new byte[data.Length + 1];
            var writeBuffer = new byte[data == null? 1 : data.Length + 1];
            var readBuffer = new byte[data == null ? 1 : data.Length];

            // Add command and address to SPI buffer
            writeBuffer[0] = (byte)(command | address);

            // Add data to SPI buffer
            if (data != null)
            {
                Array.Copy(data, 0, writeBuffer, 1, data.Length);
            }

            // Do SPI Read/Write
            _max31865.TransferSequential(writeBuffer, readBuffer);
            //_spi.WriteRead(writeBuffer, readBuffer);

            //_csPin.Write(true);

            // Return ReadBuffer
            return readBuffer;
        }

        /// <summary>
        /// Reads temperature once
        /// </summary>
        public bool ExecuteOneShot()
        {
            //Make sure we are not running a fault scan
            if ((GetRegister(0x00) & 0x0C) == 0)
            {
                byte OldValue = (byte)GetRegister(0x00);
                byte NewValue = (byte)((byte)ConfigSettings.ONE_SHOT | OldValue);
                Debug.Print("One Shot: Old:" + OldValue.ToString("X") + " New:" + NewValue.ToString("X"));
                SetRegister(0x00, (byte)NewValue);
                return true;
            }
            return false;
        }        

        /// <summary>
        ///   Get an entire Register
        /// </summary>
        /// <param name = "register">Register to read</param>
        /// <returns>Response byte. Register value</returns>
        public byte GetRegister(byte register)
        {
            CheckIsInitialized();
            var read = Execute((byte)Command.READ, register, null);
            //var result = new byte[read.Length - 1];
            //Array.Copy(read, 1, result, 0, result.Length);
            return read[0];
        }

        public double GetTempC()
        {
            //(Celc Hi - Celc Low)/(Raw Hi - Raw Lo)
            //double ConvFactor = (200.0 - (-250.0)) / (15901.0 - 1517.0);
            //((RawVal - Raw Lo) * ConvFactor) - Celc Lo)
            int rawTemp = GetTempRaw();
            rawTemp >>= 1;
            var temp = GetTemperature(rawTemp);

            Debug.Print("\nRaw Temp : " + rawTemp + "Temp : " + temp + "\n");
            //double EngVal = (((temp >> 1) - 1517.0) * ConvFactor) - 200.0;
            //return EngVal;
            return temp;
        }

        public double GetTempF()
        {
            //Convert C to F
            return (GetTempC() * (9 / 5)) + 32;
        }

        public long GetTempRawOld()
        {
            //Shift MSB to the left 8 bits)
            long RTDVala = (long)(GetRegister(0x01) << 8);
            long RTDValb = (long)(GetRegister(0x02));
            if ((GetRegister(0x02) & 0x01) > 0)
            {
                if (FaultEvent != null)
                {
                    FaultEvent(this, GetRegister((byte)Register.FLT_STATUS));
                }
            }
            //FaultEvent(this, );
            //Merge bytes
            return (RTDVala | RTDValb);
        }
        public int GetTempRaw()
        {
            //Shift MSB to the left 8 bits)
            int RTDVala = (int)(GetRegister(0x01) << 8);
            int RTDValb = (int)(GetRegister(0x02));
            if ((GetRegister(0x02) & 0x01) > 0)
            {
                if (FaultEvent != null)
                {
                    FaultEvent(this, GetRegister((byte)Register.FLT_STATUS));
                }
            }
            //FaultEvent(this, );
            //Merge bytes
            return (RTDVala | RTDValb);
        }

        //public int GetTempRaw()
        //{
        //    byte[] ret = new byte[] { GetRegister(0x01), GetRegister(0x02) };

        //    if ((GetRegister(0x02) & 0x01) > 0)
        //    {
        //        if (FaultEvent != null)
        //        {
        //            FaultEvent(this, GetRegister((byte)Register.FLT_STATUS));
        //        }
        //    }

        //    int rtd = BitConverter.ToInt16(ret, 0);
        //    return rtd;
        //}

        public double GetTemperature(int rtd)
        {
            double Z1, Z2, Z3, Z4, temp;

            double Rt = rtd;
            Rt /= 32768;
            Rt *= _rRef;

            Z1 = -RTD_A;
            Z2 = RTD_A * RTD_A - (4 * RTD_B);
            Z3 = (4 * RTD_B) / RNOMINAL;
            Z4 = 2 * RTD_B;

            temp = Z2 + (Z3 * Rt);
            var t1 = Sqrt(temp);
            temp = (t1 + Z1) / Z4;

            if (temp >= 0) return temp - _calibration;

            // ugh.
            Rt /= RNOMINAL;
            Rt *= 100;      // normalize to 100 ohm

            double rpoly = Rt;

            temp = -242.02;
            temp += 2.2228 * rpoly;
            rpoly *= Rt;  // square
            temp += 2.5859e-3 * rpoly;
            rpoly *= Rt;  // ^3
            temp -= 4.8260e-6 * rpoly;
            rpoly *= Rt;  // ^4
            temp -= 2.8183e-8 * rpoly;
            rpoly *= Rt;  // ^5
            temp += 1.5243e-10 * rpoly;

            return temp - _calibration;
        }

        double Sqrt(double number)
        {
            double root = number / 3;
            int i;
            for (i = 0; i < 32; i++)
                root = (root + number / root) / 2;

            return root;
        }

        /// <summary>
        ///   Initializes SPI connection and control pins
        ///   <param name="irqPin"> IRQ pin as a Socket.Pin
        ///   <param name="cePin"> Chip Enable(CE) pin as a Socket.Pin
        ///   <param name="irqPin"> Chip Select Not(CSN or CS\) pin as a Socket.Pin
        ///   <param name="spiClockRateKHZ"> Clock rate in KHz (i.e. 1000 = 1MHz)
        /// </summary>
        public async Task<bool> Initialize(string spiSelector, int csLine, byte config, int spiClockRateKHZ = 500000)
        {
            Debug.WriteLine($"Start init Max31865 SPI {spiSelector} Pin {csLine} Config {config}");
            var settings = new SpiConnectionSettings(csLine);
            // Set clock to 5MHz 
            settings.ClockFrequency = spiClockRateKHZ;
            settings.Mode = SpiMode.Mode1;
            settings.SharingMode = SpiSharingMode.Shared;

            Debug.WriteLine("GetSelector");
            // Get a selector string that will return our wanted SPI controller
            string aqs = SpiDevice.GetDeviceSelector(spiSelector);
            Debug.WriteLine($"Selector {aqs}");

            Debug.WriteLine("Get Controller");
            // Find the SPI bus controller devices with our selector string
            var dis = await DeviceInformation.FindAllAsync(aqs).AsTask();
            //dis.Wait();
            Debug.WriteLine($"Controller loaded {dis[0].Id}, create SPI Devce");
            // Create an SpiDevice with our selected bus controller and Spi settings
            _max31865 = await SpiDevice.FromIdAsync(dis[0].Id, settings).AsTask();
           
            Debug.WriteLine($"Device loaded");
            //_max31865 = tmp.Result;

            _initialized = true;

            _config = config;

            ResetConfig();
            Debug.WriteLine($"Init done");
            return true;
        }

        /// <summary>
        /// Reset config back to original value
        /// </summary>
        public void ResetConfig()
        {
            Debug.Print("Reset Config: From:" + GetRegister(0x00).ToString("X") + " To:" + _config.ToString("X"));
            SetRegister(0x00, _config);
        }

        /// <summary>
        /// Run an automatic fault scan
        /// </summary>
        public void RunAutoFltScan()
        {
            byte OldValue = GetRegister(0x00);
            //Write 100x010x by keeping existing values for ...x...x and adding 0x84
            byte NewValue = (byte)((OldValue & 0x11) | 0x84); //Everything by D5,D3 and D2...plus the falut clear bit
            Debug.Print("Run Fault Scan: Old:" + OldValue.ToString("X") + " New:" + NewValue.ToString("X"));
            SetRegister(0x00, NewValue);
            while ((GetRegister(0x00) & 0x0C) > 0)
            {
                ;
            }

            byte FaultByte = GetRegister((byte)Register.FLT_STATUS);
            if (FaultByte > 0)
            {
                if (FaultEvent != null)
                {
                    FaultEvent(this, FaultByte);
                }
            }
        }

        /// <summary>
        /// Put system into auto "Normally On" Mode
        /// Will read temperature at the filter frequency
        /// </summary>
        public bool SetConvToAuto()
        {
            //Make sure we are not running a fault scan
            if ((GetRegister(0x00) & 0x0C) == 0)
            {
                byte OldValue = (byte)GetRegister(0x00);
                byte NewValue = (byte)((~(byte)ConfigSettings.CONV_MODE & OldValue) | (byte)ConfigValues.CONV_MODE_AUTO | (byte)ConfigValues.VBIAS_ON);
                Debug.Print("Set Auto: Old:" + OldValue.ToString("X") + " New:" + NewValue.ToString("X"));
                SetRegister(0x00, (byte)NewValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Put system into manual "Normally Off" Mode
        /// Requires a One Shot command to get temperature
        /// </summary>
        public void SetConvToManual()
        {
            byte OldValue = (byte)GetRegister(0x00);
            byte NewValue = (byte)((~(byte)ConfigSettings.CONV_MODE & OldValue) | (byte)ConfigValues.CONV_MODE_OFF);
            Debug.Print("Set Manual: Old:" + OldValue.ToString("X") + " New:" + NewValue.ToString("X"));
            SetRegister(0x00, (byte)NewValue);
        }

        /// <summary>
        ///   Write an entire Register
        /// </summary>
        /// <param name = "register">Register to write to</param>
        /// <param name = "value">Value to be set</param>
        /// <returns>Response byte. Register value after write</returns>
        public byte SetRegister(byte register, byte value)
        {
            CheckIsInitialized();
            Execute((byte)Command.WRITE, register, new byte[] { value });
            var x = GetRegister(register);
            return value;// GetRegister(register);
        }

        /// <summary>
        ///   Executes a command
        /// </summary>
        /// <param name = "command">Command</param>
        /// <param name = "address">Register to write to</param>
        /// <param name = "data">Data to write</param>
        /// <returns>Response byte array. First byte is the status register</returns>
        public byte[] WriteBlock(byte command, byte address, byte[] data)
        {
            CheckIsInitialized();

            //_csPin.Write(false);

            // Create SPI Buffers with Size of Data + 1 (For Command)
            var writeBuffer = new byte[data.Length + 1];
            var readBuffer = new byte[data.Length + 1];

            // Add command and address to SPI buffer
            writeBuffer[0] = (byte)(command | address);

            // Add data to SPI buffer
            Array.Copy(data, 0, writeBuffer, 1, data.Length);

            // Do SPI Read/Write
            _max31865.TransferSequential(writeBuffer, readBuffer);

            //_csPin.Write(true);

            // Return ReadBuffer
            return readBuffer;
        }

        public void WriteConfigBit(Max31865.ConfigSettings Setting, Max31865.ConfigValues Value)
        {
            byte OldValue = (byte)GetRegister(0x00);
            byte NewValue = (byte)((~(byte)Setting & OldValue) | (byte)Value);
            Debug.Print("Set Config Bit: Old:" + OldValue.ToString("X") + " New:" + NewValue.ToString("X"));
            SetRegister(0x00, (byte)NewValue);
        }

        private void CheckIsInitialized()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Initialize method needs to be called before this call");
            }
        }

        //void FaultScanner_Tick(Timer timer)
        //{
        //    RunAutoFltScan();
        //}

        //void _irqPin_Interrupt(GTI.InterruptInput sender, bool value)
        //{
        //    if (DataReadyFarEvent != null)
        //        DataReadyFarEvent(this, GetTempF());
        //    if (DataReadyCelEvent != null)
        //        DataReadyCelEvent(this, GetTempC());
        //}

        #endregion Methods
    }
}

