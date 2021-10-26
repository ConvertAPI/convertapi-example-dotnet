using System;
using System.IO;
using System.Net;

namespace ConvertUrlToPdf
{
    internal class Program
    {
        public static void Main (string[] args) {
            const string fileToSave = "test.pdf";           
            const string secret = "";

            if (string.IsNullOrEmpty(secret))
                Console.WriteLine("The secret is missing, get one for free at https://www.convertapi.com/a");
            else
                try
                {
                    Console.WriteLine("Please wait, converting!");
                    using (var client = new WebClient())
                    {
                        client.QueryString.Add("url", "http://example.com/");
                        client.QueryString.Add("SetPageNo", "false");
                        client.QueryString.Add("FooterTextLeft", "My custom footer");
                        client.Headers.Add("accept", "application/octet-stream");
                        var resultFile = client.UploadValues(new Uri("http://v2.convertapi.com/url/to/pdf?Secret=" + secret), "POST", client.QueryString); 
                        File.WriteAllBytes(fileToSave, resultFile );
                        Console.WriteLine("File converted successfully");
                    }
                }
                catch (WebException e)
                {
                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    Console.WriteLine("Body : {0}", new StreamReader(e.Response.GetResponseStream()).ReadToEnd());
                }
        }
    }
}