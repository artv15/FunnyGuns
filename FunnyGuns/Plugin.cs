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

        //public static List<Exiled.API.Features.Player> playerlist = new List<Exiled.API.Features.Player>();
        public static System.Collections.Generic.IEnumerable<Exiled.API.Features.Player> active_playerlist = new List<Exiled.API.Features.Player>();
        public static int CountList;
        public static bool isMTFBigger = false;

        public static bool isPrep = true;
        public static bool isRunning = false;
        public static int stage;
        public static int secondsTillNextStage;
        

        public override string Author => "Treeshold (aka Star Buttefly) | plz dont hate me";
        public override string Name => "Funny Guns";

        public static void recalculatePlayers()
        {
            active_playerlist = Exiled.API.Features.Player.List;
            CountList = 0;
            foreach (var pl in active_playerlist)
            {
                CountList += 1;
            }
        }

        public override void OnEnabled()
        {
            Log.Info("╔══════════════════════════════════════════════╗");
            Log.Info("║               FunnyGuns Event                ║");
            Log.Info("║                   (BETA)                     ║");
            Log.Info("║ Made by Treeshold#0001 (aka. Star Butterfly) ║");
            Log.Info("║    Beta Build Identifier(BBI): 000005x000    ║");
            Log.Info("║  if you want to send feedback, include BBI!  ║");
            Log.Info("╚══════════════════════════════════════════════╝");
            Log.Warn("Warning! Beta! Some things may or may not work!");

            Player.DroppingAmmo += lh.droppingAmmo;
            Server.RoundStarted += lh.OnRoundStarted;
            Server.WaitingForPlayers += lh.OnWaitingForPlayers;
            Player.Hurting += lh.OnHurt;
            Player.ChangingRole += lh.PlayerFuckingDied;
            Player.ReloadingWeapon += lh.reloading;
            Player.ActivatingWarheadPanel += lh.UnlockingWarheadButton;
            Server.RespawningTeam += lh.respawn;
            Exiled.Events.Handlers.Map.Decontaminating += lh.Decont;
        }

        public override void OnDisabled()
        {
            Player.DroppingAmmo -= lh.droppingAmmo;
            Server.RoundStarted -= lh.OnRoundStarted;
            Server.WaitingForPlayers -= lh.OnWaitingForPlayers;
            Player.Hurting -= lh.OnHurt;
            Player.ChangingRole -= lh.PlayerFuckingDied;
            Player.ReloadingWeapon -= lh.reloading;
            Server.RespawningTeam -= lh.respawn;
            Player.ActivatingWarheadPanel -= lh.UnlockingWarheadButton;
            Exiled.Events.Handlers.Map.Decontaminating -= lh.Decont;
        }
    }
}
