namespace Tsukuru.Maps.Compiler.Business
{
	internal interface ICompileStep
	{
		string StepName { get; }

		bool Run(ILogReceiver log);
	}
}
