using Ninject.Modules;

namespace Ninject.Extensions.Diagnostic.Profiling
{
    public class ProfilingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IInvocationProfiler>().To<Profiler>();
            Bind<ProfileInterceptor>().ToSelf().InSingletonScope();
        }
    }
}
