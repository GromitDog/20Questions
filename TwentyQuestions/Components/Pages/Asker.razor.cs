using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using TwentyQuestions.Services;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using TwentyQuestions.Services;
using TwentyQuestionsConsole;

namespace TwentyQuestions.Components.Pages;

public partial class Asker : ComponentBase, IDisposable
{
    [Inject] public required GameService GameService { get; set; }
    [Inject] public required QuestionService MessageService { get; set; }
    
    [SupplyParameterFromQuery(Name = "username")]
    public string? UserName { get; set; }
    
    private const string AskerId = "Asker";
    
    private bool _isLoading;
    private List<string> _errorMessages = [];
    private GameState? _game;
    
    private bool _waitingForAnswer; // TODO: needs to be dynamic based on state
    private bool _disposed;
    
    private List<ChatUser> Users { get; set; } = new(); 
    private List<ChatMessage> Messages { get; set; } = new(); 


    protected override async Task OnInitializedAsync()
    {
        if ( string.IsNullOrWhiteSpace(UserName))
        {
            _errorMessages.Add("User name is required to start the game.");
            return;
        }
        
        _isLoading = true;
        _errorMessages.Clear();

        try
        {
            _game = await GameService.GetCurrentGame();
            
            if (_game == null) _errorMessages.Add("No active game found. Please try again later.");
            
            Users.AddRange(MessageService.GetUsers());
            
            MessageService.UsersChanged += OnUsersChanged;
            
            var asker = new ChatUser { Id = AskerId, Name = UserName, Color = "#1976d2" }; // todo: what is this colour?
            MessageService.AddUser(asker);
           
            Messages = MessageService.GetMessages().ToList();
            MessageService.QuestionAnswered += OnQuestionAnswered;
            MessageService.QuestionAsked += OnQuestionAsked;
        }
        catch (Exception ex)
        {
            _errorMessages.Add($"Error: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }
    }
    
    private void OnQuestionAnswered()
    {
        _ = InvokeAsync(() =>
        {
            Messages = MessageService.GetMessages().ToList();
            _waitingForAnswer = false;
            StateHasChanged();
        });
    }
    
    private void OnQuestionAsked()
    {       
        _ = InvokeAsync(() =>
        {
            _waitingForAnswer = true;
            StateHasChanged();
        });
    }
    
    private void OnUsersChanged(IEnumerable<ChatUser> newUsers)
    {
        _ = InvokeAsync(() =>
        {
            Users = newUsers.ToList();
            StateHasChanged();
        });
    }
    
    private void OnQuestionSent(IEnumerable<ChatMessage> newMessages)
    {
        if (_disposed) return;
        
        _ = InvokeAsync(() =>
        {
            Messages = newMessages.ToList();
            MessageService.AskQuestion(Messages.Last());
            StateHasChanged();
        });
    }
    
    public void Dispose()
    {
        _disposed = true;
        MessageService.QuestionAnswered -= OnQuestionAnswered;
        MessageService.QuestionAsked -= OnQuestionAsked;
        MessageService.UsersChanged -= OnUsersChanged;
    }
}