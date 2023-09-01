using LoginServer.Configuration;
using LoginServer.Data.Models;
using LoginServer.Repositories;

namespace LoginServer.Services
{
    public class SessionService
    {
        private readonly ISessionRepository sessionRepository;
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public SessionService(ISessionRepository sessionRepository)
        {
            this.sessionRepository = sessionRepository;
        }

        public async Task<SessionModel> EnterGame(int userId, string username)
        {
            string sessionId = GenerateSessionId();

            await semaphore.WaitAsync();
            try
            {
                string? prevSessionId = await sessionRepository.Find(userId);
                if (prevSessionId != null)
                {
                    await Kickout(userId);
                }
                await sessionRepository.Add(userId, username, sessionId);
            }
            finally
            {
                semaphore.Release();
            }
            return new SessionModel() { SessionId = sessionId };
        }

        public async Task<bool> LeaveGame(int userId, string sessionId)
        {
            await semaphore.WaitAsync();
            try
            {
                string? _sessionId = await sessionRepository.Find(userId);
                if (sessionId == _sessionId)
                {
                    await Kickout(userId);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private string GenerateSessionId()
        {
            return Guid.NewGuid().ToString();
        }

        private async Task Kickout(int userId)
        {
            await RequestKickout(userId);
            await sessionRepository.Remove(userId);
        }

        private async Task RequestKickout(int userId)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ServerConfig.MatchServerPrivateAddress);
                await httpClient.PostAsync($"session/kickout?userId={userId}", null);
            }

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ServerConfig.GameServerPrivateAddress);
                await httpClient.PostAsync($"session/kickout?userId={userId}", null);
            }
        }
    }
}
