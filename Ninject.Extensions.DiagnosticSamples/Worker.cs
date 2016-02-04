using Ninject.Extensions.Diagnostic.Profiling;

namespace Ninject.Extensions.DiagnosticSamples
{
    [Profile]
    public class Worker : IWorker
    {
        public void DoSomeWork()
        {
            // todo: do some work...
        }
    }
}
