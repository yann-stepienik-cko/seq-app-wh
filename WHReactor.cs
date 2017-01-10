using System.IO;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Text;
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
              var values = new Dictionary<string, string>
              {
                 { "Timestamp", evt.Data.LocalTimestamp },
                 { "Id", evt.Data.Id }
                 { "Level", evt.Data.Level }
                 { "RenderedMessage", evt.Data.RenderedMessage }
              };

              if(evt.Data.Properties != null) {
                values.add("Properties", evt.Data.Properties);
              }

              if(evt.Data.Exception != null) {
                values.add("Exception", evt.Data.Exception);
              }

              var content = new FormUrlEncodedContent(values);

              var response = await client.PostAsync(URL, content);

              var responseString = await response.Content.ReadAsStringAsync();

              Console.WriteLine(responseString);
          }
        }
    }
}
