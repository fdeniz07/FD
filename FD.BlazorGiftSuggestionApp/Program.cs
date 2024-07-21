using Blazored.Modal;
using FD.BlazorGiftSuggestionApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenAI.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var openAIApiKey = builder
    .Configuration
    .GetSection("OpenAIApiKey")
    .Value;

builder.Services.AddOpenAIService(settings => settings.ApiKey = openAIApiKey);

builder.Services.AddBlazoredModal();

await builder.Build().RunAsync();
