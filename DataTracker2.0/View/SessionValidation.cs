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

using System.Globalization;
using System.Windows.Controls;

namespace DataTracker.View
{
    /// <summary>
    /// Validation rule for use in XAML
    /// </summary>
    class SessionValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int session;
            
            if(int.TryParse(value.ToString(), out session))
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, "Value is not a number");
            }
        }
    }
}
