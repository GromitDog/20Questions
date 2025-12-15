using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using TwentyQuestions.Models;
using TwentyQuestions.Services;

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
        if (string.IsNullOrWhiteSpace(UserName))
        {
            _errorMessages.Add("User name is required to start the game.");
            return;
        }

        _isLoading = false;
        _errorMessages.Clear();
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
            Messages = MessageService.GetMessages(_game.Id).ToList();
            _waitingForQuestion = false;
            StateHasChanged();
        });
    }
    
    private async Task OnAnswerGiven(Answer answer, string ? customAnswerContent = null)
    {
        if (_disposed || _game is null) return;

        await GameService.AnswerQuestion(_game.Id, answer);
        
        var newMessage = new ChatMessage()
        {
            UserId = AnswererId,
            IsUser = true
        };
        
        switch (answer)
        {
            case Answer.Yes:
                newMessage.Content = "Yes";
                break;
            case Answer.No:
                newMessage.Content = "No";
                break;
            case Answer.DontKnow:
                newMessage.Content = "I don't know";
                break;
        }
        
        if (!string.IsNullOrWhiteSpace(customAnswerContent))
        {
            newMessage.Content = customAnswerContent;
        }
        
        _ = InvokeAsync(() =>
        {
            MessageService.AnswerQuestion(_game.Id, newMessage);
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
        if (string.IsNullOrWhiteSpace(UserName))
        {
            _errorMessages.Add("User name is required to start the game.");
            return;
        }

        _isLoading = true;
        _errorMessages.Clear();

        try
        {
            _game = await GameService.StartGame(UserName, _characterName);

            // Now that we have a game, set up the chat for this game
            Users.AddRange(MessageService.GetUsers(_game.Id));

            MessageService.SubscribeUsersChanged(_game.Id, OnUsersChanged);

            var answer = new ChatUser { Id = AnswererId, Name = UserName, Color = "#1976d2" }; // todo: what is this colour?
            MessageService.AddUser(_game.Id, answer);

            Messages = MessageService.GetMessages(_game.Id).ToList();
            MessageService.SubscribeQuestionAnswered(_game.Id, OnQuestionAnswered);
            MessageService.SubscribeQuestionAsked(_game.Id, OnQuestionAsked);
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

    private async Task AnswerYes()
    {
        await OnAnswerGiven(Answer.Yes);
    }
    
    private async Task AnswerNo()
    {
        await OnAnswerGiven(Answer.No);
    }
    
    private async Task AnswerDontKnow()
    {
        await OnAnswerGiven(Answer.DontKnow);
    }
        
    private async Task AskerWonGame()
    {
        if (_disposed || _game is null) return;
        
        await GameService.WinGame(_game.Id);
        _game.WinGame();
        
        await OnAnswerGiven(Answer.Yes, "Yes, you won!");
    }
    
    private async Task AbandonGame()
    {
        if (_disposed || _game is null) return;
        
        await GameService.AbandonGame(_game.Id);
        _game.GiveUp();
    }
    
    public void Dispose()
    {
        _disposed = true;
        if (_game is not null)
        {
            MessageService.UnsubscribeQuestionAnswered(_game.Id, OnQuestionAnswered);
            MessageService.UnsubscribeQuestionAsked(_game.Id, OnQuestionAsked);
            MessageService.UnsubscribeUsersChanged(_game.Id, OnUsersChanged);
        }
    }
}