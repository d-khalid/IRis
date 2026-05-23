using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.ViewModels;


public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty] private Point _MousePosition = new(0, 0);
}
