using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Logic.Recipes
{
    public class MashIn
    {
        public int LiterOfWater { get; set; }
        public double Temperature { get; set; }        
        public int Duration { get; set; }
        public int StirrerSpeed1 { get; set; }
        public int StirrerSpeed2 { get; set; }
        public int StirrerSpeed3 { get; set; }
    }
}
