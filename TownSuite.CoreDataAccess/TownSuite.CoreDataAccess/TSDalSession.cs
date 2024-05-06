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
                _unitOfWork.Dispose();
            }
        }

        private void OpenConnections(IEnumerable<AppConnNameEnum> appConnNames, IEnumerable<AppDbConnectionVM> appDbConnections)
        {
            _appConnTenants = new List<AppConnTenant>();
            foreach (AppConnNameEnum appConnName in appConnNames)
            {
                AppConnTenant appConnTenant = new AppConnTenant
                {
                    Connection = new SqlConnection(GetConnString(appDbConnections, appConnName)),
                    Transaction = null,
                    Name = appConnName
                };
                appConnTenant.Connection.Open();
                _appConnTenants.Add(appConnTenant);
            }
        }

        private string GetConnString(IEnumerable<AppDbConnectionVM> appDbConnections, AppConnNameEnum appConnName)
        {
            foreach (AppDbConnectionVM appDbConnection in appDbConnections)
            {
                if (appDbConnection.ConnectionId.Equals(appConnName))
                {
                   return appDbConnection.ConnectionString;
                }
            }
            return null;
        }
    }
}