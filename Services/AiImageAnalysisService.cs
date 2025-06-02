using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace IRis.Services;

public class AiImageAnalysisService : IAiImageAnalysisService
{
    // Returns null if the xml could not be retrieved
    public async Task<string?> GetSerializedCircuit(string imagePath)
    {
        try
        {
            // Read server URL from text file
            string url = await File.ReadAllTextAsync("server-link.txt");
            url = url.Trim(); // Remove any whitespace/newlines
            
            // Ensure URL ends with /process
            if (!url.EndsWith("/process"))
            {
                url = url.TrimEnd('/') + "/process";
            }
            
            using (var httpClient = new HttpClient())
            {
                // Specify the single file to upload
                // THIS NEEDS TO BE GIVEN HERE
                // string imagePath = "circuits/image.jpg"; // Change this to your actual file path
                
                if (!File.Exists(imagePath))
                {
                    Console.WriteLine($"File '{imagePath}' not found!");
                    return null;
                }
                
                // Create multipart form data
                using (var form = new MultipartFormDataContent())
                {
                    // Read file and add to form
                    byte[] fileBytes = await File.ReadAllBytesAsync(imagePath);
                    var fileContent = new ByteArrayContent(fileBytes);
                    
                    // Add file to form with the name "image"
                    string fileName = Path.GetFileName(imagePath);
                    form.Add(fileContent, "image", fileName);
                    
                    // Send POST request
                    var response = await httpClient.PostAsync(url, form);
                    string responseText = await response.Content.ReadAsStringAsync();
                    
                    Console.WriteLine($"Response: {responseText}");

                    return responseText;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }
}