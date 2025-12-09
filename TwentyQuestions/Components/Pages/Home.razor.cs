using Microsoft.AspNetCore.Components;
using Radzen;
using TwentyQuestions.Data;
using TwentyQuestions.Models;
using TwentyQuestions.Services;

namespace TwentyQuestions.Components.Pages;

public partial class Home
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required GameService GameService { get; set; }

    private bool _isLoading;
    private string _userName = "";
    private Guid? _selectedGameId = null;
    private IEnumerable<Guid> _availableGames = Enumerable.Empty<Guid>();

    private bool CanJoin()
    {
        return _availableGames.Count() > 0;
    }

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        _userName = "";
        await base.OnInitializedAsync();
        GameService.GameChanged += OnGameChanged;

        _availableGames = (await GameService.GetCurrentGames()).Select(g => g.Id);
        
        _isLoading = false;
    }

    private void OnGameChanged(GameState? state)
    {
        InvokeAsync(StateHasChanged);
    }
    
    private void Start()
    {
        // redirect to Answerer page
        NavigationManager.NavigateTo($"/answerer?username={Uri.EscapeDataString(_userName)}");
    }

    
    private async Task Join()
    {
        if (_selectedGameId is null)
            return;
        try
        {
            var game = await GameService.GetGame(_selectedGameId.Value);
            if (game is null)
                throw new KeyNotFoundException("Game not found");
            
            game.JoinGame(_userName);

            // redirect to Asker page
            NavigationManager.NavigateTo($"/asker?gameId={Uri.EscapeDataString(_selectedGameId!.ToString()!)}");
        } 
        catch (Exception e)
        {
            // show error message
            var messageService = new NotificationService();
            messageService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Error",
                Detail = $"The selected game does not exist: {e.Message}",
                Duration = 4000
            });
        }
    }
}