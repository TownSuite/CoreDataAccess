using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TownSuite.CoreDataAccess.Interfaces;
using TownSuite.CoreDTOs.DataAccess;

namespace TownSuite.CoreDataAccess
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        Guid _id = Guid.Empty;
        private bool _transactionCommitStatus = false;
        private bool _enableTransaction = false;
        private IEnumerable<AppConnTenant> _appConnTenants;
    
        internal UnitOfWork(IEnumerable<AppConnTenant> appConnTenants, bool enableTransaction=false)
        {
            _id = Guid.NewGuid();
            _appConnTenants = appConnTenants;
            _enableTransaction = enableTransaction; 
        }       

        public IEnumerable<AppConnTenant> TSAppTenant { get { return _appConnTenants; } }

        public IDbTransaction Transaction(AppConnNameEnum appConnName)
        {
            IDbTransaction dbTransaction = null;

            foreach(AppConnTenant appTenant in TSAppTenant)
            {
               if (appTenant.Name.Equals(appConnName))
                    dbTransaction =  appTenant.Transaction;
            }
            return dbTransaction;
        }

        Guid IUnitOfWork.Id
        {
            get { return _id; }
        }


        public void Begin(IEnumerable<AppConnNameEnum> appConnNames)
        {
            if (_enableTransaction)
            foreach (AppConnNameEnum eachTenantName in appConnNames)
            {
                foreach (AppConnTenant tenant in TSAppTenant)
                {
                    if (tenant.Name.Equals(eachTenantName))
                          tenant.Transaction =  tenant.Connection.BeginTransaction();
                }
            }
        }

        public void Commit()
        {
            foreach(AppConnTenant tenant in TSAppTenant)
            {
                if (tenant.Transaction != null)  
                    tenant.Transaction.Commit();                                   
            }
            _transactionCommitStatus = true;
            Dispose();
        }

        public void Rollback()
        {
            if (!_transactionCommitStatus)
            {
                foreach (AppConnTenant tenant in TSAppTenant)
                {
                    if (tenant.Transaction != null)
                        tenant.Transaction.Rollback();
                }
            }
            Dispose();
        }

        public void Dispose()
        {
            foreach (AppConnTenant tenant in TSAppTenant)
            {            
                tenant?.Transaction?.Dispose();
                tenant?.Connection?.Dispose();
            }
        }
    }
}
