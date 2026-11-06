# 2D Top-Down Dungeon Crawler 🗡️

![Game Engine](https://img.shields.io/badge/Engine-Unity-blue) 
![Status](https://img.shields.io/badge/Status-In%20Development-orange)

## 📖 About the Project
This project is a 2D top-down action roguelike / dungeon crawler, heavily inspired by classic arcade mechanics. It features procedurally generated levels, dynamic enemy AI, and a modular upgrade system. The core focus of this project is building scalable game architecture, smooth 2D physics-based combat, and replayable gameplay loops.

## ✨ Key Features

### 🗺️ Procedural Dungeon Generation
- **Dynamic Layouts:** The maze of rooms is dynamically generated using a grid-based algorithm, starting with 1x1 rooms.
- **Structured Levels:** Each floor follows a strict generation rule, ensuring a balanced experience: standard combat rooms, a locked **Treasure Room** (requires a key), a **Shop**, and a final **Boss Room**.
- **Reward System:** Clearing rooms rewards the player with resource chests or raw drops (bombs, coins, etc.).

### 🤺 Combat & Controls
- **Hero System:** Built on a scalable base class to easily allow the addition of new characters in the future.
- **Mechanics:** 4-directional shooting combined with fluid 2D physics and precise collision detection. 

### 🎒 Modular Item & Upgrade System
- Flexible architecture for in-game items that alter core gameplay behavior.
- Upgrades dynamically modify player stats, including damage multipliers, movement speed, projectile size/speed, and maximum health (heart containers).

### 💰 Resource Management & Economy
- Players collect and manage three core resources: **Coins**, **Bombs**, and **Keys**.
- These resources are utilized to unlock doors, destroy environmental obstacles, and purchase upgrades.

### 👾 Enemy AI & Bosses
- Multiple enemy variants designed with specific behavioral patterns:
  - **Melee Chasers:** Aggressively track and pathfind towards the player.
  - **Ranged Attackers:** Utilize varying firing patterns (directed shots vs. spread projectiles).
- **Boss Fights:** Each level culminates in a challenging boss encounter with unique mechanics.

### 🏪 Interactive Environment
- **Shops:** In-game merchants to trade collected coins for powerful items.
- **Interactive Objects:** Various interactive map elements, such as Blood Donation Machines (sacrifice HP for currency) to promote risk/reward gameplay.

### 🖥️ UI & Minimap
- **Dynamic Minimap:** The map progressively reveals itself as the player explores new rooms.
- **HUD:** Real-time user interface displaying current health, resource counters, and active effects.

## 🛠️ Technical Details
* **Language:** C#
* **Engine/Framework:** Unity
* **Architecture:** Utilizes modular component design for items and object-oriented principles for character/enemy classes.
