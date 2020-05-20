# Custom Mediumcore for Terraria (TShock plugin)

Is Classic difficulty too easy, but Mediumcore a smidgen too hard? Then this plugin is for you!

This simple plugin allows customizing which items are dropped upon death. E.g. Drop resources one would
have gathered, but keep weapons and armour. If you are familiar with Starbound, then this should sound familiar.

## Installation

Find precompiled binaries [here](https://files.catbox.moe/1vii7h.zip).

Place `CustomMediumcore.dll` under `ServerPlugins` in TShock directory, and
`drop_item_ids.txt` next to `TerrariaServer.exe`. 

## Usage

After above steps, you are ready to go! `drop_item_ids.txt` specifies the ItemIDs that
should be dropped on death, one per line. Modify this file to include/exclude items (after server reboot).

The default `drop_item_ids.txt` contains most resources, like ores and life crystals, in the spirit of
how Starbound works.

## Requirements

* TShock server running this script. Tested on v4.4_pre3.
* A server with **server-side characters (SSC)** (this part is important).
* Players have to have "Classic" difficulty. Otherwise duplicating may happen.

## Limitations

* You can only drop items from main inventory (main inventory + coins + ammo).

## Changelog

* 19.05.2020: v1.4.0.0 Initial public release
