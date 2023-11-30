const string secret = "";
var inputFile = "word-spec-demo.docx";
var outputFile = Path.Combine(Directory.GetCurrentDirectory(), "output.pdf");

if (string.IsNullOrEmpty(secret))
    Console.WriteLine("The secret is missing, get one for free at https://www.convertapi.com/a/auth");
else
{
    try
    {
        Console.WriteLine("Please wait, converting!");

        await using var fileStream = File.OpenRead(inputFile);
        var content = new MultipartFormDataContent
        {
            { new StreamContent(fileStream), "file", inputFile }
        };

        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/octet-stream"));

            using (var resultFile = File.OpenWrite(outputFile))
            {
                var response = await httpClient.PostAsync($"https://v2.convertapi.com/convert/docx/to/pdf?secret={secret}", content);

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
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine("An error occurred: {0}", e.Message);
    }
}