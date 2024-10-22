using AccountServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountServer.Controllers
{
    [Route("api/ranking")]
    [ApiController]
    public class RankingController : ControllerBase
    {
        RankingService _ranking;
        JwtTokenService _jwt;
        public RankingController(RankingService ranking, JwtTokenService jwt)
        {
            _ranking = ranking;
            _jwt = jwt;
        }

        [HttpPost]
        [Route("update")]
        public async Task<UpdateRankingPacketRes> UpdateRanking([FromBody] UpdateRankingPacketReq req)
        {
            UpdateRankingPacketRes res = new UpdateRankingPacketRes();
            res.success = false;

            bool auth = _jwt.ValidateJwtAccessToken(req.jwt);

            if (auth == false) return res;

            string? username = _jwt.GetUsernameFromToken(req.jwt);

            if (string.IsNullOrEmpty(username)) return res;

            res.success = await _ranking.UpdateScore(username, req.score);

            return res;

        }
    }
}
