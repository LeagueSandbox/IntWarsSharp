﻿using GameServerCore.Domain;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Content;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiMapFunctionManager;
using System.Collections.Generic;
using GameServerCore.Enums;

namespace MapScripts.Map1
{
    public class URF : CLASSIC
    {
        public override MapScriptMetadata MapScriptMetadata { get; set; } = new MapScriptMetadata
        {
            MaxLevel = 30
        };
        public override void Init(Dictionary<GameObjectTypes, List<MapObject>> mapObjects)
        {
            base.Init(mapObjects);
            SetGameFeatures(FeatureFlags.EnableManaCosts, false);
            GlobalData.GlobalCharacterDataConstants.PercentCooldownModMinimum = -0.8f;
        }

        public override void OnMatchStart()
        {
            base.OnMatchStart();
            foreach(var player in GetAllPlayers())
            {
                //There are mentions to a "rewindcof" buff being loaded too, but it's functions are yet unknown
                AddBuff("InternalTestBuff", float.MaxValue, 1, null, player, null, true);
            }
        }
    }
}
