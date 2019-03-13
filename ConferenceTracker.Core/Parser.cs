using ConferenceTracker.Core.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConferenceTracker.Core
{
    public class Parser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Result<HtmlNode> GetBlock(HtmlAgilityPack.HtmlDocument doc)
        {
            try
            {
                var sectionList = new List<Section>();
                var programBlockClass = "js-program-table-scroll";

                var block = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.Attributes["id"] != null && x.Attributes["id"].Value.Contains(programBlockClass));

                return new Result<HtmlNode>(block);
            }
            catch (Exception ex)
            {
                return new Result<HtmlNode>(null, true);
            }
        }



        public static Result<List<Section>> ParseSections(HtmlAgilityPack.HtmlNode sectionRow)
        {
            try
            {
                var sectionList = new List<Section>();
                var sectionCells = sectionRow.Descendants("div").Where(x => x.Attributes["class"] != null && x.Attributes["class"].Value == ("program__cell"));
                foreach (var sectionNode in sectionCells)
                {
                    var name = sectionNode.Descendants("div").FirstOrDefault().InnerHtml;
                    var areaName = sectionNode.Descendants("div").LastOrDefault().InnerHtml;

                    sectionList.Add(new Section
                    {
                        Name = name,
                        AreaName = areaName
                    });
                }
                return new Result<List<Section>>(sectionList);
            }
            catch (Exception ex)
            {
                return new Result<List<Section>>(null, true, ex.Message);
            }
        }

        public static Result<Speach> ParseSpeache(HtmlAgilityPack.HtmlNode speackerCell, int sectionId, Section sections, DateTime dateTime, string baseUrl)
        {

            //Check Empty
            var emptyClass = "program__message";

            var isEmpty = speackerCell.Descendants("div").FirstOrDefault(x => x.Attributes["class"] != null
            && x.Attributes["class"].Value.Contains(emptyClass)
            && string.IsNullOrEmpty(x.InnerText)) != null;

            if (isEmpty)
            {
                return new Result<Speach>(new Speach
                {
                    AreaName = sections.AreaName,
                    AreaType = sections.Name,
                    SpeachTesis = "Нет спикера",
                    SpeachStartTime = dateTime
                });
            }

            var mayBeOpening = speackerCell.Descendants("div").FirstOrDefault(x => x.Attributes["class"] != null
           && x.Attributes["class"].Value.Contains(emptyClass)
           && !string.IsNullOrEmpty(x.InnerText));
            var isOpening = mayBeOpening != null;
            if (isOpening)
            {
                return new Result<Speach>(new Speach
                {
                    AreaName = sections.AreaName,
                    AreaType = sections.Name,
                    SpeachTesis = mayBeOpening.InnerText,
                    SpeachStartTime = dateTime
                });
            }

            var speach = new Speach();
            //speach url
            var lectionRef = speackerCell.Descendants("a").FirstOrDefault(x => x.Attributes["href"] != null);
            speach.SpeachUrl = baseUrl + lectionRef.Attributes["href"].Value;
            //speakers 
            var speakers = speackerCell.Descendants("div").Where(x => x.Attributes["class"] != null && x.Attributes["class"].Value == "program__speaker");
            //every speaker
            var speakersArray = new Speaker[speakers.Count()];
            try
            {
                for (int i = 0; i < speakers.Count(); i++)
                {
                    speakersArray[i] = new Speaker();
                    //name
                    var nameNode = speakers.ElementAt(i).Descendants("div").FirstOrDefault(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("program__speaker-name"));
                    speakersArray[i].Name = nameNode.InnerText.Replace("\n                            ", "");


                    //icon
                    var iconNode = speakers.ElementAt(i).Descendants("div").FirstOrDefault(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("program__speaker-image"));
                    var icon = iconNode.Attributes["style"].Value;
                    icon = icon.Replace("background-image: url('", "");
                    speakersArray[i].FaceImageSource = baseUrl + icon.Replace("')", "");
                    //company
                    var companyNode = speakers.ElementAt(i).Descendants("div").FirstOrDefault(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("program__speaker-company"));
                    speakersArray[i].Company = companyNode.InnerText.Replace("\n                            ", "");
                }
                //tesis
                var tesisNode = speackerCell.Descendants("div").FirstOrDefault(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("program__topic"));
                speach.SpeachTesis = tesisNode.InnerText.Replace("\n                      ", "");

                speach.Speakers = speakersArray;
                speach.AreaName = sections.AreaName;
                speach.AreaType = sections.Name;
                speach.SpeachStartTime = dateTime;
            }
            catch (Exception ex)
            {

                return new Result<Speach>(null, true, ex.Message);
            }

            return new Result<Speach>(speach);
        }

        public static ConferenceData GetCash()
        {
            var assembly = typeof(Parser).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("ConferenceTracker.Core.Resources.Speakers.json");
            string text = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<ConferenceData>(text);
        }


        public static Result<ConferenceData> GetSpeaches(HtmlAgilityPack.HtmlDocument doc, string baseUrl)
        {
            try
            {
                var speachList = new List<Speach>();
                var programBlockClass = "js-program-table-scroll";


                var block = GetBlock(doc).Data;

                var allows1 = "program__";
                var allows2 = "program__time";
                var rows = block.ChildNodes.Where(x => x.Attributes.Contains("class") && x.Attributes["class"] != null && x.Attributes["class"].Value != null
                && x.Attributes["class"].Value.Contains(allows1));

                DateTime date = new DateTime(2019, 03, 28);
                var tempDate = date;
                var str = "";
                int sectionIndex = 0;

                var indexSection = 0;
                List<Section> localSections = null;

                List<Section> sectionsFirstDay = new List<Section>();
                List<Section> sectionsSecondDay = new List<Section>();
               
                var speaches = new List<Speach>();
                for (int i = 0; i < rows.Count(); i++)
                {
                    var node = rows.ElementAt(i);
                    var isDate = node.Descendants("div").FirstOrDefault(x => x.Attributes["class"] != null
                                        && x.Attributes["class"].Value.Contains("program-section__day")) != null;
                    if (isDate)
                    {
                        date = date.AddDays(1);
                        tempDate = date;
                        //Следующий индекс - это секции
                        indexSection = i + 1;
                        continue;
                    }

                    if (i == indexSection)
                    {
                        //Парсим секции
                        var sections = ParseSections(node);
                        if (localSections == null)
                        {
                            sectionsFirstDay = sections.Data;
                        }
                        else
                        {
                            sectionsSecondDay = sections.Data;
                        }
                        localSections = sections.Data;
                        str += $"Sections! {localSections.Count}\r\n";
                        foreach (var section in localSections)
                        {
                            str += $"{section.Name}\r\n";
                        }
                        continue;
                    }


                    var timeNode = node.Descendants("div").FirstOrDefault(x => x.Attributes["class"] != null
                                        && x.Attributes["class"].Value.Contains("program__time-value"));
                    var isTime = timeNode != null;
                    if (isTime)
                    {
                        var time = TimeSpan.Parse(timeNode.InnerHtml);
                        tempDate = date.Add(time);
                        str += tempDate + "\r\n";
                        continue;
                    }

                    var row = node;
                    var isRow = row.Attributes["class"] != null
                                        && row.Attributes["class"].Value.Contains("program__row");

                    if (isRow)
                    {
                        str += $"People!\r\n";

                        var cells = row.Descendants("div").Where(x => x.Attributes["class"] != null
                                       && x.Attributes["class"].Value.Contains("program__cell"));

                        for (int о = 0; о < cells.Count(); о++)
                        {
                            var speach = cells.ElementAt(о);
                            //Parse Speach
                            var speachResult = ParseSpeache(speach, о, localSections.ElementAt(о), tempDate, baseUrl);
                            if (!speachResult.IsError)
                            {
                                speaches.Add(speachResult.Data);
                            }

                            str += $"Speach! {sectionIndex}\r\n";
                        }
                    }
                }
                 

                return new Result<ConferenceData>(new ConferenceData
                {
                    Speaches = speaches.ToArray(),
                    SectionsFirstDay = sectionsFirstDay.ToArray(),
                    SectionsSecondDay = sectionsSecondDay.ToArray()
                });
            }
            catch (Exception ex)
            {
                return new Result<ConferenceData>(null, true, ex.Message);
            }
        }

       
    }
}
