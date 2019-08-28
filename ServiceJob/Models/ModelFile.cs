using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ServiceJob.Models
{
    public class UploadFile
    {
        public IQueryable<ModelJvnlp> Drugs { get; protected set; }

        public void ReaderFileJvnlp(IFormFile file)
        {

        }
    }
}