var outputFile = Path.Combine(Directory.GetCurrentDirectory(), "output.pdf");
const string secret = "";

if (string.IsNullOrEmpty(secret))
    Console.WriteLine("The secret is missing, get one for free at https://www.convertapi.com/a/auth");
else
    try
    {
        Console.WriteLine("Please wait, converting!");
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/octet-stream"));
            
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("url", "https://www.convertapi.com"),
                new KeyValuePair<string, string>("Footer", "<style>.right{float:right;}.left{float:left;}</style><span class='left'>page number <span class='pageNumber'></span></span><span class='right'>date <span class='date'></span></span>"),
                new KeyValuePair<string, string>("CookieConsentBlock", "true"),
            });
            
            var result = await client.PostAsync("https://v2.convertapi.com/web/to/pdf?Secret=" + secret, content);

            if (result.IsSuccessStatusCode)
            {
                var resultFile = await result.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(outputFile, resultFile);
                Console.WriteLine("File converted successfully " + outputFile);
            }
            else
            {
                Console.WriteLine("Status Code : {0}", result.StatusCode);
                Console.WriteLine("Reason Phrase : {0}", result.ReasonPhrase);
                Console.WriteLine("Body : {0}", await result.Content.ReadAsStringAsync());
            }
        }
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine("Exception Caught!");
        Console.WriteLine("Message : {0} ", e.Message);
    }