using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.IO;
using System.Linq;
using Avalonia;
using IRis.Services;
using System.Collections.Generic;

namespace IRis.Views
{
    public partial class OtherComponentsWindow : Window
    {
        public string? SelectedComponent { get; private set; }

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
                Console.WriteLine("Selected component: " + item.Content);
                // TODO: Implement custom components logic here
                test(Path.Combine("RuntimeComponents", item.Content!.ToString()! + ".xml"));

                Close(SelectedComponent);
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

        private void test(string fileName)
        {
            // 2. Get number of inputs and outputs from XML string
            string xmlContent = File.ReadAllText(fileName);
            int inputCount2 = CircuitFormulaConversionService.GetNumberOfInputs(xmlContent);
            int outputCount2 = CircuitFormulaConversionService.GetNumberOfOutputs(xmlContent);
            var formulas2 = CircuitFormulaConversionService.ConvertXmlContentToFormulas(xmlContent);

            // 5. Display the formulas
            foreach (var formula in formulas2)
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
}
