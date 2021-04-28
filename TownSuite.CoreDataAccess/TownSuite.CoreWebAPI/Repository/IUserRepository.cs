using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TownSuite.CoreDataAccess;
using TownSuite.CoreDataAccess.Interfaces;

namespace TownSuite.CoreWebAPI.Repository
{
    public interface IUserRepository 
    {
        TownUserRegisterVM InsertNewUserWeb(TownUserRegisterVM user, IUnitOfWork unitOfWork);

        TownUserRegisterVM InsertNewUserFinancial(TownUserRegisterVM user, IUnitOfWork unitOfWork);

        Task<TownUserRegisterVM> InsertNewUserWebAsync(TownUserRegisterVM user, IUnitOfWork unitOfWork);

        Task<TownUserRegisterVM> InsertNewUserFinancialAsync(TownUserRegisterVM user, IUnitOfWork unitOfWork);


    }
}
