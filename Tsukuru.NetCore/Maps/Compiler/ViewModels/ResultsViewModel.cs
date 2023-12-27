﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using Tsukuru.Maps.Compiler.Business;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class ResultsViewModel : ViewModelBase, IApplicationContentView
{
    private bool _isCloseButtonOnExecutionEnabled;
    private string _heading;
    private string _mapName;
    private string _subtitle;
    private int _progressValue;
    private int _progressMaximum;
    private bool _isLoading;
    private int _activeLogsIndex;

    public string Heading
    {
        get => _heading;
        set => SetProperty(ref _heading, value);
    }

    public string Subtitle
    {
        get => _subtitle;
        set => SetProperty(ref _subtitle, value);
    }

    public int ProgressValue
    {
        get => _progressValue;
        set => SetProperty(ref _progressValue, value);
    }

    public int ProgressMaximum
    {
        get => _progressMaximum;
        set => SetProperty(ref _progressMaximum, value);
    }

    public bool IsCloseButtonOnExecutionEnabled
    {
        get => _isCloseButtonOnExecutionEnabled;
        set
        {
            _isCloseButtonOnExecutionEnabled = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsProgressBarIndeterminate));
        }
    }

    public ObservableCollection<ResultsLogContainer> Logs { get; } = new ObservableCollection<ResultsLogContainer>();

    public int ActiveLogsIndex
    {
        get => _activeLogsIndex;
        set => SetProperty(ref _activeLogsIndex, value);
    }

    public bool IsProgressBarIndeterminate => false;

    public RelayCommand CloseCommand { get; }

    public string Name => "Compiler Results";

    public string Description => "View current compiler progress and results.";

    public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ResultsViewModel()
    {
        CloseCommand = new RelayCommand(CloseResults);
    }

    private void CloseResults()
    {
        lock (Logs)
        {
            Logs.Clear();
        }
    }

    public void Init()
    {
        _mapName = MapCompileSessionInfo.Instance.MapName;
        Subtitle = "Ready";
    }

    public void Initialise()
    {
        Heading = $"Compiling {_mapName}...";
        Subtitle = "Please wait...";
        IsCloseButtonOnExecutionEnabled = false;

        Logs.Clear();
    }

    public void NotifyComplete(TimeSpan timeElapsed)
    {
        Heading = $"Compiled {_mapName}";
        Subtitle = $"Completed in {timeElapsed}";

        IsCloseButtonOnExecutionEnabled = true;
        ProgressValue = ProgressMaximum;
    }

    public ResultsLogContainer GetLogDestination(string category)
    {
        if (Logs.All(x => x.Category != category))
        {
            Logs.Add(new ResultsLogContainer { Category = category });
        }

        return Logs.Single(x => x.Category == category);
    }

    public void NavigateToLogTab(string category)
    {
        var tab = Logs.SingleOrDefault(x => x.Category == category);

        if (tab == null)
        {
            return;
        }

        int idx = Logs.IndexOf(tab);

        ActiveLogsIndex = idx;
    }
}