using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Linqpad.AppConfigReaders
{
    public class Sitecore8AppConfigReader : Sitecore7AppConfigReader
    {
        public override bool IsSupportedVersion(Version version)
        {
            if (version == null) { return false; }
            var min = new Version("8.0");
            var max = new Version("8.1");
            if (version.CompareTo(min) != -1 && version.CompareTo(max) == -1)
            {
                return true;
            }
            return base.IsSupportedVersion(version);
        }
    }
}
