using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConvertHtmlToPdf
{
    internal class Program
    {
        public static async Task Main (string[] args) {
            const string secret = "";
            var fileToSave = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "file.pdf");           

            if(string.IsNullOrEmpty(secret))
                Console.WriteLine("The secret is missing, get one for free at https://www.convertapi.com/a");
            else
            {
                try
                {
                    Console.WriteLine("Please wait, converting!");
                    var url = new Uri($"https://v2.convertapi.com/html/to/pdf?download=attachment&secret={secret}");
                    var htmlString = "<!doctype html><html lang=en><head><meta charset=utf-8><title>ConvertAPI test</title></head><body>This page is generated from HTML string.</body></html>";
                    var content = new StringContent(htmlString, Encoding.UTF8, "application/octet-stream");
                    content.Headers.Add("Content-Disposition", "attachment; filename=\"data.html\"");
                    using (var resultFile = File.OpenWrite(fileToSave))
                    {
                        var request = new HttpClient().PostAsync(url, content);
                        if (request.Result.IsSuccessStatusCode)
                        {
                            request.Result.Content.CopyToAsync(resultFile).Wait();
                            Console.WriteLine("File converted successfully");
                        }
                        else
                        {
                            Console.WriteLine("Status Code : {0}", request.Result.StatusCode);
                            Console.WriteLine("Body : {0}", await request.Result.Content.ReadAsStringAsync());
                        }
                    }
                }
                catch (WebException e)
                {

                }
            }
        }
    }
}