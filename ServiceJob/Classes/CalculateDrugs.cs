using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Rewrite.Internal;
using Microsoft.Extensions.FileSystemGlobbing;
using Newtonsoft.Json;
using ServiceJob.Interface;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ServiceJob.Classes
{
    public class CalculateDrugs : ICalculateDrugs
    {
        public async Task<string> ReadLastDateUpdate()
        {
            using var fs = new FileStream("lastdateupdate.json", FileMode.Open);
            var date = await JsonSerializer.DeserializeAsync<string>(fs);
            return date;
        }
    }
}
