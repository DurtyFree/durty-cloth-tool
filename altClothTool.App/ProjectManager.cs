using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace altClothTool.App
{
    internal static class ProjectManager
    {
        public static void SaveProject(string filePath)
        {
            var data = JsonConvert.SerializeObject(MainWindow.Clothes, Formatting.Indented);

            File.WriteAllText(filePath, data);
            StatusController.SetStatus("Project saved.");
        }

        public static void LoadProject(string filePath)
        {
            var data = JsonConvert.DeserializeObject<List<ClothData>>(File.ReadAllText(filePath));

            MainWindow.Clothes.Clear();

            var clothes = data.OrderBy(x => x.Name, new AlphanumericComparer()).ToList();

            foreach (var cloth in clothes)
            {
                MainWindow.Clothes.Add(cloth);
            }
            StatusController.SetStatus("Project loaded. Total clothes: " + MainWindow.Clothes.Count);
        }
    }
}