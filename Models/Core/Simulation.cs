using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using Avalonia;


namespace IRis.Models.Core;


public partial class Simulation : ManagerBase, ISingleton
{
    [ObservableProperty] private Point _currentMousePos = new(0, 0);
    private readonly HashSet<Point> _forbiddenMatrix = [];


    public static object GetInstance()
    {
        if (_instance == null)
            _instance = new Simulation();

        return _instance;
    }


    public void UpdateForbiddenMatrix()
    {
        _forbiddenMatrix.Clear();
    }


    public bool IsForbidden(Point pt)
    {
        return _forbiddenMatrix.Contains(pt);
    }
}
