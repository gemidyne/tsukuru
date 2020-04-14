using System;

namespace Tsukuru
{
	public interface ILogReceiver
    {
	    void Initialise(string information);

	    void NotifyComplete(TimeSpan elapsed);

        void Write(string message);

        void WriteLine(string category, string message);
    }
}