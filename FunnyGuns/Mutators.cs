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
        This is the most unefficent way of storing mutators' functions.
        *Clap clap moment intensifies*
        */
        public static bool areLightsDown;
        public static bool noRegen;
        public static bool areShotsMoreDeadly;
        public static bool isFallDamageFatal;
        public static bool legalWH;

        public static List<int> usedMutators = new List<int>();

        public static void LightsDown()
        {
            Cassie.Message("Danger . light control system is .g4 .g4 .g4 error . error .g4 .g4 .g1. g2 3 . 2 . 1");
            Timing.CallDelayed(Cassie.CalculateDuration("Danger . light control system is .g4 .g4 .g4 error . error .g4 .g4 .g1. g2 3 . 2 . 1"), () =>
            {
                Map.TurnOffAllLights(10000f, Exiled.API.Enums.ZoneType.Unspecified);
                areLightsDown = true;
            });
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
