using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TownSuite.CoreWebAPI.ViewModel
{
    public class AppTenantConfigVM
    {
        public string ConnectionString { get; set; }
        public string ConnectionName { get; set; }
        public int ConnectionId { get; set; }
    }
}
