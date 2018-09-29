using GalaSoft.MvvmLight;
using System;

namespace Tsukuru.Maps.Compiler
{
    public abstract class BaseCompilationSettings : ObservableObject, ICompilationSettings
    {
        private string _formattedArguments;

        public string FormattedArguments
        {
            get => _formattedArguments;
            set
            {
                Set(() => FormattedArguments, ref _formattedArguments, value);
            }
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
}