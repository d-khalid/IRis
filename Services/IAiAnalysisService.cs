using System.Threading.Tasks;

namespace IRis.Services;

public interface IAiAnalysisService
{
    public Task<string> GetSerializedCircuit(string prompt, string systemPromptPath);
}