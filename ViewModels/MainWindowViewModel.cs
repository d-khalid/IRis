using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using IRis.Models;
using IRis.Services;


namespace IRis.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly CanvasService _canvasService;


        private Component _selectedComponent;

        public Component SelectedComponent
        {
            get => _selectedComponent;
            set
            {
                SetProperty(ref _selectedComponent, value);
                // Start preview when component is selected
                _canvasService.StartPreview(value);
            }
        }

        public MainWindowViewModel(CanvasService canvasService)
        {
            // Use the CanvasService for adding/removing components
            _canvasService = canvasService;

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
        }

        public ICommand CopyCommand { get; }

        private void Copy()
        {
        }

        public ICommand PasteCommand { get; }

        private void Paste()
        {
        }

        public ICommand DeleteCommand { get; }

        private void Delete()
        {
        }

        // Help command
        public ICommand AboutCommand { get; }

        private void About()
        {
        }

        // Component command
        public ICommand AddComponentCommand { get; }

        private void AddComponent(string componentType)
        {
            Console.WriteLine($"Adding component: {componentType}");

            SelectedComponent = CreateComponent(componentType);


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

        private static Component CreateComponent(string componentType)
        {
            switch (componentType)
            {
                case "AND":
                    return new AndGate(4);
                case "OR":
                    return new OrGate(2);
                case "NOT":
                    return new NotGate();
                case "NAND":
                    return new NandGate(2);
                case "NOR":
                    return new NorGate(2);
                case "XOR":
                    return new XorGate(2);
                case "XNOR":
                    return new XnorGate(2);
                case "PROBE":
                    return new LogicProbe();
                case "TOGGLE":
                    return new LogicToggle();
                    break;
                default:
                    return null; // TODO: DANGEROUS, THIS IS A FUCKING NULLPO WAITING TO HAPPEN
            }
        }
    }
}