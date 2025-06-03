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
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using IRis.Models;
using IRis.Models.Components;
using IRis.Models.Core;
using IRis.Services;
using IRis.Views;


namespace IRis.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Simulation _simulation;

        private ISerializationService _serializer = new XmlSerializationService();

        private string? _openedFileName = null;

        private string _lastAction = " - ";

        public string? OpenedFileName
        {
            get => _openedFileName == null ? "(unsaved)" : _openedFileName;
            set => SetProperty(ref _openedFileName, value);
        }

        public string CursorPosition
        {
            get => $"({(int)_simulation.CurrentMousePos.X}, {(int)_simulation.CurrentMousePos.Y})";
        }

        public string LastAction
        {
            get => _lastAction;
            set => SetProperty(ref _lastAction, value);
        }

        private string _gridToggleText = "Grid: OFF";
        public string GridToggleText
        {
            get => _gridToggleText;
            set => SetProperty(ref _gridToggleText, value);
        }

        private string _simulationToggleText = "Simulation: OFF";

        public string SimulationToggleText
        {
            get => _simulationToggleText;
            set => SetProperty(ref _simulationToggleText, value);
        }

        public MainWindowViewModel(Simulation simulation)
        {
            // Use the CanvasService for adding/removing components
            _simulation = simulation;

            // Notify cursor pos about changes in LastMousePos
            _simulation.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Simulation.CurrentMousePos))
                {
                    // Notify that CursorPosition changed
                    OnPropertyChanged(nameof(CursorPosition));
                }
            };


            // Initialize all commands
            NewCommand = new RelayCommand(New);


            OpenCommand = new AsyncRelayCommand(Open);

            SaveCommand = new AsyncRelayCommand(Save);
            SaveAsCommand = new RelayCommand(SaveAs);
            ExitCommand = new RelayCommand(Exit);

            UndoCommand = new RelayCommand(Undo);
            RedoCommand = new RelayCommand(Redo);
            CutCommand = new RelayCommand(Cut);
            CopyCommand = new RelayCommand(Copy);
            PasteCommand = new RelayCommand(Paste);
            DeleteCommand = new RelayCommand(Delete);

            AboutCommand = new RelayCommand(About);
            AiPromptCommand = new RelayCommand(AiGenerationFromPrompt);
            AiImageCommand = new RelayCommand(AiGenerationFromImage);

            AddComponentCommand = new RelayCommand<string>(AddComponent);

            GridToggleCommand = new RelayCommand(GridToggle);
            SimulationToggleCommand = new RelayCommand(SimulationToggle);
        }

        // OPTIONS
        public ICommand GridToggleCommand { get; }

        public void GridToggle()
        {
            _simulation.SnapToGridEnabled = !_simulation.SnapToGridEnabled;
            _simulation.GridEnabled = !_simulation.GridEnabled;
            
            GridToggleText = _simulation.GridEnabled ? "Grid: ON" : "Grid: OFF";
        }

        public ICommand SimulationToggleCommand { get; }

        public void SimulationToggle()
        {
            _simulation.Simulating = !_simulation.Simulating;
            
            SimulationToggleText = _simulation.Simulating ? "Simulation: ON" : "Simulation: OFF";
        }

        // File commands
        public ICommand NewCommand { get; }

        public ICommand AiPromptCommand { get; }

        private AIGenerationWindowViewModel _currentPromptVm;

        private void AiGenerationFromPrompt()
        {
            var window = new AIGenerationWindow();
            // _currentPromptVm = new AIGenerationWindowViewModel(window);
            
            var vm = window.DataContext as AIGenerationWindowViewModel;

            vm.XmlGenerated += (xml) =>
            {
                Console.WriteLine("Event received");
                var components = _serializer.DeserializeComponentsAsync(xml);

                _simulation.DeleteAllComponents();
                _simulation.LoadComponents(components);
            };

            
            // Center it relative to main window
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Get reference to main window
            var mainWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
                ?.MainWindow;

            window.ShowDialog(mainWindow);
        }

        public ICommand AiImageCommand { get; }

        private void AiGenerationFromImage()
        {
            var window = new ImageProcessingWindow();
            // _currentPromptVm = new AIGenerationWindowViewModel(window);
            
            var vm = window.DataContext as ImageProcessingWindowViewModel;

            vm.XmlGenerated += (xml) =>
            {
                Console.WriteLine("Event received");
                var components = _serializer.DeserializeComponentsAsync(xml);

                _simulation.DeleteAllComponents();
                _simulation.LoadComponents(components);
            };

            
            // Center it relative to main window
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Get reference to main window
            var mainWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
                ?.MainWindow;

            window.ShowDialog(mainWindow);
        
        }

        private void New()
        {
        }

        public ICommand OpenCommand { get; }

        private async Task Open()
        {
            // OPEN A FILE PICKER DIALOG
            var dialog = new OpenFileDialog()
            {
                Title = "Select Circuit XML file",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "XML Files", Extensions = new List<string> { "xml" } },
                    new FileDialogFilter { Name = "All Files", Extensions = new List<string> { "*" } }
                },
                AllowMultiple = false
            };

            var result = await dialog.ShowAsync(new Window());

            // Runs if the selected path exists and is valid
            if (result != null && result.Length > 0)
            {
                OpenedFileName = result[0];
                List<Component> loadedComponents = await _serializer.DeserializeFromFileAsync(OpenedFileName);
                _simulation.LoadComponents(loadedComponents);
                Console.WriteLine("Path:" + OpenedFileName);
            }
        }

        public ICommand SaveCommand { get; }

        private async Task Save()
        {
            // IF there is no opened file, ask for a path
            // Otherwise just save to that path
            if (string.IsNullOrEmpty(_openedFileName))
            {
                var dialog = new SaveFileDialog()
                {
                    Title = "Save Circuit as XML",
                    Filters = new List<FileDialogFilter>
                    {
                        new FileDialogFilter { Name = "XML Files", Extensions = new List<string> { "xml" } },
                        new FileDialogFilter { Name = "All Files", Extensions = new List<string> { "*" } }
                    },
                    DefaultExtension = "xml",
                    InitialFileName = "circuit.xml"
                };

                OpenedFileName = await dialog.ShowAsync(new Window());
            }

            if (!string.IsNullOrEmpty(_openedFileName))
            {
                _serializer.SerializeComponents(_simulation, OpenedFileName);
                Console.WriteLine("Saved to: " + _openedFileName);
            }
        }

        public ICommand SaveAsCommand { get; }

        private void SaveAs()
        {
        }

        public ICommand ExitCommand { get; }

        private void Exit()
        {
        }

        // Edit commands
        public ICommand UndoCommand { get; }

        private void Undo()
        {
        }

        public ICommand RedoCommand { get; }

        private void Redo()
        {
        }

        public ICommand CutCommand { get; }

        private void Cut()
        {
            _simulation.CutSelected();
            LastAction = "Cut to clipboard.";
        }

        public ICommand CopyCommand { get; }

        private void Copy()
        {
            // TODO: BE CAREFUL ABOUT THIS
            _simulation.CopySelected();
            LastAction = "Copied to clipboard.";
        }

        public ICommand PasteCommand { get; }

        private void Paste()
        {
            _simulation.PasteSelected();
            LastAction = "Pasted clipboard contents.";
        }

        public ICommand DeleteCommand { get; }

        private void Delete()
        {
            _simulation.DeleteSelectedComponents();
            LastAction = "Deleted selected components.";
        }

        // Help command
        public ICommand AboutCommand { get; }

        private void About()
        {
            var aboutWindow = new AboutWindow();

            // Center it relative to main window
            aboutWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Get reference to main window
            var mainWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
                ?.MainWindow;

            aboutWindow.ShowDialog(mainWindow);
        }


        // Component command
        public ICommand AddComponentCommand { get; }

        private void AddComponent(string componentType)
        {
            Console.WriteLine($"Adding component: {componentType}");

            _simulation.PreviewCompType = componentType;
            LastAction = $"Selected Component [{componentType}]";
        }
    }
}