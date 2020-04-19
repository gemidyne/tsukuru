using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Business
{
    internal interface ICompileStep
    {
        string StepName { get; }

        bool Run(ResultsLogContainer log);
    }
}
