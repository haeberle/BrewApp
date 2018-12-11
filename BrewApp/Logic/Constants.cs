using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Logic
{
    public class Constants
    {
        public const string APPSETTINGS = "AppSettings";
        public static string ApplicationSettingsPath
        {
            get
            {
                return Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, APPSETTINGS);
            }
        }

        public const string APPLOGS = "AppLogs";
        public static string ApplicationLogPath
        {
            get
            {
                return Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, APPLOGS);
            }
        }

        public const string RECIPES = "Recipes";
        public static string RecipesPath
        {
            get
            {
                return Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, RECIPES);
            }
        }

        public const string BREWERS = "Brewers";
        public static string BrewersPath
        {
            get
            {
                return Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, BREWERS);
            }
        }
    }
}
