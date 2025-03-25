# Unity Movement System with Photon Fusion 2
[![Watch the video](https://github.com/billtruong003/jmancurly-test/raw/main/Assets/Project/Sprites/Video.gif)](https://github.com/billtruong003/jmancurly-test/raw/main/Assets/Project/Sprites/imageOVR.jpg)

## Overview
<p align="center">
  <img src="https://github.com/billtruong003/Responsive-WebGL-Template/raw/main/BillTheDevSample/Visualize/profile.webp" alt="Bill The Dev" style="border-radius: 50%; border: 5px solid #ff6600; width: 200px"/>
  <br>
  <strong style="font-size: 32px;">
    <span style="color: #FFA500; text-shadow: -1px -1px 0 white, 1px -1px 0 white, -1px 1px 0 white, 1px 1px 0 white;">Bill</span>
    <span style="color: #808080; text-shadow: -1px -1px 0 white, 1px -1px 0 white, -1px 1px 0 white, 1px 1px 0 white;">The</span>
    <span style="color: #000000; text-shadow: -1px -1px 0 white, 1px -1px 0 white, -1px 1px 0 white, 1px 1px 0 white;">Dev</span>
</strong>
</p>
This project implements a character movement system with multiplayer support using Photon Fusion 2's KCC (Kinematic Character Controller). It meets the requirements for a Unity Developer test, featuring smooth movement, character customization, and multiplayer capabilities.

![Image](https://github.com/billtruong003/jmancurly-test/raw/main/Assets/Project/Sprites/Img1.png)

## Features

1. **Character Controller**
   - Smooth WASD/arrow key movement
   - Mouse-based rotation
   - Jumping (spacebar)
   - Sprint functionality (shift key)
   - Physics-based movement with proper weight and feel
   - Implemented using Photon Fusion 2's KCC

2. **Multiplayer Implementation**
   - Built with Photon Fusion 2 in Shared Mode
   - Real-time movement synchronization
   - Multiple players in the same session
   - Player-controlled characters

3. **Character Customization**
   - 7 unique characters

4. **Models & Assets**
   - 4 KayKit models (chibi-style with single mecanim)
   - 3 Synty models (with advanced mecanim - locomotion)

## Screenshots

<p align="center">
  <img src="https://github.com/billtruong003/jmancurly-test/raw/main/Assets/Project/Sprites/imageOVR.jpg" alt="Gameplay Screenshot 1" style="width: 30%; margin: 0 1%;">
  <img src="https://github.com/billtruong003/jmancurly-test/raw/main/Assets/Project/Sprites/img2.png" alt="Gameplay Screenshot 2" style="width: 30%; margin: 0 1%;">
  <img src="https://github.com/billtruong003/jmancurly-test/raw/main/Assets/Project/Sprites/img3.png" alt="Gameplay Screenshot 3" style="width: 30%; margin: 0 1%;">
</p>

<p align="center">
  <img src="https://github.com/billtruong003/jmancurly-test/raw/main/Assets/Project/Sprites/img4.png" alt="Character Customization" style="width: 45%; margin: 0 1%;">
  <img src="https://github.com/billtruong003/jmancurly-test/raw/main/Assets/Project/Sprites/img5.png" alt="Multiplayer Gameplay" style="width: 45%; margin: 0 1%;">
  <img src="https://github.com/billtruong003/jmancurly-test/raw/main/Assets/Project/Sprites/img6.png" alt="Multiplayer Gameplay" style="width: 45%; margin: 0 1%;">
  <img src="https://github.com/billtruong003/jmancurly-test/raw/main/Assets/Project/Sprites/img7.png" alt="Multiplayer Gameplay" style="width: 45%; margin: 0 1%;">
  <img src="https://github.com/billtruong003/jmancurly-test/raw/main/Assets/Project/Sprites/img8.png" alt="Multiplayer Gameplay" style="width: 45%; margin: 0 1%;">
  <img src="https://github.com/billtruong003/jmancurly-test/raw/main/Assets/Project/Sprites/img9.png" alt="Multiplayer Gameplay" style="width: 45%; margin: 0 1%;">
</p>

## Technical Specifications
- **Unity Version**: 2022.3.50f1
- **Rendering**: Universal Render Pipeline (URP)
- **Networking**: Photon Fusion 2 (Shared Mode with KCC)
- **Input System**: Unity New Input System
- **Camera**: Custom camera implementation

## Setup Instructions
1. Clone or download the project
2. Or download the latest build from [Releases](https://github.com/billtruong003/jmancurly-test/releases/tag/PhotonFusion2)
3. Open with Unity 2022.3.50f1
4. Import required dependencies
5. Configure Photon App ID in NetworkRunnerManager
6. Open and play the main scene to test locally

## For Evaluators / Judges - Quick Start & Testing Guide
Thank you for evaluating this project! To test multiplayer functionality:
1. Download the latest release build from [Releases](https://github.com/billtruong003/jmancurly-test/releases/tag/PhotonFusion2) and extract it.
2. Open 2–4 instances of the game (e.g., via tabs or windows).
3. In one instance, select "Start Shared Client (P)"; in others, join with the same mode.
4. **Note**: Start one "Shared Client (P)" first, then have others join for better stability.
5. **Rare Issue**: If characters aren’t visible, restart the affected instance.

This setup allows you to evaluate real-time movement synchronization and multiplayer features using Photon Fusion 2's Shared Mode. Thank you again for your time!

---

## Multiplayer Mode Descriptions
- **Start Single Player (I)**: Offline single-player mode. No network or other players.
- **Start Shared Client (P)**: Development mode. Connects to a Shared Mode session for testing multiplayer features.
- **Start Server (S)**: Dedicated server mode. Runs independently for large-scale games.
- **Start Host (H)**: Host & play mode. Your PC acts as both server and player for medium-scale games.
- **Start Client (C)**: Client mode. Joins a game hosted by others.
- **Start Auto Host Or Client (A)**: Auto mode. Joins an existing game or creates one if none is found.

---

## More of my products
Explore more of my work:
- **[Spaceship Modular Project](https://billthedevlab.com/)**: Modular spaceship design with advanced 3D modeling.
- **[3000 Sketchfab Model Scraping Project](https://billthedev.online/)**: Scraping and organizing 3000 Sketchfab models.
- **[SpellTech-Storage](https://github.com/billtruong003/SpellTech-Storage)**: Web-based 3D model storage with AR features.
- **[Responsive WebGL Template](https://github.com/billtruong003/Responsive-WebGL-Template)**: Unity WebGL template for responsive scaling.
- **[Portfolio](https://billthedev.com)**: My full range of game development projects.

Connect with me on [LinkedIn](https://www.linkedin.com/in/billtruong003/) or via [my portfolio](https://billthedev.com) for collaboration opportunities.

## Implementation Notes
- Character movement uses Photon Fusion 2's KCC for smooth, reliable control.
- Two model types:
  - KayKit: Chibi-style with single mecanim.
  - Synty: Detailed with advanced locomotion.
- Custom third-person camera implementation.
- Network synchronization via Photon Fusion 2 in Shared Mode.

---

*This project was created as part of a technical assessment for a Unity Developer position.*
