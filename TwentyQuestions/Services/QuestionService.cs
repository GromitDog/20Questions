using Radzen.Blazor;

namespace TwentyQuestions.Services;

public class QuestionService
{
    private readonly Dictionary<Guid, List<ChatMessage>> _messagesByGame = new();
    private readonly Dictionary<Guid, List<ChatUser>> _usersByGame = new();
    private readonly Dictionary<Guid, GameEvents> _eventsByGame = new();
    private readonly object _sync = new();

    private class GameEvents
    {
        public event Action? QuestionAsked;
        public event Action? QuestionAnswered;
        public event Action<IEnumerable<ChatUser>>? UsersChanged;

        public void InvokeQuestionAsked() => QuestionAsked?.Invoke();
        public void InvokeQuestionAnswered() => QuestionAnswered?.Invoke();
        public void InvokeUsersChanged(IEnumerable<ChatUser> users) => UsersChanged?.Invoke(users);
    }

    private GameEvents GetOrCreateEvents(Guid gameId)
    {
        lock (_sync)
        {
            if (!_eventsByGame.TryGetValue(gameId, out var events))
            {
                events = new GameEvents();
                _eventsByGame[gameId] = events;
            }
            return events;
        }
    }

    public void SubscribeQuestionAsked(Guid gameId, Action handler)
    {
        GetOrCreateEvents(gameId).QuestionAsked += handler;
    }

    public void UnsubscribeQuestionAsked(Guid gameId, Action handler)
    {
        if (_eventsByGame.TryGetValue(gameId, out var events))
        {
            events.QuestionAsked -= handler;
        }
    }

    public void SubscribeQuestionAnswered(Guid gameId, Action handler)
    {
        GetOrCreateEvents(gameId).QuestionAnswered += handler;
    }

    public void UnsubscribeQuestionAnswered(Guid gameId, Action handler)
    {
        if (_eventsByGame.TryGetValue(gameId, out var events))
        {
            events.QuestionAnswered -= handler;
        }
    }

    public void SubscribeUsersChanged(Guid gameId, Action<IEnumerable<ChatUser>> handler)
    {
        GetOrCreateEvents(gameId).UsersChanged += handler;
    }

    public void UnsubscribeUsersChanged(Guid gameId, Action<IEnumerable<ChatUser>> handler)
    {
        if (_eventsByGame.TryGetValue(gameId, out var events))
        {
            events.UsersChanged -= handler;
        }
    }

    public IReadOnlyList<ChatMessage> GetMessages(Guid gameId)
    {
        lock (_sync)
        {
            if (!_messagesByGame.TryGetValue(gameId, out var messages))
            {
                messages = new List<ChatMessage>();
                _messagesByGame[gameId] = messages;
            }
            return messages.ToList().AsReadOnly();
        }
    }

    public IEnumerable<ChatUser> GetUsers(Guid gameId)
    {
        lock (_sync)
        {
            if (!_usersByGame.TryGetValue(gameId, out var users))
            {
                users = new List<ChatUser>();
                _usersByGame[gameId] = users;
            }
            return users.ToList();
        }
    }

    public void AskQuestion(Guid gameId, ChatMessage message)
    {
        GameEvents events;
        lock (_sync)
        {
            if (!_messagesByGame.TryGetValue(gameId, out var messages))
            {
                messages = new List<ChatMessage>();
                _messagesByGame[gameId] = messages;
            }
            messages.Add(message);
            events = GetOrCreateEvents(gameId);
        }

        // Invoke handlers asynchronously to avoid blocking and to ensure callers
        // don't run component StateHasChanged on a non-dispatcher thread.
        Task.Run(() => events.InvokeQuestionAsked());
    }

    public void AnswerQuestion(Guid gameId, ChatMessage message)
    {
        GameEvents events;
        lock (_sync)
        {
            if (!_messagesByGame.TryGetValue(gameId, out var messages))
            {
                messages = new List<ChatMessage>();
                _messagesByGame[gameId] = messages;
            }
            messages.Add(message);
            events = GetOrCreateEvents(gameId);
        }

        // Invoke handlers asynchronously to avoid blocking and to ensure callers
        // don't run component StateHasChanged on a non-dispatcher thread.
        Task.Run(() => events.InvokeQuestionAnswered());
    }

    public void AddUser(Guid gameId, ChatUser user)
    {
        GameEvents events;
        IEnumerable<ChatUser> usersCopy;
        lock (_sync)
        {
            if (!_usersByGame.TryGetValue(gameId, out var users))
            {
                users = new List<ChatUser>();
                _usersByGame[gameId] = users;
            }

            var existing = users.Find(u => u.Id == user.Id);
            if (existing == null)
            {
                users.Add(user);
            }
            else
            {
                existing.Name = user.Name;
                existing.Color = user.Color;
            }

            events = GetOrCreateEvents(gameId);
            usersCopy = users.ToList();
        }

        Task.Run(() => events.InvokeUsersChanged(usersCopy));
    }
}