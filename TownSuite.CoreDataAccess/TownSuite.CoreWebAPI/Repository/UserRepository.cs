using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TownSuite.CoreDataAccess;
using TownSuite.CoreDataAccess.Interfaces;
using TownSuite.CoreDTOs.DataAccess;

namespace TownSuite.CoreWebAPI.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {

        #region Private Declaration
        
        private const string SP_INSERT_NEW_TOWN_USER_WEB = "Insert_TownUserData";
        private const string SP_SP_INSERT_NEW_TOWN_USER_FINANCIAL = "Insert_TownUserData";             
        #endregion

        public UserRepository(IUnitOfWork unitOfWork=null):base(unitOfWork)
        {

        }

        public TownUserRegisterVM InsertNewUserFinancial(TownUserRegisterVM user, IUnitOfWork unitOfWork)
        {
            return WithConnection(c =>
            {
                c.Execute(SP_SP_INSERT_NEW_TOWN_USER_FINANCIAL, PrepareTownUserInsertUpdateParameter(user), 
                    transaction: unitOfWork.Transaction(AppConnNameEnum.Financial), commandType: CommandType.StoredProcedure);
                return user;
            },unitOfWork, AppConnNameEnum.Financial);
        }

        public async Task<TownUserRegisterVM> InsertNewUserFinancialAsync(TownUserRegisterVM user, IUnitOfWork unitOfWork)
        {
            return await WithConnectionAsync(async c =>
            {
                await c.ExecuteAsync(SP_SP_INSERT_NEW_TOWN_USER_FINANCIAL, PrepareTownUserInsertUpdateParameter(user), 
                    transaction: unitOfWork.Transaction(AppConnNameEnum.Financial), commandType: CommandType.StoredProcedure);
                return user;
            }, unitOfWork, AppConnNameEnum.Financial);
        }

        public TownUserRegisterVM InsertNewUserWeb(TownUserRegisterVM user, IUnitOfWork unitOfWork)
        {
            return WithConnection(c =>
            {
                c.Execute(SP_INSERT_NEW_TOWN_USER_WEB, PrepareTownUserInsertUpdateParameter(user), 
                    transaction: unitOfWork.Transaction(AppConnNameEnum.Web), commandType: CommandType.StoredProcedure);
                return user;
            },unitOfWork, AppConnNameEnum.Web);
        }

        public async Task<TownUserRegisterVM> InsertNewUserWebAsync(TownUserRegisterVM user, IUnitOfWork unitOfWork)
        {
            return await WithConnectionAsync(async c =>
            {
                await c.ExecuteAsync(SP_INSERT_NEW_TOWN_USER_WEB, PrepareTownUserInsertUpdateParameter(user), transaction: unitOfWork.Transaction(AppConnNameEnum.Web), commandType: CommandType.StoredProcedure);
                return user;
            }, unitOfWork, AppConnNameEnum.Web);
        }


        private object PrepareTownUserInsertUpdateParameter(TownUserRegisterVM townUserRegisterVM)
        {
            return new
            {
                Id = townUserRegisterVM.Id,
                Name = townUserRegisterVM.Name,
                TownCode = townUserRegisterVM.TownCode,
               
            };
        }
    }
}
