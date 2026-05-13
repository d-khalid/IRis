// using System;
// using System.ComponentModel;
// using System.Runtime.CompilerServices;
// using System.Windows.Input;
// using CommunityToolkit.Mvvm.Input;

// namespace IRis.ViewModels
// {
//     public class ExportComponentWindowViewModel : INotifyPropertyChanged
//     {
//         private string _componentName = string.Empty;

//         public event PropertyChangedEventHandler? PropertyChanged;
//         public event EventHandler<string?>? RequestClose;

//         public string ComponentName
//         {
//             get => _componentName;
//             set
//             {
//                 if (_componentName != value)
//                 {
//                     _componentName = value;
//                     OnPropertyChanged();
//                     ExportCommand.NotifyCanExecuteChanged();
//                 }
//             }
//         }

//         public RelayCommand ExportCommand { get; }
//         public RelayCommand CancelCommand { get; }

//         public ExportComponentWindowViewModel()
//         {
//             ExportCommand = new RelayCommand(ExecuteExport, CanExecuteExport);
//             CancelCommand = new RelayCommand(ExecuteCancel);
//         }

//         private bool CanExecuteExport()
//         {
//             return !string.IsNullOrWhiteSpace(ComponentName);
//         }

//         private void ExecuteExport()
//         {
//             if (!CanExecuteExport()) return;

//             // TODO: Implement export logic here
//             // Return the component name on success
//             RequestClose?.Invoke(this, ComponentName);
//         }

//         private void ExecuteCancel()
//         {
//             RequestClose?.Invoke(this, null);
//         }

//         protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
//         {
//             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//         }
//     }
// }