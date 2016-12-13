using System;
using System.Windows.Input;

namespace DataTracker.Model
{
    public class ProgressMonitor
    {
        public class KeyEventTag
        {
        }

        public string Code { get; set; }
        public string Key { get; set; }
        public string Time { get; set; }
        public Key KeyCode { get; set; }
        public string KeyString { get; set; }
        public View.SessionWindow.KeyTags KeyTag { get; set; }
        public View.SessionWindow.ScheduleTags ScheduleTag { get; set; }
        public TimeSpan TimePressed { get; set; }

    }
}
