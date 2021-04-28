using System;
using System.Collections.Generic;
using System.Text;

namespace TownSuite.CoreDTOs.DataAccess
{
    public class AppDbConnectionVM
    {
        public string ConnectionString { get; set; }
        public string ConnectionName { get; set; }
        public int ConnectionId { get; set; }
        //ConnectionId sould map to AppDbNameEnum value
    }
}
