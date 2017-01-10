using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Seq.Apps;
using Seq.Apps.LogEvents;

namespace Seq.App.WH
{
    [SeqApp("WebHook",
        Description = "Send event to URL.")]
    public class EmailReactor : Reactor, ISubscribeTo<LogEventData>
    {

        [SeqAppSetting(
            DisplayName = "URL",
            HelpText = "Url to send event.")]
        public string URL { get; set; }

        public void On(Event<LogEventData> evt)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, object>
                {
                    { "Timestamp", evt.Data.LocalTimestamp },
                    { "Id", evt.Data.Id },
                    { "Level", evt.Data.Level },
                    { "RenderedMessage", evt.Data.RenderedMessage }
                };

                if (evt.Data.Properties != null)
                {
                    values.Add("Properties", evt.Data.Properties);
                }

                if (evt.Data.Exception != null)
                {
                    values.Add("Exception", evt.Data.Exception);
                }

                var json = JsonConvert.SerializeObject(values);
                var request = new HttpRequestMessage(HttpMethod.Post, URL);
                request.Content = new StringContent(json);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseString);
                }
            }
        }
    }
}
