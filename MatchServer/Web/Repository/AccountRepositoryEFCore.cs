using MatchServer.Web.Data;
using MatchServer.Web.Data.Entities;
using MatchServer.Web.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MatchServer.Web.Repository
{
    public class AccountRepositoryEFCore : IAccountRepository
    {
        private readonly AppDbContext dbContext;

        public AccountRepositoryEFCore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<StaminaModel> Get(int userId)
        {
            User? user = await dbContext.Users
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return new StaminaModel()
                {
                    Stamina = -1
                };
            }

            return new StaminaModel()
            {
                LastStaminaUpdateTime = user.LastStaminaUpdateTime,
                Stamina = user.Stamina
            };
        }

        public async Task Add(int userId, int value)
        {
            User? user = await dbContext.Users
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return;
            }

            int seconds = (int)(DateTime.UtcNow - user.LastStaminaUpdateTime).TotalSeconds;
            int currentStamina = Math.Min(120, user.Stamina + (seconds / 360) + value);

            user.LastStaminaUpdateTime = DateTime.UtcNow;
            user.Stamina = currentStamina;
            await dbContext.SaveChangesAsync();
        }
    }
}
