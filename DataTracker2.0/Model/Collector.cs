namespace DataTracker.Model
{
    class Collector
    {
        public static Collector CreateCollector(string collectorName)
        {
            return new Collector { CollectorsName = collectorName };
        }

        public string CollectorsName { get; set; }
    }
}
