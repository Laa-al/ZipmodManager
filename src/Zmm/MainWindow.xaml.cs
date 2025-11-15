using System;
using System.Windows;
using Microsoft.Win32;
using Zmm.Zipmods;

namespace Zmm;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{

    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        BlazorWebView.Services = serviceProvider;
    }

}
