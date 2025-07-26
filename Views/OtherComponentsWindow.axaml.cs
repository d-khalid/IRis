using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.IO;
using System.Linq;
using Avalonia;

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

        private void OnCancelClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
