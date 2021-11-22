using Exiled.API.Features;
using Exiled.Events.EventArgs;
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
            Classes.Mutator.disableAll();
            Plugin.shopDict.Clear();
            Mutators.areShotsMoreDeadly = false;
            Mutators.areLightsDown = false;
            Map.TurnOffAllLights(0f);
            Mutators.isFallDamageFatal = false;
            Mutators.noRegen = false;
            Plugin.isRunning = true;
            Timing.RunCoroutine(playerHealing(), "heal");
            Plugin.active_playerlist = Exiled.API.Features.Player.List;
            Plugin.recalculatePlayers();
            PlayerSpawn();
            Timing.RunCoroutine(GameController(), "eventcontrol");
            Plugin.stage = 1;
            Plugin.isEventFrozen = false;
        }

        public static void StopEvent()
        {
            Classes.Mutator.disableAll();
            Plugin.shopDict.Clear();
            Plugin.isMTFBigger = false;
            Plugin.isRunning = false;
            Plugin.recalculatePlayers();
            Plugin.stage = 1;
            Plugin.secondsTillNextStage = 300;
            Plugin.allowRespawningWithRA = false;
            Timing.KillCoroutines("eventcontrol");
            Timing.KillCoroutines("gamePrep");
            Timing.KillCoroutines("respawnci");
            Mutators.fastRun = false;
            Mutators.areShotsMoreDeadly = false;
            Mutators.areLightsDown = false;
            Map.TurnOffAllLights(0f);
            Mutators.isFallDamageFatal = false;
            Mutators.noRegen = false;
            Plugin.isEventFrozen = false;
        }

        public static void PlayerFuckingDied(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole == RoleType.Spectator && (ev.Player.IsCHI || ev.Player.IsNTF))
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
                else
                {
                    var msg = "<color=red>Вы умерли...</color> Вот вам небольшой совет:\n";
                    int rand = UnityEngine.Random.Range(1, 12);
                    switch (rand) {
                        case 1:
                            msg += "<color=yellow>Не обязательно спускаться в комплекс, можно караулить поверхность!</color>";
                            break;
                        case 2:
                            msg += "<color=yellow>Чем больше стадия - тем больше множитель урона. Во время 5-й стадии - вообще 5X!</color>";
                            break;
                        case 3:
                            msg += "<color=yellow>У всех классов приблизительно одинаковый инвентарь. Раньше хаоситы имели при себе пулемёты.</color>";
                            break;
                        case 4:
                            msg += "<color=yellow>Если вы видите толпу противников, не бегите на неё. Лучше сгруппируйтесь с вашими тимейтами!</color>";
                            break;
                        case 5:
                            msg += "<color=yellow>Некоторые мутаторы могут сводить эффекты мутаторов на ноль. Например, рентгеновское зрение и выключенный свет!</color>";
                            break;
                        case 6:
                            msg += "<color=yellow>Подкласс отряда не имеет значения. Все равны, и это неотлемлемое право каждого игрока</color>";
                            break;
                        case 7:
                            msg += "<color=yellow>Если вы получаете крайне высокий урон от оружия - вы будете оглушены и замедлены!</color>";
                            break;
                        case 8:
                            msg += "<color=yellow>Вы можете выпить колу во время мутатора `скорость движения увеличина` и бежать ещё быстрее без потерь!</color>";
                            break;
                        case 9:
                            msg += "<color=yellow>Слушайте шаги, если нет другого источника информации. Он поможет вам понять направление противника!</color>";
                            break;
                        case 10:
                            msg += "<color=yellow>У хаоситов и мога - совершенно разные звуки шагов! Так вы можете определить, рядом с вами тиммейт или враг!</color>";
                            break;
                        case 11:
                            msg += "<color=yellow>Если в лифте не зажать C, то ваше нахождение в нём будет раскрыто!</color>";
                            break;
                    }
                    ev.Player.Broadcast(10, msg);
                }
            }
            else
            {
                if (Plugin.isRunning && !Plugin.allowRespawningWithRA && !(ev.NewRole == RoleType.Spectator) && !(Plugin.overrideHisRespawn == ev.Player.Id))
                {
                    ev.IsAllowed = false;
                    ev.Player.Broadcast(5, "Админ пытался сменить твой класс. Мы, конечно, не дали такому случится, ведь это <color=red>абуз</color>!", Broadcast.BroadcastFlags.Normal, true);
                    Log.Warn("Abuse attempt detected! But maybe it's false positive!");
                }
                else
                {
                    if (Plugin.engagedMutators.Count > 0)
                    {
                        foreach (var mut in Plugin.engagedMutators)
                        {
                            Log.Debug($"Executing {mut.commandName} onrespawn method.");
                            Timing.CallDelayed(1f, () => mut.onRespawn.Invoke(ev.Player));
                        }
                    }
                }
            }
        }

        static void PlayerSpawn()
        {
            /*
            Gonna comment almost everything, because testing required!
            not amymore, but im lazy
            */
            Log.Debug("Called Player spawn and enabled spawn override!");
            Plugin.allowRespawningWithRA = true; //Giving plugin ability to override spawn coroutine
            Plugin.recalculatePlayers(); //used for determining how many players are going to be spawned!
            int playersINT = Plugin.CountList;
            int MTFUnitAmout;
            int CIAmount;
            if (playersINT % 2 == 0) //Equal spawn
            {
                MTFUnitAmout = playersINT / 2;
                CIAmount = playersINT / 2;
            }
            else //UnEqual spawn
            {
                MTFUnitAmout = (playersINT + 1) / 2;
                CIAmount = (playersINT - 1) / 2;
            }
            Log.Debug($"Spawning {MTFUnitAmout} MTFs, {CIAmount} CI's");
            if (MTFUnitAmout > CIAmount)
            {
                Plugin.isMTFBigger = true; //Used for "fair CI" respawn
            }
            foreach (var pl in Plugin.active_playerlist)
            {

                if (MTFUnitAmout != 0 && CIAmount != 0) //Still could randomly spawn!
                {
                    try
                    {
                        int randint = UnityEngine.Random.Range(1, 3); //Select CI or MTF!
                        Log.Debug($"Spawning `{pl.Nickname} ({pl.UserId})` as {(randint == 1 ? "MTF" : "CI")}!"); //Test it plz
                        if (randint == 1) //MTF
                        {
                            int roleSel = UnityEngine.Random.Range(1, 4);
                            switch (roleSel) //MTF Subclasses
                            {
                                case 1:
                                    pl.Role = RoleType.NtfPrivate;
                                    pl.ClearInventory();
                                    pl.AddItem(ItemType.GunE11SR);
                                    pl.AddItem(ItemType.GrenadeHE);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Adrenaline);
                                    pl.AddItem(ItemType.ArmorCombat);
                                    pl.AddItem(ItemType.KeycardNTFCommander);
                                    break;
                                case 2:
                                    pl.Role = RoleType.NtfSergeant;
                                    pl.ClearInventory();
                                    pl.AddItem(ItemType.GunE11SR);
                                    pl.AddItem(ItemType.GrenadeHE);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Adrenaline);
                                    pl.AddItem(ItemType.ArmorCombat);
                                    pl.AddItem(ItemType.KeycardNTFCommander);
                                    break;
                                case 3:
                                    pl.Role = RoleType.NtfCaptain;
                                    pl.ClearInventory();
                                    pl.AddItem(ItemType.GunE11SR);
                                    pl.AddItem(ItemType.GrenadeHE);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Adrenaline);
                                    pl.AddItem(ItemType.ArmorCombat);
                                    pl.AddItem(ItemType.KeycardNTFCommander);
                                    break;
                            }
                            MTFUnitAmout -= 1;
                        }
                        else //CI
                        {
                            Log.Debug($"Spawning `{pl.Nickname} ({pl.UserId})` as {(randint == 1 ? "MTF" : "CI")}!"); //Test it plz
                            int roleSel = UnityEngine.Random.Range(1, 3);
                            switch (roleSel) //CI Subclasses
                            {
                                case 1:
                                    pl.Role = RoleType.ChaosRifleman;
                                    pl.ClearInventory();
                                    pl.AddItem(ItemType.GunAK);
                                    pl.AddItem(ItemType.GrenadeHE);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Adrenaline);
                                    pl.AddItem(ItemType.ArmorCombat);
                                    pl.AddItem(ItemType.KeycardChaosInsurgency);
                                    break;
                                case 2:
                                    pl.Role = RoleType.ChaosMarauder;
                                    pl.ClearInventory();
                                    pl.AddItem(ItemType.GunAK);
                                    pl.AddItem(ItemType.GrenadeHE);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Adrenaline);
                                    pl.AddItem(ItemType.ArmorCombat);
                                    pl.AddItem(ItemType.KeycardChaosInsurgency);
                                    break;
                            }
                            CIAmount -= 1;
                        }

                    }
                    catch (Exception ex) //In case of error
                    {
                        Log.Warn($"Couldn't spawn player with nickname {pl.Nickname}! The player is either non-existant, or Plugin is broken. If you beleive, that this is an error, please contact Treeshold#0001. (and include BBI)");
                    }
                }
                else //One team has exausted their tickets!
                {
                    try
                    {
                        if (MTFUnitAmout == 0) //If MTF ran out of tickets
                        {
                            Log.Debug($"Spawning `{pl.Nickname} ({pl.UserId})` as CI!"); //Test it plz
                            int roleSel = UnityEngine.Random.Range(1, 3);
                            switch (roleSel)
                            {
                                case 1:
                                    pl.Role = RoleType.ChaosRifleman;
                                    pl.ClearInventory();
                                    pl.AddItem(ItemType.GunAK);
                                    pl.AddItem(ItemType.GrenadeHE);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Adrenaline);
                                    pl.AddItem(ItemType.ArmorCombat);
                                    pl.AddItem(ItemType.KeycardChaosInsurgency);
                                    break;
                                case 2:
                                    pl.Role = RoleType.ChaosMarauder;
                                    pl.ClearInventory();
                                    pl.AddItem(ItemType.GunAK);
                                    pl.AddItem(ItemType.GrenadeHE);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Adrenaline);
                                    pl.AddItem(ItemType.ArmorCombat);
                                    pl.AddItem(ItemType.KeycardChaosInsurgency);
                                    break;
                            }
                        }
                        else //If CI ran out of tickets
                        {
                            Log.Debug($"Spawning `{pl.Nickname} ({pl.UserId})` as MTF!"); //Test it plz
                            int roleSel = UnityEngine.Random.Range(1, 4);
                            switch (roleSel)
                            {
                                case 1:
                                    pl.Role = RoleType.NtfPrivate;
                                    pl.ClearInventory();
                                    pl.AddItem(ItemType.GunE11SR);
                                    pl.AddItem(ItemType.GrenadeHE);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Adrenaline);
                                    pl.AddItem(ItemType.ArmorCombat);
                                    pl.AddItem(ItemType.KeycardNTFCommander);
                                    break;
                                case 2:
                                    pl.Role = RoleType.NtfSergeant;
                                    pl.ClearInventory();
                                    pl.AddItem(ItemType.GunE11SR);
                                    pl.AddItem(ItemType.GrenadeHE);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Adrenaline);
                                    pl.AddItem(ItemType.ArmorCombat);
                                    pl.AddItem(ItemType.KeycardNTFCommander);
                                    break;
                                case 3:
                                    pl.Role = RoleType.NtfCaptain;
                                    pl.ClearInventory();
                                    pl.AddItem(ItemType.GunE11SR);
                                    pl.AddItem(ItemType.GrenadeHE);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Medkit);
                                    pl.AddItem(ItemType.Adrenaline);
                                    pl.AddItem(ItemType.ArmorCombat);
                                    pl.AddItem(ItemType.KeycardNTFCommander);
                                    break;
                            }
                        }
                    }
                    catch (Exception ex) //In case if we failed to spawn a player
                    {
                        Log.Warn($"Couldn't spawn player with nickname {pl.Nickname}! The player is either non-existant, or Plugin is broken. If you beleive, that this is an error, please contact Treeshold#0001. (and include BBI)");
                    }
                }
            }
            Plugin.allowRespawningWithRA = false; //Disabling override
        }

        public static void KilledPlayer(DiedEventArgs ev)
        {
            var pl = ev.Killer;
            if (!Plugin.shopDict.ContainsKey($"{pl.Nickname} ({pl.UserId})") && !Plugin.playerClientDict.ContainsKey($"{pl.Nickname} ({pl.UserId})"))
            {
                Plugin.shopDict.Add($"{pl.Nickname} ({pl.UserId})", 0);
                Plugin.playerClientDict.Add($"{pl.Nickname} ({pl.UserId})", pl);
            }
            if ((ev.Killer != ev.Target || Plugin.suicideisKill) && Plugin.isRunning)
            {
                int currentBal = Plugin.shopDict[$"{pl.Nickname} ({pl.UserId})"];
                Plugin.shopDict[$"{pl.Nickname} ({pl.UserId})"] = currentBal += 10;
                pl.Broadcast(5, $"Вы получили <color=yellow>10</color> монет за убийтво <color=red>{ev.Target.Nickname}</color>.\nВаш баланс: <color=yellow>{Plugin.shopDict[$"{pl.Nickname} ({pl.UserId})"]}</color>", Broadcast.BroadcastFlags.Normal, true);
            }
        }

        static IEnumerator<float> gameStartingNotification(Player pl)
        {
            int i = 55;
            while (i > 0)
            {
                pl.ShowHint($"\n\n\n\n\n\nФаза подготовки, <color=blue>идите в комплекс</color> или <color=red>готовьтесь обороняться</color>!\n<color=green>Урон во время фазы подготовки отключён.</color>\nОсталось <color=yellow>{i}</color> секунд.\n<color=yellow>.fg_event - инфа об ивенте  .fg_updates - обновления  .shop - магазин</color>");
                i -= 1;
                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static void onReload(ReloadingWeaponEventArgs ev)
        {
            if (Plugin.isRunning)
            {
                ev.IsAllowed = true; //We will always try to reload!
                var firearmType = ev.Firearm.Type; //Storing weapon ItemType
                ItemType ammoType;
                switch (firearmType) //Funny switch
                {
                    case ItemType.GunCOM15:
                        ammoType = ItemType.Ammo9x19;
                        break;
                    case ItemType.GunE11SR:
                        ammoType = ItemType.Ammo556x45;
                        break;
                    case ItemType.GunCrossvec:
                        ammoType = ItemType.Ammo9x19;
                        break;
                    case ItemType.GunFSP9:
                        ammoType = ItemType.Ammo9x19;
                        break;
                    case ItemType.GunLogicer:
                        ammoType = ItemType.Ammo762x39;
                        break;
                    case ItemType.GunCOM18:
                        ammoType = ItemType.Ammo9x19;
                        break;
                    case ItemType.GunRevolver:
                        ammoType = ItemType.Ammo44cal;
                        break;
                    case ItemType.GunAK:
                        ammoType = ItemType.Ammo762x39;
                        break;
                    case ItemType.GunShotgun:
                        ammoType = ItemType.Ammo12gauge;
                        break;
                    default:
                        ammoType = ItemType.Ammo9x19;
                        ev.Player.ShowHint("<color=red>Error occured while defining ammo type!</color>");
                        break;
                }
                Timing.CallDelayed(0.5f, () => ev.Player.Ammo[ammoType] = (ushort)(ev.Firearm.MaxAmmo + 10)); //Setting player's ammo to (weapon_max_ammo + 10) as a failsafe delayed
            }
        }

        public static void onAmmoDrop(DroppingAmmoEventArgs ev)
        {
            if (Plugin.isRunning)
            {
                ev.IsAllowed = false; //To prevent ammo spam!
            }
        }

        public static void ItemAdded(SpawningItemEventArgs ev)
        {
            //Nothing, ignore this one!
        }

        static IEnumerator<float> RespawnCI(Player pl)
        {
            int i = 15; //Timer

            while (i > 0) //Ticking down to 0
            {
                i--; //Deincrement i
                var bc = new Exiled.API.Features.Broadcast(); //Creating broadcast
                bc.Duration = 1; //Duration 1 second <= Interval 1 second
                bc.Content = $"Вы возродитесь через <color=green>{i}</color> секунд, так как моговцев было заспавнено больше,\nчем хаоса!"; //Text
                bc.Show = true; //Show (show (show))
                bc.Type = Broadcast.BroadcastFlags.Normal; //I dunno
                pl.Broadcast(bc);
                if (i == 1)
                {
                    Plugin.overrideHisRespawn = pl.Id; //We need to override anti-abuse for him!
                }
                yield return Timing.WaitForSeconds(1f);
            }
            pl.Role = RoleType.ChaosRepressor;
            pl.ClearInventory();
            if (Mutators.legalWH)
            {
                pl.EnableEffect(Exiled.API.Enums.EffectType.Visuals939, 1000000);
            }
            if (Mutators.fastRun)
            {
                pl.EnableEffect(Exiled.API.Enums.EffectType.Scp207, 1000000);
            }
            pl.AddItem(ItemType.GunLogicer);
            pl.AddItem(ItemType.GunAK);
            pl.AddItem(ItemType.GrenadeHE);
            pl.AddItem(ItemType.Medkit);
            pl.AddItem(ItemType.Medkit);
            pl.AddItem(ItemType.Adrenaline);
            pl.AddItem(ItemType.ArmorCombat);
            pl.AddItem(ItemType.KeycardChaosInsurgency);
            Plugin.overrideHisRespawn = 0;
        }

        static IEnumerator<float> GameController() //Main coroutine
        {
            Plugin.isPrep = true;
            foreach (var door in Map.Doors)
            {
                door.IsOpen = false; //Why not?
                if (door.Nametag == "SURFACE_GATE")
                {
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
            Plugin.secondsTillNextStage = 60;
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

                    if (Plugin.secondsTillNextStage >= 1 && Plugin.stage != 5)
                    {
                        MTF = 0;
                        CI = 0;
                        foreach (var pl in Player.List)
                        {
                            if (pl.IsCHI)
                            {
                                CI += 1;
                            }
                            else if (pl.IsNTF)
                            {
                                MTF += 1;
                            }
                        }
                        if (!Plugin.isPlayerOverriden)
                        {
                            if (CI == 0 || MTF == 0)
                            {
                                Cassie.Message($"All {(CI == 0 ? "Chaos Agents" : "MTFUnits")} have been terminated");
                                Log.Debug($"Event ended, Results: CI = {CI}; MTF = {MTF}");
                                Log.Debug($"If any errors occured, contact Treeshold#0001 for assistance! Include BBI: {Plugin.BBI}");
                                StopEvent();
                            }
                        }
                        if (!Plugin.isEventFrozen)
                        {
                            Plugin.secondsTillNextStage -= 1;
                        }
                        yield return Timing.WaitForSeconds(1f);
                        //Status bar and mutators display here!
                        string color;
                        switch (Plugin.stage)
                        {
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
                            case 5:
                                color = "black";
                                break;
                            default:
                                color = "green";
                                break;
                        }
                        foreach (var pl in Player.List)
                        {
                            string msg = $"\n\n\n\n\n\nТекущая стадия: <color={color}>{Plugin.stage}</color>. {(Plugin.isEventFrozen ? "<color=red>Development override: Time frozen.</color>" : $"Время до следующей стадии: <color=orange>{Plugin.secondsTillNextStage}</color>")}";
                            if (Plugin.engagedMutators.Count > 0)
                            {
                                msg += "\nАктивные мутаторы: ";

                                int i = 0;
                                int elements = Plugin.engagedMutators.Count;
                                foreach (var mutator in Plugin.engagedMutators)
                                {
                                    i++;
                                    msg += mutator.hudName;
                                    if (i != elements)
                                    {
                                        msg += ", ";
                                    }
                                    else
                                    {
                                        msg += ".";
                                    }
                                }
                            }
                            pl.ShowHint(msg, 2);
                        }
                    }
                    else
                    {
                        if (Plugin.stage <= 3)
                        {
                            Plugin.secondsTillNextStage = 90;
                            Plugin.stage += 1;
                            int LoadedMutators = Plugin.loadedMutators.Count;
                            int i = 0;
                            int ttl = 50;
                            bool keepsearching = true;
                            Classes.Mutator selected = Classes.Mutator.initialize("<color=red>[ERROR] Failed to randomly select mutator, no mutator initiated.</color>", "FailedToLoad", () => { }, () => { }, (pl) => { });
                            while (keepsearching)
                            {
                                ttl--;
                                if (ttl < 1)
                                {
                                    break;
                                }
                                i = 0;
                                int randomMutator = UnityEngine.Random.Range(1, LoadedMutators + 1); //Everything is fine, do not edit!
                                foreach (var mut in Plugin.loadedMutators)
                                {
                                    i++;
                                    if (i == randomMutator)
                                    {
                                        if (!(Plugin.engagedMutators.Contains(mut)))
                                        {
                                            selected = mut;
                                            keepsearching = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            selected.engage.Invoke();
                            Plugin.engagedMutators.Add(selected);

                            Cassie.Message(".g4 .g4 .g4", false, false); //some signal of stage changing, i guess...
                        }
                        else
                        {
                            Classes.Mutator.disableAll();
                            foreach (var pl in Player.List)
                            {
                                pl.Broadcast(10, "<color=red>We want more!</color>\n Наступила <color=red>внезапная смерть</color>! Запаситесь аптечками, и отстреливайтесь от вражеской команды!");
                                if (pl.Role != RoleType.Spectator)
                                {
                                    Timing.RunCoroutine(damagePlayer(pl), "insdeath");
                                }
                            }
                            while (true)
                            {
                                if (CI == 0 || MTF == 0)
                                {
                                    break;
                                }
                                CI = 0;
                                MTF = 0;
                                foreach (var pl in Player.List)
                                {
                                    if (pl.IsNTF)
                                    {
                                        MTF += 1;
                                    }
                                    else if (pl.IsCHI)
                                    {
                                        CI += 1;
                                    }
                                }
                                yield return Timing.WaitForSeconds(0.5f);
                            }
                            Cassie.Message($"{(CI == 0 ? "All chaos agents have been terminated" : "All MTFUnits have been terminated")}");
                            Timing.KillCoroutines("insdeath");
                            Log.Debug($"Event ended with: CI = {CI}; MTF = {MTF}; This data may be not accurate!");
                            Log.Debug($"If some errors occured during the event, feel free to contact Treeshold#0001 for assistance. And include BBI: {Plugin.BBI}");
                            StopEvent();
                        }
                    }
                }
            }
        }

        static IEnumerator<float> damagePlayer(Player pl)
        {
            Timing.KillCoroutines("heal");
            while (Round.IsStarted)
            {
                pl.ShowHint($"\n\n\n\n\n\nТекущая стадия: <color=black>5</color>.\nАктивна <color=red>внезапная смерть</color>!", 1);
                pl.Hurt(1, DamageTypes.Bleeding);
                yield return Timing.WaitForSeconds(0.35f);
                if (pl.Health < 1)
                {
                    yield break;
                }
            }
        }

        public static void DoorInteract(InteractingDoorEventArgs ev)
        {
            if (Mutators.doorJam && ev.IsAllowed)
            {
                int rand = UnityEngine.Random.Range(1, 101);
                if (rand < 85)
                {
                    ev.IsAllowed = false;
                    ev.Player.Broadcast(3, "<color=red>Дверь заклинило!</color>", Broadcast.BroadcastFlags.Normal, true);
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
                        ev.Amount = (int)(ev.Amount * 3.5f);
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
                    else
                    {
                        if (Plugin.stage == 1)
                        {
                            ev.Amount /= 2;
                        }
                        else if (Plugin.stage == 3)
                        {
                            ev.Amount *= 1.5f;
                        }
                        else if (Plugin.stage == 4)
                        {
                            ev.Amount *= 2;
                        }
                        else if (Plugin.stage == 5)
                        {
                            ev.Amount *= 4;
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
                }
                else if (ev.DamageType == DamageTypes.Falldown)
                {
                    if (Mutators.isFallDamageFatal)
                    {
                        ev.Amount = 690420;
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
                bool shouldHeal = true;
                if (Classes.Mutator.mutatorExists("moveOrDie")) //ok
                {
                    shouldHeal = false;
                    break;
                }
                yield return Timing.WaitForSeconds(0.75f);
                if (shouldHeal)
                {
                    foreach (var pl in Player.List)
                    {
                        if ((pl.Health < pl.MaxHealth && pl.Role != RoleType.Spectator) && !Mutators.noRegen)
                        {
                            pl.Health += 1;
                        }
                    }
                }
            }
        }

        static void genActivated(GeneratorActivatedEventArgs ev)
        {
            ev.Generator.Engaged = false;
            ev.Generator.Activating = false;
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
            Plugin.suicideisKill = false;
            Plugin.shopInventory.Clear();
            Classes.Mutator.disableAll();
            Plugin.isRunning = false;
            Plugin.isOverriden = false;
            Plugin.isPlayerOverriden = false;
            StopEvent();
            Plugin.shopInventory.Add(Classes.shopItem.initialize("60 Дополнительного здоровья", "ahp", 10, (pl) =>
            {
                pl.ArtificialHealth += 60;
            }));
            Plugin.shopInventory.Add(Classes.shopItem.initialize("SCP-500", "scp500", 20, (pl) =>
            {
                if (pl.Items.Count == 8)
                {
                    pl.Broadcast(5, "Нет свободных слотов, монеты не списаны!", Broadcast.BroadcastFlags.Normal, true);
                    Plugin.shopDict[$"{pl.Nickname} ({pl.UserId})"] += 20;
                }
                else
                {
                    pl.AddItem(ItemType.SCP500);
                }
            }));
            Plugin.shopInventory.Add(Classes.shopItem.initialize("Кола", "coke", 10, (pl) =>
            {
                if (pl.Items.Count == 8)
                {
                    pl.Broadcast(5, "Нет свободных слотов, монеты не списаны!", Broadcast.BroadcastFlags.Normal, true);
                    Plugin.shopDict[$"{pl.Nickname} ({pl.UserId})"] += 10;
                }
                else
                {
                    pl.AddItem(ItemType.SCP207);
                }
            }));
            Plugin.shopInventory.Add(Classes.shopItem.initialize("Тинькофф блэк (чёрная карта)", "o5", 20, (pl) =>
            {
                if (pl.Items.Count == 8)
                {
                    pl.Broadcast(5, "Нет свободных слотов, монеты не списаны!", Broadcast.BroadcastFlags.Normal, true);
                    Plugin.shopDict[$"{pl.Nickname} ({pl.UserId})"] += 20;
                }
                else
                {
                    pl.AddItem(ItemType.KeycardO5);
                }
            }));
            Plugin.shopInventory.Add(Classes.shopItem.initialize("Граната", "frag", 15, (pl) =>
            {
                if (pl.Items.Count == 8)
                {
                    pl.Broadcast(5, "Нет свободных слотов, монеты не списаны!", Broadcast.BroadcastFlags.Normal, true);
                    Plugin.shopDict[$"{pl.Nickname} ({pl.UserId})"] += 15;
                }
                else
                {
                    pl.AddItem(ItemType.GrenadeHE);
                }
            }));
            Plugin.shopInventory.Add(Classes.shopItem.initialize("Респавн за случайную команду", "respawn", 50, (pl) =>
            {
                if (pl.Role == RoleType.Spectator)
                {
                    int randomTeam = UnityEngine.Random.Range(1, 3);
                    if (randomTeam == 1)
                    {
                        Plugin.overrideHisRespawn = pl.Id;
                        pl.Role = RoleType.NtfSpecialist;
                        Plugin.overrideHisRespawn = 0;
                        pl.ClearInventory();
                        pl.AddItem(ItemType.ArmorCombat);
                        pl.AddItem(ItemType.GrenadeHE);
                        pl.AddItem(ItemType.Medkit);
                        pl.AddItem(ItemType.Medkit);
                        pl.AddItem(ItemType.Adrenaline);
                        pl.AddItem(ItemType.KeycardNTFCommander);
                        pl.AddItem(ItemType.GunE11SR);
                        pl.AddItem(ItemType.Flashlight);
                    }
                    else
                    {
                        Plugin.overrideHisRespawn = pl.Id;
                        pl.Role = RoleType.ChaosConscript;
                        Plugin.overrideHisRespawn = 0;
                        pl.ClearInventory();
                        pl.AddItem(ItemType.ArmorCombat);
                        pl.AddItem(ItemType.GrenadeHE);
                        pl.AddItem(ItemType.Medkit);
                        pl.AddItem(ItemType.Medkit);
                        pl.AddItem(ItemType.Adrenaline);
                        pl.AddItem(ItemType.KeycardNTFCommander);
                        pl.AddItem(ItemType.GunAK);
                        pl.AddItem(ItemType.Flashlight);
                    }
                }
                else
                {
                    pl.Broadcast(5, "<color=yellow>Ты ещё жив, сначала умри, а потом уже ресайся =)</color>", Broadcast.BroadcastFlags.Normal, true);
                    Plugin.shopDict[$"{pl.Nickname} ({pl.UserId})"] += 50;
                }
            }));
            Log.Debug("DEBUG: Shop has these items initialised:\n");
            foreach (var item in Plugin.shopInventory)
            {
                Log.Debug($"Item {item.commandname} initialized!");
            }

            /*
             How 2 add Mutators
            by Treeshold#0001 for Treeshold#0001
            Step 1: Create name with color! It will be shown to a player!
            Step 2: Create name for development/admins. It will be shown in logs and can be called using fg_override
            Step 3: Add 3 functions. From 1st to last: Mutator chosen, Mutator stopped, Player respawned when Mutator was active.
            Psst. If you don't need to check respawn, just type "(pl) => { }" as shown in the example
            If you are planning to do something with newly respawned player, use "(Player pl) => { //Your code here }". 
            You will probably need one, if you use effects OR if mutator is tethered to only alive players, and if player is dead, the effect is no more.
            Player object is called only in repsawn method, other methods don't receive anything!
            Now go, and add your mutators!
             */
            Plugin.loadedMutators.Add(Classes.Mutator.initialize("<color=orange>Двери заклинило</color>", "doorJam", () => { Mutators.doorJam = true; }, () => { Mutators.doorJam = false; }, (pl) => { }));
            Plugin.loadedMutators.Add(Classes.Mutator.initialize("<color=blue>Скорость передвижения увеличена!</color>", "speed++", () =>
            {
                Mutators.fastRun = true;
                foreach (var pl in Player.List)
                {
                    if (pl.Role != RoleType.Spectator)
                    {
                        pl.EnableEffect(Exiled.API.Enums.EffectType.Scp207);
                    }
                }
            },
            () =>
            {
                Mutators.fastRun = false;
                foreach (var pl in Player.List)
                {
                    pl.DisableEffect(Exiled.API.Enums.EffectType.Scp207);
                }
            },
            (Player pl) =>
            { pl.EnableEffect(Exiled.API.Enums.EffectType.Scp207); }));
            Plugin.loadedMutators.Add(Classes.Mutator.initialize($"<color=yellow>Нет света</color>", "lightsOut", () =>
            {
                Cassie.Message("Danger . light control system is .g4 .g4 .g4 error . error .g4 .g4 .g1. g2 3 . 2 . 1");
                Timing.CallDelayed(Cassie.CalculateDuration("Danger . light control system is .g4 .g4 .g4 error . error .g4 .g4 .g1. g2 3 . 2 . 1") + 3f, () =>
                {
                    Map.TurnOffAllLights(10000f, Exiled.API.Enums.ZoneType.Unspecified);
                });
            }, () =>
            { Map.TurnOffAllLights(0); }, (pl) => { }));
            Plugin.loadedMutators.Add(Classes.Mutator.initialize($"<color=red>Урон от оружий усилен! (3.5X)</color>", "damage++", () => { Mutators.areShotsMoreDeadly = true; }, () => { Mutators.areShotsMoreDeadly = false; }, (pl) => { }));
            Plugin.loadedMutators.Add(Classes.Mutator.initialize($"<color=red>Падение с любой высоты фатально</color>", "fallDamageFatal", () => { Mutators.isFallDamageFatal = true; }, () => { Mutators.isFallDamageFatal = false; }, (pl) => { }));
            Plugin.loadedMutators.Add(Classes.Mutator.initialize($"<color=red>Нет пассивной регенерации</color>", "noPassiveRegen", () => { Mutators.noRegen = true; }, () => { Mutators.noRegen = false; }, (pl) => { }));
            Plugin.loadedMutators.Add(Classes.Mutator.initialize($"<color=green>Рентгеновское зрение</color>", "legalWH", () =>
            {
                foreach (var pl in Player.List)
                {
                    if (pl.Role != RoleType.Spectator)
                    {
                        pl.EnableEffect(Exiled.API.Enums.EffectType.Visuals939);
                    }
                }
            }, () =>
            {
                foreach (var pl in Player.List)
                {
                    pl.DisableEffect(Exiled.API.Enums.EffectType.Visuals939);
                }
            }, (Player pl) =>
            {
                pl.EnableEffect(Exiled.API.Enums.EffectType.Visuals939);
            }));

            //Unless I will find a way to heal player, if he move... WAIT! I GOT IT!
            Plugin.loadedMutators.Add(Classes.Mutator.initialize($"<color=blue>Move or Die</color>", "moveOrDie", () =>
            {
                foreach (var pl in Player.List)
                {
                    Timing.RunCoroutine(Mutators.doMoveOrdie(pl), "MoveOrDie");
                }
            }, () =>
            {
                Timing.KillCoroutines("MoveOrDie");

            }, (Player pl) =>
            {
                Timing.RunCoroutine(Mutators.doMoveOrdie(pl), "MoveOrDie");
            }));
            Plugin.loadedMutators.Add(Classes.Mutator.initialize("<color=orange>Тесла-ворота отключены.</color>", "teslagatesareIdle", () =>
            {
                foreach (var tesla in Map.TeslaGates)
                {
                    tesla.enabled = false;
                }
            }, () =>
            {
                foreach (var tesla in Map.TeslaGates)
                {
                    tesla.enabled = true;
                }
            }, (pl) => { }));
            Plugin.loadedMutators.Add(Classes.Mutator.initialize("<color=orange>Густой туман</color>", "denseFog", () =>
            {
                foreach (var pl in Player.List)
                {
                    pl.EnableEffect(Exiled.API.Enums.EffectType.Amnesia);
                }
            }, () =>
            {
                foreach (var pl in Player.List)
                {
                    pl.DisableEffect(Exiled.API.Enums.EffectType.Amnesia);
                }
            }, (pl) =>
            {
                pl.EnableEffect(Exiled.API.Enums.EffectType.Amnesia);
            }));

            Log.Debug("These mutators were successfully loaded!");
            foreach (var mut in Plugin.loadedMutators)
            {
                Log.Debug($"Name: {mut.commandName}\n");
            }

            /*
             so i am close to 1069 lines
             this is pog
             i'd say.
             hello my nervous system
             we fucking
             finally
             nailed it!
             */
        }
    }
}