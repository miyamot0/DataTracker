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

namespace DataTracker.Model
{
    /// <summary>
    /// Retaining class for saved state
    /// </summary>
    public class SavedState
    {
        public string Group { get; set; }
        public string Individual { get; set; }
        public string Evaluation { get; set; }
        public string Condition { get; set; }
        public string KeySet { get; set; }
        public string Therapist { get; set; }
        public string DataCollector { get; set; }
        public string Role { get; set; }
        public string Duration { get; set; }

        public SavedState() { }
    }
}
