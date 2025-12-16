using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using TwentyQuestions.Services;
using TwentyQuestions.Models;

namespace TwentyQuestions.Components.Pages;

public partial class Asker : ComponentBase, IDisposable
{
    [Inject] public required GameService GameService { get; set; }
    [Inject] public required QuestionService MessageService { get; set; }
    
    [SupplyParameterFromQuery(Name = "gameId")]
    public Guid GameId { get; set; }
    
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
        if (string.IsNullOrWhiteSpace(GameId.ToString()))
        {
            _errorMessages.Add("GameId is required to join a game.");
            return;
        }
        
        _isLoading = true;
        _errorMessages.Clear();

        try
        {
            _game = await GameService.GetGame(GameId);

            if (_game == null)
            {
                _errorMessages.Add($"No game with ID {GameId} found. Please try again.");
                return;
            }

            var userName = _game.AskerUserName;

            Users.AddRange(MessageService.GetUsers(GameId));

            MessageService.SubscribeUsersChanged(GameId, OnUsersChanged);

            var asker = new ChatUser { Id = AskerId, Name = userName, Color = "#1976d2" }; // todo: what is this colour?
            MessageService.AddUser(GameId, asker);

            Messages = MessageService.GetMessages(GameId).ToList();
            MessageService.SubscribeQuestionAnswered(GameId, OnQuestionAnswered);
            MessageService.SubscribeQuestionAsked(GameId, OnQuestionAsked);
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
            Messages = MessageService.GetMessages(GameId).ToList();
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
    
    private async Task OnQuestionSent(IEnumerable<ChatMessage> newMessages)
    {
        if (_disposed || _game is null) return;
        
        var messages = newMessages.ToList();
        
        await GameService.AskQuestion(_game.Id, messages.Last().Content);
            
        _ = InvokeAsync(() =>
        {
            MessageService.AskQuestion(_game.Id, messages.Last());
            StateHasChanged();
        });
    }
    
    private async Task GiveUp()
    {
        if (_disposed || _game is null) return;
        
        await GameService.AbandonGame(_game.Id);
        _game.GiveUp();
        
        // Post a message to the chat to provide visual feedback
        var giveUpMessage = new ChatMessage()
        {
            UserId = AskerId,
            IsUser = true,
            Content = "I give up!"
        };
        
        _ = InvokeAsync(() =>
        {
            MessageService.AskQuestion(_game.Id, giveUpMessage);
            Messages.Add(giveUpMessage);
            StateHasChanged();
        });
    }
    
    public void Dispose()
    {
        _disposed = true;
        MessageService.UnsubscribeQuestionAnswered(GameId, OnQuestionAnswered);
        MessageService.UnsubscribeQuestionAsked(GameId, OnQuestionAsked);
        MessageService.UnsubscribeUsersChanged(GameId, OnUsersChanged);
    }
}