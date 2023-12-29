namespace Tsukuru.Schemas.Translations;

public interface IFormatArgument
{
    string Description { get; }

    string Render(int index);
}