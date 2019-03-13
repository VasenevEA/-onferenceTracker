using ConferenceTracker.Core.Models;
using HtmlAgilityPack;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ConferenceTracker.Core
{
    public class Grabber
    {

        public static async Task<Result<HtmlDocument>> GetHtmlDoc(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                WebResponse myResponse = await request.GetResponseAsync();
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.OptionFixNestedTags = true;
                htmlDoc.Load(myResponse.GetResponseStream());

                var web = new HtmlWeb();
                var doc = await web.LoadFromWebAsync(url);
                return new Result<HtmlDocument>(doc);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                return new Result<HtmlDocument>(null, true, "Ошибка запроса");
            }
            catch (Exception ex)
            {
                return new Result<HtmlDocument>(null, true, $"Что-то пошло не так: {ex.Message}");
            }
        }
    }
}
