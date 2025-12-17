# bulletheck
## Project Checklist
### 1. Multiplayer
- [x] Set up FishNet in the Unity Project
- [ ] Host/Client-Connection is possible
- [ ] Two instances can establish a stable connection
- [ ] The NetworkManager is configured correctly

### 2. Multiplayer
- [ ] Top-Down movement
  - [x] Player
  - [x] Bullet
  - [ ] Enemy
- [ ] Players are implemented as NetworkObjects with NetworkBehaviour
- [ ] Ownership is correctly implemented (only a client's owned objects take that client's input)
- [ ] Movement within the game is handled under server authority
- [x] At least one SyncVar

### 3. Shooting & Bullet Hell mechanics
- [ ] Network-capable projectile-system
- [ ] At least two different bullet patterns
- [ ] Projectiles are synchronized properly
- [ ] Rate of fire and cooldown system present
- [ ] Hit detection and damage functions correctly

### 4. Enemy-, Wave-, or Boss-System 
- [ ] At least two different enemy-types
- [ ] Enemies are spawned from server-side
- [ ] Either
  - [ ] one Boss enemy
  - [ ] a clearly defined wave-structure
- [ ] Enemy- and Boss-bullets are consistently visible on both clients

### 5. Health, Hits & Gamesequence
- [ ] Players feature a health- or lives-system
- [ ] Damage and Hits are correctly synchronized
- [ ] Clear game-flow (Start ➡️ Game ➡️ Game Over / Victory ➡️ Endscreen or Restart

### 6. HUD & Points
- [ ] HUD features at least
  - [ ] Health / Lives
  - [ ] Score
- [ ] Distribute points for kills and/or time survived
- [ ] Score is synchronized and displayed correctly
- [ ] history/listing of High Scores is maintained server-side via PHP & SQL
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
