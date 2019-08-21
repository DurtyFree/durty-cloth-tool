using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltTool
{
    class StatusController
    {

        public static void SetStatus(string status)
        {
            MainWindow.SetStatus(status);
        }

        public static void SetProgress(double progress)
        {
            MainWindow.SetProgress(progress);
        }
    }
}
