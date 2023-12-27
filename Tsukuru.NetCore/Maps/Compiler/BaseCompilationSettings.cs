﻿using System;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler;

public abstract class BaseCompilationSettings : ViewModelBase, ICompilationSettings
{
    private string _formattedArguments;

    public string FormattedArguments
    {
        get => _formattedArguments;
        set => SetProperty(ref _formattedArguments, value);
    }

    public abstract string BuildArguments();

    public void OnArgumentChanged()
    {
        FormattedArguments = BuildArguments();
    }

    protected string ConditionalArg(Func<bool> input, string commandLineArgument)
    {
        return input()
            ? $"{commandLineArgument} "
            : string.Empty;
    }
}