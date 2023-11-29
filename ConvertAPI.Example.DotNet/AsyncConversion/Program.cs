using System.Net;
using System.Net.Http.Headers;
using System.Web;

const string secret = "";

const string inputFile = "SampleDOCFile_1000kb.docx";
var outputFile = Path.Combine(Path.GetTempPath(), "output.pdf");

var fileConverter = new FileConverter(new HttpClient(), secret);
var jobId = await fileConverter.SubmitFileForConversion(inputFile);

Console.WriteLine($"The Word document has been sent for conversion. The task is identified by the Async ID: {jobId}. Now, we are awaiting the result.");

//This code can be shifted to a task to prevent blocking the execution flow of the program.
var httpStatusCode = HttpStatusCode.Accepted;
while (httpStatusCode == HttpStatusCode.Accepted)
{
    httpStatusCode = await fileConverter.WaitForConversion(jobId, outputFile);
    if (httpStatusCode == HttpStatusCode.Accepted)
    {
        await Task.Delay(5000);   
    }
}


Console.WriteLine("File converted.");
Console.ReadLine();

public class FileConverter
{
    private readonly string _secret;
    private readonly HttpClient _client;

    public FileConverter(HttpClient client, string secret)
    {
        _secret = secret;
        _client = client;
    }

    public async Task<string> SubmitFileForConversion(string inputFile)
    {
        if (string.IsNullOrEmpty(_secret))
            throw new ArgumentException("The secret is missing, get one for free at https://www.convertapi.com/a/auth", nameof(_secret));

        await using var fs = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
        var convertBuilder = new UriBuilder("https://v2.convertapi.com/async/convert/docx/to/pdf");
        var convertQuery = HttpUtility.ParseQueryString(convertBuilder.Query);
        convertQuery["Secret"] = _secret;
        convertBuilder.Query = convertQuery.ToString();

        var streamContent = new StreamContent(fs);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
            FileName = Path.GetFileName(inputFile)
        };

        var convertResponse = await _client.PostAsync(convertBuilder.Uri, streamContent);

        if (convertResponse.StatusCode != HttpStatusCode.OK) throw new Exception("An error occurred in the response.");

        var responseBody = await convertResponse.Content.ReadAsStringAsync();
        var asyncResponse = System.Text.Json.JsonSerializer.Deserialize<AsyncResponse>(responseBody);
        return asyncResponse.JobId;
    }

    public async Task<HttpStatusCode> WaitForConversion(string jobId, string outputFile)
    {
        var resultResponse = await _client.GetAsync($"https://v2.convertapi.com/async/job/{jobId}");

        switch (resultResponse.StatusCode)
        {
            case HttpStatusCode.OK:
                await SaveConvertedFile(resultResponse, outputFile);
                break;

            case HttpStatusCode.Accepted:
                Console.WriteLine("File conversion is in progress...");
                break;

            default:
                Console.WriteLine("An error occurred");
                var message = await resultResponse.Content.ReadAsStringAsync();
                Console.WriteLine(message);
                break;
        }

        return resultResponse.StatusCode;
    }

    private async Task SaveConvertedFile(HttpResponseMessage response, string outputFile)
    {
        var resultFile = await response.Content.ReadAsStreamAsync();

        await using var outputFileStream = new FileStream(outputFile, FileMode.Create);
        await resultFile.CopyToAsync(outputFileStream);
        Console.WriteLine($"File conversion completed. The file saved to {outputFile}");
    }
}

public class AsyncResponse
{
    public string JobId { get; set; }
}