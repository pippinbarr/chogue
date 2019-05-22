using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelNameGenerator
{

    static string[] firsts = new string[] {
    "Kasparov's",
    "The Sicilian",
    "The King's",
    "The Pawn's",
    "The Queen's",
    "The Knight's",
    "The Rook's",
    "The Bishop's",
    "The Algebraic",
    "The Blundering",
    "The Captured",
    "The Discovered",
    "The Forked",
    "The Skewered",
    "The Queened",
    "The Fianchettoed",
    "The Fool's",
    "The Master",
    "The Pinned",
    "The Promoted",
    "The Poisoned",
    "The Resigned",
    "The Scholar's",
    "The Stale",
    "The Zugzwanged"
        };

    static string[] seconds = new string[]
    {
    "Dungeon",
    "Scroll",
    "Wand",
    "Aquator",
    "Bat",
    "Centaur",
    "Dragon",
    "Emu",
    "Venus",
    "Griffin",
    "Hobgoblin",
    "Ice",
    "Jabberwock",
    "Kestrel",
    "Leprechaun",
    "Medusa",
    "Nymph",
    "Orc",
    "Phantom",
    "Quagga",
    "Rattlesnake",
    "Snake",
    "Troll",
    "Ur",
    "Vampire",
    "Wraith",
    "Xeroc",
    "Yeti",
    "Zombie",
    "Mace",
    "Long Sword",
    "Short Bow",
    "Arrow",
    "Dagger",
    "Two-Handed Sword",
    "Dart",
    "Shuriken",
    "Spear",
    "Leather Armor",
    "Ring Mail",
    "Studded Leather",
    "Scale Mail",
    "Chain Mail",
    "Splint Mail",
    "Banded Mail",
    "Plate Mail",
    "Scroll",
    "Potion",
    "Ring"
    };

// Start is called before the first frame update
public static string GetName()
    {
        string first = firsts[Random.Range(0,firsts.Length)];
        string second = seconds[Random.Range(0, seconds.Length)];
        return first + " " + second;
    }

}
