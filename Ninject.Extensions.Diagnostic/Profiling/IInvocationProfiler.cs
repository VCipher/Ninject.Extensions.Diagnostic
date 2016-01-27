using Ninject.Extensions.Interception;

namespace Ninject.Extensions.Diagnostic.Profiling
{
    public interface IInvocationProfiler : IProfiler
    {
        void BeginMeasure(ProfilingContext ctx, IInvocation invocation);
        void EndMeasure(ProfilingContext ctx, IInvocation invocation);
        void Measure(IInvocation invocation);
    }
}
