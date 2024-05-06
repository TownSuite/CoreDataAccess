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

        private bool _transactionCommitStatus = false;

        private bool _enableTransaction = false;

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
            foreach (AppConnTenant appTenant in TSAppTenant)
            {
                if (appTenant.Name.Equals(appConnName))
                {
                    result = appTenant.Transaction;
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
                foreach (AppConnTenant tenant in TSAppTenant)
                {
                    if (tenant.Name.Equals(appConnName))
                    {
                        tenant.Transaction = tenant.Connection.BeginTransaction();
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

        public void Dispose()
        {
            foreach (AppConnTenant item in TSAppTenant)
            {
                if (item.Transaction != null)
                {
                    item.Transaction.Dispose();
                }
                if (item.Connection != null && item.Connection.State == ConnectionState.Open)
                {
                    item.Connection.Close();
                    item.Connection.Dispose();
                }
            }
        }
    }
}
