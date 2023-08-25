namespace GameServer.Room
{
    public class GameBoard
    {
        int[,] board = new int[15, 15];
        int[] dy = new int[4] { 0, 1, 1, 1 };
        int[] dx = new int[4] { 1, 0, 1, -1 };

        public bool CanPlaceStone(int y, int x)
        {
            if (IsOutsideBoard(y, x))
            {
                return false;
            }
            return board[y, x] == 0;
        }

        // Returns true if the game ends
        public void PlaceStone(int y, int x, int turn)
        {
            board[y, x] = turn;
        }

        public bool CheckGameOver(int y, int x)
        {
            for (int i = 0; i < 4; i++)
            {
                int cnt = 1;

                for (int j = 1; j < 15; j++)
                {
                    int ny = y + dy[i] * j;
                    int nx = x + dx[i] * j;
                    if (IsOutsideBoard(ny, nx) || board[ny, nx] != board[y, x])
                    {
                        break;
                    }
                    else
                    {
                        cnt++;
                    }
                }

                for (int j = 1; j < 15; j++)
                {
                    int ny = y - dy[i] * j;
                    int nx = x - dx[i] * j;
                    if (IsOutsideBoard(ny, nx) || board[ny, nx] != board[y, x])
                    {
                        break;
                    }
                    else
                    {
                        cnt++;
                    }
                }

                if (cnt == 5)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsOutsideBoard(int y, int x)
        {
            return y < 0 || y >= 15 || x < 0 || x >= 15;
        }
    }
}
