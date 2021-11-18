# Funny Guns
Some sort of auto-event created by stoopid me

## Description
Auto-event consists of 5 stages, at which:

 - Stage 1: No mutators, damage is halved!
 - Stage 2: 1 mutator, damage is normal!
 - Stage 3: 2 mutators, damage is 1.5x!
 - Stage 4: 3 mutators, damage is 2x!
 - Stage 5: Instant death (health is drained), no mutators, damage is 5x!

## How mutators work?
~~Pure random~~ 
**I am not talking how they are assigned!**
At first, plugin loads all mutators, designated at WaitingForPlayers. Adding one looks like this:

    Plugin.loadedMutators.Add(Classes.Mutator.initialize("<color=orange>Двери заклинило</color>", "doorJam", () => { Mutators.doorJam = true; }, () => { Mutators.doorJam = false; }, (pl) => { }));
As you can see, mutator is added to Plugin.loadedMutators list. When stage changes, we select random event, based on index. To select one, we do some unity random number generator. 
That's how we do it.

 1. We get count of loaded mutators
 2. We assign a variable named i
 3. We give the plugin 50 attempts to select random mutator
 4. If it selects anything, we override an error placeholder
 5. If not, we assign a mutator, which does nothing, but displays this: ```[ERROR] Failed to randomly select mutator, no mutator initiated.```
 6. We add it to selected mutators and invoke it's engaged method
 
`
    selected.engage.Invoke();
    Plugin.engagedMutators.Add(selected);
`

That's it! That's how you do it! Good luck in modifying the plugin!
