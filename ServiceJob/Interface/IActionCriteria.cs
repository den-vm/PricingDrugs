using System.Threading.Tasks;

namespace ServiceJob.Interface
{
    internal interface IActionCriteria<T>
    {
        Task SavedAsync(T criterias);
        Task<T> LoadAsync();
    }
}