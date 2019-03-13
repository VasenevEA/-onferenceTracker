using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceTracker.Core.Models
{
    public class ConferenceData
    {
        public Speach[] Speaches { get; set; }
        public Section[] SectionsFirstDay { get; set; }
        public Section[] SectionsSecondDay { get; set; }
    }
}
