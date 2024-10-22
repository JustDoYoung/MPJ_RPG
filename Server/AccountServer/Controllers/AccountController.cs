using AccountServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        AccountService _account;

        public AccountController(AccountService account)
        {
            _account = account;
        }

        [HttpPost]
        [Route("login/facebook")]
        public async Task<LoginAccountPacketRes> LoginAccount([FromBody] LoginAccountPacketReq req)
        {
            LoginAccountPacketRes res = await _account.LoginFacebookAccount(req.token);
            return res;
        }

        [HttpPost]
        [Route("login/guest")]
        public async Task<LoginAccountPacketRes> LoginGuest([FromBody] LoginAccountPacketReq req)
        {
            LoginAccountPacketRes res = await _account.LoginGuestAccount(req.userId);
            return res;
        }
    }
}
