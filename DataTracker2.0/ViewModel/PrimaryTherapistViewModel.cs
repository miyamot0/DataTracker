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
using System.Windows.Input;

namespace DataTracker.ViewModel
{
    class PrimaryTherapistViewModel : ViewModelBase
    {
        public TherapistRepository _therapistRepository;
        private ObservableCollection<Therapist> _therapistList;

        string _therapist;

        public InterfaceSetup mInt;

        /// <summary>
        /// Selected therapist
        /// </summary>
        public string TherapistSelection
        {
            get { return _therapist; }

            set
            {
                _therapist = value;
                OnPropertyChanged("TherapistSelection");

                if (value != null)
                    mInt.TherapistChangeInterfaceMethod(value);
            }
        }

        /// <summary>
        /// Collection of therapists
        /// </summary>
        public ObservableCollection<Therapist> AllTherapists
        {
            get { return _therapistList; }
            set
            {
                _therapistList = value;
                OnPropertyChanged("AllTherapists");
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PrimaryTherapistViewModel()
        {
            if (_therapistRepository == null)
            {
                _therapistRepository = new TherapistRepository();
            }

            _therapistList = _therapistRepository.GetTherapists();
            AllTherapists = _therapistRepository.GetTherapists();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        protected override void OnDispose()
        {
            this.AllTherapists.Clear();
        }

        /// <summary>
        /// Parse JSON file if exists
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="indivName"></param>
        public void RefreshRepository(string groupName, string indivName)
        {

            var target = Properties.Settings.Default.SaveLocation + "\\" + groupName + "\\" + indivName + "\\Therapists.json";

            if (File.Exists(@target))
            {
                string mInput = File.ReadAllText(@target);
                Therapists items = JsonConvert.DeserializeObject<Therapists>(mInput);

                AllTherapists.Clear();

                foreach (Therapist item in items.PrimaryTherapists)
                {
                    AllTherapists.Add(item);
                }
            }

        }
    }
}
