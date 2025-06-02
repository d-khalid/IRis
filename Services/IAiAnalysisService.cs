using System.Threading.Tasks;

namespace IRis.Services;

public interface IAiPromptAnalysisService
{
    public Task<string> GetSerializedCircuit(string prompt, string systemPromptPath);
}

public interface IAiImageAnalysisService
{
    public Task<string?> GetSerializedCircuit(string imagePath);

}