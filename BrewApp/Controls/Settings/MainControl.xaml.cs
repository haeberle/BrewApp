using BrewApp.Controls.Settings.ViewModel;
using BrewApp.Hardware;
using BrewApp.Hardware.BK500;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BrewApp.Controls.Settings
{
    public sealed partial class MainControl : UserControl
    {
        SettingsViewModel _viewModel = new SettingsViewModel();

        public MainControl()
        {
            this.InitializeComponent();
            DataContext = _viewModel;

            // Load Values
            var mashTempReaderconfig = ConfigLoader.GetConfiguration("MashTemperatureProbe");
            var vesselTempReaderconfig = ConfigLoader.GetConfiguration("VesselTemperatureProbe");

            _viewModel.MashCalibTemperature = mashTempReaderconfig.Calibration;
            _viewModel.VesselCalibTemperature = vesselTempReaderconfig.Calibration;

        }

        private async void btnSaveClick(object sender, RoutedEventArgs e)
        {

            var mashTempReaderconfig = ConfigLoader.GetConfiguration("MashTemperatureProbe");
            var vesselTempReaderconfig = ConfigLoader.GetConfiguration("VesselTemperatureProbe");

            mashTempReaderconfig.Calibration = _viewModel.MashCalibTemperature;
            vesselTempReaderconfig.Calibration = _viewModel.VesselCalibTemperature;

            ConfigLoader.SetConfiguration(mashTempReaderconfig, "MashTemperatureProbe");
            ConfigLoader.SetConfiguration(vesselTempReaderconfig, "VesselTemperatureProbe");

            ContentDialog deleteFileDialog = new ContentDialog
            {
                Title = "Neustart?",
                Content = "Die Einstellungen werden gespeichert, aber nur bei einem Neustart aktiv. Program neu starten ?",
                PrimaryButtonText = "Ok",
                CloseButtonText = "Nein"
            };

            ContentDialogResult result = await deleteFileDialog.ShowAsync();
           
            if (result == ContentDialogResult.Primary)
            {
                await CoreApplication.RequestRestartAsync("New Calibration Settings");
            }
            

            


            //var content = File.ReadAllText(path);

            //var bytes = System.Text.Encoding.Unicode.GetBytes(content);

            //configs = JsonConvert.DeserializeObject<Dictionary<string, Config>>(content, new JsonSerializerSettings
            //{
            //    TypeNameHandling = TypeNameHandling.Auto,
            //    Formatting = Formatting.Indented
            //});
            //if (configs.ContainsKey(configId))
            //{
            //    return configs[configId];
            //}
            //else
            //{
            //    return default(Config);
            //}
        }

        private void btnResetClick(object sender, RoutedEventArgs e)
        {
            var mashTempReaderconfig = ConfigLoader.GetConfiguration("MashTemperatureProbe");
            var vesselTempReaderconfig = ConfigLoader.GetConfiguration("VesselTemperatureProbe");

            _viewModel.MashCalibTemperature = mashTempReaderconfig.Calibration;
            _viewModel.VesselCalibTemperature = vesselTempReaderconfig.Calibration;
        }
    }
}
