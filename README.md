# Legend Of Kevin
The legend of a kid who crash lands on an alien planet.

Install the dotnet runtime v7.0 from MS and the MGCB

    sudo apt install dotnet-sdk-7.0
    cd git/legend-of-kevin
    dotnet tool install -g dotnet-mgcb
    cd ./data/game-content
    dotnet mgcb --outputDir=../../bin/Debug/net7.0/game-content --intermediateDir=../../obj/mgcb-temp ./game-content.mgcb

    should put the game-content in the /bin dir

## Controls
Use the arrow keys to move the character and click things with the mouse.

Right mouse button uses teh equipped weapon.

Use the mouse wheel to srcoll between different weapons.
