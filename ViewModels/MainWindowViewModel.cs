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
using CommunityToolkit.Mvvm.Input;
using IRis.Models;
using IRis.Models.Components;
using IRis.Models.Core;
using IRis.Views;


namespace IRis.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Simulation _simulation;

        private string? _openedFileName = null;

        private string _lastAction = " - ";

        public string? OpenedFileName
        {
            get => _openedFileName == null ? "NONE" : _openedFileName;
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

            AddComponentCommand = new RelayCommand<string>(AddComponent);
        }

        // 

        // File commands
        public ICommand NewCommand { get; }

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
                List<Component> components = await DeserializeComponentsAsync(OpenedFileName);
                _simulation.LoadComponents(components);


                Console.WriteLine("Path:" + OpenedFileName);
            }
        }

        public ICommand SaveCommand { get; }

        private async Task Save()
        {
            //string? savePath = await SaveFilePickerAsync(MainWindow);
            // SerializeXml("circuit.xml");

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
                SerializeXml(_openedFileName);
                Console.WriteLine("Saved to: " + _openedFileName);
            }
        }

        // TODO: SERIALIZATION/DESERIALIZATION NEEDS TO BECOME ITS OWN CLASS
        private void SerializeXml(string? filePath)
        {
            if (filePath == null)
            {
                Console.WriteLine("No file selected!");
                return;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<ComponentDto>));
            List<ComponentDto> dtoList = _simulation.Components.Select(p => p.ToDto()).ToList();


            StreamWriter writer = new StreamWriter(filePath);
            serializer.Serialize(writer, dtoList);

            writer.Close();
        }

        public static async Task<List<Component>> DeserializeComponentsAsync(string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<ComponentDto>));

            // TODO: THIS IS ERROR PRONE, CLEANLY EXCEPTION HANDLE THIS PART
            await using (var stream = File.OpenRead(filePath))
            {
                var dtos = (List<ComponentDto>)serializer.Deserialize(stream);
                return dtos.Select(dto => ConvertDtoToComponent(dto)).ToList();
            }
        }

        private static Component ConvertDtoToComponent(ComponentDto dto)
        {
            int numInputs = ParseProperty<int>(dto, "NumInputs");
            double width = ParseProperty<double>(dto, "Width");
            double height = ParseProperty<double>(dto, "Height");
            double rotation = ParseProperty<double>(dto, "Rotation");


            Component component = dto.Type switch
            {
                "AndGate" => new AndGate(numInputs),
                "OrGate" => new OrGate(numInputs),
                "NorGate" => new NorGate(numInputs),
                "NandGate" => new NandGate(numInputs),
                "XorGate" => new XorGate(numInputs),
                "XnorGate" => new XnorGate(numInputs),
                "NotGate" => new NotGate(),


                _ => throw new NotSupportedException($"Unknown component type: {dto.Type}")
            };

            // Set common properties
            Canvas.SetLeft(component, dto.X);
            Canvas.SetTop(component, dto.Y);

            component.Rotation = rotation;


            // // Set component-specific properties
            // switch (component)
            // {
            //    
            //     case Wire w:
            //         // Already handled in CreateWire
            //         break;
            // }


            return component;
        }

        private static T ParseProperty<T>(ComponentDto dto, string name)
        {
            var prop = dto.Properties.FirstOrDefault(p => p.Name == name);
            if (prop == null) return default;

            return (T)Convert.ChangeType(prop.Value, typeof(T));
        }

        // FILE PICKER/SAVE DIALOGS
        // TODO: PUT THESE IN A SEPARATE SERVCE 
     
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