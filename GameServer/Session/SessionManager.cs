namespace Server.Session
{
    // Singleton
    internal class SessionManager
    {
        static SessionManager instance = new SessionManager();
        public static SessionManager Instance { get { return instance; } }

        object sessionLock = new object();
        Dictionary<int, ClientSession> sessions = new Dictionary<int, ClientSession>();

        private SessionManager() { }

        public ClientSession Create()
        {
            lock (sessionLock)
            {
                ClientSession session = new ClientSession();
                return session;
            }
        }

        public bool Add(int id, ClientSession session)
        {
            lock (sessionLock)
            {
                ClientSession? _session = null;
                if (sessions.TryGetValue(id, out _session))
                {
                    return false;
                }
                else
                {
                    sessions.Add(id, session);
                    return true;
                }
            }
        }

        public ClientSession? Find(int id)
        {
            lock (sessionLock)
            {
                ClientSession? session = null;
                sessions.TryGetValue(id, out session);
                return session;
            }
        }

        public void Remove(int id)
        {
            lock (sessionLock)
            {
                sessions.Remove(id);
            }
        }
    }
}
