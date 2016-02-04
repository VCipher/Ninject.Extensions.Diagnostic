using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Extensions.Diagnostic.Profiling;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ninject.Extensions.DiagnosticTests
{
    [TestClass]
    public class ProfilerInterceptorTests
    {
        [TestMethod]
        public void ActionTest()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                item.Action();
                Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count());
            }
        }

        [TestMethod]
        public void FunctionTest()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                Assert.AreEqual(10, item.Function());
                Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count());
            }
        }

        [TestMethod]
        public void AsyncActionTest()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                item.AsyncAction().Wait();
                Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count());
            }
        }

        [TestMethod]
        public void AsyncActionTest_Await()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                Task.Run(async () =>
                {
                    await item.AsyncAction();
                    Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count());
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

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                Assert.AreEqual(10, item.AsyncFunction().Result);
                Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count());
            }
        }

        [TestMethod]
        public void AsyncFunctionTest_Await()
        {
            using (var kernel = new StandardKernel())
            {
                // bindings
                kernel.Bind<ITestItem>().To<TestItem>();

                // initialize
                var item = kernel.Get<ITestItem>();
                var inteceptor = kernel.Get<ProfileInterceptor>();

                // asserting
                Task.Run(async () => 
                {
                    var res = await item.AsyncFunction();

                    Assert.AreEqual(10, res);
                    Assert.AreEqual(1, inteceptor.Profiler.Snapshots.Count());
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