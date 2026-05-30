using CommunityToolkit.Mvvm.ComponentModel;
using System;


namespace IRis.Services.Singleton;


/// <summary>
/// This can be used to create a singleton class. Just inherit from it and pass the name of
/// the child class as type argument.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract partial class SingletonBase<T> : ObservableObject
    where T : SingletonBase<T>, new()
{
    private static T? _instance = null;
    public static T Get() => _instance ??= new();


    public SingletonBase()
    {
        if (_instance != null)
            throw new Exception("use GetInstance() instead pls.");
    }
}
