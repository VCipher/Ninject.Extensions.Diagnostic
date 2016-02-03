using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Extensions.Diagnostic.Profiling;
using Ninject.Extensions.Interception.Infrastructure.Language;
using Ninject.Modules;
using System;
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

        //[Profile]
        public class FailItem : ITestItem
        {
            public void Action()
            {
                throw new NotImplementedException();
            }

            public async Task AsyncAction()
            {
                await Task.Delay(10);
                throw new NotImplementedException();
            }

            public async Task<int> AsyncFunction()
            {
                await Task.Delay(10);
                throw new NotImplementedException();
            }

            public int Function()
            {
                throw new NotImplementedException();
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

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ActionTest_Exception()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<FailItem>();
                kernel.Bind<IInvocationProfiler>().To<Profiler>();
                kernel.Bind<ProfileInterceptor>().ToSelf().InSingletonScope();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                item.Action();
                Assert.Fail("Exception is not rised");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void FunctionTest_Exception()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<FailItem>();
                kernel.Bind<IInvocationProfiler>().To<Profiler>();
                kernel.Bind<ProfileInterceptor>().ToSelf().InSingletonScope();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                var res = item.Function();                
                Assert.Fail("Exception is not rised");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void AsyncActionTest_Exception()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<FailItem>();
                kernel.Bind<IInvocationProfiler>().To<Profiler>();
                kernel.Bind<ProfileInterceptor>().ToSelf().InSingletonScope();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                item.AsyncAction().Wait();
                Assert.Fail("Exception is not rised");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void AsyncFunctionTest_Exception()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<FailItem>();
                kernel.Bind<IInvocationProfiler>().To<Profiler>();
                kernel.Bind<ProfileInterceptor>().ToSelf().InSingletonScope();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                var res = item.AsyncFunction().Result;
                Assert.Fail("Exception is not rised");
            }
        }
    }
}