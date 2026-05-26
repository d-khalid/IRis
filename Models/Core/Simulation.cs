using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using Avalonia;
using IRis.Base;


namespace IRis.Models.Core;


public partial class Simulation : ManagerBase<Simulation>
{
    public bool Running = false;
    [ObservableProperty] private Point _currentMousePos = new(0, 0);
    private readonly HashSet<Point> _forbiddenMatrix = [];


    public void UpdateForbiddenMatrix()
    {
        _forbiddenMatrix.Clear();
    }


    public bool IsForbidden(Point pt)
    {
        return _forbiddenMatrix.Contains(pt);
    }


    public void Start()
    {
        Running = true;
    }


    public void End()
    {
        Running = false;
    }
}
