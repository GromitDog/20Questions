using Microsoft.EntityFrameworkCore;
using Radzen;
using TwentyQuestions.Components;
using TwentyQuestions.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();
builder.Services.AddControllers();

builder.Services.AddScoped<GameService>();
builder.Services.AddSingleton<QuestionService>();

builder.Services.AddDbContext<TwentyQuestions.Data.GameContext>(options =>
    options.UseSqlite("Data Source=games.db"));

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
