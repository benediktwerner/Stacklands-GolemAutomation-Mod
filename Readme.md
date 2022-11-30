# Stacklands Golem Automation Mod

This mod adds golems which can be used to automate moving around cards, selling them, and crafting things.

To find the ideas for the cards:
- Buy `Logic and Reason` packs (on the mainland) for storage place and filter
- Buy `Island Insights` packs for all the golem stuff
- Explore the `Jungle` to find the ideas for humongous golems and get some ressources

## Some basics

### Storage Place

After using a filter on it (explained below), all produced cards in the area around
the storage place matching the filter will automatically be moved onto it.

You can also place other buildings like stoves, brickyards, or smelters on top of it.

### Filter

Basically just a list of cards. Place any cards on top of it to add them to the filter. Use a villager to clear the list.

You can then add it onto a card that accepts a filter to set the filter of that card.

### Location Glyph

Location glyphs are used to tell golems where to take cards from or where to move them to.

To bind a location glyph, simply place the building you want to bind it to on top of the glyph.

Hovering over the glyph will highlight the bound building in blue.

Area Glyphs are a variation of Location Glyphs that instead make the golem take cards from nearby Storage Piles.

### Golems

Golems come in three sizes: Normal, Large, and Humongous.

The larger the golem, the slower they act by default but the more cards they can hold and move at once.
Larger golems can also hold more golem modules which can make them much faster.

To use a golem place a location glyph on top of it. The golem will then periodically take cards from the building the glyph is bound to.

If you use a filter on the golem (while it doesn't have any glyphs) it will then only take those cards.

If you add a second location glyph onto the golem, it will then move the cards to that location. If you don't add a second glyph, it will
just dump the cards next to it (you can use storage places to catch them).

### Golem Modules

Golems Modules can be used to make golems work faster or adjust their behavior.

To insert a module into a golem, simply place it on top of it while it doesn't have any location glyphs. To remove modules again,
place a villager onto the golem.

- **Speed Module**: Makes the golem work twice as fast. Can be stacked.
- **Selling Module**: Makes the golem sell all the cards it takes and then dump or move the goal onto the target location. If the target location is a chest, any gold that doesn't fit into the chest will be voided to avoid overflow. Mutually exclusive with the Crafting Module.
- **Counter Module**: Use coins onto the module before inserting it to set the counter to that amount of coins. The golem will then only take cards from a stack if it has more than that number of cards. This can for example be used to keep a certain number of cards but sell everything above that.
- **Crafting Module**: Set a recipe by placing the corresponding cards onto the module before inserting it. The golem will then take cards to build up that exact stack and then dump it next to itself to make the crafting start. You can use a Storage Place to collect the result. Mutually exclusive with the Selling Module.

## Manual Installation
This mod requires BepInEx to work. BepInEx is a modding framework which allows multiple mods to be loaded.

1. Download and install BepInEx from the [Thunderstore](https://stacklands.thunderstore.io/package/BepInEx/BepInExPack_Stacklands/).
4. Download this mod and extract it into `BepInEx/plugins/`
5. Launch the game

## Development
1. Install BepInEx
2. This mod uses publicized game DLLs to get private members without reflection
   - Use https://github.com/CabbageCrow/AssemblyPublicizer for example to publicize `Stacklands/Stacklands_Data/Managed/GameScripts.dll` (just drag the DLL onto the publicizer exe)
   - This outputs to `Stacklands_Data\Managed\publicized_assemblies\GameScripts_publicized.dll` (if you use another publicizer, place the result there)
3. Compile the project. This copies the resulting DLL into `<GAME_PATH>/BepInEx/plugins/`.
   - Your `GAME_PATH` should automatically be detected. If it isn't, you can manually set it in the `.csproj` file.
   - If you're using VSCode, the `.vscode/tasks.json` file should make it so that you can just do `Run Build`/`Ctrl+Shift+B` to build.

## Links
- Github: https://github.com/benediktwerner/Stacklands-GolemAutomation-Mod
- Thunderstore: https://stacklands.thunderstore.io/package/benediktwerner/GolemAutomation
- Nexusmods: https://www.nexusmods.com/stacklands/mods/8

## Changelog

- v1.2.6: Make Paper and Area Glpyh ideas findable
- v1.2.5: Optimize icon size
- v1.2.4: Add card icons (Thanks a lot to @lopidav for making all the art!)
- v1.2.3: Fix card names not showing up for non-English languages
- v1.2.2: Fix gold not getting deleted when trying to insert into full chests
- v1.2.1: Fix golems not loading filter on save load
- v1.2.0:
  - Witch Forest Update Compatibility
  - Fix Counter Module cardopedia entry
- v1.1:
  - Rework unlock progression
  - Make storage places and paper available without going to the island
  - Add Area Glyph
  - Allow Crafting Golems to output to location
- v1.0: Initial release
