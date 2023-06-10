using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using TownSuite.CoreDTOs.DataAccess;

namespace TownSuite.CoreDataAccess
{
    public class AppConnTenant
    {
        public DbConnection Connection { get; set; }
        public DbTransaction Transaction { get; set; }
        public AppConnNameEnum Name { get; set; }

    }
}
