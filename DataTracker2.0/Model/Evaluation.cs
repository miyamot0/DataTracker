namespace DataTracker.Model
{
    class Evaluation
    {
        public static Evaluation CreateEvaluation(string evaluationName)
        {
            return new Evaluation { EvaluationName = evaluationName };
        }

        public string EvaluationName { get; set; }
    }
}
