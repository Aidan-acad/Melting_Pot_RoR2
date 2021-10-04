# Melting Pot

A mixed bag.
## Included Items
### --Tier 1--
--Lead Fetters--
Increases fall speed and knockback resist.
### --Tier 3--
--Midas Cosh--
Chance to stun enemies. Enemies stunned by this effect grant gold on hit.
--Reactive Armour--
Absorb a portion of incoming damage while stationary. Any skill cast while moving returns absorbed damage as a nova.
### --Lunar--
--Burning Soul--
Burn enemies for own max health damage, at the cost of current health.
## Changelog
**0.0.25**

Hopefully corrected issue with clients being turbo laggy

--Burning Soul Lunar Item--

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
--Lead Fetters--

	Added tier 1 item lead fetters
	Lowered fall speed and growth.
	Fixed issue with enemies in void fields causing errors when checking for character motors.
	
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