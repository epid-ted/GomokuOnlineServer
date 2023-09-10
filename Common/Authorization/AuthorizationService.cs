namespace Common.Authorization
{
    public class AuthorizationService
    {
        private readonly IAuthorizationRepository authorizationRepository;

        public AuthorizationService(IAuthorizationRepository authorizationRepository)
        {
            this.authorizationRepository = authorizationRepository;
        }

        public async Task<bool> AuthorizeHttpRequestFromUser(int userId, string sessionId)
        {
            return await authorizationRepository.FindUser(userId, sessionId);
        }

        public async Task<bool> AuthorizeHttpRequestFromServer(string serverName, string serverId)
        {
            return await authorizationRepository.FindServer(serverName, serverId);
        }
    }
}
