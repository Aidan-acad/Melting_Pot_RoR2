# Melting Pot

A mixed bag.
## Included Items

### **_Tier 1_**

| Lead Fetters | Penitent's Fang | Festering Fang (Void) | Sapping Bloom |
| :- | :-: | :-: | :-: |
|<img src="https://imgur.com/wZvnv9N.png" width="200" height="200" />|<img src="https://i.imgur.com/n4ONzTP.png" width="150" height="200" /> | <img src="https://i.imgur.com/fPpF1Vm.png" width="150" height="200" /> | <img src="https://i.imgur.com/2fj510X.png" width="200" height="200" /> |
|*Increases __armour__ and __knockback resist__.*| *Adds a chance to apply __poison__ on hit* | *__Corrupts all Penitent's Fangs__<br> Adds a chance to blight on hit,<br> Past 100% more than one stack can be applied* | *Chance to __Weaken__ on hit*|

### **_Tier 2_**

| Echo Ring | Shield of Thorns | Mechanical Mosquito | Serrated Pellets |
| :- | - | - | -: |
| <img src="https://imgur.com/AKdw21x.png" width="200" height="200" /> | <img src="https://imgur.com/xXXZDO8.png" width="200" height="200" /> | <img src="https://i.imgur.com/iCLQrr4.png" width="250" height="200" /> | <img src="https://i.imgur.com/kwHqnTD.png" width="300" height="200" /> |
|*__Echoes attacks__ after a delay.*|*__Deal damage__ to enemies </br> attacking at __close range__*| *Drones and Turrets gain __bleed__ on hit,<br> and __heal__ for a portion of bleed damage dealt *| *Hitting an enemy with enough stacks of bleed __Haemorrhages__ them*|

### **_Tier 3_**


| Midas Cosh | Reactive Armour | Rage Toxin|
|:-:|:-:|:-:|
| <img src="https://imgur.com/5XBDT4y.png" width="200" height="200" /> | <img src="https://imgur.com/9zozM0g.png" width="220" height="200" /> | <img src="https://imgur.com/dG6temm.png" width="200" height="200" /> |
| *Chance to __stun__ enemies.<br> Enemies stunned by this effect<br> grant __gold on hit__.* | *__Absorb__ a portion of incoming damage <br>while stationary. Any skill cast while<br> moving returns absorbed damage as a __nova__.* | *Forces attacked enemies to __focus you__.<br> Enraged enemies __attack faster__ but deal<br>__less damage__, and have a chance to <br>__fumble attacks__ mini-stunning the enemy* |


### __*Lunar*__

| Burning Soul | Scrap Vamp |
|:-:|:-:|
| <img src="https://imgur.com/ChiC1QC.png" width="200" height="200" /> | <img src="https://imgur.com/5cowucP.png" width="200" height="200" /> |
| *__Burn enemies__ for own __max <br> health damage__, at the __cost<br> of current health__.* | Slay mechanical enemies for *__Buffs__ <br> but all drones are __Hostile__.* |


## Changelog
**0.0.55**
Added four new items, item concepts courtesy of EnderGrimm.
Added Item displays for new items, minus serrated pellets (Working on display model atm)

Removed debug log from reverb ring which may have been causing slowdowns lategame.
Reworked net code for scrap vamp, should be more consistent now. Added Mechanical Mosquito - Thanks to EnderGrimm for their input.

Reverb Ring no longer procs on non-generator attacks, looking at you plasma shrimp. Remade Scrap Vamp model.

Swapped out scrap vamp buff icons, added ItemStats and BetterUI support

Sound SFX now should scale properly, reworked lead fetters, buffed echo ring, added new lunar item scrap vamp

Fixed file hierarchy bug

Now updated for Sotv

Reverted Debug values

New Shield of Thorns icon curtosy of SOM. Fixed compat issue with MelT

Fixed item prefab sizes in game being too large

New Readme.

Ported to thunderkit
Now using hopoo shaders
New textures/Icons for items
Thanks to SOM for helping with the icon colour outlines and formatting.


--Burning Soul--

	Model and change to lunar rarity performed.
	Rewrote Lore and fixed positioning on REX and Engi
	Self damage now applies as a stacking debuff that ignores armor, shouldnt be able to kill the player at sub-100 health at one stack.
	Reverted accidental change to Tier 1 rarity
	Currently self-bleed seems to only apply to the host in mp.
	Added VFX, might be too big
	Adjusted vfx.
	Hopefully fixed mp inconsistencies
	Added RPC command to call dot on server instead of client.
	Changed DoT on self to non-lethal damage, added a 0.01 second cd to self burn application to hopefully stop multi-abilities, looking at you Miner R, from one-shotting the player
	Rewrote to hopefully sort MP desync
	Remade Model and icon
	Remade Model and icon again
	
--Lead Fetters--

	Added tier 1 item lead fetters
	Lowered fall speed and growth.
	Fixed issue with enemies in void fields causing errors when checking for character motors.
	
--Echo Ring--
	
	Added tier 2 item echo ring
	
--Shield of Thorns--
	
	Added tier 2 item shield of thorns
	Increased armour damage scaling -  was too weak before for the short range
	
--Midas Cosh--
	
	Added Tier 3 item midas cosh
	~Hopefully fixed issues with multi-triggering freeze explosions from glacial jellyfish~
	Wrapped problematic code in a try catch, should allow for uninterrupted.
	Actually fixed the problem this time. :)
	Upped proc chance, increased stun duration, and gave a tick of money on proc as late game too few hits were landing on the stunned enemy.
	Added Overlays for golden enemies. Redid cd buff icon.
	Added catch around overlays, should stop dying enemies from causing issues.
	
--Reactive Armour--

	Added Tier 3 item Reactive Armour
	Hopefully fixed mp bug
	Reverted accidental change to tier 1, upped damage resist to 20% base, still working on the mp bug.
	Rewrote to hopefully sort MP desync
	Set to be default blacklisted for AI use. Getting wallsplat by a scavenger was 0 fun.

--Rage Toxin--

	Added Tier 3 item Rage Toxin
	Added additional effects for SP viability.