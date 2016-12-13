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

using System;
using System.Diagnostics;

namespace DataTracker.Model
{
    public class DurationModels
    { 
        public bool Recording { get; set; }
        public Stopwatch Timer { get; set; }
        public TimeSpan TimeSpanOutput { get; set; }
        public string PastTime { get; set; }
        public int ActiveIntervals = 0;
        public int TotalIntervals = 0;
        public bool WasActive = false;
        public bool WasObserved = false;
    }
}
