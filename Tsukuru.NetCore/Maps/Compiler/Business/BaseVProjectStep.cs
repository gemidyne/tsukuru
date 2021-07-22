using System.IO;
using Tsukuru.Core.SourceEngine;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Business
{
    internal abstract class BaseVProjectStep : ICompileStep
    {
        public string VProject => VProjectHelper.Path;

        public string SdkToolsPath
        {
            get
            {
                var parent = Directory.GetParent(VProject);
                return parent.FullName;
            }
        }

        public abstract string StepName { get; }

        public abstract bool Run(ResultsLogContainer log);
    }
}