using System;
using System.ClientModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace IRis.Services;

public class GptAiAnalysisService : IAiAnalysisService
{
    public async Task<string> GetSerializedCircuit(string prompt, string systemPromptPath)
    {
        string systemPrompt;
        
        using (StreamReader reader = new StreamReader(systemPromptPath))
        {
            systemPrompt = await reader.ReadToEndAsync();
        }
        
        // Get the API keys from a file
        string json = await File.ReadAllTextAsync("config.json");
        var config = JsonSerializer.Deserialize<OpenAiConfig>(json);

        Console.WriteLine($"ENDPOINT: {config.Endpoint}\n" +
                          $"KEY: {config.Key}\n");

        var openAIClient = new AzureOpenAIClient(
            new Uri(config.Endpoint),
            new ApiKeyCredential(config.Key));


        ChatClient chatClient = openAIClient.GetChatClient("gpt-4.1");
        
  
        ChatCompletion completion = chatClient.CompleteChat(
        [
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(prompt),
        ]);

        //Console.WriteLine($"{completion.Role}: {completion.Content[0].Text}");
        
        return completion.Content[0].Text;
    }
}

public class OpenAiConfig
{
    public string Endpoint { get; set; }
    public string Key { get; set; }
}