# alt:V Cloth Tool

### Generate **alt:V resource** / **Singleplayer dlc** or **FiveM resource** for addon ped components / props
#### Join our GTA 5 Modding Discord server: https://discord.gg/hgSutAU

![screen-1](https://image.prntscr.com/image/MKOD2aGBQ5GRtIytkFx2cQ.png)
![screen-2](https://image.prntscr.com/image/W9Mx-YdXTFijeJB6Mih0sA.png)

# Useful projects
If you are in search for an easy way to browse all GTA V clothes data, try **Pleb Masters: Forge**.
[![Pleb Masters Forge Logo](https://i.imgur.com/hotlSPf.png)](https://forge.plebmasters.de)

# Additional notes
## from `grzybeek#9100`
```
The only components that SHOULD have _r is lowr, feet and head. (Only if .ydd model contains skin, otherwise it should be _u)
Rest should have _u

.ydd model ends with _u = .ytd texture needs to have _uni
.ydd model ends with _r = .ytd texture needs to have _whi
```
## from `DurtyFree#3216`
#### peds .ymt postfixes for models & textures
For models (`_postfix`)
```
_r (race)
_u (universal)
_m (?? - only used 4 times)
_g (?? - only used twice)
_p (props - used in cutscenes)
```
For textures (`texIds` = `_postfix`)
```
0  = '_uni' (universal)
1  = '_whi' (white)
2  = '_bla' (black)
3  = '_chi' (chinese)
4  = '_lat' (latino)
5  = '_ara' (arabic)
6  = '_bal' (baltic)
7  = '_jam' (jamaican)
8  = '_kor' (korean)
9  = '_ita' (italian)
10 = '_pak' (pakistani - resembles indians mostly, like shopkeepers)
```
## How to fix addon hats
#### See the following tutorial on how to fix addon hats: https://forum.cfx.re/t/how-to-fix-addon-hats

# Credits
### - [Tuxick](https://github.com/emcifuntik) for initial version of this tool :)
### - [GiZz (indilo53)](https://github.com/indilo53) for various fixes and improvements
### - [DurtyFree](https://github.com/durtyfree) for various fixes and improvements
### - [alt:V](https://altv.mp/) for the icon, branding and being the superior GTA 5 mp platform
