using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CodeAnalyzer.UI.Interfaces;

namespace CodeAnalyzer.UI.Controls;

public partial class LogViewerControl : UserControl, ILoggerUi
{
    public LogViewerControl()
    {
        InitializeComponent();
    }
    
    public void Log(string message)
    {
        LogTextBlock.Text += $"\n[{DateTime.Now}] {message}";
    }

    public void Log()
    {
        LogTextBlock.Text += $"\n[{DateTime.Now}]";
    }

    public void Log(Exception ex)
    {
        LogTextBlock.Text += $"\n[{DateTime.Now}] {ex.Message}";
        LogTextBlock.Text += $"\n{ex.StackTrace}";
    }

    private void OnClearLogsClicked(object? sender, RoutedEventArgs e)
    {
        LogTextBlock.Text = "";
    }
}