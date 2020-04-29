using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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

        public async Task<bool> RunAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            _log.ProgressMaximum = _steps.Count;

            for (var i = 0; i < _steps.Count; i++)
            {
                ICompileStep step = _steps[i];

                _log.Subtitle = $"Running: \"{step.StepName}\"... ({i}/{_steps.Count})";

                var receiver = _log.GetLogDestination(step.StepName);

                _log.NavigateToLogTab(step.StepName);

                bool result = await Task.Run(() => step.Run(receiver));

                if (result)
                {
                    _log.ProgressValue++;
                    continue;
                }

                stopwatch.Stop();

                _log.Heading = "An error was encountered during compilation.";
                _log.NotifyComplete(stopwatch.Elapsed);

                return false;
            }

            stopwatch.Stop();
            _log.NotifyComplete(stopwatch.Elapsed);

            return true;
        }
    }
}