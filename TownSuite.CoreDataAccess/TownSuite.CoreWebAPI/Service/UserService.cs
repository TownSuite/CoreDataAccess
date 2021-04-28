using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TownSuite.CoreDataAccess;
using TownSuite.CoreWebAPI.Repository;
using TownSuite.CoreDTOs.DataAccess;

namespace TownSuite.CoreWebAPI.Service
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;      
        private IEnumerable<AppDbConnectionVM> _appDbConnections;

        public UserService(IUserRepository userRepository, IEnumerable<AppDbConnectionVM> appDbConnections)
        {
            _userRepository = userRepository;         
            _appDbConnections = appDbConnections;
        }

        public TownUserRegisterVM InsertNewUser(TownUserRegisterVM user)
        {            
            IEnumerable<AppConnNameEnum> transactionEnableTenants = new List<AppConnNameEnum>()
            {
                AppConnNameEnum.Web,
                AppConnNameEnum.Financial
            };            

            using (TSDalSession dalSession = new TSDalSession(_appDbConnections, true, transactionEnableTenants))
            {
                var resultWeb = _userRepository.InsertNewUserWeb(user, dalSession.UnitOfWork);
                var resultFinancial = _userRepository.InsertNewUserFinancial(user, dalSession.UnitOfWork);
                dalSession.UnitOfWork.Commit();
            }
            return user;
        }

        public async Task<TownUserRegisterVM> InsertNewUserAsync(TownUserRegisterVM user)
        {
            IEnumerable<AppConnNameEnum> transactionEnableTenants = new List<AppConnNameEnum>()
            {
                AppConnNameEnum.Web,
                AppConnNameEnum.Financial
            };

            using (TSDalSession dalSession = new TSDalSession(_appDbConnections, true, transactionEnableTenants))
            {
                var resultWeb = await _userRepository.InsertNewUserWebAsync(user, dalSession.UnitOfWork);
                var resultFinancial = await _userRepository.InsertNewUserFinancialAsync(user, dalSession.UnitOfWork);
                dalSession.UnitOfWork.Commit();
            }
            return user;
        }       
    }
}
