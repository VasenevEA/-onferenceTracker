using ConferenceTracker.Core.Models;
using ConferenceTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ConferenceTracker.ViewModels
{
    public class SectionViewModel : BaseViewModel
    {
        public string SectionName { get; set; }
        public string SectionArea { get; set; }

        public ObservableCollection<Speach> Speaches { get; set; }= new ObservableCollection<Speach>();


        public SectionViewModel(SectionItem speaches)
        {
            SectionName = speaches.SectionName;
            SectionArea = speaches.SectionArea;
            Speaches = new ObservableCollection<Speach>(speaches.Speachs);
        }
    }
}
