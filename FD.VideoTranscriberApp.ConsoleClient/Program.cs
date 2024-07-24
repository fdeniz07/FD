using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using System;
using System.Resources;
using Xabe.FFmpeg;
using OpenAI.Managers;
using OpenAI;

var videoFilePath = "C:\\Videos\\Cat.mp4";

//Get audio file path
//string audioFilePath = Path.ChangeExtension(Path.GetTempFileName(), "mp3");
string audioFilePath = Path.ChangeExtension(videoFilePath, "mp3");

CancellationTokenSource cts = new CancellationTokenSource();

//CancelationToken --> Asenkron islemlerinde mutlaka kullanilmasi gerekir.

var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(videoFilePath, audioFilePath);

await conversion.Start(cts.Token);

Console.WriteLine("Conversion completed.");
Console.WriteLine(audioFilePath);

const string fileName = "Cat.mp4";
var sampleFile = await File.ReadAllBytesAsync(audioFilePath);
//var sampleFile = await FileExtensions.ReadAllBytesAsync($"SampleData/{fileName}");


var openAiService = new OpenAIService(new OpenAiOptions()
{
    ApiKey = File.ReadAllText("C:\\OpenAIApiKey.txt")
    //ApiKey = Environment.GetEnvironmentVariable("MY_OPEN_AI_API_KEY")
});


var audioResult = await openAiService.Audio.CreateTranscription(new AudioCreateTranscriptionRequest
{
    FileName = fileName,
    File = sampleFile,
    Model = Models.WhisperV1,
    ResponseFormat = StaticValues.AudioStatics.ResponseFormat.Srt
});

var transcription = audioResult.Text;

Console.WriteLine(transcription);


await File.WriteAllTextAsync("C:\\Users\\PRODOC\\Desktop\\WhisperResponse.srt", transcription, cts.Token);

var language = "Turkish";

var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
{
    Messages = new List<ChatMessage>
    {
        ChatMessage.FromSystem(
            $"You are a helpful translator from English to {language}. You've worked as a translator your whole life and are very good at it. Don't include any extra explanations in your responses!"),
        ChatMessage.FromUser(
            $"Could you please translate the text given below to {language}. The text is a \".srt\" file. Therefore, do not change the time values! If I like your suggestions, I'll tip you $5000 for your help.\n{transcription}"),
    },
    Model = Models.Gpt_4o,
});

if (completionResult.Successful)
{
    var turkishTranscription = completionResult.Choices.First().Message.Content;

    Console.WriteLine(turkishTranscription);

    await File.WriteAllTextAsync("C:\\Users\\PRODOC\\Desktop\\WhisperResponse.tr-TR.srt", turkishTranscription, cts.Token);
}



//if (audioResult.Successful)
//{
//    //Console.WriteLine(string.Join("\n", audioResult.Text));
//    Console.WriteLine(string.Join("\n", audioResult.Text));
//    await File.WriteAllTextAsync("C:\\WhisperResponse.srt", audioResult.Text, cts.Token);
//}
//else
//{
//    if (audioResult.Error == null)
//    {
//        throw new Exception("Unknown Error");
//    }
//    Console.WriteLine($"{audioResult.Error.Code}: {audioResult.Error.Message}");
//}

