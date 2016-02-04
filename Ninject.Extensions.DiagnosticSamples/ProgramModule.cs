using Ninject.Modules;

namespace Ninject.Extensions.DiagnosticSamples
{
    public class ProgramModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IWorker>().To<Worker>();
        }
    }
}
