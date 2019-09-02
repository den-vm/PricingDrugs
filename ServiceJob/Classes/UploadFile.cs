using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ServiceJob.Models
{
    public class UploadFile
    {
        public IQueryable<JvnlpModel> Drugs { get; protected set; }

        public void ReaderFileJvnlp(IFormFile file)
        {

        }
    }
}