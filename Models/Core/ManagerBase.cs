using IRis.ViewModels.Circuit;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System;


namespace IRis.Models.Core;


public abstract partial class ManagerBase : ObservableObject
{
    protected static ManagerBase? _instance;
    public ObservableCollection<CircuitObjectViewModel> Objects { get; } = [];


    public ManagerBase()
    {
        if (_instance != null)
            throw new Exception("use GetInstance function instead pls.");
    }


    public void Add(CircuitObjectViewModel obj)
    {
        Objects.Add(obj);
    }


    public void Remove(CircuitObjectViewModel obj)
    {
        Objects.Remove(obj);
    }


    public void AddCollection(ObservableCollection<CircuitObjectViewModel> collection)
    {
        foreach (var co in collection)
            Objects.Add(co);
    }


    public void RemoveCollection(ObservableCollection<CircuitObjectViewModel> collection)
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
}
