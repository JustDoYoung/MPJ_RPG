using AccountDB;
using AccountServer.Data;

namespace AccountServer.Services
{
    public class AccountService
    {
        AccountDbContext _dbContext;
        FacebookService _facebookService;
        JwtTokenService _jwt;

        public AccountService(AccountDbContext context, FacebookService facebookService, JwtTokenService jwt)
        {
            _dbContext = context;
            _facebookService = facebookService;
            _jwt = jwt;
        }

        internal async Task<LoginAccountPacketRes> LoginFacebookAccount(string token)
        {
            LoginAccountPacketRes res = new LoginAccountPacketRes();

            FacebookTokenData? tokenData = await _facebookService.GetUserTokenData(token);

            if (tokenData == null || tokenData.is_valid == false) return res;

            AccountDb? accountDb = _dbContext.Accounts.FirstOrDefault(a => a.LoginProviderUserId == tokenData.user_id && a.LoginProviderType == ProviderType.Facebook);

            if (accountDb == null)
            {
                accountDb = new AccountDb()
                {
                    LoginProviderUserId = tokenData.user_id,
                    LoginProviderType = ProviderType.Facebook
                };

                _dbContext.Accounts.Add(accountDb);
                await _dbContext.SaveChangesAsync();
            }

            res.success = true;
            res.accountDbId = accountDb.AccountDbId;
            res.jwt = _jwt.CreateJwtAccessToken(accountDb.AccountDbId);

            return res;
        }

        public async Task<LoginAccountPacketRes> LoginGuestAccount(string userID)
        {
            LoginAccountPacketRes res = new LoginAccountPacketRes();


            return res;
        }
    }
}
