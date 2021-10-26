using System;
using System.IO;
using System.Net;

namespace AsyncConversionWithPushNotification
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // start web server
            var webserver = new MyWebServer();
            webserver.WebhookReceived += ProcessWebhook;
            // set your secret key here
            const string secret = "";

            if (string.IsNullOrEmpty(secret))
                Console.WriteLine("The secret is missing, get one for free at https://www.convertapi.com/a");
            else
                try
                {
                    Console.WriteLine("Please wait, converting!");
                    using (var client = new WebClient())
                    {
                        // send file for conversion
                        client.QueryString.Add("file",
                            "https://github.com/Baltsoft/CDN/raw/master/cara/testfiles/document-small.docx");
                        client.QueryString.Add("Secret", secret);
                        client.QueryString.Add("Async", "true");
                        client.QueryString.Add("StoreFile", "true");
                        client.QueryString.Add("WebHook", "https://Async-Conversions.convertapi.repl.co");

                        client.UploadValues(new Uri("http://v2.convertapi.com/docx/to/pdf"), "POST",
                            client.QueryString);

                        Console.WriteLine("Waiting for callback...");
                    }
                }
                catch (WebException e)
                {
                    HandleException(e);
                }
        }

        private static void PollResultRequest(string jobId)
        {
            try
            {
                var url = "http://v2.convertapi.com/job/" + jobId;
                Console.WriteLine("Polling the file result...");
                using (var client = new WebClient())
                {
                    var result = client.DownloadString(new Uri(url));
                    Console.WriteLine("Conversion Result: " + result);
                }
            }
            catch (WebException e)
            {
                HandleException(e);
            }
        }

        private static void ProcessWebhook(string request)
        {
            var iStartPos = 0;
            iStartPos = request.IndexOf("HTTP", 1, StringComparison.Ordinal);
            var sRequest = request.Substring(0, iStartPos - 1);
            //Extract the requested job ID
            iStartPos = sRequest.LastIndexOf("JobId=", StringComparison.Ordinal) + 6;
            var jobId = sRequest.Substring(iStartPos);
            if (!string.IsNullOrEmpty(jobId))
            {
                Console.WriteLine("Webhook push notification received");
                PollResultRequest(jobId);
            }
        }

        private static void HandleException(WebException e)
        {
            Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
            Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
            Console.WriteLine("Body : {0}", new StreamReader(e.Response.GetResponseStream()).ReadToEnd());
        }
    }
}