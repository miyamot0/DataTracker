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

using DataTracker.DataAccess;
using DataTracker.Model;
using DataTracker.Utilities;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace DataTracker.ViewModel
{
    class CollectorListViewModel : ViewModelBase
    {
        public InterfaceSetup mInt;

        public CollectorRepository _collectorRepository;

        private string _collector;
        public string CollectorSelection
        {
            get { return _collector; }
            set
            {
                _collector = value;
                OnPropertyChanged("CollectorSelection");

                if (value != null)
                    mInt.CollectorChangeInterfaceMethod(value);
            }
        }

        private ObservableCollection<Collector> _collectorList;
        public ObservableCollection<Collector> AllCollectors
        {
            get { return _collectorList; }
            set
            {
                _collectorList = value;
                OnPropertyChanged("AllCollectors");
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CollectorListViewModel()
        {
            if (_collectorRepository == null)
            {
                _collectorRepository = new CollectorRepository();
            }

            _collectorList = _collectorRepository.GetCollectors();
            AllCollectors = _collectorRepository.GetCollectors();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        protected override void OnDispose()
        {
            AllCollectors.Clear();
        }

        /// <summary>
        /// Reparse directory
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="indivName"></param>
        public void RefreshRepository(string groupName, string indivName)
        {            
            var target = Properties.Settings.Default.SaveLocation + "\\" + groupName + "\\" + indivName + "\\DataCollectors.json";

            if (File.Exists(@target))
            {
                string mInput = File.ReadAllText(@target);
                var items = JsonConvert.DeserializeObject<Collectors>(mInput);

                AllCollectors.Clear();

                foreach (Collector item in items.DataCollectors)
                {
                    AllCollectors.Add(item);
                }
            }
            
        }
    }
}
