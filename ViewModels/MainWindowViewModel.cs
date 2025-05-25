using System;
using System.Windows.Input;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using IRis.Models;
using IRis.Views;


namespace IRis.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        private readonly Simulation _simulation;
        
        
        
        private string _openedFileName = " - ";
       
        private string _lastAction = " - ";

        public string OpenedFileName
        {
            get => _openedFileName;
            set => SetProperty(ref _openedFileName, value);
        }

        public string CursorPosition
        {
            get => $"[{_simulation.CurrentMousePos.X}, {_simulation.CurrentMousePos.Y}]";
        }
        public string LastAction
        {
            get => _lastAction;
            set => SetProperty(ref _lastAction, value);
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
            OpenCommand = new RelayCommand(Open);
            SaveCommand = new RelayCommand(Save);
            SaveAsCommand = new RelayCommand(SaveAs);
            ExitCommand = new RelayCommand(Exit);

            UndoCommand = new RelayCommand(Undo);
            RedoCommand = new RelayCommand(Redo);
            CutCommand = new RelayCommand(Cut);
            CopyCommand = new RelayCommand(Copy);
            PasteCommand = new RelayCommand(Paste);
            DeleteCommand = new RelayCommand(Delete);

            AboutCommand = new RelayCommand(About);

            AddComponentCommand = new RelayCommand<string>(AddComponent);
        }

        // 

        // File commands
        public ICommand NewCommand { get; }

        private void New()
        {
        }

        public ICommand OpenCommand { get; }

        private void Open()
        {
          
        }

        public ICommand SaveCommand { get; }

        private void Save()
        {
           
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
            _simulation.StartPastePreview();
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
            var mainWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
    
            aboutWindow.ShowDialog(mainWindow);

        }

        // Component command
        public ICommand AddComponentCommand { get; }

        private void AddComponent(string componentType)
        {
            Console.WriteLine($"Adding component: {componentType}");

            _simulation.PreviewCompType = componentType;
            LastAction = $"Selected Component [{componentType}]";

            //SelectedComponent = CreateComponent(componentType);



            // switch (componentType)
            // {
            //     case "AND":
            //         _canvasService.AddComponent(new AndGate(4));
            //         break;
            //     case "OR":
            //         _canvasService.AddComponent(new OrGate(2));
            //         break;
            //     case "NOT":
            //         _canvasService.AddComponent(new NotGate());
            //         break;
            //     case "NAND":
            //         _canvasService.AddComponent(new NandGate(2));
            //         break;
            //     case "NOR":
            //         _canvasService.AddComponent(new NorGate(2));
            //         break;
            //     case "XOR":
            //         _canvasService.AddComponent(new XorGate(2));
            //         break;
            //     case "XNOR":
            //         _canvasService.AddComponent(new XnorGate(2));
            //         break;
            // }
        }

       
    }
}