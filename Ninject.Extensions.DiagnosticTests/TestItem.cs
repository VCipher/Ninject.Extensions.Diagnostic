using Ninject.Extensions.Diagnostic.Profiling;
using System.Threading;
using System.Threading.Tasks;

namespace Ninject.Extensions.DiagnosticTests
{
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
}
