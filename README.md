
# Unity Movement System with Photon Fusion 2
![Image](Assets/Project/Sprites/imageOVR.jpg)

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
This project implements a character movement system with multiplayer support using Photon Fusion 2's KCC (Kinematic Character Controller). The implementation follows the requirements for a Unity Developer test, featuring smooth movement, character customization, and multiplayer capabilities.
![Image](Assets/Project/Sprites/imageOVR.jpg)
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
   - Multiple players in same session
   - Player-controlled characters

3. **Character Customization**
   - 7 Different Characters

4. **Models & Assets**
   - 4 KayKit models (chibi-style with single mecanim)
   - 3 Synty models (with advanced mecanim - locomotion)

## Screenshots

<p align="center">
  <img src="Assets/Project/Sprites/Img1.png" alt="Gameplay Screenshot 1" style="width: 30%; margin: 0 1%;">
  <img src="Assets/Project/Sprites/img2.png" alt="Gameplay Screenshot 2" style="width: 30%; margin: 0 1%;">
  <img src="Assets/Project/Sprites/img3.png" alt="Gameplay Screenshot 3" style="width: 30%; margin: 0 1%;">
</p>

<p align="center">
  <img src="Assets/Project/Sprites/img4.png" alt="Character Customization" style="width: 45%; margin: 0 1%;">
  <img src="Assets/Project/Sprites/img5.png" alt="Multiplayer Gameplay" style="width: 45%; margin: 0 1%;">
  <img src="Assets/Project/Sprites/img6.png" alt="Multiplayer Gameplay" style="width: 45%; margin: 0 1%;">
  <img src="Assets/Project/Sprites/img7.png" alt="Multiplayer Gameplay" style="width: 45%; margin: 0 1%;">
  <img src="Assets/Project/Sprites/img8.png" alt="Multiplayer Gameplay" style="width: 45%; margin: 0 1%;">
  <img src="Assets/Project/Sprites/img9.png" alt="Multiplayer Gameplay" style="width: 45%; margin: 0 1%;">
</p>

## Technical Specifications
- **Unity Version**: 2022.3.50f1
- **Rendering**: Universal Render Pipeline (URP)
- **Networking**: Photon Fusion 2 (Shared Mode with KCC)
- **Input System**: Unity New Input System
- **Camera**: Custom camera implementation

## Setup Instructions

1. Clone or download the project
2. Or download the latest build from [Releases](https://github.com/username/repo/releases)
3. Open with Unity 2022.3.50f1
4. Import any required dependencies
5. Configure Photon App ID in NetworkRunnerManager
6. Open and play the main scene to test locally

## More of my products

If you're interested in seeing more of my work, you can check out these projects and resources:

*   **[Spaceship Modular Project](https://billthedevlab.com/):** Explore a modular spaceship project showcasing advanced 3D modeling and design.
*   **[3000 Sketchfab Model Scraping Project](https://billthedev.online/):** Discover a project involving the scraping and organization of 3000 models from Sketchfab, demonstrating web scraping and data handling skills.
*   **[SpellTech-Storage - 3D Model Web Storage (GitHub)](https://github.com/billtruong003/SpellTech-Storage):**  View the source code for SpellTech-Storage, a web-based 3D model storage platform similar to Sketchfab, featuring Augmented Reality (AR) capabilities for web-based object viewing.
*   **[Responsive WebGL Template for Unity](https://github.com/billtruong003/Responsive-WebGL-Template):** A Unity WebGL template that provides responsive scaling and full-screen support for WebGL builds, ensuring your WebGL games look great on any browser and device.
*   **[BillTheDev's Portfolio Website](https://billthedev.com):**  (Remember to replace with your actual main portfolio link if you have one, otherwise remove this line) Visit my portfolio website to see a wider range of my game development projects, including demos, videos, and more detailed information about my skills and experience.

This is just a small selection of my work. I am always exploring new technologies and creating exciting projects. Feel free to connect with me on [LinkedIn](https://www.linkedin.com/in/billtruong003/) (Remember to replace with your LinkedIn profile) or reach out through my portfolio website to discuss potential collaborations or opportunities.
## Implementation Notes

- Character movement uses Photon Fusion 2's KCC for smooth, reliable movement
- Two character model types:
  - KayKit models: Simpler chibi-style with single mecanim controller
  - Synty models: More detailed with advanced animation system
- Custom camera implementation for proper third-person perspective
- Network synchronization via Photon Fusion 2 in Shared Mode

---

*This project was created as part of a technical assessment for a Unity Developer position.*
