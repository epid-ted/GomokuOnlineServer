using MatchServer.Web.Data;
using MatchServer.Web.Data.Entities;
using MatchServer.Web.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MatchServer.Web.Repository
{
    public class StaminaRepositoryEFCore : IStaminaRepository
    {
        private readonly AppDbContext dbContext;

        public StaminaRepositoryEFCore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<StaminaModel> Get(int userId)
        {
            UserStamina? userStamina = await dbContext.UserStamina
                .Where(us => us.UserId == userId)
                .FirstOrDefaultAsync();

            if (userStamina == null)
            {
                return new StaminaModel()
                {
                    Stamina = -1
                };
            }

            return new StaminaModel()
            {
                LastUpdated = userStamina.LastUpdated,
                Stamina = userStamina.Stamina
            };
        }

        public async Task AddStamina(int userId, int value)
        {
            UserStamina? userStamina = await dbContext.UserStamina
                .Where(us => us.UserId == userId)
                .FirstOrDefaultAsync();

            if (userStamina == null)
            {
                return;
            }

            int seconds = (int)(DateTime.UtcNow - userStamina.LastUpdated).TotalSeconds;
            int currentStamina = Math.Min(120, userStamina.Stamina + (seconds / 360) + value);

            userStamina.LastUpdated = DateTime.UtcNow;
            userStamina.Stamina = currentStamina;
            await dbContext.SaveChangesAsync();
        }
    }
}
