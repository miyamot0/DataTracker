namespace DataTracker.Model
{
    class Condition
    {
        public static Condition CreateCondition(string evaluationName)
        {
            return new Condition { ConditionName = evaluationName };
        }

        public string ConditionName { get; set; }
    }
}
