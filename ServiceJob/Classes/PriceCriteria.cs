using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceJob.Interface;

namespace ServiceJob.Classes
{
    public class PriceCriteria<T> : IActionCriteria<T>
    {
        public async Task<T> LoadAsync()
        {
            using (var fs = new FileStream("configcriteria.json", FileMode.Open))
            {
                var priceCriteria = await JsonSerializer.DeserializeAsync<T>(fs);
                return priceCriteria;
            }
        }

        public async Task SavedAsync(T criterias)
        {
            using (var fs = new FileStream("configcriteria.json", FileMode.Create))
            {
                await JsonSerializer.SerializeAsync(fs, criterias);
            }
        }
    }
}