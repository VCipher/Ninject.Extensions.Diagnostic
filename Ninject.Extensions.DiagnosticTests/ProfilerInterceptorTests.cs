using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Extensions.Interception.Infrastructure.Language;
using Ninject.Modules;
using System.Threading;
using System.Threading.Tasks;

namespace Ninject.Extensions.Diagnostic.Tests
{
    [TestClass]
    public class ProfilerInterceptorTests
    {
        public interface ITestItem
        {
            void Action();
            int Function();
            Task AsyncAction();
            Task<int> AsyncFunction();
        }

        [Profile]
        public class TestItem : ITestItem
        {
            public void Action()
            {
                Thread.Sleep(50);
            }

            public async Task AsyncAction()
            {
                await Task.Delay(1000);
            }

            public async Task<int> AsyncFunction()
            {
                await Task.Delay(1000);
                return 10;
            }

            public int Function()
            {
                Thread.Sleep(50);
                return 10;
            }
        }
        
        [TestMethod]
        public void DirectSetupTest_Action()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();
                kernel.Bind<IProfiler, IInvocationProfiler>().To<Profiler>();
                kernel.Bind<ProfilerInterceptor>().ToSelf().InSingletonScope();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfilerInterceptor>();

                // asserting
                item.Action();
                Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count);
            }
        }

        [TestMethod]
        public void DirectSetupTest_Function()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();
                kernel.Bind<IProfiler, IInvocationProfiler>().To<Profiler>();
                kernel.Bind<ProfilerInterceptor>().ToSelf().InSingletonScope();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfilerInterceptor>();

                // asserting
                Assert.AreEqual(10, item.Function());
                Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count);
            }
        }

        [TestMethod]
        public void DirectSetupTest_AsyncAction()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();
                kernel.Bind<IProfiler, IInvocationProfiler>().To<Profiler>();
                kernel.Bind<ProfilerInterceptor>().ToSelf().InSingletonScope();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfilerInterceptor>();

                // asserting
                item.AsyncAction().Wait();
                Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count);
            }
        }

        [TestMethod]
        public void DirectSetupTest_AsyncFunction()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();
                kernel.Bind<IProfiler, IInvocationProfiler>().To<Profiler>();
                kernel.Bind<ProfilerInterceptor>().ToSelf().InSingletonScope();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfilerInterceptor>();

                // asserting
                Assert.AreEqual(10, item.AsyncFunction().Result);
                Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count);
            }
        }
    }
}