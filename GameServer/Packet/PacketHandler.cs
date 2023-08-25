using GameServer.Room;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using NetworkLibrary;
using Server.Session;

namespace Server.Packet
{
    internal class PacketHandler
    {
        public static void C_EnterHandler(PacketSession session, IMessage packet)
        {
            ClientSession clientSession = session as ClientSession;
            C_Enter cEnterPacket = packet as C_Enter;

            GameRoom? room = RoomManager.Instance.Find(cEnterPacket.RoomId);
            if (room == null)
            {
                return;
            }
            else
            {
                room.Push(room.HandleEnter, clientSession, cEnterPacket);
            }
        }

        public static void C_MoveHandler(PacketSession session, IMessage packet)
        {
            ClientSession clientSession = session as ClientSession;
            C_Move cMovePacket = packet as C_Move;

            GameRoom? room = clientSession.Room;
            if (room == null)
            {
                return;
            }
            else
            {
                room.Push(room.HandleMove, clientSession, cMovePacket);
            }
        }
    }
}
