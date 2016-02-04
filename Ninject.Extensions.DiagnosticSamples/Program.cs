using Ninject.Extensions.Diagnostic.Profiling;
using System;

namespace Ninject.Extensions.DiagnosticSamples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var kernel = new StandardKernel(new ProgramModule()))
            {
                var interceptor = kernel.Get<ProfileInterceptor>();
                var worker = kernel.Get<IWorker>();

                worker.DoSomeWork();

                foreach (var snapshot in interceptor.Profiler.Snapshots)
                {
                    Console.WriteLine(snapshot.Time);
                }
            }
        }
    }
}
