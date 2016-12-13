/*
    Copyright 2016 Shawn Gilroy

    This file is part of DataTracker.

    Discounting Model Selector is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 3.

    DataTracker is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with DataTracker.  If not, see <http://www.gnu.org/licenses/gpl-3.0.html>.
*/

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
