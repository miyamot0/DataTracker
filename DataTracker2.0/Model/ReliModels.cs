using System.Collections.Generic;

namespace DataTracker.Model
{
    public class ReliModels
    {
        public string Name { get; set; }
        public bool AssumedReliable { get; set; }
        public bool IsReliable { get; set; }
        public int Sessions { get; set; }
        public List<double> Reliability { get; set; }
    }
}
