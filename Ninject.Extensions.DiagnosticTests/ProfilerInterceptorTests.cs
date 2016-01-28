using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Extensions.Diagnostic.Profiling;
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
                Thread.Sleep(200);
            }

            public async Task AsyncAction()
            {
                await Task.Delay(200);
            }

            public async Task<int> AsyncFunction()
            {
                await Task.Delay(200);
                return 10;
            }

            public int Function()
            {
                Thread.Sleep(200);
                return 10;
            }
        }
        
        [TestMethod]
        public void ActionTest()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();
                kernel.Bind<IInvocationProfiler>().To<Profiler>();
                kernel.Bind<ProfileInterceptor>().ToSelf().InSingletonScope();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                item.Action();
                Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count);
            }
        }

        [TestMethod]
        public void FunctionTest()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();
                kernel.Bind<IInvocationProfiler>().To<Profiler>();
                kernel.Bind<ProfileInterceptor>().ToSelf().InSingletonScope();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                Assert.AreEqual(10, item.Function());
                Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count);
            }
        }

        [TestMethod]
        public void AsyncActionTest()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();
                kernel.Bind<IInvocationProfiler>().To<Profiler>();
                kernel.Bind<ProfileInterceptor>().ToSelf().InSingletonScope();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                item.AsyncAction().Wait();
                Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count);
            }
        }

        [TestMethod]
        public void AsyncActionTest_Await()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();
                kernel.Bind<IInvocationProfiler>().To<Profiler>();
                kernel.Bind<ProfileInterceptor>().ToSelf().InSingletonScope();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                Task.Run(async () =>
                {
                    await item.AsyncAction();
                    Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count);
                }).Wait();
            }
        }

        [TestMethod]
        public void AsyncFunctionTest()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();
                kernel.Bind<IInvocationProfiler>().To<Profiler>();
                kernel.Bind<ProfileInterceptor>().ToSelf().InSingletonScope();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                Assert.AreEqual(10, item.AsyncFunction().Result);
                Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count);
            }
        }

        [TestMethod]
        public void AsyncFunctionTest_Await()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();
                kernel.Bind<IInvocationProfiler>().To<Profiler>();
                kernel.Bind<ProfileInterceptor>().ToSelf().InSingletonScope();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                Task.Run(async () => 
                {
                    var res = await item.AsyncFunction();

                    Assert.AreEqual(10, res);
                    Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count);
                }).Wait();                
            }
        }
    }
}