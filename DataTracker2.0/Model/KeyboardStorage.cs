using System.Collections.ObjectModel;

namespace DataTracker.Model
{
    public class KeyboardStorage
    {
        public static KeyboardStorage CreateKeyboard(ObservableCollection<KeyDefinitions> freqKeys, ObservableCollection<KeyDefinitions> durKeys, string mName)
        {
            return new KeyboardStorage { frequencyKeys = freqKeys, durationKeys = durKeys, name = mName };
        }

        public ObservableCollection<KeyDefinitions> frequencyKeys { get; set; }
        public ObservableCollection<KeyDefinitions> durationKeys { get; set; }

        public string name { get; set; }
    }
}
