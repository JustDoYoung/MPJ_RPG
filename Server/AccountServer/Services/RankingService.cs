﻿
using AccountDB;
using Microsoft.EntityFrameworkCore;

namespace AccountServer.Services
{
    public class RankingService
    {
        AccountDbContext _dbcontext;
        
        public RankingService(AccountDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> UpdateScore(string username, int score)
        {
            RankingDb? rankingDb = _dbcontext.Rankings.FirstOrDefault(r => r.Username == username);

            if (rankingDb == null)
            {
                rankingDb = new RankingDb()
                {
                    Username = username
                };
            }

            if (rankingDb.Score > score) return true;

            rankingDb.Score = score;

            _dbcontext.Rankings.Add(rankingDb);
            await _dbcontext.SaveChangesAsync();

            return true;
        }

        internal async Task<List<RankingDb>> GetRankers(int num)
        {
            return await _dbcontext.Rankings
                .OrderByDescending(t => t.Score)
                .Take(num)
                .ToListAsync();
        }
    }
}
