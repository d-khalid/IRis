using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.IO;
using System.Linq;
using Avalonia;
using IRis.Services;
using System.Collections.Generic;
using Tmds.DBus.Protocol;

namespace IRis.Views
{
    public partial class OtherComponentsWindow : Window
    {
        public int InputCount { get; set; }
        public int OutputCount { get; set; }
        public List<CircuitFormulaConversionService.CircuitFormula> Formulas { get; set; } = [];

        private const string ComponentFolder = "RuntimeComponents";

        public OtherComponentsWindow()
        {
            InitializeComponent();
            LoadComponentList();
        }

        private void LoadComponentList()
        {
            ComponentListBox.Items.Clear();

            if (Directory.Exists(ComponentFolder))
            {
                foreach (var file in Directory.GetFiles(ComponentFolder, "*.xml"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(file).Trim();

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        ComponentListBox.Items.Add(new ListBoxItem { Content = fileName });
                        // Console.WriteLine($"Loaded component: {fileName}");
                    }
                }
            }
        }

        private void OnAddClick(object? sender, RoutedEventArgs e)
        {
            if (ComponentListBox.SelectedItem is ListBoxItem item)
            {
                // Console.WriteLine("Selected component: " + item.Content);
                // TODO: Implement custom components logic here
                ExtractFormulasFromCircuit(Path.Combine("RuntimeComponents", item.Content!.ToString()! + ".xml"));

                var result = new CustomComponentData
                {
                    Name = item.Content!.ToString()!,
                    InputCount = this.InputCount,
                    OutputCount = this.OutputCount,
                    Formulas = this.Formulas
                };
                Close(result);
            }
            else
            {
                var dlg = new Window
                {
                    Title = "Error",
                    Width = 300,
                    Height = 100,
                    Content = new TextBlock
                    {
                        Text = "Please select a component first.",
                        Margin = new Thickness(10)
                    }
                };
                dlg.ShowDialog(this);
            }
        }

        private void ExtractFormulasFromCircuit(string fileName)
        {
            // 2. Get number of inputs and outputs from XML string
            string xmlContent = File.ReadAllText(fileName);
            InputCount = CircuitFormulaConversionService.GetNumberOfInputs(xmlContent);
            OutputCount = CircuitFormulaConversionService.GetNumberOfOutputs(xmlContent);
            Formulas = CircuitFormulaConversionService.ConvertXmlContentToFormulas(xmlContent);

            // 5. Display the formulas
            foreach (CircuitFormulaConversionService.CircuitFormula formula in Formulas)
            {
                Console.WriteLine($"{formula.OutputName} = {formula.Formula}");
                Console.WriteLine($"Input Variables: {string.Join(", ", formula.InputVariables)}");
            }
        }

        private void OnCancelClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
    
    public class CustomComponentData
    {
        public required string Name { get; set; }
        public int InputCount { get; set; }
        public int OutputCount { get; set; }
        public List<CircuitFormulaConversionService.CircuitFormula> Formulas { get; set; } = [];
    }

}
