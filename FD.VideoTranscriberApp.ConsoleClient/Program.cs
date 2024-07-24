using System.Resources;
using Xabe.FFmpeg;

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