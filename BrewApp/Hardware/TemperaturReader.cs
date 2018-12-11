using BrewApp.Hardware.Driver;
using BrewApp.Logic;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace BrewApp.Hardware
{
    public class ConfigLoader
    {
        const string PATH = "TempReader.cfg";
        public enum SPIPin
        {
            PIN0 = 0,
            PIN1 = 1
        }
        public class Config
        {
            public string SPIInterfaceName { get; set; }
            public SPIPin SPIPin { get; set; }
            public byte Configuration { get; set; }
            public double Calibration { get; set; }
            public int RRef { get; set; } = 430;
        }

        public static Config GetConfiguration(string configId)
        {
            Dictionary<string, Config> configs = null;
            var path = Path.Combine(Constants.ApplicationSettingsPath, PATH);
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Constants.ApplicationSettingsPath);
                
                // save dummy

                configs = new Dictionary<string, Config>();

                var config = new Config()
                {
                    SPIInterfaceName = "SPI0",
                    Configuration = (byte)(
            (byte)Max31865.ConfigValues.VBIAS_ON |
            (byte)Max31865.ConfigValues.FOUR_WIRE |
            (byte)Max31865.ConfigValues.FILTER_50Hz),
                    SPIPin = SPIPin.PIN0

                };
                configs.Add("Config_4wires", config);
                var config2 = new Config()
                {
                    SPIInterfaceName = "SPI0",
                    Configuration = (byte)(
            (byte)Max31865.ConfigValues.VBIAS_ON |
            (byte)Max31865.ConfigValues.THREE_WIRE |
            (byte)Max31865.ConfigValues.FILTER_50Hz),
                    SPIPin = SPIPin.PIN1
                };
                configs.Add("Config_3wires", config2);

                string content = string.Empty;
                var serializer = new JsonSerializer();
                // Lots of possible configurations:
                // serializer.PreserveReferencesHandling = PreserveReferencesHandling.All; 
                // Nice for debugging:
                // content = JsonConvert.SerializeObject(instance, Formatting.Indented); 
                content = JsonConvert.SerializeObject(configs, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                });
                File.WriteAllText(path, content);
                return config2;
            }
            else
            {
                //path = Path.Combine(Constants.ApplicationSettingsPath, PATH);
                var content = File.ReadAllText(path);

                var bytes = System.Text.Encoding.Unicode.GetBytes(content);

                configs = JsonConvert.DeserializeObject<Dictionary<string, Config>>(content, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                });
                if (configs.ContainsKey(configId))
                {
                    return configs[configId];
                }
                else
                {
                    return default(Config);
                }
            }

        }
    }

    public class TemperaturReader
    {
#if (SIMULATOR)
        string _config = null;
#else
         Max31865 _driver = null;
#endif

        public TemperaturReader(string configId)
        {

#if (SIMULATOR)
            //if (!((App)App.Current).Properties.ContainsKey(_config))
            //{
            //    ((App)App.Current).Properties.Add(_config, "10.0");
            //}
            _config = configId;
#else
            var config = ConfigLoader.GetConfiguration(configId);

            _driver = new Max31865(config.Calibration, config.RRef);
            _driver.Initialize(config.SPIInterfaceName, (int)config.SPIPin, config.Configuration);
#endif
        }

        public double GetTemperature()
        {
#if (SIMULATOR)
            if (!((App)App.Current).Properties.ContainsKey(_config))
            {
                ((App)App.Current).Properties.Add(_config, "10.0");
            }

            return double.Parse(((App)App.Current).Properties[_config]);
#else

            _driver.ExecuteOneShot();
            return _driver.GetTempC();
#endif
        }
    }
}
