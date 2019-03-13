using ConferenceTracker.Core;
using ConferenceTracker.Core.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace ConferenceTracker.Test
{
    public class GrabberTests
    {
        private string baseUrl = "https://2019.codefest.ru";
        private string programUrl = "https://2019.codefest.ru/program/";

        private Task<Result<HtmlDocument>> LoadDoc(string url)
        {
            return Grabber.GetHtmlDoc(url);
        }

        [Fact]
        public async Task Load_CheckValid()
        {
            var actual = await LoadDoc(programUrl);
            Assert.False(actual.IsError);
            Assert.NotNull(actual.Data);
        }


        [Fact]
        public async Task Load_CheckInValid()
        {
            var actual = await LoadDoc("metalgear?...keptyouwaiting,huh...");

            Assert.True(actual.IsError);
            Assert.Null(actual.Data);
        }



        [Fact]
        public async Task GetSections_CheckValid()
        {
            var actualDoc = await Grabber.GetHtmlDoc(programUrl);

            Assert.False(actualDoc.IsError, "Ошибка получения документа");
            Assert.NotNull(actualDoc.Data);

            var actualSections = Parser.GetBlock(actualDoc.Data);
            Assert.False(actualSections.IsError, "Ошибка получения секций");
            Assert.NotNull(actualSections.Data);
        }

        [Fact]
        public async Task GetSpeches_CheckValid()
        {
            var actualDoc = await Grabber.GetHtmlDoc(programUrl);

            Assert.False(actualDoc.IsError, "Ошибка получения документа");
            Assert.NotNull(actualDoc.Data);

            var actualSections = Parser.GetSpeaches(actualDoc.Data, baseUrl);
            Assert.False(actualSections.IsError, "Ошибка получения секций");
            Assert.NotNull(actualSections.Data);


            var json = JsonConvert.SerializeObject(actualSections.Data, Newtonsoft.Json.Formatting.Indented);
        }


        [Fact]
        public async Task GetCash_CheckValid()
        { 

            var cash = Parser.GetCash();
            Assert.NotNull(cash);
            Assert.NotEmpty(cash.Speaches);
            Assert.NotEmpty(cash.SectionsFirstDay);
            Assert.NotEmpty(cash.SectionsSecondDay);
        }
    }
}
