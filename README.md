# Legend Of Kevin
Platform game prototype with wall climbing mechanic and weapon toolbelt.

![lok](lok.jpg)

## Build
* Linux: 
  1. Install the dotnet runtime v7.0 and the MGCB
  
     * sudo apt install dotnet-sdk-7.0
      
     * cd legend-of-kevin
    
  2. Install & Run MGCB
    
     * dotnet tool install -g dotnet-mgcb
    
     * cd ./data/game-content
    
     * dotnet mgcb --outputDir=../../bin/Debug/net7.0/game-content --intermediateDir=../../obj/mgcb-temp ./game-content.mgcb  

## Controls
WSAD: Move

Mouse: Aim

Right Mouse Button: use equipped weapon.

Mouse Wheel: Switch Weapon

Spacebar: Jump

Spacebar (when landing): Spring Jump

