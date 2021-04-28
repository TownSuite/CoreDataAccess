using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TownSuite.CoreDTOs.DataAccess;

namespace TownSuite.CoreDataAccess
{
    public class AppConnTenant
    {
        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }
        public AppConnNameEnum Name { get; set; }

    }
}
