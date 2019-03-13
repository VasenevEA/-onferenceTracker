using System;

namespace ConferenceTracker.Core.Models
{
    public class Speach
    {
        public Speaker[] Speakers { get; set; }
        public string AreaName { get; set; }
        public string AreaType { get; set; }
        public DateTime SpeachStartTime { get; set; }
        public string SpeachTesis { get; set; }
        public string SpeachDescription { get; set; }
        public string AboutUser { get; set; }

        public string SpeachUrl { get; set; }
    } 
}
