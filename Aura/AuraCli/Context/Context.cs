using Aura.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraCli.Context
{
    public class CurrentContext 
    {
        public Project Project { get; set; }

        public static CurrentContext GetCurrentContext()
        {
            string filePath = Directory.GetFiles(Helpers.MainDir).FirstOrDefault(file => file.Split('\\')[^1] == ".auractx");
            if (filePath != null)
            {
                using StreamReader sr = new StreamReader(filePath);
                string asStr = sr.ReadToEnd();
                
                return JsonConvert.DeserializeObject<CurrentContext>(asStr);
            }
            return null;
        }
        public void Save()
        {
            using var file = File.Create(Helpers.MainDir + "\\" + ".auractx");

            file.Write(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)));
        }
    }
}
