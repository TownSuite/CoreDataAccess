using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TownSuite.CoreWebAPI.Service;
using TownSuite.CoreWebAPI.ViewModel;

namespace TownSuite.CoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Insert")]
        public WebAppResponse InsertNewUser([FromBody] TownUserRegisterVM townUserVM)
        {
            WebAppResponse result = new WebAppResponse();
            try
            {
                result.IsSuucess = true;
                _userService.InsertNewUser(townUserVM);
            }
            catch (Exception ex)
            {

                result.Error = ex.Message;
                result.ErrorDEsciption = ex.StackTrace;
            }

            return result;
        }

        [HttpPost("InsertAsync")]
        public async Task<WebAppResponse> InsertNewUserAsync([FromBody] TownUserRegisterVM townUserVM)
        {
            WebAppResponse result = new WebAppResponse();
            try
            {
                result.IsSuucess = true;
                await _userService.InsertNewUserAsync(townUserVM);
            }
            catch (Exception ex)
            {

                result.Error = ex.Message;
                result.ErrorDEsciption = ex.StackTrace;
            }

            return result;
        }

    }
}
