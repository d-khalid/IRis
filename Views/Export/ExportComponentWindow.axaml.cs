using Avalonia.Controls;
using IRis.ViewModels;

namespace IRis.Views
{
    public partial class ExportComponentWindow : Window
    {
        public ExportComponentWindow()
        {
            InitializeComponent();
            DataContext = new ExportComponentWindowViewModel();

            // Subscribe to view model events
            if (DataContext is ExportComponentWindowViewModel vm)
            {
                vm.RequestClose += OnRequestClose;
            }
        }

        public ExportComponentWindow(ExportComponentWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Subscribe to view model events
            if (DataContext is ExportComponentWindowViewModel vm)
            {
                vm.RequestClose += OnRequestClose;
            }
        }

        private void OnRequestClose(object? sender, string? result)
        {
            Close(result);
        }

        protected override void OnClosed(System.EventArgs e)
        {
            // Unsubscribe from events to prevent memory leaks
            if (DataContext is ExportComponentWindowViewModel vm)
            {
                vm.RequestClose -= OnRequestClose;
            }

            base.OnClosed(e);
        }
    }
}