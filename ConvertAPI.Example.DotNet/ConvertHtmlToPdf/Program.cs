using System.Text;

namespace ConvertHtmlToPdf
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            const string secret = "";
            var outputFile = Path.Combine(Directory.GetCurrentDirectory(), "output.pdf");

            if (string.IsNullOrEmpty(secret))
                Console.WriteLine("The secret is missing, get one for free at https://www.convertapi.com/a/auth");
            else
            {
                try
                {
                    Console.WriteLine("Please wait, converting!");
                    const string htmlString = "<!doctype html><html lang=en><head><meta charset=utf-8><title>ConvertAPI test</title></head><body>This page is generated from HTML string.</body></html>";
                    var content = new StringContent(htmlString, Encoding.UTF8, "application/octet-stream");
                    content.Headers.Add("Content-Disposition", "attachment; filename=\"data.html\"");

                    using (var httpClient = new HttpClient())
                    using (var resultFile = File.OpenWrite(outputFile))
                    {
                        var response = await httpClient.PostAsync($"https://v2.convertapi.com/html/to/pdf?download=attachment&secret={secret}", content);

                        if (response.IsSuccessStatusCode)
                        {
                            await response.Content.CopyToAsync(resultFile);
                            Console.WriteLine("File converted successfully " + outputFile);
                        }
                        else
                        {
                            Console.WriteLine($"Status Code : {response.StatusCode}");
                            Console.WriteLine($"Body : {await response.Content.ReadAsStringAsync()}");
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("An error occurred: {0}", e.Message);
                }
            }
        }
    }
}