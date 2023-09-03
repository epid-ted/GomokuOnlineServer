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
                var staminaInfo = await dbContext.Users.AsNoTracking()
                    .Where(u => u.UserId == userId)
                    .Select(u => new
                    {
                        LastStaminaUpdateTime = u.LastStaminaUpdateTime,
                        Stamina = u.Stamina
                    })
                    .FirstAsync();

                // TODO: NULL check

                DateTime dateTimeUtcNow = DateTime.UtcNow;
                int seconds = (int)(dateTimeUtcNow - staminaInfo.LastStaminaUpdateTime).TotalSeconds;
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
                User user = await dbContext.Users
                    .Where(u => u.UserId == userId)
                    .FirstAsync();

                // TODO: NULL check

                DateTime dateTime = DateTime.UtcNow;
                int seconds = (int)(dateTime - user.LastStaminaUpdateTime).TotalSeconds;
                int currentStamina = Math.Min(maxStamina, user.Stamina + (seconds / staminaRecoveryTimeInSeconds));

                // Calculate the last moment when stamina value changed
                if (currentStamina < maxStamina)
                {
                    int mod = seconds % staminaRecoveryTimeInSeconds;
                    dateTime = dateTime.AddSeconds(-mod);
                }

                user.LastStaminaUpdateTime = dateTime;
                user.Stamina = currentStamina - staminaCost;

                await dbContext.SaveChangesAsync();
                return currentStamina;
            }
        }
    }
}
