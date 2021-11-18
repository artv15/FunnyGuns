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
        public static int MutatorOverride = 0;
        public static bool allowRespawningWithRA;
        public static int overrideHisRespawn;
        public static bool suicideisKill;
        //shop
        public static Dictionary<string, int> shopDict = new Dictionary<string, int>(); //Used as (PlayerDefinitionID, Balance)!!11! Rember that stoopid treeshuld
        public static Dictionary<string, Exiled.API.Features.Player> playerClientDict = new Dictionary<string, Exiled.API.Features.Player>(); //Defenitions for string stuff
        //Loaded and Engaged Mutators
        /*
         What are loaded and engaged mutators and what's the difference?
        Loaded mutators, are mutators which will be chosen from by random mutator selector every stage OR can be called by admin.
        Engaged mutators is updated by Mutator Assignment and contains all used mutators. Also, you can add them during the event by using fg_override mutator [commandname];
         */
        public static List<Classes.Mutator> loadedMutators = new List<Classes.Mutator>(); //Loaded mutators
        public static List<Classes.Mutator> engagedMutators = new List<Classes.Mutator>(); //Engaged mutators
        //Shop's loaded items.
        public static List<Classes.shopItem> shopInventory = new List<Classes.shopItem>(); //Shop Items Live Here

        public static bool isPrep = true;
        public static bool isRunning = false;
        public static bool isEventFrozen = false;
        public static int stage;
        public static int secondsTillNextStage;

        public static bool isDevMode = false; //Remember to disable it plz
        public static string BBI = "000014x001"; //DO NOT REFORMAT!
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
            Log.Info($"╔══════════════════════════════════════════════╗");
            Log.Info($"║               FunnyGuns Event                ║");
            Log.Info($"║                  (Stable)                    ║");
            Log.Info($"║ Made by Treeshold#0001 (aka. Star Butterfly) ║");
            Log.Info($"║       Build Identifier(BI): {BBI}       ║"     ); //IT'S OKAY, DO NOT EDIT!
            Log.Info($"║  if you want to send feedback, include BBI!  ║");
            Log.Info($"╚══════════════════════════════════════════════╝");
            Log.Warn("Warning! Beta! Some things may or may not work!");

            Server.RoundStarted += lh.OnRoundStarted;
            Server.WaitingForPlayers += lh.OnWaitingForPlayers;
            Player.Died += lh.KilledPlayer;
            Player.InteractingDoor += lh.DoorInteract;
            Player.Hurting += lh.OnHurt;
            Player.ChangingRole += lh.PlayerFuckingDied;
            Player.ReloadingWeapon += lh.onReload;
            Player.DroppingAmmo += lh.onAmmoDrop;
            Player.ActivatingWarheadPanel += lh.UnlockingWarheadButton;
            Server.RespawningTeam += lh.respawn;
            Exiled.Events.Handlers.Map.Decontaminating += lh.Decont;
        }

        public override void OnDisabled()
        {
            Server.RoundStarted -= lh.OnRoundStarted;
            Server.WaitingForPlayers -= lh.OnWaitingForPlayers;
            Player.InteractingDoor -= lh.DoorInteract;
            Player.Died -= lh.KilledPlayer;
            Player.Hurting -= lh.OnHurt;
            Player.ChangingRole -= lh.PlayerFuckingDied;
            Player.ReloadingWeapon -= lh.onReload;
            Player.DroppingAmmo -= lh.onAmmoDrop;
            Server.RespawningTeam -= lh.respawn;
            Player.ActivatingWarheadPanel -= lh.UnlockingWarheadButton;
            Exiled.Events.Handlers.Map.Decontaminating -= lh.Decont;
        }
    }
}
