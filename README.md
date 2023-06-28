# Legend Of Kevin
Platform game prototype with wall climbing mechanic and weapon toolbelt.

![lok](lok.jpg)

## Build
* Linux: 
  1. Install the dotnet runtime v7.0 and the MGCB
  
     * sudo apt install dotnet-sdk-7.0
      
     * cd legend-of-kevin
    
  2. Install MGCB
    
     * dotnet tool install -g dotnet-mgcb
    
     * VSCode pre-launch should build the content if not run:

         * cd ./data/game-content

         * dotnet mgcb --outputDir=../../bin/Debug/net7.0/game-content --intermediateDir=../../obj/mgcb-temp ./game-content.mgcb  


## Controls
    * WSAD: Move

    * W: Interact / Talk

    * Mouse: Aim

    * Right Mouse Button: use equipped weapon.

    * Mouse Wheel: Switch Weapon

    * Spacebar: Jump

    * Spacebar (when landing): Spring Jump

## Misc
   * Publish Instructions
      * https://docs.monogame.net/articles/packaging_games.html
         * dotnet publish -c Release -r linux-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained
         * dotnet publish -c Release -r osx-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained
         * dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained
Build a .deb: https://www.baeldung.com/linux/create-debian-package
copy DEBIAN to the publish dir
dpkg-deb --root-owner-group --build ./publish legend-of-kevin
https://github.com/AppImageCommunity/pkg2appimage/releases/tag/continuous

Windows MSI       
* sudo apt-get install msitools nodejs npm && sudo npm install -g msi-packager