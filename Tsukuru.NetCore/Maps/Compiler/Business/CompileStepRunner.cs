using System.Collections.Generic;
using System.Diagnostics;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Business
{
    internal class CompileStepRunner
    {
        private readonly ResultsViewModel _log;
        private readonly List<ICompileStep> _steps = new List<ICompileStep>();

        public CompileStepRunner(ResultsViewModel log)
        {
            _log = log;
        }

        public void AddStep(ICompileStep step)
        {
            _steps.Add(step);
        }

        public void Run()
        {
            var stopwatch = Stopwatch.StartNew();

            _log.ProgressMaximum = _steps.Count;

            for (var i = 0; i < _steps.Count; i++)
            {
                ICompileStep step = _steps[i];

                _log.Subtitle = $"Running: \"{step.StepName}\"... ({i}/{_steps.Count})";

                bool result = step.Run(_log);

                if (result)
                {
                    _log.ProgressValue++;
                    continue;
                }

                stopwatch.Stop();

                _log.WriteLine("Tsukuru", "An error was encountered in the compilation process.");
                _log.NotifyComplete(stopwatch.Elapsed);

                return;
            }

            stopwatch.Stop();
            _log.NotifyComplete(stopwatch.Elapsed);
        }
    }
}