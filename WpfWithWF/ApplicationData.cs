using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfWithWF
{
    public class ApplicationData
    {
        public string ApplicationName { get; set; }
        public string NewApplication { get; set; }
    }

    public class ProjectInformation
    {
        public string ProjectName { get; set; }
        public List<ApplicationData> ApplicationInfo { get; set; }
    }

}
