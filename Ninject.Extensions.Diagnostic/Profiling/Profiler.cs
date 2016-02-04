using Ninject.Extensions.Diagnostic.Common;
using Ninject.Extensions.Interception;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Ninject.Extensions.Diagnostic.Profiling
{
    public class Profiler : IProfiler, IInvocationProfiler
    {
        private readonly ConcurrentBag<Snapshot> _snapshots;

        public IEnumerable<Snapshot> Snapshots { get { return _snapshots; } }        

        public Profiler()
        {
            _snapshots = new ConcurrentBag<Snapshot>();
        }

        /// <summary>
        /// Executes passed object with delegate type, 
        /// measuring execution time and storing the passed parameters.
        /// </summary>
        public void Measure(Action action, params object[] data)
        {
            var sw = Stopwatch.StartNew();
            action();

            var snapshot = new Snapshot(sw.Elapsed, data);
            _snapshots.Add(snapshot);
        }

        /// <summary>
        /// Executes passed object with delegate type, 
        /// measuring execution time and storing the passed parameters.
        /// </summary>
        /// <param name="action">Object with delegate type to execution</param>
        /// <param name="data">Parameters of execution</param>
        public T Measure<T>(Func<T> action, params object[] data)
        {
            var sw = Stopwatch.StartNew();
            T res = action();

            var snapshot = new Snapshot(sw.Elapsed, data.Concat(res));
            _snapshots.Add(snapshot);

            return res;
        }

        /// <summary>
        /// Asynchronously executes passed object with delegate type,
        /// measuring execution time and storing the passed parameters.
        /// </summary>
        /// <param name="action">Object with delegate type to execution</param>
        /// <param name="data">Parameters of execution</param>
        public async Task MeasureAsync(Func<Task> action, params object[] data)
        {
            var sw = Stopwatch.StartNew();
            await action().ContinueWith(t => { return t; }).Unwrap();

            var snapshot = new Snapshot(sw.Elapsed, data);
            _snapshots.Add(snapshot);
        }

        /// <summary>
        /// Asynchronously executes passed object with delegate type,
        /// measuring execution time and storing the passed parameters.
        /// </summary>
        /// <param name="action">Object with delegate type to execution</param>
        /// <param name="data">Parameters of execution</param>
        public async Task<T> MeasureAsync<T>(Func<Task<T>> action, params object[] data)
        {
            var sw = Stopwatch.StartNew();
            T res = await action();

            var snapshot = new Snapshot(sw.Elapsed, data.Concat(res));
            _snapshots.Add(snapshot);

            return res;
        }

        void IInvocationProfiler.BeginMeasure(ProfilingContext ctx, IInvocation invocation)
        {
            string typeName = invocation.Request.Method.DeclaringType.FullName;
            string methodName = invocation.Request.Method.Name;

            ctx.Timer = Stopwatch.StartNew();
            ctx.Data = new List<object>(invocation.Request.Arguments);
            ctx.Data.Add($"{typeName}.{methodName}");
        }

        void IInvocationProfiler.EndMeasure(ProfilingContext ctx, IInvocation invocation)
        {
            ctx.Timer.Stop();
            ctx.Data.Add(invocation.ReturnValue);

            var snapshot = new Snapshot(ctx.Timer.Elapsed, ctx.Data.ToArray());
            _snapshots.Add(snapshot);
        }

        void IInvocationProfiler.Measure(IInvocation invocation)
        {
            var ctx = new ProfilingContext();
            var profiler = this as IInvocationProfiler;

            profiler.BeginMeasure(ctx, invocation);
            invocation.Proceed();
            profiler.EndMeasure(ctx, invocation);
        }
    }
}
