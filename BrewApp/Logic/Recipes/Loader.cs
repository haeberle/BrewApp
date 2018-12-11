using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Logic.Recipes
{
    public class Loader
    {
        const string PATH = "Recipes.cfg";
        public static IList<Recipe> LoadRecipes()
        {
            List<Recipe> recipes = null;
            if (!Directory.Exists(Constants.RecipesPath))
            {
                Directory.CreateDirectory(Constants.RecipesPath);
                var path = Path.Combine(Constants.RecipesPath, PATH);
                // save dummy

                recipes = new List<Recipe>();

                var recipe = new Recipe()
                {
                    Name = "Test Recipe",
                    Creator = "Application",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    MashIn = new MashIn()
                    {
                        Duration = 1,
                        LiterOfWater = 300,
                        StirrerSpeed1 = 20,
                        StirrerSpeed2 = 30,
                        StirrerSpeed3 = 70,
                        Temperature = 53
                    }
                };
                recipe.Steps.Add(new Step()
                {
                    Name = "Heating up to 1",
                    Duration = 0,
                    StepType = StepType.Heating,
                    StirrerSpeed = 100,
                    Temperature = 63,
                    SortOrder = 1
                });
                recipe.Steps.Add(new Step()
                {
                    Name = "Hold 1",
                    Duration = 43,
                    StepType = StepType.HoldAuto,
                    StirrerSpeed = 40,
                    Temperature = 63,
                    SortOrder = 2
                });
                recipes.Add(recipe);

                string content = string.Empty;
                var serializer = new JsonSerializer();
                // Lots of possible configurations:
                // serializer.PreserveReferencesHandling = PreserveReferencesHandling.All; 
                // Nice for debugging:
                // content = JsonConvert.SerializeObject(instance, Formatting.Indented); 
                content = JsonConvert.SerializeObject(recipes, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                });
                File.WriteAllText(path, content);                
            }
            else
            {
                var path = Path.Combine(Constants.RecipesPath, PATH);
                var content = File.ReadAllText(path);

                var bytes = Encoding.Unicode.GetBytes(content);

                recipes = JsonConvert.DeserializeObject<List<Recipe>>(content, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                });
            }
            return recipes;
        }
    }
}
