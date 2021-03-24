Weapon Features
Add unique features to your weapons.

## **This is an OpenMod exclusive plugin.**
For more information on migrating to OpenMod, [click here.](https://openmod.github.io/openmod-docs/user-guide/installation/unturned/)
Feel free to ask questions in the [OpenMod Discord.](https://discord.gg/6yy7gxk)

### **Have you ever wanted to add extra features to Unturned's basic weapons?**
### **Would you like players' legs to be broken when shot by a Dragonfang?**
### **How about a health boost whenever a player is killed?**
# Features

With Weapon Features, you can add unique features to your weapons.
The full list of features are:
- Legs breaking when damaged
- Burning the player when damaged
- Cause drug effects to the damaged player
- Dehydrate the player when damaged
- Starve the player when damaged
- Heal the weapon holder when they kill a player
- Drain a player's stamina when damaged

**More features can be requested.**

All of these features can be based on chance. Allowing you to use many features on the same weapon without making it overpowered.

This plugin is reloadable, allowing on-the-fly changes to its configuration.

Rather than running every frame, this plugin uses events to ensure **zero lag** caused by this plugin.

# Default Configuration (contains guide):

```yaml
# Adding a '#' before text comments it out

# Here is a run down of how the configuration works:

#    The id specifies which weapon this should effect.
#    This id can either be the name of the weapon (i.e. Eaglefire) or the number-based id (i.e. 4).

#    Every weapon configuration is followed by the features.
#    Each feature has its own properties. As seen below, 'StaminaDrain' has Chance and Amount.

# Chances:
#    Every feature has the 'Chance' property, but it doesn't need to be set.
#    If no chance is set (or ~), the feature will always apply.
#    If chances are set, only one feature will take effect.

#    For example, if the Dehydrate and Starve effects are configured with chances of 0.3 (30%),
#    there will be a 30% chance of the Dehydrate effect being applied,
#    a 30% chance of the Starve effect being applied, and a remaining
#    40% chance that nothing will happen.

#    If in the same weapon configuration, a DrugEffect feature is configured with no chance set,
#    the DrugEffect feature will still always apply.

# You can uncomment (remove the '#') features below to add them to the Eaglefire config

# The example below will:
# - Always break the victim's legs
# - Always heal the weapon holder upon killing (20 hp, heals broken legs and bleeding)
# - Have a 30% chance of dehydrating the victim
# - Have a 30% chance of draining the victim's stamina

Weapons:
- Id: Eaglefire
  BreakLegs: # Break the victim's legs when damaged
    Chance: ~ # Same as not specifying a chance. This is required for the BreakLegs feature.
    
#  BurnEffect: # Applies the burning effect to the victim when damaged (similar to campfires)
#    Duration: 2 # seconds
#    Damage: 10 # this is optional and defaults to 10.
    
  Dehydrate: # Dehydrates the victim when damaged
    Chance: 0.3 # Chances must be written in decimal. 0.3 is the same as a 30% chance.
    Amount: 20
    
#  DrugEffect: # Applies the drug effect to the victim when damaged (same as glue/berries)
#    Duration: 10 # seconds
    
  HealHolder: # Heals the holder when they kill a player
    Amount: 20 # HP
    HealBleeding: true # Heals the player's bleeding
    HealBroken: true # Heals the player's broken legs
    
  StaminaDrain: # Drains the victim's stamina when damaged
    Chance: 0.3 # 30% chance of taking effect
    Amount: 10
    
#  Starve: # Starves the victim when damaged
#    Amount: 10
```

## **Questions? Found a bug? Need a plugin?**

## [Join my Discord Server](https://discord.gg/SjFYeFr), [Add me on Steam](https://steamcommunity.com/id/iamsilk), or even [Email me](mailto:silksplugins@gmail.com)