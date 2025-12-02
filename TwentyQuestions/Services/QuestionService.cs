using Radzen.Blazor;

namespace TwentyQuestions.Services;

public class QuestionService
{
    private readonly List<ChatMessage> _messages = new();
    private readonly List<ChatUser> _users = new();
    private readonly object _sync = new();

    // Keep same signature to avoid breaking existing consumers
    public event Action? QuestionAsked;        
    public event Action? QuestionAnswered;
    public event Action<IEnumerable<ChatUser>>? UsersChanged;

    public IReadOnlyList<ChatMessage> GetMessages()
    {
        lock (_sync)
        {
            return _messages.ToList().AsReadOnly();
        }
    }

    public IEnumerable<ChatUser> GetUsers()
    {
        lock (_sync)
        {
            return _users.ToList();
        }
    }
    
    public void AskQuestion(ChatMessage message)
    {
        Action? handlers;
        lock (_sync)
        {
            _messages.Add(message);
            handlers = QuestionAsked; // capture delegate while holding lock
        }

        // Invoke handlers asynchronously to avoid blocking and to ensure callers
        // don't run component StateHasChanged on a non-dispatcher thread.
        if (handlers is not null)
        {
            Task.Run(() =>
            {
                handlers.Invoke();
            });
        }
    }
        
    public void AnswerQuestion(ChatMessage message)
    {
        Action? handlers;
        lock (_sync)
        {
            _messages.Add(message);
            handlers = QuestionAnswered; // capture delegate while holding lock
        }

        // Invoke handlers asynchronously to avoid blocking and to ensure callers
        // don't run component StateHasChanged on a non-dispatcher thread.
        if (handlers is not null)
        {
            Task.Run(() =>
            {
                handlers.Invoke();
            });
        }
    }
    
    public void AddUser(ChatUser user)
    {
        Action<IEnumerable<ChatUser>>? handlers;
        lock (_sync)
        {
            var existing = _users.Find(u => u.Id == user.Id);
            if (existing == null)
            {
                _users.Add(user);
            }
            else
            {
                existing.Name = user.Name;
                existing.Color = user.Color;
            }

            handlers = UsersChanged;
        }

        if (handlers is not null)
        {
            Task.Run(() =>
            {
                handlers.Invoke(_users);
            });
        }
    }
}