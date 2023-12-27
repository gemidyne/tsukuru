using System.Text;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class ResultsLogContainer : ViewModelBase
{
    private readonly StringBuilder _builder = new StringBuilder();
    private readonly object _door = new object();
    private string _category;
    private string _text;

    public string Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    public string ConsoleText
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    public void Append(char c)
    {
        lock (_door)
        {
            _builder.Append(c);

            RebuildStringCache();
        }
    }

    public void Append(string text)
    {
        lock (_door)
        {
            _builder.Append(text);

            RebuildStringCache();
        }
    }

    public void AppendLine(string category, string message)
    {
        lock (_door)
        {
            _builder.AppendFormat("[{0}]: {1}", category, message).AppendLine();

            RebuildStringCache();
        }
    }

    private void RebuildStringCache()
    {
        lock (_door)
        {
            ConsoleText = _builder.ToString();
        }
    }
}