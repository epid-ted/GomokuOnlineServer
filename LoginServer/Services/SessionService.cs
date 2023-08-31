using LoginServer.Configuration;
using LoginServer.Data.DTOs.GameServer;
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

        public async Task<SessionModel> EnterGame(int userId)
        {
            string sessionId = GenerateSessionId();

            await semaphore.WaitAsync();
            try
            {
                string? prevSessionId = await sessionRepository.Find(userId);
                if (prevSessionId != null)
                {
                    await Kickout(userId, prevSessionId);
                }
                await sessionRepository.Add(userId, sessionId);
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
                    await Kickout(userId, sessionId);
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

        private async Task Kickout(int userId, string sessionId)
        {
            await RequestKickout(userId, sessionId);
            await sessionRepository.Remove(userId);
        }

        private async Task RequestKickout(int userId, string sessionId)
        {
            KickoutRequestDto kickoutRequestDto = new KickoutRequestDto()
            {
                UserId = userId,
                SessionId = sessionId
            };

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ServerConfig.MatchServer);
                await httpClient.PostAsJsonAsync("session/kickout", kickoutRequestDto);
            }

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ServerConfig.GameServer);
                await httpClient.PostAsJsonAsync("session/kickout", kickoutRequestDto);
            }
        }
    }
}
