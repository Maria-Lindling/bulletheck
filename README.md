# bulletheck
## Description
placeholder

## Instructions
### Host
- Import SQL database found at `Assets\CSharp\DatabaseClient\phpSQLdb\bulletheck.sql` into xampp phpMyAdmin
- Copy files found at `Assets\CSharp\DatabaseClient\phpApi\*` to `C:\xampp\htdocs\bulletheck_api\*`
- Start the game using the Unity Editor on the machine running xampp
- Select the option to Host game.
- Start a second instance of the game.
- Select the option to Join game.
- Have fun.

## Technical Overview
### RPCs used
- ServerRPCs used
  - `ClientShell` utilizes several ServerRPCs to transmit PlayerInput to the server.
  - `GameFinishMenuOverlay` utilizes a ServerRPC to allow the client to submit InputField information asynchronously.
  - `LogDriver` utilizes a ServerRPC used to log messages to the server console. (⬇️low relevance)
- ObserversRPCs used
  - `GameFinishMenuOverlay` uses an ObserversRPC to open itself for all players, while players can still navigate this menu asynchronously.
  - `BulletController` uses an ObserversRPC to orient the bullet sprite independently of the bullet's own transformation.
  - `EnemyController` uses an ObserversRPC for the same reason as `BulletController`.
- TargetRPCs used
  - `ClientShell` utilizes a TargetRPC to selectively switch a client's input map.
  - `WorldManager` utilizes a TargetRPC to assign the local client to that local `WorldManager`. (⬇️low relevance)

### SyncVars used
- `ClientShell`
  - `syncPlayerSeat` to distinguish if this client is Pilot or Gunner
  - `syncPlayerVessel` to allow the server to assign a vessel to the player/s
  - `syncIsReady` to sync the ready-state of the client (⬇️low relevance / not used)
- `GameFinishMenuOverlay`
  - `syncHostName` to sync input information, but "live update" for the InputField representing the other player isn't implemented (⬇️low relevance)
  - `syncClientName` to sync input information, but "live update" for the InputField representing the other player isn't implemented (⬇️low relevance)
  - `syncSessionScore` to display the server-authoritative score to all clients and hold this data for leaderboard submission
- `LeaderboardEntry` (because only the server connects to the leaderboard db)
  - `syncVarHost` name of the entry's pilot
  - `syncVarClient` name of the entry's gunner
  - `syncVarScore` the score of the entry
- `LogField`
  - `syncText` the text of the log message
- `ScoreDisplay`
  - `Label` the label of the score display
  - `Value` the value of the score display
- `WaitMessage`
  - `syncMessage` the text of the wait message
  - `syncVisibility` the visibility of the wait message
- `PlayerVessel`
  - `syncHitPoints` the player vessel's hitpoints
  - `isReady` flag for both players being ready (⬇️low relevance / not used)
  - `syncMove` current movement derived from player input
  - `syncLook` current mouse look derived from player input
  - `syncAtk1` active/inactive status of primary attack
  - `syncAtk2` active/inactive status of secondary attack
  - `syncAtk3` active/inactive status of turret attack
- `WorldManager`
  - `player1Name` (⬇️low relevance / not used)
  - `player2Name` (⬇️low relevance / not used)
  - `syncPlayerScore` the server authoritative tracking of the players' score
  - `gameState` the current state of the game (playing/paused/finished/etc)
### Bullet-logic
Bullets are spawned with an orign, a speed, an orientation and a despawn timer, then proceed to move forward until they are either despawned, or they connect with a hitbox eligible for them to do damage to; upon which they will stop moving, do damage and then despawn early.

Bullets are spawned in radial patterns which consist of an initial orientation, how many lines of bullets are spawned, how many bullets each of these lines contains and how they are spaced, and how big the angle between each line of bullets is.

### Enemy-logic
Enemies are spawned in waves called "Encounters" and move forward (screen-down) at a fixed rate, firing their weapon at a fixed interval. They vary in appearance, bullet-pattern and health pool. They are despawned if they take sufficient damage, or if their despawn timer runs out.

## Persistence
> examples: PHP/SQL, JSON, PlayerPrefs
The only persistence used is the saving of scores and player names in the SQL database.

## Overview of bonus features
### Game-Event System
Steuerung von und Kommunikation zwischen verschiedenen Spielelementen wird über ein Ereignissystem geregelt, welches auf dem Server
### Modulares Bullet-Pattern System
Schussmuster können als ScriptableObject erstellt und beliebig konfiguriert werden.
### Out-Of-Editor Spielbarkeit
Das Spiel beinhaltet eine Setupmenü, mit welchem man den Build des Spiels außerhalb des Unity-Editor's erfolgreich spielen kann. Zwei lokale Clients ist erfolgreich getestet worden, aber Spielbarkeit zwischen verschiedenen Maschinen ist noch ungewiss.   

## Known Issues
Working alone on a very open-ended project made it extremely difficult to pre-plan, maintain focus/organisation and avoid episodes of panic.

***

## Project Checklist
### 1. Multiplayer
- [x] Set up FishNet in the Unity Project
- [x] Host/Client-Connection is possible
- [x] Two instances can establish a stable connection
- [x] The NetworkManager is configured correctly

### 2. Multiplayer
- [x] Top-Down movement
  - [x] Player
  - [x] Bullet
  - [x] Enemy
- [x] Players are implemented as NetworkObjects with NetworkBehaviour
- [x] Ownership is correctly implemented (only a client's owned objects take that client's input)
- [x] Movement within the game is handled under server authority
- [x] At least one SyncVar

### 3. Shooting & Bullet Hell mechanics
- [x] Network-capable projectile-system
- [x] At least two different bullet patterns
- [x] Projectiles are synchronized properly
- [x] Rate of fire and cooldown system present
- [x] Hit detection and damage functions correctly

### 4. Enemy-, Wave-, or Boss-System 
- [x] At least two different enemy-types
- [x] Enemies are spawned from server-side
- [x] Either
  - [ ] one Boss enemy
  - [x] a clearly defined wave-structure
- [x] Enemy- and Boss-bullets are consistently visible on both clients

### 5. Health, Hits & Gamesequence
- [x] Players feature a health- or lives-system
- [x] Damage and Hits are correctly synchronized
- [x] Clear game-flow (Start ➡️ Game ➡️ Game Over / Victory ➡️ Endscreen or Restart)

### 6. HUD & Points
- [x] HUD features at least
  - [x] Health / Lives
  - [x] Score
- [x] Distribute points for kills and/or time survived
- [x] Score is synchronized and displayed correctly
- [x] history/listing of High Scores is maintained server-side via PHP & SQL

### 7. Bonus features
- [x] Scrolling background
  - [ ] With parallax effect
- [x] Custom connection / game-setup menu
- [x] Game Event System
- [x] Modular Bullet Patterns

***

## Asset Sources
- [Backdrop_Galaxy_01](https://pixabay.com/illustrations/universe-space-galaxy-wormhole-7140671/)
