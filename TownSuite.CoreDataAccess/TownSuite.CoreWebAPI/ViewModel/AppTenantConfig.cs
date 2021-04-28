using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TownSuite.CoreWebAPI.ViewModel
{
    public class AppTenantConfig
    {
        public IEnumerable<AppTenantConfigVM> AppTenantConfigs { get; set; }
    }
}
