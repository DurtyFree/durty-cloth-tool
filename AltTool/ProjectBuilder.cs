using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AltTool
{
    class ProjectBuilder
    {
        public static void SaveProject(string outputFile)
        {
            var data = JsonConvert.SerializeObject(MainWindow.Clothes, Formatting.Indented);

            File.WriteAllText(outputFile, data);
        }

        public static void LoadProject(string inputFile)
        {
            var data = JsonConvert.DeserializeObject<List<ClothData>>(File.ReadAllText(inputFile));

            MainWindow.Clothes.Clear();

            var clothes = data.OrderBy(x => x.Name, new AlphanumericComparer()).ToList();

            foreach (var cd in clothes)
            {
                MainWindow.Clothes.Add(cd);
            }
        }
    }
}