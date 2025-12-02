using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using TwentyQuestions.Services;
using TwentyQuestionsConsole;

namespace TwentyQuestions.Components.Pages;

public partial class Answerer : ComponentBase, IDisposable
{    
    [Inject] public required GameService GameService { get; set; }
    [Inject] public required QuestionService MessageService { get; set; }
    
    [SupplyParameterFromQuery(Name = "UserName")]
    public string? UserName { get; set; }
    
    private bool GameStarted => _game is not null;
    
    private const string AnswererId = "Answerer";
    private string _characterName = string.Empty;
    private bool _isLoading;
    private List<string> _errorMessages = new();
    private GameState? _game;
    private bool _waitingForQuestion; // TODO: needs to be dynamic based on state
    private bool _disposed;
    
    
    private List<ChatUser> Users  { get; set; } = new(); 
    private List<ChatMessage> Messages { get; set; } = new(); 

    private bool CanStart => !string.IsNullOrWhiteSpace(_characterName) && 
                             !_isLoading;

    
    protected override void OnInitialized()
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
            Users.AddRange(MessageService.GetUsers());
            
            MessageService.UsersChanged += OnUsersChanged;
            
            var answer = new ChatUser { Id = AnswererId, Name = UserName, Color = "#1976d2" }; // todo: what is this colour?
            MessageService.AddUser(answer);
           
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
            _waitingForQuestion = true;
            StateHasChanged();
        });
    }
    
    private void OnQuestionAsked()
    {       
        _ = InvokeAsync(() =>
        {
            Messages = MessageService.GetMessages().ToList();
            _waitingForQuestion = false;
            StateHasChanged();
        });
    }
    
    private void OnAnswerGiven(ChatMessage newMessage)
    {
        if (_disposed) return;
        
        _ = InvokeAsync(() =>
        {
            MessageService.AnswerQuestion(newMessage);
            Messages.Add(newMessage);
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
    
    private async Task StartGame()
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
             _game = await GameService.StartGame(UserName, _characterName);
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

    private void AnswerYes()
    {
        OnAnswerGiven(new ChatMessage()
        {
            Content = "Yes",
            UserId = AnswererId,
            IsUser = true
        });
    }
    
    private void AnswerNo()
    {
        OnAnswerGiven(new ChatMessage()
        {
            Content = "No",
            UserId = AnswererId,
            IsUser = true
        });
    }
    
    private void AnswerDontKnow()
    {
        OnAnswerGiven(new ChatMessage()
        {
            Content = "I don't know",
            UserId = AnswererId,
            IsUser = true
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