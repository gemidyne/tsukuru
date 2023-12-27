using Tsukuru.ViewModels;

namespace Tsukuru.SourcePawn;

public class CompilationMessage : ViewModelBase
{
    private int? _firstLine;
    private int _lastLine;
    private string _fileName;
    private string _prefix;
    private string _message;
    private string _rawLine;

    public int? FirstLine
    {
        get => _firstLine;
        set
        {
            SetProperty(ref _firstLine, value);
            OnPropertyChanged(nameof(LineNumberDisplay));
        }
    }

    public int LastLine
    {
        get => _lastLine;
        set
        {
            SetProperty(ref _lastLine, value);
            OnPropertyChanged(nameof(LineNumberDisplay));
        }
    }

    public string FileName
    {
        get => _fileName;
        set => SetProperty(ref _fileName, value);
    }

    public string Prefix
    {
        get => _prefix;
        set => SetProperty(ref _prefix, value);
    }

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public string RawLine
    {
        get => _rawLine;
        set => SetProperty(ref _rawLine, value);
    }

    public string LineNumberDisplay => FirstLine.HasValue 
        ? FirstLine + " - " + LastLine 
        : LastLine.ToString();
}