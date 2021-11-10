using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;

namespace FunnyGuns
{
    class Mutators
    {
        /*
        This is the most unefficent way of storing mutators' functions and booleans.
        *Clap clap moment intensifies*
        */

        /*
        I need to create some sort of `Temporary` mutators. They should REALLY change the game rules
        and I mean REALLLY change them. You probably do know what I am talking about.

        Some Ideas:
        1) Broken cassie - Deafen for 45 seconds.
        -) Door control system failure - hello old friend, hello Fall of The Facility! ~~ NO LONGER TEMPORARY ~~
        3) get more ideas!
        */
        public static bool areLightsDown;
        public static bool noRegen;
        public static bool areShotsMoreDeadly;
        public static bool isFallDamageFatal;
        public static bool legalWH;
        public static bool fastRun;
        public static bool doorJam;

        public static void disableMutators()
        {
            doorJam = false;
        }

        public static List<int> usedMutators = new List<int>();

        public static void LightsDown()
        {
            Cassie.Message("Danger . light control system is .g4 .g4 .g4 error . error .g4 .g4 .g1. g2 3 . 2 . 1");
            Timing.CallDelayed(Cassie.CalculateDuration("Danger . light control system is .g4 .g4 .g4 error . error .g4 .g4 .g1. g2 3 . 2 . 1") + 3f, () =>
            {
                Map.TurnOffAllLights(10000f, Exiled.API.Enums.ZoneType.Unspecified);
                areLightsDown = true;
            });
        }

        public static void runFastOn()
        {
            fastRun = true;
            foreach (var pl in Player.List)
            {
                pl.EnableEffect(Exiled.API.Enums.EffectType.Scp207);
            }
        }

        public static void runFastOff()
        {
            fastRun = false;
            foreach (var pl in Player.List)
            {
                pl.DisableEffect(Exiled.API.Enums.EffectType.Scp207);
            }
        }

        public static void jamDoors()
        {
            doorJam = true;
        }

        public static void RegenOff()
        {
            noRegen = true;
        }

        public static void ShotsDeadlier()
        {
            areShotsMoreDeadly = true;
        }

        public static void FatalGravity()
        {
            isFallDamageFatal = true;
        }

        public static void EnableWH()
        {
            legalWH = true;
            foreach (var player in Plugin.active_playerlist)
            {
                if (player.Role != RoleType.Spectator)
                {
                    player.EnableEffect<CustomPlayerEffects.Visuals939>(10000, false);
                }
            }
        }

        public static void DisableWH()
        {
            legalWH = false;
            foreach (var player in Player.List)
            {
                player.DisableEffect<CustomPlayerEffects.Visuals939>();
            }
        }
    }
}
