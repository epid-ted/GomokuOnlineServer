using MatchServer.Configuration;
using MatchServer.Web.Data;
using MatchServer.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchServer.WaitingQueue
{
    public class StaminaManager
    {
        static StaminaManager instance = new StaminaManager();
        public static StaminaManager Instance { get { return instance; } }

        private const int maxStamina = 120;
        private const int staminaRecoveryTimeInSeconds = 360;

        private StaminaManager() { }

        public async Task<int> GetStamina(int userId)
        {
            // TODO: Refactor
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySQL(ServerConfig.AccountConnectionString);

            using (var dbContext = new AppDbContext(optionsBuilder.Options))
            {
                var staminaInfo = await dbContext.UserStamina.AsNoTracking()
                    .Where(us => us.UserId == userId)
                    .Select(us => new
                    {
                        LastUpdated = us.LastUpdated,
                        Stamina = us.Stamina
                    })
                    .FirstAsync();

                // TODO: NULL check

                DateTime dateTime = DateTime.UtcNow;
                int seconds = (int)(dateTime - staminaInfo.LastUpdated).TotalSeconds;
                int currentStamina = Math.Min(maxStamina, staminaInfo.Stamina + (seconds / staminaRecoveryTimeInSeconds));
                return currentStamina;
            }
        }

        // Stamina must be bigger than "value"
        public async Task<int> ConsumeStamina(int userId, int staminaCost)
        {
            // TODO: Refactor
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySQL(ServerConfig.AccountConnectionString);

            using (var dbContext = new AppDbContext(optionsBuilder.Options))
            {
                UserStamina userStamina = await dbContext.UserStamina
                    .Where(us => us.UserId == userId)
                    .FirstAsync();

                // TODO: NULL check

                DateTime dateTime = DateTime.UtcNow;
                int elapsedTimeInSeconds = (int)(dateTime - userStamina.LastUpdated).TotalSeconds;
                int currentStamina = Math.Min(maxStamina, userStamina.Stamina + (elapsedTimeInSeconds / staminaRecoveryTimeInSeconds));

                // Calculate the last moment when stamina value changed
                if (currentStamina < maxStamina)
                {
                    int mod = elapsedTimeInSeconds % staminaRecoveryTimeInSeconds;
                    dateTime = dateTime.AddSeconds(-mod);
                }

                userStamina.LastUpdated = dateTime;
                userStamina.Stamina = currentStamina - staminaCost;

                await dbContext.SaveChangesAsync();
                return currentStamina;
            }
        }
    }
}
