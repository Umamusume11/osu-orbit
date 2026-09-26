# osu!orbit 🪐

A custom ruleset for **osu!lazer** (compatible with version `2026.921.0`) inspired by the mechanics of *A Dance of Fire and Ice*. 

"I know it from A Dance of Fire and Ice."

## ✨ Features
- **Dual Planet Mechanics**: Control two orbiting planets (Fire & Water) that switch pivots on every beat.
- **Modern Architecture**: Fully updated and built using **.NET 10.0** and the latest `ppy.osu.Game` framework.
- **Standard Beatmap Conversion**: Play your favorite standard osu! maps converted into orbital paths.

## 🛠️ How to Installation

### Prerequisites
- **osu!lazer** version `2026.921.0` or newer.
- **[.NET 10.0 SDK](https://microsoft.com)** installed on your machine (required for building).

### Building from Source
Because this ruleset targets the latest 2026 framework releases, it is highly recommended to compile it locally to prevent version mismatch crashes:

1. Clone this repository:
   ```bash
   git clone https://github.com
   ```
2. Navigate to the project directory:
   ```bash
   cd osu-orbit
   ```
3. Build the project using the Release configuration:
   ```bash
   dotnet build -c Release
   ```
4. Copy the compiled `.dll` file found in `bin/Release/net10.0/` and paste it into your osu!lazer `rulesets` folder.
5. Restart osu!lazer and enjoy!

## 🎮 Default Controls
- **Button 1**: `Z`
- **Button 2**: `X`
*(You can customize these bindings inside the osu!lazer input settings menu)*

## 📜 Credits & License
- Inspired by *A Dance of Fire and Ice* by 7th Beat Games.
- Built using the official [osu!lazer framework](https://github.com).
- Distributed under the **MIT License**. Feel free to contribute!
