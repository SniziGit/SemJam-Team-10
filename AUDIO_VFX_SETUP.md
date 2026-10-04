# Audio and VFX Setup Guide

This guide will help you set up the sound system and visual effects for your game.

## 1. AudioManager Setup

**Create AudioManager GameObject:**
1. In Unity, create an empty GameObject named "AudioManager"
2. Add the `AudioManager` script to it
3. Configure the following in the Inspector:
   - **Audio Sources**: Two AudioSources will be auto-created (musicSource and sfxSource)
   - **Sound Effects**: Assign your audio clips:
     - `footstepSound`: Walking sound effect
     - `attractSound`: Magnetic attraction sound
     - `repelSound`: Magnetic repulsion sound
     - `winSound`: Victory/level complete sound
     - `deathSound`: Player death/fall sound
     - `playerCollisionSound`: Sound when players collide in win zone
   - **Background Music**: Assign your background music clips:
     - `mainMenuMusic`: Main menu background music
     - `level1Music`: Level 1 background music
     - `level2Music`: Level 2 background music
     - `level3Music`: Level 3 background music
   - **Audio Mixer**: Optional - assign your audio mixer for better control
   - **Volume Settings**: Adjust master, music, and SFX volumes as needed

**Note:** The AudioManager uses DontDestroyOnLoad, so it will persist across scenes.

## 2. VFX Prefab Creation

### Footstep Dust VFX
1. Create an empty GameObject
2. Add the `FootstepDustVFX` script
3. Save as a prefab in `Assets/Prefabs/VFX/`
4. The script auto-configures a particle system for dust clouds

### Magnetic Attract VFX
1. Create an empty GameObject
2. Add the `MagneticAttractVFX` script
3. Save as a prefab in `Assets/Prefabs/VFX/`
4. The script auto-configures cyan/blue particles for attraction

### Magnetic Repel VFX
1. Create an empty GameObject
2. Add the `MagneticRepelVFX` script
3. Save as a prefab in `Assets/Prefabs/VFX/`
4. The script auto-configures red/orange particles for repulsion

### Win Celebration VFX
You already have `EndPoint VFX .prefab` - you can use this or create a new one with particle effects.

### Death VFX
1. Create an empty GameObject
2. Add the `DeathVFX` script
3. Save as a prefab in `Assets/Prefabs/VFX/`
4. The script auto-configures yellow-to-red explosion particles

## 3. PlayerController Setup

For each player character (Aster and Roy):

1. Select the Player GameObject
2. In the Inspector, find the new **Footstep Settings** section:
   - `Footstep VFX Prefab`: Assign your FootstepDustVFX prefab
   - `Footstep Interval`: Adjust timing (default 0.4s)
   - `Footstep Spawn Point`: Optional - assign a Transform for where dust spawns (defaults to groundCheck)
3. In the Inspector, find the new **Death Settings** section:
   - `Death VFX Prefab`: Assign your DeathVFX prefab

## 4. MagnetController Setup

For each player's MagnetController component:

1. Select the Player GameObject
2. In the Inspector, find the new **VFX Settings** section:
   - `Attract VFX Prefab`: Assign your MagneticAttractVFX prefab
   - `Repel VFX Prefab`: Assign your MagneticRepelVFX prefab
   - `VFX Spawn Point`: Optional - assign a Transform for where VFX spawns (defaults to player position)

## 5. EndFlag Setup

For your level's end flag/trigger:

1. Select the EndFlag GameObject
2. In the Inspector, find the new **VFX Settings** section:
   - `Win VFX Prefab`: Assign your win celebration VFX prefab (or use existing "EndPoint VFX .prefab")
   - `VFX Spawn Point`: Optional - assign a Transform for where VFX spawns (defaults to flag position)
3. In the Inspector, find the new **Collision Settings** section:
   - `Player Collision Distance`: Adjust the distance at which players trigger the collision sound (default 1f)

## 6. Level Music Setup

For each scene (MainMenu, Level 1, Level 2, Level 3):

1. Create an empty GameObject named "LevelMusicManager"
2. Add the `LevelMusicManager` script to it
3. This will automatically play the appropriate music based on the scene name:
   - Scenes with "mainmenu", "menu", or "title" → mainMenuMusic
   - Scenes with "level1", "level 1", or "lvl1" → level1Music
   - Scenes with "level2", "level 2", or "lvl2" → level2Music
   - Scenes with "level3", "level 3", or "lvl3" → level3Music
   - Any other scene → defaults to mainMenuMusic

**Note:** Ensure your scene names match these patterns for automatic music detection.

## Audio Professional Tips

### Sound Design:
- **Footsteps**: Use subtle, soft sounds. Vary pitch slightly (already implemented in AudioManager)
- **Attract/Repel**: Use whoosh/magnetic sounds. Attract should feel "pulling", repel should feel "pushing"
- **Win**: Use a triumphant, celebratory sound
- **Background Music**: Loop seamlessly, keep volume low enough not to overpower SFX

### Audio Mixer Setup (Optional):
Create an Audio Mixer with these exposed parameters:
- `MasterVolume`: Overall game volume
- `MusicVolume`: Background music volume
- `SFXVolume`: Sound effects volume

Assign the mixer to the AudioManager for professional volume control.

### VFX Polish:
- The VFX scripts auto-configure particle systems
- Adjust particle colors/sizes in the scripts if needed
- All VFX prefabs auto-destroy after playing

## Testing Checklist

- [ ] Background music plays when level loads
- [ ] Footstep sounds play when walking on ground
- [ ] Footstep dust VFX appears when walking
- [ ] Attract sound plays when attracting objects
- [ ] Attract VFX (cyan/blue) appears on attraction
- [ ] Repel sound plays when repelling objects
- [ ] Repel VFX (red/orange) appears on repulsion
- [ ] Win sound plays when both players enter end zone
- [ ] Win VFX appears on level completion
- [ ] Death sound plays when player falls into KillPlane
- [ ] Death VFX (yellow-to-red explosion) appears on death
- [ ] Player collision sound plays when players collide in win zone after winning

## File Structure

```
Assets/
├── Scripts/
│   ├── AudioManager.cs
│   ├── LevelMusicManager.cs
│   ├── PlayerController.cs (updated)
│   ├── MagnetController.cs (updated)
│   ├── EndFlag.cs (updated)
│   └── VFX/
│       ├── FootstepDustVFX.cs
│       ├── MagneticAttractVFX.cs
│       └── MagneticRepelVFX.cs
└── Prefabs/
    └── VFX/ (create this folder)
        ├── FootstepDustVFX.prefab
        ├── MagneticAttractVFX.prefab
        ├── MagneticRepelVFX.prefab
        └── DeathVFX.prefab
```
