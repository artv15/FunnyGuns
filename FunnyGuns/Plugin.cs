using System;
using Exiled.API.Features;
using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using lh = FunnyGuns.EventHandler;
using System.Collections.Generic;

namespace FunnyGuns
{
    class Plugin : Plugin<Config>
    {
        public static bool isOverriden = false;
        public static bool isPlayerOverriden = false;

        public static List<Exiled.API.Features.Player> playerlist = new List<Exiled.API.Features.Player>();
        public static System.Collections.Generic.IEnumerable<Exiled.API.Features.Player> active_playerlist = new List<Exiled.API.Features.Player>();
        public static int CountList;

        public static bool isPrep = true;
        public static bool isRunning = false;
        public static int stage;
        public static int secondsTillNextStage;
        

        public override string Author => "Treeshold (aka Star Buttefly) | plz dont hate me";
        public override string Name => "Funny Guns";

        public static void recalculatePlayers()
        {
            Plugin.CountList = 0;
            foreach (var count in Plugin.active_playerlist)
            {
                if (count.Role != RoleType.Spectator)
                {
                    Plugin.CountList += 1;
                }
            }
        }

        public override void OnEnabled()
        {
            Log.Info("╔════════════════════════════════════════╗");
            Log.Info("║            FunnyGuns Event             ║");
            Log.Info("║                 (BETA)                 ║");
            Log.Info("║ Made by Treeshold(aka. Star Butterfly) ║");
            Log.Info("╚════════════════════════════════════════╝");
            Log.Error("Warning! Private Beta! This can be trash or something idk");

            Server.RoundStarted += lh.OnRoundStarted;
            Server.WaitingForPlayers += lh.OnWaitingForPlayers;
            Player.Hurting += lh.OnHurt;
            Player.Joined += lh.PlayerJoined;
            Player.Left += lh.PlayerLeft;
            Player.Died += lh.PlayerFuckingDied;
            Player.ActivatingWarheadPanel += lh.UnlockingWarheadButton;
            Server.RespawningTeam += lh.respawn;
            Exiled.Events.Handlers.Map.Decontaminating += lh.Decont;
        }

        public override void OnDisabled()
        {
            Server.RoundStarted -= lh.OnRoundStarted;
            Server.WaitingForPlayers -= lh.OnWaitingForPlayers;
            Player.Hurting -= lh.OnHurt;
            Player.Joined -= lh.PlayerJoined;
            Player.Left -= lh.PlayerLeft;
            Player.Died -= lh.PlayerFuckingDied;
            Server.RespawningTeam -= lh.respawn;
            Player.ActivatingWarheadPanel -= lh.UnlockingWarheadButton;
            Exiled.Events.Handlers.Map.Decontaminating -= lh.Decont;
        }
    }
}
