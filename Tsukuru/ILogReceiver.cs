namespace Tsukuru
{
    public interface ILogReceiver
    {
        void Write(string message);

        void WriteLine(string category, string message);
    }
}