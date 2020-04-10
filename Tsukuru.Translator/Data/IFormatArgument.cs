namespace Tsukuru.Translator.Data
{
    public interface IFormatArgument
    {
        string Description { get; }

        string Render(int index);
    }
}