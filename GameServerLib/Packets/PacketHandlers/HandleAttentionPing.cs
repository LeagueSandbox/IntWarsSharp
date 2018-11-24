﻿using GameServerCore;
using GameServerCore.Packets.Enums;
using GameServerCore.Packets.Handlers;
using GameServerCore.Packets.PacketDefinitions.Requests;

namespace LeagueSandbox.GameServer.Packets.PacketHandlers
{
    public class HandleAttentionPing : PacketHandlerBase<AttentionPingRequest>
    {
        private readonly Game _game;
        private readonly IPlayerManager _playerManager;

        public override PacketCmd PacketType => PacketCmd.PKT_C2S_ATTENTION_PING;
        public override Channel PacketChannel => Channel.CHL_C2S;

        public HandleAttentionPing(Game game)
        {
            _game = game;
            _playerManager = game.PlayerManager;
        }

        public override bool HandlePacket(int userId, AttentionPingRequest req)
        {
            var client = _playerManager.GetPeerInfo(userId);
            _game.PacketNotifier.NotifyPing(client, req.X, req.Y, req.TargetNetId, req.Type);
            return true;
        }
    }
}
