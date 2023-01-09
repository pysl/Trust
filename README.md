# Trust
A Unity Game meant to mimic the game described by this [video](https://www.youtube.com/watch?v=Z3A7slXw7mM).

## How to Play
Upon joining a server, (an active one is hosted at ```nahbruhfr.org``` on default port) the player can move to a different tile with space. If that tile is inhabited by another player, you will attack the player. If you are attacked, you will lose health. If you lose all your health, you die. Attacking or moving uses action points (which are given every "round", decided by the server owner). If you run out of action points, you cannot move or attack. If you die, you are out of the game. The last player standing wins. The twist is that players may give other players action points (by hovering over a player and press "Enter"). This creates an incentive to make friends and work together, but also creates a risk of betrayal.

## How to Host
Simply go to www.nahbruhfr.org/trust/ to download the latest server software (or clone the repository and download the server software). Run ```install.bat``` for Windows or ```install.sh``` for MacOS and Linux. NodeJS is required. Then run ```run.bat``` for Windows or ```run.sh``` for MacOS and Linux. The settings can be changed in ```settings.json```. The server will be hosted on port 3000 by default. The server can be stopped by pressing Ctrl+C in the terminal.

## Current Issues
I did the vast majority of this project in 2 days, so there are a lot of issues. Many of these issues are small bugs but a lot of them are structural issues. I will try to fix these issues as I have time, but I am not sure when I will have time to do so. If you would like to help, feel free to make a pull request.

### Some notable issues:
    [+] I use http requests for the multiplayer aspect of the game. (I know this is dumb but it was late and it worked.)
    [+] An issue with the size of players remaining on tiles that they have left.
    [+] The server will crash randomly following a lookup for an item that doesn't exist in the database.