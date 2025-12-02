using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Radzen;
using TwentyQuestions.Services;
using TwentyQuestionsConsole;

namespace TwentyQuestions.Components.Pages;

public partial class Home
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required GameService GameService { get; set; }

    private bool _isLoading = false;
    private string _userName = "";
    private GameState? _currentGame = null;

    private bool CanJoin()
    {
        return !string.IsNullOrWhiteSpace(_userName) && _currentGame is not null;
    }

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        _userName = "";
        await base.OnInitializedAsync();
        GameService.GameChanged += OnGameChanged;

        _currentGame = await GameService.GetCurrentGame();
        
        _isLoading = false;
    }    
    
    private void OnGameChanged(GameState? state)
    {
        if (state is null || state.IsOver)
            _currentGame = null;
        else
            _currentGame = state;
        
        InvokeAsync(StateHasChanged);
    }
    
    private void Start()
    {
        // redirect to Answerer page
        NavigationManager.NavigateTo($"/answerer?username={Uri.EscapeDataString(_userName)}");
    }

    
    private void Join()
    {
        // redirect to Answerer page
        NavigationManager.NavigateTo($"/asker?username={Uri.EscapeDataString(_userName)}");
    }
}