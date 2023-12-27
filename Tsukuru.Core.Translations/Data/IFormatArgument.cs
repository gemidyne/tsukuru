namespace Tsukuru.Core.Translations.Data;

public interface IFormatArgument
{
    string Description { get; }

    string Render(int index);
}