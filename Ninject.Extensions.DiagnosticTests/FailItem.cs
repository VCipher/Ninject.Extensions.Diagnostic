using Ninject.Extensions.Diagnostic.Profiling;
using System;
using System.Threading.Tasks;

namespace Ninject.Extensions.DiagnosticTests
{
    [Profile]
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
}
