using System;
using System.Threading.Tasks;
using LLMUnity;

public interface IConversationGeneration
{
    bool IsGenerating { get; }

    event Action<string> OnMessageReceived;
    event Action OnMessageReceivedCompleted;

    Task<string> StartGeneration(string message);
    void StopGeneration();
}

public class ConversationGeneration : IConversationGeneration
{
    public event Action<string> OnMessageReceived;
    public event Action OnMessageReceivedCompleted;

    public bool IsGenerating { get; private set; }

    private readonly LLMCharacter _llmCharacter = ServiceLocator.Instance.Get<LLMCharacter>();

    public async Task<string> StartGeneration(string message)
    {
        if (IsGenerating) return null;

        IsGenerating = true;

        return await _llmCharacter.Chat(message, MessageReceived, MessageReceivedComplete);
    }

    public void StopGeneration()
    {
        _llmCharacter.CancelRequests();

        IsGenerating = false;
    }

    private void MessageReceived(string message)
    {
        OnMessageReceived?.Invoke(message);
    }

    private void MessageReceivedComplete()
    {
        IsGenerating = false;
        OnMessageReceivedCompleted?.Invoke();
    }
}