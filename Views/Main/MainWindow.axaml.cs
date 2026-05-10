using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using IRis.Models;
using IRis.Services;
using IRis.ViewModels;
using IRis.Views;


namespace IRis.Views;


public partial class MainWindow : Window
{
    public Simulation Simulation { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        Simulation = new(MainCanvas);
    }
}

