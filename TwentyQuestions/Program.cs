using Radzen;
using TwentyQuestions.Components;
using TwentyQuestions.Services;
using TwentyQuestionsConsole;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();
builder.Services.AddControllers();

builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration.GetValue<string>("BaseAddress") ?? "https://localhost:7000/")
    };
    return httpClient;
});

builder.Services.AddScoped<GameService>();
builder.Services.AddSingleton<GameRepository>();
builder.Services.AddSingleton<QuestionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAntiforgery();

app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();