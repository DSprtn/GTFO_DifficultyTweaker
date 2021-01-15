# GTFO_DifficultyTweaker
## A hacky configurable and in-game chat interactable mod for tweaking enemy spawn populations.

## Installation: 

 	1. Download and extract BepInEx into your GTFO game folder (SteamLibrary\steamapps\common\GTFO\) 
	from https://builds.bepis.io/projects/bepinex_be (x64, il2cpp version)
	
	2. Launch the game once and close it
	
	3. Download the latest version of the plugin from the releases tab.
	
	4. Place it into GTFO/Bepinex/Plugins/
	
	5. Launch the game again, done!
	
## Usage notes:

#### IN-GAME USAGE
    Type "Help" in the in-game chat to get available commands and game modes.
    
    Use ENEMY_MODES MODE to switch to an existing mode

    Use ENEMY_MULT NUMBER_HERE to set a multiplier for the enemy population
  
#### CONFIGURATION
    To create custom modes copy one of the existing ones and place it in the same folder. Edit it to
    your liking - use the help.txt file to help you along.
	
  
## Features:
    Weighted/randomized enemy population type/amount tweaking
    Weighted/randomized scout wave population and settings tweaking
    Fully configurable
    Works only if the host of the game has it.
	
## Known issues: 
    You need to restart the game if you want to play with the default enemy population after
    using one of the modes.
    
    Random crashes may occur, il2cpp seems to be quite unstable.
