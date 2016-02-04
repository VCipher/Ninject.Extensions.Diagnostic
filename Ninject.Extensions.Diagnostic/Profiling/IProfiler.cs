using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ninject.Extensions.Diagnostic.Profiling
{
    public interface IProfiler
    {
        IEnumerable<Snapshot> Snapshots { get; }

        void Measure(Action action, params object[] data);
        T Measure<T>(Func<T> action, params object[] data);
        Task MeasureAsync(Func<Task> action, params object[] data);
        Task<T> MeasureAsync<T>(Func<Task<T>> action, params object[] data);
    }
}