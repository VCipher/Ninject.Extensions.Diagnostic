using Ninject.Extensions.Interception;
using System.Reflection;
using System.Threading.Tasks;

namespace Ninject.Extensions.Diagnostic.Profiling
{
    public class ProfileInterceptor : IInterceptor
    {
        private readonly static MethodInfo startTaskMethodInfo = typeof(ProfileInterceptor)
            .GetMethod(nameof(InterceptTaskWithResult), BindingFlags.Instance | BindingFlags.NonPublic);
        
        public IInvocationProfiler Profiler { get; set; }
        
        public ProfileInterceptor(IInvocationProfiler profiler)
        {
            Profiler = profiler;
        }

        /// <summary>
        /// Intercepts the specified invocation
        /// </summary>
        /// <param name="invocation">The invocation to intercept</param>
        public void Intercept(IInvocation invocation)
        {
            var returnType = invocation.Request.Method.ReturnType;
            if (returnType == typeof(Task))
            {
                // async action
                InterceptTask(invocation);
                return;
            }

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                // async function
                var resultType = returnType.GetGenericArguments()[0];
                var mi = startTaskMethodInfo.MakeGenericMethod(resultType);
                mi.Invoke(this, new object[] { invocation });
                return;
            }

            // sync action/function
            Profiler.Measure(invocation);
        }

        private void InterceptTask(IInvocation invocation)
        {
            var ctx = new ProfilingContext();
            var invocationClone = invocation.Clone();

            invocation.ReturnValue = Task.Run(() => BeforeInvoke(ctx, invocation))
                .ContinueWith(t =>
                {
                    invocationClone.Proceed();
                    return invocationClone.ReturnValue as Task;
                })
                .Unwrap()
                .ContinueWith(t => AfterInvoke(ctx, invocation));
        }

        private void InterceptTaskWithResult<TResult>(IInvocation invocation)
        {
            var ctx = new ProfilingContext();
            var invocationClone = invocation.Clone();

            invocation.ReturnValue = Task.Run(() => BeforeInvoke(ctx, invocation))
                .ContinueWith(t =>
                {
                    invocationClone.Proceed();
                    return invocationClone.ReturnValue as Task<TResult>;
                })
                .Unwrap()
                .ContinueWith(t =>
                {
                    invocationClone.ReturnValue = t.Result;
                    AfterInvoke(ctx, invocationClone);
                    return (TResult)invocationClone.ReturnValue;
                });
        }

        /// <summary>
        /// Takes profile action before the invocation proceeds
        /// </summary>
        /// <param name="ctx">The context of current profiling</param>
        /// <param name="invocation">The invocation that is being intercepted</param>
        private void BeforeInvoke(ProfilingContext ctx, IInvocation invocation)
        {
            Profiler.BeginMeasure(ctx, invocation);
        }

        /// <summary>
        /// Takes profile action after the invocation proceeds
        /// </summary>
        /// <param name="ctx">The context of current profiling</param>
        /// <param name="invocation">The invocation that is being intercepted</param>
        private void AfterInvoke(ProfilingContext ctx, IInvocation invocation)
        {
            Profiler.EndMeasure(ctx, invocation);
        }
    }
}
