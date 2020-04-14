using System.Collections.Generic;
using System.Diagnostics;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Business
{
	internal class CompileStepRunner
	{
		private readonly MapCompilerResultsViewModel _log;
		private readonly List<ICompileStep> _steps = new List<ICompileStep>();

		public CompileStepRunner(MapCompilerResultsViewModel log)
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

			foreach (ICompileStep step in _steps)
			{
				_log.Subtitle = $"Running compile step: \"{step.StepName}\"";

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