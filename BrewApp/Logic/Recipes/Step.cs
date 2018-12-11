namespace BrewApp.Logic.Recipes
{
    public enum StepType
    {
        Heating,        
        HoldMaunally,
        HoldAuto,
    }

    public class Step
    {
        public string Name { get; set; }
        public StepType StepType { get; set; }
        public int Duration { get; set; }
        public double Temperature { get; set; }
        public int StirrerSpeed { get; set; }
        public int SortOrder { get; set; }
    }
}
