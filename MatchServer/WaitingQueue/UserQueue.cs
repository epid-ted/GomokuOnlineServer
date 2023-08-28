using Google.Protobuf.MatchProtocol;
using MatchServer.Configuration;
using MatchServer.Web.Data.DTOs.GameServer;
using Server.Session;
using System.Text.Json;

namespace MatchServer.WaitingQueue
{
    // Singleton
    public class UserQueue
    {
        private static UserQueue instance = new UserQueue();
        public static UserQueue Instance { get { return instance; } }

        static Random rnd = new Random();

        object _lock = new object();
        SortedDictionary<int, ClientSession> waitingUsers = new SortedDictionary<int, ClientSession>();

        private UserQueue() { }

        public async Task Add(ClientSession session)
        {
            // Check if the user has enough stamina
            if (!await HasEnoughStamina(session))
            {
                S_Response sResponsePacket = new S_Response()
                {
                    Successed = false
                };
                session.Send(sResponsePacket);
                return;
            }

            int[] participants = new int[2];

            lock (_lock)
            {
                int userId = session.SessionId;

                // Check if the user already added to queue
                if (waitingUsers.ContainsKey(userId))
                {
                    return;
                }

                waitingUsers.Add(userId, session);
                if (waitingUsers.Count == 2)
                {
                    for (int i = 0; i < participants.Length; i++)
                    {
                        participants[i] = Pop();
                    }
                }
            }

            // Send packet
            S_Response packet = new S_Response()
            {
                Successed = true
            };
            session.Send(packet);

            // For random turn
            if (rnd.Next(0, 2) == 1)
            {
                int tmp = participants[0];
                participants[0] = participants[1];
                participants[1] = tmp;
            }

            CreateRoomRequest(participants);
        }

        public void Remove(int userId)
        {
            lock (_lock)
            {
                waitingUsers.Remove(userId);
            }
        }

        public int Pop()
        {
            int userId = waitingUsers.Keys.Min();
            waitingUsers.Remove(userId);
            return userId;
        }

        private async Task CreateRoomRequest(int[] participants)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                CreateRoomRequestDto createRoomRequestDto = new CreateRoomRequestDto()
                {
                    Participants = participants
                };

                httpClient.BaseAddress = new Uri(ServerConfig.GameServerPrivateAddress);
                HttpResponseMessage response = await httpClient.PostAsJsonAsync("room/create", createRoomRequestDto);

                string responseBody = await response.Content.ReadAsStringAsync();

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                CreateRoomResponseDto createRoomResponseDto = JsonSerializer.Deserialize<CreateRoomResponseDto>(responseBody, options);

                int roomId = createRoomResponseDto.RoomId;
                JoinRoomRequest(participants, roomId);
            }
        }

        private void JoinRoomRequest(int[] participants, int roomId)
        {
            foreach (int i in participants)
            {
                ClientSession? clientSession = SessionManager.Instance.Find(i);
                if (clientSession != null)
                {
                    S_Ready packet = new S_Ready() { RoomId = roomId };
                    clientSession.Send(packet);
                }
            }
        }

        private async Task<bool> HasEnoughStamina(ClientSession session)
        {
            return await session.GetStamina(session.SessionId) >= 10;
        }
    }
}
