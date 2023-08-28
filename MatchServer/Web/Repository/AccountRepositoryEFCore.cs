using MatchServer.Web.Data;
using MatchServer.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchServer.Web.Repository
{
    public class AccountRepositoryEFCore : IAccountRepository
    {
        private readonly AppDbContext dbContext;

        public AccountRepositoryEFCore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Update(int userId)
        {
            User? user = await dbContext.Users
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return;
            }

            int seconds = (int)(DateTime.UtcNow - user.LastStaminaUpdateTime).TotalSeconds;
            int currentStamina = Math.Min(120, user.Stamina + (seconds / 360) - 10);

            user.LastStaminaUpdateTime = DateTime.UtcNow;
            user.Stamina = currentStamina;
            await dbContext.SaveChangesAsync();
        }
    }
}
