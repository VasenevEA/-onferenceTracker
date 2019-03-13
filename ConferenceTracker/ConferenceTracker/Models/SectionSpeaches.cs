using ConferenceTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceTracker.Models
{
    public class SectionItem
    {
        public string SectionName { get; set; }
        public string SectionArea { get; set; }

        public Speach[] Speachs { get; set; }
    }
}
