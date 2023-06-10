using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Text;
using Npgsql;
using TownSuite.CoreDataAccess.Interfaces;
using TownSuite.CoreDTOs.DataAccess;

namespace TownSuite.CoreDataAccess
{
    public sealed class TSDalSession : IDisposable
    {
        private readonly UnitOfWork _unitOfWork = null;
        private List<AppConnTenant> _appConnTenants;

        public TSDalSession(IEnumerable<AppDbConnectionVM> appDbConnections, bool autoBeginTransaction = false,
            IEnumerable<AppConnNameEnum> transactonEnableTenants = null)
        {
            OpenConnections(transactonEnableTenants, appDbConnections);
            _unitOfWork = new UnitOfWork(_appConnTenants, true);
            if (autoBeginTransaction) _unitOfWork.Begin(transactonEnableTenants);
        }

        public UnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        public void Dispose()
        {
            if (_unitOfWork != null) _unitOfWork.Rollback();
            else _unitOfWork?.Dispose();
        }

        private void OpenConnections(IEnumerable<AppConnNameEnum> appConnNames,
            IEnumerable<AppDbConnectionVM> appDbConnections)
        {
            _appConnTenants = new List<AppConnTenant>();
            foreach (AppConnNameEnum eachConnection in appConnNames)
            {
                var connInfo = GetConnString(appDbConnections, eachConnection);
                AppConnTenant appConnTenant = new AppConnTenant()
                {
                    Connection =
                        string.Equals(connInfo?.DbType, "postgresql", StringComparison.InvariantCultureIgnoreCase)
                            ? new NpgsqlConnection(connInfo.ConnectionString) as DbConnection
                            : new SqlConnection(connInfo.ConnectionString) as DbConnection,
                    Transaction = null,
                    Name = eachConnection
                };
                appConnTenant.Connection.Open();
                _appConnTenants.Add(appConnTenant);
            }
        }

        private AppDbConnectionVM GetConnString(IEnumerable<AppDbConnectionVM> appDbConnections,
            AppConnNameEnum appConnName)
        {
            foreach (AppDbConnectionVM appDbConnection in appDbConnections)
            {
                if (((AppConnNameEnum)appDbConnection.ConnectionId).Equals(appConnName))
                    return appDbConnection;
            }

            return null;
        }
    }
}