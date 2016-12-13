using System.Windows.Input;

namespace DataTracker.Model
{
    public class KeyDefinitions
    {
        public string KeyName { get; set; }
        public string KeyDescription { get; set; }
        public int Counts { get; set; }
        public string CountString { get; set; }
        public Key KeyCode { get; set; }
        public bool isRunning { get; set; }
    }
}
