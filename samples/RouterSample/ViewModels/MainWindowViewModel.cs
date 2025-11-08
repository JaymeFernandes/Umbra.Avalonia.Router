using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RouterSample.ViewModels.Shared;

namespace RouterSample.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public Dictionary<string, string> Titles = new Dictionary<string, string>()
    {
        { "home", "Home" },
        { "sub/first", "First Sub" },
        { "sub/second", "Second Sub" },
        { "sub/third", "Third Sub" },
    };
    
    [ObservableProperty] private Control _content;
    [ObservableProperty] private string _title;

    public MainWindowViewModel()
    {
        this.RouterHistory.TitleChanged += (title) =>
        {
            Title = string.IsNullOrWhiteSpace(title) ? "Sample Router" : $"Sample Router - {title}";
        };
        this.RouterHistory.PageChanged += (page) => Content = page;

        this.RouterHistory.Navigate("home", "Home");
    }

    [RelayCommand]
    public void Navigate(string route) 
        => this.RouterHistory.Navigate(route, Titles.TryGetValue(route, out var value) ? value : "");
}