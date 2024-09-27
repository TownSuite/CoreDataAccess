using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TownSuite.CoreDataAccess.Interfaces;
using TownSuite.CoreDTOs.DataAccess;

namespace TownSuite.CoreDataAccess
{
    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private Guid _id = Guid.Empty;

        private bool _transactionCommitStatus;

        private bool _enableTransaction;

        private IEnumerable<AppConnTenant> _appConnTenants;

        public IEnumerable<AppConnTenant> TSAppTenant => _appConnTenants;

        Guid IUnitOfWork.Id => _id;

        internal UnitOfWork(IEnumerable<AppConnTenant> appConnTenants, bool enableTransaction = false)
        {
            _id = Guid.NewGuid();
            _appConnTenants = appConnTenants;
            _enableTransaction = enableTransaction;
        }

        public IDbTransaction Transaction(AppConnNameEnum appConnName)
        {
            IDbTransaction result = null;
            foreach (AppConnTenant item in TSAppTenant)
            {
                if (item.Name.Equals(appConnName))
                {
                    result = item.Transaction;
                }
            }
            return result;
        }

        public IDbConnection Connection(AppConnNameEnum appConnName)
        {
            IDbConnection result = null;
            foreach (AppConnTenant appTenant in TSAppTenant)
            {
                if (appTenant.Name.Equals(appConnName))
                {
                    result = appTenant.Connection;
                }
            }
            return result;
        }

        public void Begin(IEnumerable<AppConnNameEnum> appConnNames)
        {
            if (!_enableTransaction)
            {
                return;
            }
            foreach (AppConnNameEnum appConnName in appConnNames)
            {
                foreach (AppConnTenant item in TSAppTenant)
                {
                    if (item.Name.Equals(appConnName))
                    {
                        item.Transaction = item.Connection.BeginTransaction();
                    }
                }
            }
        }

        public void Commit()
        {
            foreach (AppConnTenant item in TSAppTenant)
            {
                if (item.Transaction != null)
                {
                    item.Transaction.Commit();
                }
            }
            _transactionCommitStatus = true;
            Dispose();
        }

        public void Rollback()
        {
            if (!_transactionCommitStatus)
            {
                foreach (AppConnTenant item in TSAppTenant)
                {
                    if (item.Transaction != null)
                    {
                        item.Transaction.Rollback();
                    }
                }
            }
            Dispose();
        }

        /// <summary>
        /// Used to help the transition to TownSuite.Data.SqlClient
        /// </summary>
        /// <param name="connName"></param>
        /// <returns></returns>
        public TownSuite.Data.SqlClient.SqlConnection TownSuiteDataConnection(AppConnNameEnum connName)
        {
            return (TownSuite.Data.SqlClient.SqlConnection)Connection(connName);
        }

        /// <summary>
        /// Used to help the transition to TownSuite.Data.SqlClient
        /// </summary>
        /// <param name="connName"></param>
        /// <returns></returns>
        public TownSuite.Data.SqlClient.SqlTransaction TownSuiteDataTransaction(AppConnNameEnum connName)
        {
            return (TownSuite.Data.SqlClient.SqlTransaction)Transaction(connName);
        }

        public void Dispose()
        {
            foreach (AppConnTenant item in TSAppTenant)
            {
                item?.Transaction?.Dispose();
                item?.Connection?.Dispose();
            }
        }
    }
}
