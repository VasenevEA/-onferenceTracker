using ConferenceTracker.Core;
using ConferenceTracker.Core.Models;
using ConferenceTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConferenceTracker
{
    public class ConferenceManager
    {
        public string BaseUrl { get; set; }

        public ConferenceManager(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        ConferenceData conferenceData;

        public Task<Result<ConferenceData>> GetCashData()
        {
            return Task.Run(() =>
             {
                 try
                 {
                     conferenceData = Parser.GetCash();
                     return new Result<ConferenceData>(conferenceData);
                 }
                 catch (Exception ex)
                 {
                     return new Result<ConferenceData>(null, true, ex.Message);
                 }
             });
        }


        public Task<Result<List<SectionItem>>> GetSectionItems()
        {
            return Task.Run(() =>
            {
                try
                {
                    if (conferenceData == null)
                    {
                        conferenceData = Parser.GetCash();
                    }

                    //Separate by sectionName (aka type of speach)
                    List<Section> data = new List<Section>();
                    data.AddRange(conferenceData.SectionsFirstDay);
                    data.AddRange(conferenceData.SectionsSecondDay);

                    var allSectionNames = RemoveDuplicates(data);
                    var list = new List<SectionItem>();
                    foreach (var item in allSectionNames)
                    {
                        var allSpeachByTypeName = conferenceData.Speaches.Where(x => x.AreaType == item.Name);
                        var sectionItem = new SectionItem()
                        {
                            SectionArea = item.AreaName,
                            SectionName = item.Name,
                            Speachs = allSpeachByTypeName.ToArray()
                        };
                        list.Add(sectionItem);
                    }

                    return new Result<List<SectionItem>>(list);
                }
                catch (Exception ex)
                {
                    return new Result<List<SectionItem>>(null, true, ex.Message);
                }
            });
        }


        public static List<Section> RemoveDuplicates(List<Section> sections)
        {
            List<Section> tempList = new List<Section>();

            foreach (var section in sections)
            {
                if (!tempList.Exists(x => x.Name == section.Name))
                {
                    tempList.Add(section);
                }
            }
            return tempList;
        }

        public static IEnumerable<DateTime> RemoveDuplicates(IEnumerable<DateTime> times)
        {
            List<DateTime> tempList = new List<DateTime>();

            foreach (var time in times)
            {
                if (!tempList.Exists(x => x == time))
                {
                    tempList.Add(time);
                }
            }
            return tempList;
        }
    }
}
