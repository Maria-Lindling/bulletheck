# bulletheck
## Instructions
- Import SQL database found at `Assets\CSharp\DatabaseClient\phpSQLdb\bulletheck.sql` into xampp phpMyAdmin
- Copy files found at `Assets\CSharp\DatabaseClient\phpApi\*` to `C:\xampp\htdocs\bulletheck_api\*`
- Start the game using the Unity Editor on the machine running xampp
- Select the option to Host game.
- Start a second instance of the game.
- Select the option to Join game.
- Have fun.

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
- [ ] TBD/cont.

***

## Description
placeholder

## Instructions
### Host
placeholder
### Client
placeholder

## Technical Overview
### RPCs used
placeholder
### SyncVars used
placeholder
### Bullet-logic
placeholder
### Enemy-logic
placeholder

## Persistence
> examples: PHP/SQL, JSON, PlayerPrefs
placeholder

## Overview of bonus features
placeholder

## Known Issues
placeholder

***

## Asset Sources
- [Backdrop_Galaxy_01](https://pixabay.com/illustrations/universe-space-galaxy-wormhole-7140671/)
