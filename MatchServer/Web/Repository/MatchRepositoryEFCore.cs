using MatchServer.Web.Data;
using MatchServer.Web.Data.Entities;
using MatchServer.Web.Data.Models;

namespace MatchServer.Web.Repository
{
    public class MatchRepositoryEFCore : IMatchRepository
    {
        private readonly AppDbContext dbContext;

        public MatchRepositoryEFCore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Add(MatchResultModel matchResultModel)
        {
            MatchResult result = new MatchResult()
            {
                StartTime = matchResultModel.StartTime,
                EndTime = matchResultModel.EndTime,
                Result = matchResultModel.Result,
                User1Id = matchResultModel.Participants[0],
                User2Id = matchResultModel.Participants[1]
            };

            dbContext.Add(result);
            await dbContext.SaveChangesAsync();
        }
    }
}
