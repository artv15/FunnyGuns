using Exiled.API.Features;
using Exiled.Events.EventArgs;
using InventorySystem.Items;
using MEC;
using System;
using System.Collections.Generic;

namespace FunnyGuns
{
    class EventHandler
    {
        static bool isWeaponDamage(DamageTypes.DamageType dt)
        {
            bool result;
            if (!Plugin.isOverriden)
            {
                result = dt.Weapon == ItemType.GunAK || dt.Weapon == ItemType.GunCOM15 || dt.Weapon == ItemType.GunCOM18 || dt.Weapon == ItemType.GunCrossvec || dt.Weapon == ItemType.GunE11SR
                    || dt.Weapon == ItemType.GunFSP9 || dt.Weapon == ItemType.GunLogicer || dt.Weapon == ItemType.GunRevolver || dt.Weapon == ItemType.GunShotgun;
            }
            else
            {
                result = true;
            }
            return result;
        }

        public static void UnlockingWarheadButton(ActivatingWarheadPanelEventArgs ev)
        {
            if (Plugin.isRunning)
            {
                ev.IsAllowed = false;
            }
        }

        public static void Decont(DecontaminatingEventArgs ev)
        {
            if (Plugin.isRunning)
            {
                ev.IsAllowed = false;
            }
        }

        public static void respawn(RespawningTeamEventArgs ev)
        {
            if (Plugin.isRunning)
            {
                ev.IsAllowed = false;
            }
        }

        public static void StartEvent()
        {
            Mutators.areShotsMoreDeadly = false;
            Mutators.areLightsDown = false;
            Map.TurnOffAllLights(0f);
            Mutators.isFallDamageFatal = false;
            Mutators.noRegen = false;
            Mutators.DisableWH();
            Plugin.isRunning = true;
            Timing.RunCoroutine(playerHealing());
            Plugin.active_playerlist = Exiled.API.Features.Player.List;
            Plugin.recalculatePlayers();
            PlayerSpawn();
            Timing.RunCoroutine(GameController(), "eventcontrol");
            Plugin.stage = 1;
        }

        public static void StopEvent()
        {
            Plugin.isMTFBigger = false;
            Plugin.isRunning = false;
            Plugin.recalculatePlayers();
            Plugin.stage = 1;
            Plugin.secondsTillNextStage = 300;
            Timing.KillCoroutines("eventcontrol");
            Timing.KillCoroutines("gamePrep");
            Timing.KillCoroutines("respawnci");
            Mutators.usedMutators.Clear();
            Mutators.fastRun = false;
            Mutators.DisableWH();
            Mutators.areShotsMoreDeadly = false;
            Mutators.areLightsDown = false;
            Map.TurnOffAllLights(0f);
            Mutators.isFallDamageFatal = false;
            Mutators.noRegen = false;
        }

        public static void PlayerFuckingDied(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole == RoleType.Spectator)
            {
                if (Plugin.isRunning)
                {
                    if (ev.Player.IsCHI || ev.Player.IsNTF)
                    {
                        Cassie.Message($"{(ev.Player.IsCHI ? "Chaos agent " : "MTFUnit")} {ev.Player.Id} terminated", false, false);
                    }
                }
                if (Plugin.isMTFBigger && Plugin.isRunning)
                {
                    Plugin.isMTFBigger = false;
                    Timing.RunCoroutine(RespawnCI(ev.Player), "respawnci");
                }
            }
        }

