﻿using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Tsukuru.SourcePawn.ViewModels
{
    public class CompilationFileViewModel : ViewModelBase
    {
        private readonly SourcePawnCompileViewModel _parentViewModel;
        private string _file;
        private ObservableCollection<CompilationMessage> _messages;
        private ObservableCollection<CompilationMessage> _errors;
        private ObservableCollection<CompilationMessage> _warnings;

        private string _errorsHeader;
        private string _rawOutput;
        private string _warningsHeader;
        private ECompilationResult _result = ECompilationResult.Unknown;
        private bool _canShowDetails;
        private bool _isSuccessfulCompile;
        private bool _isBusy;
        private bool _isCompiledWithErrors;
        private bool _isCompiledWithWarnings;
        private bool _isUnknownState;
        private string _shortStatus;

        public string File
        {
            get => _file;
            set => Set(() => File, ref _file, value);
        }

        public ECompilationResult Result
        {
            get => _result;
            set => Set(() => Result, ref _result, value);
        }

        public ObservableCollection<CompilationMessage> Messages
        {
            get => _messages;
            set => Set(() => Messages, ref _messages, value);
        }

        public bool IsSuccessfulCompile
        {
            get => _isSuccessfulCompile;
            set => Set(() => IsSuccessfulCompile, ref _isSuccessfulCompile, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(() => IsBusy, ref _isBusy, value);
        }

        public bool IsCompiledWithErrors
        {
            get => _isCompiledWithErrors;
            set => Set(() => IsCompiledWithErrors, ref _isCompiledWithErrors, value);
        }

        public bool IsCompiledWithWarnings
        {
            get => _isCompiledWithWarnings;
            set => Set(() => IsCompiledWithWarnings, ref _isCompiledWithWarnings, value);
        }

        public bool IsUnknownState
        {
            get => _isUnknownState;
            set => Set(() => IsUnknownState, ref _isUnknownState, value);
        }

        public bool CanShowDetails
        {
            get => _canShowDetails;
            set => Set(() => CanShowDetails, ref _canShowDetails, value);
        }

        public string ShortStatus
        {
            get => _shortStatus;
            set => Set(() => ShortStatus, ref _shortStatus, value);
        }

        public ObservableCollection<CompilationMessage> Errors
        {
            get => _errors;
            set => Set(() => Errors, ref _errors, value);
        }

        public ObservableCollection<CompilationMessage> Warnings
        {
            get => _warnings;
            set => Set(() => Warnings, ref _warnings, value);
        }

        public string RawOutput
        {
            get => _rawOutput;
            set => Set(() => RawOutput, ref _rawOutput, value);
        }

        public string ErrorsHeader
        {
            get => _errorsHeader;
            set => Set(() => ErrorsHeader, ref _errorsHeader, value);
        }

        public string WarningsHeader
        {
            get => _warningsHeader;
            set => Set(() => WarningsHeader, ref _warningsHeader, value);
        }

        public RelayCommand RemoveCommand { get; }

        public CompilationFileViewModel(SourcePawnCompileViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;
            RemoveCommand = new RelayCommand(RemoveFile);
            IsUnknownState = true;
            ShortStatus = "Not compiled.";

            Messages = new ObservableCollection<CompilationMessage>();
        }

        public void UpdateStatus(bool isCompiling = false)
        {
            if (isCompiling)
            {
                Result = ECompilationResult.Compiling;
                IsBusy = true;
                IsUnknownState = false;
                IsSuccessfulCompile = false;
                IsCompiledWithWarnings = false;
                IsCompiledWithErrors = false;
                CanShowDetails = false;
                ShortStatus = "Compiling...";
                return;
            }

            IsBusy = false;

            if (!Messages.Any())
            {
                Result = ECompilationResult.Unknown;
                IsUnknownState = true;
                IsSuccessfulCompile = false;
                IsCompiledWithWarnings = false;
                IsCompiledWithErrors = false;
                CanShowDetails = false;
                ShortStatus = "Not compiled.";
                return;
            }

            IsUnknownState = false;

            int errorCount = Messages.Count(m => CompilationMessageParser.IsLineError(m.Prefix));
            int warningCount = Messages.Count(m => CompilationMessageParser.IsLineWarning(m.Prefix));

            if (errorCount > 0)
            {
                Result = ECompilationResult.FailedWithErrors;
                IsSuccessfulCompile = false;
                IsCompiledWithWarnings = false;
                IsCompiledWithErrors = true;
                CanShowDetails = true;
                ShortStatus = "Failed to compile.";
            }
            else if (warningCount > 0)
            {
                Result = ECompilationResult.CompletedWithWarnings;
                IsSuccessfulCompile = false;
                IsCompiledWithWarnings = true;
                IsCompiledWithErrors = false;
                CanShowDetails = true;
                ShortStatus = "Compiled with warning(s).";
            }
            else
            {
                Result = ECompilationResult.Completed;
                IsSuccessfulCompile = true;
                IsCompiledWithWarnings = false;
                IsCompiledWithErrors = false;
                CanShowDetails = false;
                ShortStatus = "Compiled successfully.";
            }

            Errors = new ObservableCollection<CompilationMessage>(Messages.Where(m => CompilationMessageParser.IsLineError(m.Prefix)));
            ErrorsHeader = $"Errors ({Errors.Count})";

            Warnings = new ObservableCollection<CompilationMessage>(Messages.Where(m => CompilationMessageParser.IsLineWarning(m.Prefix)));
            WarningsHeader = $"Warnings ({Warnings.Count})";

            RawOutput = string.Join("\r\n", Messages.Select(m => m.RawLine));
        }

        private void RemoveFile()
        {
            _parentViewModel.FilesToCompile.Remove(this);
        }
    }
}
