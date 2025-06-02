using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Serialization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IRis.Models;
using IRis.Models.Components;
using IRis.Models.Core;
using IRis.Services;
using IRis.Views;       // just ignore these useless uses lol

namespace IRis.ViewModels
{
    public partial class AIGenerationWindowViewModel : ViewModelBase
    {
        public event Action<string>? XmlGenerated;
        
        private IAiAnalysisService aiAnalysisService = new GptAiAnalysisService();
        
        AIGenerationWindow promptWindow;

        [ObservableProperty]
        private String promptText = "NONE";

        public AIGenerationWindowViewModel(AIGenerationWindow promptWindow)
        {
            GenerateCommand = new AsyncRelayCommand(Generate);
            
            

            this.promptWindow = promptWindow;       // get the prompt window for use in this scope
        }
        
        public ICommand GenerateCommand { get; }
        public async Task Generate()
        {
            Console.WriteLine("Generate {PromptText}");

            // Relative Path            XmlGenerated.Invoke(xml);

            string xml = await aiAnalysisService.GetSerializedCircuit(PromptText, "circuit-gen-prompt.txt");
            
            // Invoke event when Xml is done
            XmlGenerated?.Invoke(xml);
            
            Console.WriteLine("\n\nXML:\n" + xml);
       
            promptWindow.Close();     
        }
    }
    
}