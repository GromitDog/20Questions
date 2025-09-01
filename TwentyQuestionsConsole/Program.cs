using System.Text;
using TwentyQuestionsConsole;
using dotenv.net;
using Microsoft.Extensions.Hosting;
using OpenAI.Chat;
using OpenAI.Responses;

DotEnv.Load();


string provider = "openai";
string model = "gpt-5";
for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "--provider" && i + 1 < args.Length)
        provider = args[i + 1].ToLower();
    if (args[i] == "--model" && i + 1 < args.Length)
        model = args[i + 1];
}

var builder = Host.CreateApplicationBuilder(args);
Startup.ConfigureServices(builder, provider, model);
var host = builder.Build();


while (true)
{
    await QuestionMaster.Play20Questions(host.Services);
    Console.WriteLine("Play again? (y/n)");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input) || input.ToLower() != "y") break;  
}