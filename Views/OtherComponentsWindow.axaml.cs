using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.IO;
using System.Linq;
using Avalonia;
using IRis.Services;
using System.Collections.Generic;
using Tmds.DBus.Protocol;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;
using Avalonia.Layout;
using Avalonia.Media;
using IRis.Models.Core;

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

            // Add Multiplexer
            var stackPanel2 = new StackPanel { Orientation = Orientation.Horizontal };
            stackPanel2.Children.Add(new TextBlock{ Text = "Multiplexer",
                VerticalAlignment = VerticalAlignment.Center });
            var label2 = new Border { Child = new TextBlock {
                    Text = "default", FontSize = 11,
                    Foreground = Brush.Parse("#00a2ff"),
                    FontWeight = FontWeight.SemiBold },
                BorderBrush = Brush.Parse("#016d9e"),
                Background = Brush.Parse("#00173b"),
                BorderThickness = new Thickness(0.5),
                CornerRadius = new CornerRadius(15),
                Padding = new Thickness(5, 2),
                Margin = new Thickness(10, 0, 0, 0) };
            stackPanel2.Children.Add(label2);
            ComponentListBox.Items.Add(new ListBoxItem { Content = stackPanel2 });

            if (Directory.Exists(ComponentFolder))
            {

                // Get all XML files in the folder
                foreach (var file in Directory.GetFiles(ComponentFolder, "*.xml"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(file).Trim();

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
                        // Add the file name as the component name
                        stackPanel.Children.Add(new TextBlock
                        {
                            Text = fileName,
                            VerticalAlignment = VerticalAlignment.Center
                        });

                        // Add a label to it for differentiability
                        var label = new Border
                        {
                            Child = new TextBlock
                            {
                                Text = "user created",
                                FontSize = 11,
                                Foreground = Brush.Parse("#00ffc8"),
                                FontWeight = FontWeight.SemiBold
                            },
                            BorderBrush = Brush.Parse("#00744c"),
                            Background = Brush.Parse("#003b33"),
                            BorderThickness = new Thickness(0.5),
                            CornerRadius = new CornerRadius(15),
                            Padding = new Thickness(5, 2),
                            Margin = new Thickness(10, 0, 0, 0)
                        };

                        stackPanel.Children.Add(label);
                        ComponentListBox.Items.Add(new ListBoxItem { Content = stackPanel });
                    }
                }
            }
        }

        private void OnAddClick(object? sender, RoutedEventArgs e)
        {
            if (ComponentListBox.SelectedItem is ListBoxItem item)
            {
                if (item.Content is StackPanel stackPanel && stackPanel.Children[0] is TextBlock textBlock)
                {
                    string componentName = textBlock.Text!;
                    
                    // Check the label type from the second child (Border -> TextBlock)
                    if (stackPanel.Children[1] is Border border && border.Child is TextBlock labelText)
                    {
                        string labelType = labelText.Text!;
                        
                        if (labelType == "default")
                        {
                            Console.WriteLine("Default component implementation placeholder");
                            // defaultComponentName will be set as previewComponent on closing
                            string defaultComponentName = null!;
                            if (componentName == "Multiplexer") defaultComponentName = "MUX";
                            else return;    // This should not happen
                            var result = new CustomComponentData { Name=defaultComponentName };
                            Close(result);
                        }
                        else
                        {
                            ExtractFormulasFromCircuit(Path.Combine("RuntimeComponents", componentName + ".xml"));

                            var result = new CustomComponentData
                            {
                                Name = componentName,
                                InputCount = this.InputCount,
                                OutputCount = this.OutputCount,
                                Formulas = this.Formulas
                            };
                            Close(result);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No component selected.");
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

        private async void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (ComponentListBox.SelectedItem is ListBoxItem item)
            {
                string fileName = item.Content!.ToString()!;
                string fullFilePath = Path.Combine(ComponentFolder, fileName + ".xml");
                bool confirmed;

                // Dialog box to confirm deletion
                var result = await MessageBoxManager
                .GetMessageBoxStandard("Confirm", 
                                     "Are you sure you want to delete this item?",
                                     ButtonEnum.YesNo,
                                     MsBox.Avalonia.Enums.Icon.Question)
                .ShowAsync();

                if (result == ButtonResult.Yes) confirmed = true;
                else confirmed = false;
                
                if (confirmed)
                {
                    File.Delete(fullFilePath);
                    LoadComponentList();
                }
            }
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
