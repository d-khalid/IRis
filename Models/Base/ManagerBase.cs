using IRis.ViewModels.Main.Canvas;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System;


namespace IRis.Models.Base;


public abstract partial class ManagerBase<T> : ObservableObject
    where T : ManagerBase<T>, new()
{
    private static T? _instance = null;
    [ObservableProperty] private bool _isVisible;
    public ObservableCollection<CircuitObjectViewModel> Objects { get; } = [];


    public ManagerBase()
    {
        if (_instance != null)
            throw new Exception("use GetInstance function instead pls.");
    }


    public static T GetInstance()
    {
        _instance ??= new();
        return _instance;
    }


    public virtual void Add(CircuitObjectViewModel obj)
    {
        Objects.Add(obj);
    }


    public virtual void Remove(CircuitObjectViewModel obj)
    {
        Objects.Remove(obj);
    }


    public virtual void AddCollection(ObservableCollection<CircuitObjectViewModel> collection)
    {
        foreach (var co in collection)
            Objects.Add(co);
    }


    public virtual void RemoveCollection(ObservableCollection<CircuitObjectViewModel> collection)
    {
        foreach (var co in collection)
        {
            for (int i = Objects.Count-1; i >= 0; i--)
            {
                if (Objects[i] == co)
                {
                    Objects.Remove(co);
                    break;
                }
            }
        }
    }


    public virtual void Ditch()
    {
        for (int i = Objects.Count-1; i >= 0; i--)
            Remove(Objects[i]);
    }


    public bool HasObjects()
    {
        return Objects.Count > 0;
    }


    public virtual void Show()
    {
        IsVisible = true;
    }


    public virtual void Hide()
    {
        IsVisible = false;
    }
}
