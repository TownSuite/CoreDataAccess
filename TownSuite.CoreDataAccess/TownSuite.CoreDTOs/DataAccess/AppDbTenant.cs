using System;
using System.Collections.Generic;
using System.Text;

namespace TownSuite.CoreDTOs.DataAccess
{
    public class AppDbTenant
    {
        public string TenantId { get; set; }
        public IEnumerable<AppDbConnectionVM> AppDbConnections { get; set; }
    }
}
