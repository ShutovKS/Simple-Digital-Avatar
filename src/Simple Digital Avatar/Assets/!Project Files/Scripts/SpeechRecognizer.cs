using System.Threading.Tasks;
using Whisper;

public interface ISpeechRecognizer
{
    Task<string> GetTextAsync(float[] samples);
}

public class SpeechRecognizer : ISpeechRecognizer
{
    private readonly WhisperManager _whisperManager = ServiceLocator.Instance.Get<WhisperManager>();

    public async Task<string> GetTextAsync(float[] samples)
    {
        var whisperResult = await _whisperManager.GetTextAsync(samples, 16000, 1);
        return whisperResult.Result;
    }
}