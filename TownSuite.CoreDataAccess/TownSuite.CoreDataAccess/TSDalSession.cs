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
        public const string POSTGRES_DBTYPE = "postgresql";
        public const string SQLSERVER_DBTYPE = "sqlserver";
        public const string TOWNSUITE_SQLSERVER_DBTYPE = "townsuite_sqlserver";

        private readonly UnitOfWork _unitOfWork;

        private List<AppConnTenant> _appConnTenants;

        public UnitOfWork UnitOfWork => _unitOfWork;

        public TSDalSession(IEnumerable<AppDbConnectionVM> appDbConnections, bool autoBeginTransaction = false, IEnumerable<AppConnNameEnum> transactonEnableTenants = null)
        {
            OpenConnections(transactonEnableTenants, appDbConnections);
            _unitOfWork = new UnitOfWork(_appConnTenants, enableTransaction: true);
            if (autoBeginTransaction)
            {
                _unitOfWork.Begin(transactonEnableTenants);
            }
        }

        public void Dispose()
        {
            if (_unitOfWork != null)
            {
                _unitOfWork.Rollback();
            }
            else
            {
                _unitOfWork?.Dispose();
            }
        }

        private void OpenConnections(IEnumerable<AppConnNameEnum> appConnNames, IEnumerable<AppDbConnectionVM> appDbConnections)
        {
            _appConnTenants = new List<AppConnTenant>();
            foreach (AppConnNameEnum appConnName in appConnNames)
            {
                AppDbConnectionVM connString = GetConnString(appDbConnections, appConnName);
                DbConnection connection = null;
                if (string.Equals(connString?.DbType, POSTGRES_DBTYPE, StringComparison.InvariantCultureIgnoreCase))
                {
                    connection = new NpgsqlConnection(connString.ConnectionString);
                }
                else if (string.Equals(connString?.DbType, TOWNSUITE_SQLSERVER_DBTYPE, StringComparison.InvariantCultureIgnoreCase))
                {
                    connection = new TownSuite.Data.SqlClient.SqlConnection(connString.ConnectionString);
                }
                else
                {
                    connection = new SqlConnection(connString.ConnectionString);
                }
                AppConnTenant appConnTenant = new AppConnTenant
                {
                    Connection = connection,
                    Transaction = null,
                    Name = appConnName
                };
                appConnTenant.Connection.Open();
                _appConnTenants.Add(appConnTenant);
            }
        }

        private AppDbConnectionVM GetConnString(IEnumerable<AppDbConnectionVM> appDbConnections, AppConnNameEnum appConnName)
        {
            foreach (AppDbConnectionVM appDbConnection in appDbConnections)
            {
                if (((AppConnNameEnum)appDbConnection.ConnectionId).Equals(appConnName))
                {
                    return appDbConnection;
                }
            }
            return null;
        }
    }

}