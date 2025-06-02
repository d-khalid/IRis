
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using IRis.Models;
using IRis.Services;
using IRis.ViewModels;

public class ImageProcessingWindowViewModel : ViewModelBase
{
    IAiImageAnalysisService _aiImageAnalysisService = new AiImageAnalysisService(); 
    
    public event Action<string>? XmlGenerated;

    
    private bool _hasImage;
    public bool HasImage 
    {
        get => _hasImage;
        set => SetProperty(ref _hasImage, value);
    }

    private Bitmap? _selectedImage;
    public Bitmap? SelectedImage
    {
        get => _selectedImage;
        set 
        {
            if (SetProperty(ref _selectedImage, value))
            {
                HasImage = value != null;
            }
        }
    }
    
    private string? _filePath;
    public string? FilePath
    {
        get => _filePath;
        set => SetProperty(ref _filePath, value);
    }
    

    // Commands
    public ICommand SelectImageCommand { get; }
    public ICommand GenerateCommand { get; }
    
    private readonly Window _hostWindow;

    public ImageProcessingWindowViewModel(Window hostWindow)
    {
       _hostWindow = hostWindow;
        SelectImageCommand = new RelayCommand(SelectImage);
        GenerateCommand = new AsyncRelayCommand(GenerateCircuit);
    }

    private async void SelectImage()
    {
        var storageProvider = TopLevel.GetTopLevel(_hostWindow)?.StorageProvider;
        if (storageProvider == null) return;


        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Circuit Image",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Images")
                {
                    Patterns = new[] { "*.png", "*.jpg", "*.jpeg" }
                }
            }
        });

        if (files.Count == 0) return;
        
        var file = files[0];
        FilePath = file.Path.AbsolutePath;
        
        await using var stream = await file.OpenReadAsync();
        SelectedImage = new Bitmap(stream);
        
    }

    private async Task GenerateCircuit()
    {
        if (SelectedImage == null || FilePath == null) return;
        
        try
        {
            // Convert image to byte array
            await using var memoryStream = new MemoryStream();
            SelectedImage.Save(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();
            
            // Send to server
            string? xmlResponse = await _aiImageAnalysisService.GetSerializedCircuit(FilePath);

            if (xmlResponse == null)
            {
                // // Info dialog
                // await MessageBox.Show(
                //     parentWindow: this,  // Your current Window instance
                //     title: "Failure",
                //     text: "Couldn't generate XML from image!",
                //     buttons: MessageBoxButtons.Ok
                // );

                Console.WriteLine("COULDN'T GENERATE XML FROM IMAGE");

                return;
            }
            
            XmlGenerated?.Invoke(xmlResponse);
            
            _hostWindow.Close();
        }
        catch (Exception ex)
        {
            // Handle errors (show message box, etc.)
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}