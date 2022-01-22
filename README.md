
# Funny Guns
Some sort of auto-event for SCP:Secret Lab servers based on Exiled created by stoopid me

## Description
Auto-event consists of 5 stages, at which:

 - Stage 1: No mutators, damage is halved!
 - Stage 2: 1 mutator, damage is normal!
 - Stage 3: 2 mutators, damage is 1.5x!
 - Stage 4: 3 mutators, damage is 2x!
 - Stage 5: Instant death (health is drained), no mutators, damage is 5x!


## Mutators' events and their initialization
Each mutator object has 3 methods.
On assigned (first one)
On unassigned (second one)
and On player respawn (third one)

2 first methods don't receive any arguments, but have code, which will be executed.
The last method get's player object. This event will be called only when a player respawns **AND** passes abuse check. *(check PlayerFuckingDied function)*

All mutators are initialized in WaitingForPlayers function (EventHandler.cs)
This is an example of assignment in pseudo-code

    LoadedMutator list -> Add object with properties: 
    1. Hud name - "<color=yellow>Жёлтая пчёлка - жжжъъж</color>"
    2. Console name - "zzhzhzh"
    3. Engaged method: () => { Log.Debug("zzhzhzh"); }
    4. Disengaged method () => { Log.Debug("no zzhzhzh"); }
    5. Player respawn method (Exiled.API.Features.Player pl) => { pl.Broadcast(5, "Bees are going in!"); }
    And that's it!

Player respawn method is required!

## Important

Plugin is no longer updated and probably outdated.
Tested on pre-christmas update.

# Have an idea, suggestion?
File an issue! I'm gonna look through it!