        static void PlayerSpawn()
        {
            Log.Debug("Called Player spawn!");
            Plugin.recalculatePlayers();
            int playersINT = Plugin.CountList;
            int MTFUnitAmout;
            int CIAmount;
            if (playersINT % 2 == 0)
            {
                MTFUnitAmout = playersINT / 2;
                CIAmount = playersINT / 2;
            }
            else
            {
                MTFUnitAmout = (playersINT + 1) / 2;
                CIAmount = (playersINT - 1) / 2;
            }
            Log.Debug($"Spawning {MTFUnitAmout} MTFs, {CIAmount} CI's");
            if (MTFUnitAmout > CIAmount)
            {
                Plugin.isMTFBigger = true;
            }
            foreach (var pl in Plugin.active_playerlist)
            {
                Log.Debug($"Spawning {pl.Nickname}!");
                if (MTFUnitAmout != 0 && CIAmount != 0)
                {
                    try
                    {
                        int randint = UnityEngine.Random.Range(1, 3);
                        if (randint == 1)
                        {
                            int roleSel = UnityEngine.Random.Range(1, 4);
                            switch (roleSel)
                            {
                                case 1:
                                    pl.Role = RoleType.NtfPrivate;
                                    pl.AddItem(ItemType.KeycardNTFCommander);
                                    break;
                                case 2:
                                    pl.Role = RoleType.NtfSergeant;
                                    pl.AddItem(ItemType.KeycardNTFCommander);
                                    break;
                                case 3:
                                    pl.Role = RoleType.NtfCaptain;
                                    break;
                            }
                            MTFUnitAmout -= 1;
                        }
                        else
                        {
                            int roleSel = UnityEngine.Random.Range(1, 3);
                            switch (roleSel)
                            {
                                case 1:
                                    pl.Role = RoleType.ChaosRifleman;
                                    break;
                                case 2:
                                    pl.Role = RoleType.ChaosMarauder;
                                    break;
                            }
                            CIAmount -= 1;
                        }

                    }
                    catch (Exception ex)
                    {
                        Log.Warn($"Couldn't spawn player with nickname {pl.Nickname}");
                    }
                }
                else
                {
                    try
                    {
                        if (MTFUnitAmout == 0)
                        {
                            int roleSel = UnityEngine.Random.Range(1, 3);
                            switch (roleSel)
                            {
                                case 1:
                                    pl.Role = RoleType.ChaosRifleman;
                                    pl.ClearInventory();
                                    pl.AddItem(ItemType.KeycardChaosInsurgency);
                                    pl.AddItem(ItemType.GunRevolver);
                                    pl.AddItem(ItemType.GunAK); //Return to here plz
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.Ammo[ItemType.Ammo12gauge] = 6942;
                                    pl.Ammo[ItemType.Ammo44cal] = 6942;
                                    pl.Ammo[ItemType.Ammo556x45] = 6942;
                                    pl.Ammo[ItemType.Ammo762x39] = 6942;
                                    pl.Ammo[ItemType.Ammo9x19] = 6942;
                                    break;
                                case 2:
                                    pl.Role = RoleType.ChaosMarauder;
                                    pl.Ammo[ItemType.Ammo12gauge] = 6942;
                                    pl.Ammo[ItemType.Ammo44cal] = 6942;
                                    pl.Ammo[ItemType.Ammo556x45] = 6942;
                                    pl.Ammo[ItemType.Ammo762x39] = 6942;
                                    pl.Ammo[ItemType.Ammo9x19] = 6942;
                                    break;
                            }
                        }
                        else
                        {
                            int roleSel = UnityEngine.Random.Range(1, 4);
                            switch (roleSel)
                            {
                                case 1:
                                    pl.Role = RoleType.NtfPrivate;
                                    pl.Ammo[ItemType.Ammo12gauge] = 6942;
                                    pl.Ammo[ItemType.Ammo44cal] = 6942;
                                    pl.Ammo[ItemType.Ammo556x45] = 6942;
                                    pl.Ammo[ItemType.Ammo762x39] = 6942;
                                    pl.Ammo[ItemType.Ammo9x19] = 6942;
                                    break;
                                case 2:
                                    pl.Role = RoleType.NtfSergeant;
                                    pl.Ammo[ItemType.Ammo12gauge] = 6942;
                                    pl.Ammo[ItemType.Ammo44cal] = 6942;
                                    pl.Ammo[ItemType.Ammo556x45] = 6942;
                                    pl.Ammo[ItemType.Ammo762x39] = 6942;
                                    pl.Ammo[ItemType.Ammo9x19] = 6942;
                                    break;
                                case 3:
                                    pl.Role = RoleType.NtfCaptain;
                                    pl.Ammo[ItemType.Ammo12gauge] = 6942;
                                    pl.Ammo[ItemType.Ammo44cal] = 6942;
                                    pl.Ammo[ItemType.Ammo556x45] = 6942;
                                    pl.Ammo[ItemType.Ammo762x39] = 6942;
                                    pl.Ammo[ItemType.Ammo9x19] = 6942;
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warn($"Couldn't spawn player with nickname {pl.Nickname}");
                    }
                }

            }
        }

        public static void droppingAmmo (DroppingAmmoEventArgs ev)
        {
            if (Plugin.isRunning || Plugin.isPrep)
            {
                ev.IsAllowed = false;
            }
        }

        static IEnumerator<float> gameStartingNotification(Player pl)
        {
            int i = 55;
            while (i > 0)
            {
                pl.ShowHint($"\n\n\n\n\n\nФаза подготовки, <color=blue>идите в комплекс</color> или <color=red>готовьтесь обороняться</color>!\n<color=green>Урон во время фазы подготовки отключён.</color>\nОсталось <color=yellow>{i}</color> секунд.\n<color=yellow>Чтобы узнать об ивенте, напишите в консоли .fg_event_info</color>");
                i -= 1;
                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static void reloading(ReloadingWeaponEventArgs ev)
        {

        }

        static IEnumerator<float> RespawnCI(Player pl)
        {
            int i = 30;
            while (i > 0)
            {
                i--;
                var bc = new Exiled.API.Features.Broadcast();
                bc.Duration = 1;
                bc.Content = $"Вы возродитесь через <color=green>{i}</color> секунд, так как моговцев было заспавнено больше,\nчем хаоса!";
                bc.Show = true;
                bc.Type = Broadcast.BroadcastFlags.Normal;
                pl.Broadcast(bc);
                yield return Timing.WaitForSeconds(1f);
            }
            pl.Role = RoleType.ChaosRepressor;
        }

        static IEnumerator<float> GameController()
        {
            Plugin.isPrep = true;
            foreach (var door in Map.Doors)
            {
                if (door.Nametag == "SURFACE_GATE")
                {
                    door.IsOpen = false;
                    door.ChangeLock(Exiled.API.Enums.DoorLockType.AdminCommand);
                }
            }
            foreach (var pl in Plugin.active_playerlist)
            {
                Timing.RunCoroutine(gameStartingNotification(pl), "gamePrep");
            }
            yield return Timing.WaitForSeconds(55f);
            Cassie.Message(".g4 .g4 .g4 . . .g4 .g4 .g4", false, false);
            foreach (var door in Map.Doors)
            {
                if (door.Nametag == "SURFACE_GATE")
                {
                    door.ChangeLock(Exiled.API.Enums.DoorLockType.None);
                }
            }
            Plugin.isPrep = false;
            int MTF = 0;
            int CI = 0;
            int lastStanding = 0;
            Plugin.stage = 1;
            Plugin.secondsTillNextStage = 150;
            while (true)
            {
                foreach (var pl in Plugin.active_playerlist)
                {
                    Plugin.recalculatePlayers();
                    if (pl.IsNTF)
                    {
                        MTF += 1;
                    }
                    else if (pl.IsCHI)
                    {
                        CI += 1;
                    }
                    //Log.Debug($"CI: {CI}, NTF: {MTF}");
                    if (Plugin.CountList == 1)
                    {
                        lastStanding = pl.Id;
                        break;
                    }
                }
                if ((MTF == 0 || CI == 0) && !Plugin.isPlayerOverriden)
                {
                    string message = $"All {(MTF == 0 ? "MTFUnits" : "Chaos agents")} have been terminated .";
                    if (Plugin.CountList == 1)
                    {
                        message += $"Last {(MTF == 0 ? "Chaos agent" : "MTFUnit")} detected . Unit {lastStanding} has survived";
                    }
                    Cassie.Message(message);
                    StopEvent();
                    yield break;
                }
                else
                {
                    MTF = 0;
                    CI = 0;
                    if (Plugin.secondsTillNextStage >= 1 && Plugin.stage != 4)
                    {
                        Plugin.secondsTillNextStage -= 1;
                        yield return Timing.WaitForSeconds(1f);
                        //Status bar and mutators display here!
                        string color;
                        switch (Plugin.stage) {
                            case 1:
                                color = "green";
                                break;
                            case 2:
                                color = "yellow";
                                break;
                            case 3:
                                color = "red";
                                break;
                            case 4:
                                color = "red";
                                break;
                            default:
                                color = "green";
                                break;
                        }
                        foreach (var pl in Player.List) 
                        {
                            string msg = $"\n\n\n\n\n\nТекущая стадия: <color={color}>{Plugin.stage}</color>. Время до следующей стадии: <color=orange>{Plugin.secondsTillNextStage}</color>";
                            if (Mutators.areLightsDown || Mutators.noRegen || Mutators.areShotsMoreDeadly || Mutators.isFallDamageFatal || Mutators.legalWH)
                            {
                                msg += "\nАктивные мутаторы: ";
                            }
                            if (Mutators.fastRun)
                            {
                                msg += $"<color=blue>Скорость передвижения увеличена!</color>";
                                if (Mutators.areLightsDown || Mutators.areShotsMoreDeadly || Mutators.isFallDamageFatal || Mutators.noRegen || Mutators.legalWH)
                                {
                                    msg += ", ";
                                }
                                else
                                {
                                    msg += ".";
                                }
                            }
                            if (Mutators.areLightsDown)
                            {
                                msg += $"<color=yellow>Нет света</color>";
                                if (Mutators.areShotsMoreDeadly || Mutators.isFallDamageFatal || Mutators.noRegen || Mutators.legalWH)
                                {
                                    msg += ", ";
                                }
                                else
                                {
                                    msg += ".";
                                }
                            }
                            if (Mutators.areShotsMoreDeadly)
                            {
                                msg += $"<color=red>Урон от оружий усилен!</color>";
                                if (Mutators.isFallDamageFatal || Mutators.noRegen || Mutators.legalWH)
                                {
                                    msg += ", ";
                                }
                                else
                                {
                                    msg += ".";
                                }
                            }
                            if (Mutators.isFallDamageFatal)
                            {
                                msg += $"<color=red>Падение с любой высоты фатально</color>";
                                if (Mutators.noRegen || Mutators.legalWH)
                                {
                                    msg += ", ";
                                }
                                else
                                {
                                    msg += ".";
                                }
                            }
                            if (Mutators.noRegen)
                            {
                                msg += $"<color=red>Нет пассивной регенерации</color>";
                                if (Mutators.legalWH)
                                {
                                    msg += ", ";
                                }
                                else
                                {
                                    msg += ".";
                                }
                            }
                            if (Mutators.legalWH)
                            {
                                msg += $"<color=green>Рентгеновское зрение</color>.";
                            }
                            pl.ShowHint(msg, 2);
                        }
                    }
                    else
                    {
                        if (Plugin.stage <= 2)
                        {
                            Plugin.secondsTillNextStage = 150;
                            Plugin.stage += 1;
                            int randomMutator = UnityEngine.Random.Range(1, 6);
                            while (true)
                            {
                                if (Mutators.usedMutators.Contains(randomMutator))
                                {
                                    randomMutator = UnityEngine.Random.Range(1, 7);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            
                            
                            switch (randomMutator)
                            {
                                case 1:
                                    Mutators.FatalGravity();
                                    break;
                                case 2:
                                    Mutators.LightsDown();
                                    break;
                                case 3:
                                    Mutators.RegenOff();
                                    break;
                                case 4:
                                    Mutators.ShotsDeadlier();
                                    break;
                                case 5:
                                    Mutators.EnableWH();
                                    break;
                                case 6:
                                    Mutators.runFastOn();
                                    break;
                                default:
                                    Log.Error("Random Mutator selector chose out of range.");
                                    break;
                            }
                            Cassie.Message(".g4 .g4 .g4", false, false);
                            Mutators.usedMutators.Add(randomMutator);
                        }
                        else
                        {
                            foreach (var pl in Player.List)
                            {
                                pl.ShowHint($"Внимание! 4 Стадия! Запущена ядерная боеголовка!", 10);
                            }
                            Mutators.areShotsMoreDeadly = false;
                            Mutators.areLightsDown = false;
                            Map.TurnOffAllLights(0f);
                            Mutators.isFallDamageFatal = false;
                            Mutators.noRegen = false;
                            Mutators.runFastOff();
                            Mutators.DisableWH();
                            Mutators.usedMutators.Clear();
                            Exiled.API.Features.Warhead.Start();
                            Exiled.API.Features.Warhead.IsLocked = true;
                            while (Warhead.DetonationTimer > 0f)
                            {
                                foreach (var pl in Player.List)
                                {
                                    pl.ShowHint($"\n\n\n\n\n\nАктивна <color=red>ядерная боеголовка</color>. До детонации {(int)Warhead.DetonationTimer} секунд!", 1);
                                    yield return Timing.WaitForSeconds(0.5f);
                                }
                            }
                            while (true)
                            {
                                foreach (var pl in Plugin.active_playerlist)
                                {
                                    Plugin.recalculatePlayers();
                                    if (pl.IsNTF)
                                    {
                                        MTF += 1;
                                    }
                                    else if (pl.IsCHI)
                                    {
                                        CI += 1;
                                    }
                                }
                                if (MTF == 0 || CI == 0)
                                {
                                    break;
                                }
                                else
                                {
                                    foreach (var pl in Plugin.active_playerlist)
                                    {
                                        if (pl.Role != RoleType.Spectator)
                                        {
                                            pl.ShowHint($"Уничтожьте <color=red>вражескую команду</color>.\nОсталось {(pl.IsNTF ? $"<color=green>{CI} хаосит(ов)</color>" : $"<color=blue>{MTF} NTF</color>")}");
                                        }
                                    }
                                }
                                yield return Timing.WaitForSeconds(0.5f);
                            }
                            Warhead.IsLocked = false;
                            Warhead.Start();
                            Warhead.Stop();
                            string message = $"All {(MTF == 0 ? "MTFUnits" : "Chaos agents")} have been terminated .";
                            Cassie.Message(message);
                            StopEvent();
                            yield break;
                        }
                    }
                }
            }
        }

        public static void OnHurt(HurtingEventArgs ev)
        {
            if (!Plugin.isPrep && Plugin.isRunning)
            {
                if (isWeaponDamage(ev.DamageType) && Plugin.isRunning)
                {
                    if (Mutators.areShotsMoreDeadly)
                    {
                        ev.Amount = (int)(ev.Amount * 1.5f);
                    }
                    if (ev.Amount <= 20)
                    {
                        ev.Target.EnableEffect(Exiled.API.Enums.EffectType.Concussed, 5f, false);
                    }
                    else if (ev.Amount <= 50)
                    {
                        ev.Target.EnableEffect(Exiled.API.Enums.EffectType.Concussed, 7f, false);
                        ev.Target.EnableEffect(Exiled.API.Enums.EffectType.SinkHole, 4f, false);
                    }
                    else if (ev.Amount > 50)
                    {
                        ev.Target.EnableEffect(Exiled.API.Enums.EffectType.Concussed, 8f, false);
                        ev.Target.EnableEffect(Exiled.API.Enums.EffectType.SinkHole, 7f, false);
                        ev.Target.EnableEffect(Exiled.API.Enums.EffectType.Amnesia, 9f, false);
                    }
                }
                else if (ev.DamageType == DamageTypes.Falldown)
                {
                    if (Mutators.isFallDamageFatal)
                    {
                        ev.Target.Kill(DamageTypes.Falldown);
                    }
                }
                else if (ev.DamageType == DamageTypes.Scp207)
                {
                    if (Mutators.fastRun)
                    {
                        ev.IsAllowed = false;
                    }
                }
            }
            else if (Plugin.isRunning && Plugin.isPrep)
            {
                ev.IsAllowed = false;
            }
        }

        static IEnumerator<float> playerHealing()
        {
            while (Round.IsStarted && Plugin.isRunning)
            {
                yield return Timing.WaitForSeconds(0.75f);
                foreach (var pl in Plugin.active_playerlist)
                {
                    if ((pl.Health < pl.MaxHealth && pl.Role != RoleType.Spectator) && !Mutators.noRegen)
                    {
                        pl.Health += 1;
                    }
                }
            }
        }

        public static void OnRoundStarted()
        {

        }

        public static void OnItemUsed(UsedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.SCP500 && Mutators.fastRun)
            {
                Timing.CallDelayed(1f, () => ev.Player.EnableEffect(Exiled.API.Enums.EffectType.Scp207));
            }
        }

        public static void OnWaitingForPlayers()
        {
            Mutators.usedMutators.Clear();
            Plugin.isRunning = false;
            Plugin.isOverriden = false;
            Plugin.isPlayerOverriden = false;
            StopEvent();
        }
    }
}