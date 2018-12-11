using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Logic.Recipes
{
    public class Recipe
    {
        public string Name { get; set; }
        public string Creator { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public MashIn MashIn { get; set; }
        public List<Step> Steps { get; private set; } = new List<Step>();
    }
}
