using System.Threading.Tasks;

namespace Ninject.Extensions.DiagnosticTests
{
    public interface ITestItem
    {
        void Action();
        int Function();
        Task AsyncAction();
        Task<int> AsyncFunction();
    }
}
