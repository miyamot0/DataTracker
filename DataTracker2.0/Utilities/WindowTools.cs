using System.Windows;

namespace DataTracker.Utilities
{
    class WindowTools
    {
        public static Window GetWindowRef(string WindowName)
        {
            Window retVal = null;
            foreach (Window window in Application.Current.Windows)
            {
                // The window's Name is set in XAML. See comment below
                if (window.Name.Trim().ToLower() == WindowName.Trim().ToLower())
                {
                    retVal = window;
                    break;
                }
            }

            return retVal;
        }
    }
}
