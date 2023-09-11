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

        public async Task AddRanking(string username)
        {
            await sessionRepository.AddRanking(username);
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
            HttpClient httpClientForMatchServer = HttpClientConfig.HttpClientForMatchServer;
            await httpClientForMatchServer.PostAsync($"session/kickout/{userId}", null);

            HttpClient httpClientForGameServer = HttpClientConfig.HttpClientForGameServer;
            await httpClientForGameServer.PostAsync($"session/kickout/{userId}", null);
        }
    }
}
