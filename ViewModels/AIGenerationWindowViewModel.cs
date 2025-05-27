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
using IRis.Views;       // just ignore these useless uses lol

namespace IRis.ViewModels
{
    public partial class AIGenerationWindowViewModel : ViewModelBase
    {
        AIGenerationWindow promptWindow;

        [ObservableProperty]
        private String promptText;

        public AIGenerationWindowViewModel(AIGenerationWindow promptWindow)
        {

            GenerateCommand = new RelayCommand(Generate);

            this.promptWindow = promptWindow;       // get the prompt window for use in this scope
        }
        
        public ICommand GenerateCommand { get; }
        public void Generate()
        {
            Console.WriteLine("Generate {PromptText}");
            // 
            // 
            // AI Generation logic goes here
            // 
            promptWindow.Close();       // closes the AI prompt window
        }
    }
    
}