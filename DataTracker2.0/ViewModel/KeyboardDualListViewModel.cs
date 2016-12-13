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

using DataTracker.Model;
using System.Collections.ObjectModel;

namespace DataTracker.ViewModel
{
    class KeyboardDualListViewModel : ViewModelBase
    {
        private ObservableCollection<KeyDefinitions> _durations;
        public ObservableCollection<KeyDefinitions> AllDurations
        {
            get { return _durations; }
            set
            {
                _durations = value;
                OnPropertyChanged("AllKeyboards");
            }
        }

        private ObservableCollection<KeyDefinitions> _frequencies;
        public ObservableCollection<KeyDefinitions> AllFrequencies
        {
            get { return _frequencies; }
            set
            {
                _frequencies = value;
                OnPropertyChanged("AllFrequencies");
            }
        }

        public KeyboardDualListViewModel()
        {
            if (_durations == null)
                _durations = new ObservableCollection<KeyDefinitions>();

            if (_frequencies == null)
                _frequencies = new ObservableCollection<KeyDefinitions>();
        }
    }
}
