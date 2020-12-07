using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace AltTool
{
    class ProjectBuilder
    {
        public static void BuildProject(string outputFile)
        {
            var data = JsonConvert.SerializeObject(MainWindow.Clothes, Formatting.Indented);

            File.WriteAllText(outputFile, data);
        }

        public static void LoadProject(string inputFile)
        {
            string dir = Path.GetDirectoryName(inputFile);
            var data = JsonConvert.DeserializeObject<List<ClothData>>(File.ReadAllText(inputFile));

            MainWindow.Clothes.Clear();

            var _clothes = data.OrderBy(x => x.Name, new AlphanumericComparer()).ToList();

            foreach (var cd in _clothes)
            {
                MainWindow.Clothes.Add(cd);
            }
        }
    }
}