namespace DataTracker.Model
{
    class Individual
    {
        public static Individual CreateIndividual(string individualName)
        {
            return new Individual { IndividualName = individualName };
        }

        public string IndividualName { get; set; }
    }
}
