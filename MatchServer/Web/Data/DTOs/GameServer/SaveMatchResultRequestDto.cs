﻿namespace MatchServer.Web.Data.DTOs.GameServer
{
    public class SaveMatchResultRequestDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Result { get; set; }
        public int[] Participants { get; set; }
    }
}
