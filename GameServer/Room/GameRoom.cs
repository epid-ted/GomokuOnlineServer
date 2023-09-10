using GameServer.Configuration;
using GameServer.Job;
using GameServer.Web.Data.DTOs;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using Google.Protobuf.WellKnownTypes;
using Server.Session;
using System.Net.Http.Headers;

namespace GameServer.Room
{
    public class GameRoom : JobQueue
    {
        public int RoomId { get; set; }
        public GameBoard Board { get; private set; } = new GameBoard();
        public int[] PlayerIds { get; set; } = new int[2];
        public ClientSession[] Players { get; set; } = new ClientSession[2];

        // State of the game
        int state = 0;

        // Number of entered players
        int enteredPlayers = 0;

        // Current turn
        int turn = 1;

        // Number of stones
        int numOfStones = 0;

        // Timer
        Timer enterTimer;
        Timer gameTimer;

        // Timer Callback
        TimerCallback enterTimerCallback;
        TimerCallback gameTimerCallback;

        // Time Limit
        const int enterTimeLimit = 30000;
        const int gameTimeLimit = 30000;
        const int extraTime = 1000;

        // Time
        DateTime createdTime = DateTime.UtcNow;
        DateTime startTime;
        DateTime endTime;
        DateTime lastMoveTime;

        public GameRoom()
        {
            enterTimerCallback = (o) => { Push(End, -1, -1, -1, -1); };
            gameTimerCallback = (o) => { Push(End, -1, -1, 3 - turn, 3 - turn); };

            enterTimer = new Timer(enterTimerCallback, null, enterTimeLimit + extraTime, Timeout.Infinite);
            gameTimer = new Timer(gameTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void HandleEnter(ClientSession session, C_Enter packet)
        {
            // Check if the player belongs to this Room
            if (!PlayerIds.Contains(session.SessionId))
            {
                return;
            }

            // Prevent duplicate entry
            if (enteredPlayers == 2)
            {
                return;
            }

            // Check if the time has exceeded.
            if (CheckTimeOver(createdTime, enterTimeLimit))
            {
                return;
            }

            // Add player to this Room
            session.Room = this;

            enteredPlayers++;

            // Start the game when all players have joined
            if (enteredPlayers == 2)
            {
                Start();
            }
        }

        public void HandleMove(ClientSession session, C_Move packet)
        {
            // Check if the game is currently in progress
            if (state == 0)
            {
                return;
            }

            // Check if it's the player's turn
            if (PlayerIds[turn - 1] != session.SessionId)
            {
                return;
            }

            // Check if the position is valid for placing a stone
            if (!Board.CanPlaceStone(packet.Y, packet.X))
            {
                return;
            }
            Board.PlaceStone(packet.Y, packet.X, turn);
            numOfStones++;

            // Check if the time has exceeded.
            if (CheckTimeOver(lastMoveTime, gameTimeLimit))
            {
                return;
            }
            else
            {
                // Reset gameTimer
                gameTimer.Change(gameTimeLimit + extraTime, Timeout.Infinite);
                lastMoveTime = DateTime.UtcNow;
            }

            if (Board.CheckGameOver(packet.Y, packet.X))
            {
                End(packet.Y, packet.X, turn, turn);
            }
            else if (numOfStones == 15 * 15)
            {
                // If the game board is filled up, it's a draw
                End(packet.Y, packet.X, turn, 0);
            }
            else
            {
                S_Move responsePacket = new S_Move()
                {
                    Y = packet.Y,
                    X = packet.X,
                    Turn = turn,
                    Time = Timestamp.FromDateTime(lastMoveTime)
                };
                Broadcast(responsePacket);
                turn = 3 - turn;
            }
        }

        public void HandleLeave(ClientSession session)
        {
            // Check if the game is currently in progress
            if (state == 0)
            {
                return;
            }

            for (int i = 0; i < PlayerIds.Length; i++)
            {
                if (PlayerIds[i] == session.SessionId)
                {
                    End(-1, -1, -1, 3 - (i + 1));
                    return;
                }
            }
        }

        public void Update()
        {
            Flush();
        }

        private void Start()
        {
            // Change game state
            state = 1;

            // Cancel enterTimer
            startTime = DateTime.UtcNow;
            enterTimer.Change(Timeout.Infinite, Timeout.Infinite);

            // Start gameTimer
            lastMoveTime = DateTime.UtcNow;
            gameTimer.Change(gameTimeLimit + extraTime, Timeout.Infinite);

            //Players[]
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i] = SessionManager.Instance.Find(PlayerIds[i]);
            }

            for (int i = 0; i < Players.Length; i++)
            {
                S_Start packet = new S_Start()
                {
                    Turn = i + 1,
                    Time = Timestamp.FromDateTime(lastMoveTime)
                };
                Players[i].Send(packet);
            }
        }

        // Return value
        // - Error: returns -1
        // - Draw: returns 0
        // - Player 1 wins: returns 1
        // - Player 2 wins: returns 2
        private void End(int y, int x, int turn, int result)
        {
            if (state == 0)
            {
                return;
            }
            state = 0;

            S_End packet = new S_End()
            {
                Y = y,
                X = x,
                Turn = turn,
                Result = result
            };
            Broadcast(packet);

            // Save the result of this match
            endTime = DateTime.UtcNow;
            SaveMatchResult(result);

            // Clean up the room when the game is finished
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i] = null;
            }
            RoomManager.Instance.Remove(RoomId);
        }

        private void Broadcast(IMessage packet)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i].Send(packet);
            }
        }

        private bool CheckTimeOver(DateTime prvTime, int timeLimit)
        {
            DateTime curTime = DateTime.UtcNow;
            int diff = (int)(curTime - prvTime).TotalMilliseconds;
            return diff > timeLimit;
        }

        private async Task SaveMatchResult(int result)
        {
            string[] PlayerNames = { Players[0].Username, Players[1].Username };

            SaveMatchResultRequestDto saveMatchResultRequestDto = new SaveMatchResultRequestDto()
            {
                StartTime = startTime,
                EndTime = endTime,
                Result = result,
                UserIds = PlayerIds,
                Usernames = PlayerNames
            };

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ServerConfig.MatchServerPrivateAddress);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ServerSessionId", ServerConfig.ServerSessionId);
                await httpClient.PostAsJsonAsync("match/result", saveMatchResultRequestDto);
            }
        }
    }
}
