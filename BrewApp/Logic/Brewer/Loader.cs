using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Logic.Brewer
{
    public class Brewer
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
    public class Loader
    {
        const string PATH = "Brewers.cfg";

        public static IList<Brewer> Load()
        {
            List<Brewer> brewers = null;
            var path = Path.Combine(Constants.BrewersPath, PATH);
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Constants.BrewersPath);
                //var path = Path.Combine(Constants.BrewersPath, PATH);
                // save dummy

                brewers = new List<Brewer>();
                brewers.Add(new Brewer() { Name = "Anna", Id = "Anna.Fry" });
                brewers.Add(new Brewer() { Name = "John", Id = "John.Doe" });
                string content = string.Empty;
                var serializer = new JsonSerializer();
                // Lots of possible configurations:
                // serializer.PreserveReferencesHandling = PreserveReferencesHandling.All; 
                // Nice for debugging:
                // content = JsonConvert.SerializeObject(instance, Formatting.Indented); 
                content = JsonConvert.SerializeObject(brewers, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                });
                File.WriteAllText(path, content);
            }
            else
            {
                //var path = Path.Combine(Constants.BrewersPath, PATH);
                var content = File.ReadAllText(path);

                var bytes = Encoding.Unicode.GetBytes(content);

                brewers = JsonConvert.DeserializeObject<List<Brewer>>(content, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                });
            }
            return brewers;
        }
    }
}
