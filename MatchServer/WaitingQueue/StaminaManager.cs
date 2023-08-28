using MatchServer.Configuration;
using MatchServer.Web.Data;
using MatchServer.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchServer.WaitingQueue
{
    public static class StaminaManager
    {
        // static
        public static async Task<int> GetStamina(int userId)
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
                int currentStamina = Math.Min(120, staminaInfo.Stamina + (seconds / 360));
                return currentStamina;
            }
        }

        // Stamina must be bigger than "value"
        public static async Task<int> ConsumeStamina(int userId, int value)
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
                int currentStamina = Math.Min(120, user.Stamina + (seconds / 360) - value);

                // Calculate the last moment when stamina value changed
                int mod = seconds % 360;
                dateTime.AddSeconds(-mod);

                user.LastStaminaUpdateTime = dateTime;
                user.Stamina = currentStamina;

                await dbContext.SaveChangesAsync();
                return currentStamina;
            }
        }
    }
}
