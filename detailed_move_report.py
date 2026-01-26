#!/usr/bin/env python3
"""
Create detailed move availability report with move names
"""
import re

# Parse move names from Move.cs
def parse_move_names(filename):
    move_dict = {}
    with open(filename, 'r') as f:
        content = f.read()
        # Find all move enum entries
        pattern = r'(\w+)\s*=\s*(\d+)'
        matches = re.findall(pattern, content)
        for name, id_str in matches:
            move_dict[int(id_str)] = name
    return move_dict

# Dummied moves bitflags from PKHeX
DUMMIED_GEN8 = bytes([
    0x1C, 0x20, 0x00, 0x0C, 0x00, 0x02, 0x02, 0x00, 0x00, 0x00,
    0x04, 0x00, 0x09, 0x00, 0xA1, 0x22, 0x19, 0x10, 0x36, 0xC0,
    0x40, 0x0A, 0x00, 0x02, 0x02, 0x00, 0x00, 0x45, 0x10, 0x20,
    0x00, 0x00, 0x00, 0x02, 0x04, 0x80, 0x66, 0x70, 0x00, 0x50,
    0x91, 0x00, 0x00, 0x04, 0x64, 0x08, 0x20, 0x67, 0x84, 0x00,
    0x00, 0x00, 0x00, 0xA4, 0x00, 0x28, 0x03, 0x01, 0x07, 0x20,
    0x22, 0x00, 0x04, 0x08, 0x10, 0x00, 0x08, 0x02, 0x08, 0x00,
    0x08, 0x02, 0x00, 0x00, 0x02, 0x01, 0x00, 0xE2, 0xFF, 0xFF,
    0xFF, 0xFF, 0x07, 0x82, 0x01, 0x40, 0x84, 0xFF, 0x00, 0x80,
    0xF8, 0xFF, 0x3F,
])

DUMMIED_BDSP = bytes([
    0x1C, 0x20, 0x00, 0x0C, 0x00, 0x02, 0x02, 0x00, 0x00, 0x00,
    0x04, 0x00, 0x09, 0x00, 0xA1, 0x22, 0x19, 0x10, 0x26, 0xC0,
    0x00, 0x0A, 0x00, 0x02, 0x02, 0x00, 0x00, 0x45, 0x10, 0x00,
    0x00, 0x00, 0x00, 0x02, 0x04, 0x80, 0x26, 0x70, 0x00, 0x50,
    0x91, 0x00, 0x00, 0x04, 0x60, 0x08, 0x20, 0x67, 0x04, 0x00,
    0x00, 0x00, 0x00, 0x24, 0x00, 0x28, 0x00, 0x01, 0x04, 0x20,
    0x22, 0x00, 0x04, 0x18, 0xD0, 0x81, 0xB8, 0xAA, 0xFF, 0xE7,
    0x8B, 0x0E, 0x45, 0x98, 0x07, 0xCB, 0xE4, 0xE3, 0xFF, 0xFF,
    0xFF, 0xFF, 0xFF, 0xA7, 0x74, 0xEB, 0xAF, 0xFF, 0xB7, 0xF7,
    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE, 0x7F, 0xFF,
    0xFF, 0xFF, 0xFF, 0x07,
])

DUMMIED_GEN9 = bytes([
    0x1C, 0x20, 0x00, 0x0C, 0x00, 0x02, 0x02, 0x00, 0x04, 0x00,
    0x04, 0x00, 0x09, 0x00, 0xA1, 0x22, 0x5D, 0x50, 0x36, 0xC8,
    0x00, 0x0E, 0x00, 0x42, 0x02, 0x00, 0x00, 0x45, 0x10, 0x22,
    0x00, 0x00, 0x04, 0x0A, 0xA4, 0x80, 0x27, 0x70, 0x00, 0x51,
    0x91, 0x00, 0x00, 0x04, 0x60, 0x08, 0xA0, 0x67, 0x04, 0x00,
    0x00, 0x00, 0x00, 0xA4, 0x00, 0x28, 0x01, 0x01, 0x04, 0x28,
    0x23, 0x00, 0x04, 0x08, 0x10, 0x00, 0x0C, 0x83, 0x07, 0x00,
    0x8A, 0x02, 0x4C, 0x10, 0x80, 0x03, 0xF0, 0xC3, 0xFF, 0xFF,
    0xFF, 0xFF, 0x07, 0x80, 0x26, 0xA0, 0x80, 0xFF, 0x11, 0xE1,
    0xFB, 0xFF, 0xFF, 0x00, 0xEE, 0xFF, 0x7F, 0x08, 0x00, 0x0D,
])

# Move type data from Gen 9 (17 = Fairy)
MOVE_TYPES_GEN9 = [
    0, 0, 1, 0, 0, 0, 0, 9, 14, 12, 0, 0, 0, 0, 0, 0, 2, 2, 0, 2,
    0, 0, 11, 0, 1, 0, 1, 1, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    3, 6, 6, 0, 16, 0, 0, 0, 0, 0, 0, 3, 9, 9, 14, 10, 10, 10, 14, 14,
    13, 10, 14, 0, 2, 2, 1, 1, 1, 1, 0, 11, 11, 11, 0, 11, 11, 3, 11, 11,
    11, 6, 15, 9, 12, 12, 12, 12, 5, 4, 4, 4, 3, 13, 13, 13, 13, 13, 0, 0,
    13, 7, 0, 0, 0, 0, 0, 0, 0, 7, 10, 0, 13, 13, 14, 13, 0, 0, 0, 2,
    0, 0, 7, 3, 3, 4, 9, 10, 10, 0, 0, 0, 0, 13, 13, 0, 1, 0, 13, 3,
    0, 6, 0, 2, 0, 10, 0, 11, 0, 13, 0, 3, 10, 0, 0, 4, 13, 5, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 1, 16, 6, 0, 7, 9, 0, 7, 0, 0, 2, 11, 1,
    7, 14, 0, 1, 0, 16, 17, 0, 3, 4, 10, 4, 12, 0, 7, 0, 14, 1, 4, 0,
    15, 5, 11, 0, 17, 5, 0, 0, 0, 12, 6, 8, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 9, 4, 1, 6, 15, 0, 0, 16, 0, 0, 8, 8, 1, 0, 11, 17, 0, 1, 15,
    10, 9, 16, 13, 0, 0, 5, 7, 13, 1, 10, 16, 0, 0, 0, 0, 0, 9, 14, 16,
    16, 9, 16, 0, 1, 0, 0, 0, 12, 16, 0, 13, 13, 0, 0, 11, 1, 13, 0, 1,
    1, 0, 16, 0, 9, 13, 13, 0, 7, 16, 0, 10, 1, 0, 6, 13, 13, 2, 0, 9,
    4, 14, 11, 0, 0, 3, 0, 9, 10, 8, 7, 0, 11, 16, 2, 9, 0, 5, 6, 8,
    11, 0, 13, 10, 6, 7, 13, 1, 4, 14, 10, 11, 2, 14, 8, 0, 0, 15, 11, 1,
    2, 4, 3, 0, 12, 11, 10, 13, 11, 15, 5, 12, 10, 8, 13, 2, 13, 13, 1, 1,
    8, 13, 10, 0, 0, 2, 2, 0, 8, 6, 1, 16, 16, 16, 16, 13, 0, 13, 0, 13,
    3, 0, 0, 0, 13, 13, 16, 0, 11, 16, 3, 13, 10, 12, 9, 1, 1, 5, 3, 16,
    16, 10, 11, 2, 6, 6, 15, 15, 5, 1, 1, 1, 11, 2, 4, 16, 0, 16, 8, 14,
    14, 7, 12, 14, 9, 7, 4, 13, 13, 8, 8, 0, 2, 13, 15, 12, 9, 11, 11, 5,
    3, 3, 8, 8, 5, 0, 5, 11, 2, 0, 6, 12, 11, 10, 6, 6, 6, 5, 0, 15,
    15, 13, 0, 9, 16, 11, 7, 7, 16, 5, 13, 13, 13, 13, 3, 8, 6, 13, 13, 5,
    1, 9, 3, 6, 8, 13, 12, 10, 9, 3, 1, 3, 16, 0, 0, 0, 0, 0, 0, 3,
    13, 1, 13, 10, 0, 13, 7, 2, 8, 1, 9, 16, 2, 0, 0, 1, 0, 9, 10, 9,
    11, 12, 6, 4, 14, 15, 0, 12, 12, 4, 15, 13, 11, 1, 10, 9, 11, 6, 11, 16,
    13, 0, 2, 0, 8, 9, 0, 0, 1, 14, 12, 9, 9, 14, 14, 16, 14, 9, 9, 12,
    1, 1, 3, 4, 6, 6, 7, 7, 0, 12, 12, 11, 11, 14, 17, 16, 16, 17, 17, 17,
    11, 17, 12, 17, 17, 17, 0, 17, 8, 0, 0, 5, 10, 13, 10, 9, 11, 17, 12, 3,
    6, 17, 12, 0, 12, 17, 0, 0, 17, 12, 0, 6, 1, 2, 4, 4, 4, 17, 10, 4,
    2, 16, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8,
    9, 9, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14, 15, 15, 16, 16, 17, 17, 12, 4,
    6, 3, 7, 16, 10, 14, 17, 4, 11, 11, 11, 0, 3, 0, 8, 16, 6, 8, 13, 6,
    9, 16, 9, 13, 8, 3, 0, 15, 11, 13, 2, 15, 15, 16, 14, 7, 16, 10, 17, 7,
    12, 0, 0, 13, 9, 17, 13, 4, 7, 5, 10, 13, 7, 8, 7, 0, 12, 17, 0, 12,
    9, 12, 13, 13, 8, 7, 17, 5, 15, 12, 10, 2, 12, 10, 12, 9, 13, 16, 11, 14,
    17, 0, 8, 0, 15, 10, 16, 0, 1, 5, 13, 15, 0, 1, 12, 10, 0, 9, 6, 12,
    0, 1, 7, 14, 3, 10, 2, 17, 15, 13, 5, 4, 16, 11, 8, 15, 1, 17, 11, 11,
    9, 8, 8, 12, 15, 11, 12, 11, 11, 17, 17, 10, 16, 16, 1, 15, 8, 13, 8, 15,
    5, 3, 17, 11, 12, 0, 6, 9, 16, 7, 3, 1, 10, 14, 2, 4, 11, 16, 10, 12,
    15, 13, 16, 1, 14, 7, 13, 3, 13, 0, 5, 17, 13, 9, 10, 11, 14, 1, 4, 3,
    13, 7, 8, 1, 7, 16, 2, 12, 4, 13, 13, 0, 6, 1, 7, 13, 15, 10, 11, 8,
    0, 14, 15, 0, 5, 10, 3, 0, 0, 16, 11, 9, 10, 0, 8, 13, 10, 16, 1, 12,
    0, 14, 0, 14, 6, 11, 10, 0, 13, 7, 9, 9, 12, 8, 16, 10, 9, 16, 3, 1,
    17, 0, 11, 11, 11, 12, 0, 15, 9, 12, 5, 8, 8, 15, 17, 9, 12, 13, 1, 3,
]

TYPE_NAMES = ["Normal", "Fighting", "Flying", "Poison", "Ground", "Rock", "Bug", "Ghost", "Steel",
              "Fire", "Water", "Grass", "Electric", "Psychic", "Ice", "Dragon", "Dark", "Fairy"]

def decode_bitflags(bitflags):
    """Decode bitflag array to list of move IDs"""
    dummied = []
    for byte_idx, byte_val in enumerate(bitflags):
        for bit_idx in range(8):
            if byte_val & (1 << bit_idx):
                move_id = byte_idx * 8 + bit_idx
                dummied.append(move_id)
    return dummied

def main():
    # Parse move names
    move_names = parse_move_names('PKHeX.Core/Game/Enums/Move.cs')

    gen8_dummied = set(decode_bitflags(DUMMIED_GEN8))
    bdsp_dummied = set(decode_bitflags(DUMMIED_BDSP))
    gen9_dummied = set(decode_bitflags(DUMMIED_GEN9))

    print("=" * 80)
    print("DETAILED POKÉMON MOVE AVAILABILITY REPORT")
    print("=" * 80)
    print()

    # Question 1: Which moves returned in Gen 9?
    returned_from_gen8 = gen8_dummied - gen9_dummied
    print("MOVES THAT RETURNED IN GEN 9 (after being missing in Gen 8):")
    print("-" * 80)
    for move_id in sorted(returned_from_gen8):
        name = move_names.get(move_id, f"Unknown_{move_id}")
        print(f"  {move_id:3d} - {name}")
    print(f"\nTotal: {len(returned_from_gen8)} moves returned")
    print()

    # Question 2: Fairy-type moves in BDSP
    print("FAIRY-TYPE MOVES IN BDSP:")
    print("-" * 80)
    fairy_moves_bdsp = []
    for move_id, move_type in enumerate(MOVE_TYPES_GEN9):
        if move_type == 17 and move_id not in bdsp_dummied and move_id <= 826:
            fairy_moves_bdsp.append(move_id)
            name = move_names.get(move_id, f"Unknown_{move_id}")
            print(f"  {move_id:3d} - {name}")
    print(f"\nTotal: {len(fairy_moves_bdsp)} Fairy-type moves in BDSP")
    print()

    # Question 3: Post-Gen4 moves in BDSP
    print("MOVES IN BDSP THAT WEREN'T IN ORIGINAL DIAMOND/PEARL:")
    print("-" * 80)
    print("(Showing notable examples - moves from Gen 5 onwards)")
    print()
    new_in_bdsp = []
    for move_id in range(468, 827):
        if move_id not in bdsp_dummied:
            new_in_bdsp.append(move_id)

    # Show first 20 and last 20 as examples
    print("First 20 post-Gen4 moves in BDSP:")
    for move_id in sorted(new_in_bdsp)[:20]:
        name = move_names.get(move_id, f"Unknown_{move_id}")
        move_type = TYPE_NAMES[MOVE_TYPES_GEN9[move_id]] if move_id < len(MOVE_TYPES_GEN9) else "Unknown"
        print(f"  {move_id:3d} - {name:25s} ({move_type})")

    print(f"\n... and {len(new_in_bdsp) - 40} more moves ...")

    print("\nLast 20 post-Gen4 moves in BDSP:")
    for move_id in sorted(new_in_bdsp)[-20:]:
        name = move_names.get(move_id, f"Unknown_{move_id}")
        move_type = TYPE_NAMES[MOVE_TYPES_GEN9[move_id]] if move_id < len(MOVE_TYPES_GEN9) else "Unknown"
        print(f"  {move_id:3d} - {name:25s} ({move_type})")

    print(f"\nTotal: {len(new_in_bdsp)} post-Gen4 moves available in BDSP")
    print()

    # Notable mentions
    print("NOTABLE MOVES:")
    print("-" * 80)
    notable = {
        581: "Freeze-Dry (mentioned by user)",
    }
    for move_id, description in notable.items():
        in_bdsp = "YES" if move_id not in bdsp_dummied else "NO"
        in_gen8 = "YES" if move_id not in gen8_dummied else "NO"
        in_gen9 = "YES" if move_id not in gen9_dummied else "NO"
        print(f"  {description}")
        print(f"    Gen 8 (SWSH): {in_gen8} | BDSP: {in_bdsp} | Gen 9 (SV): {in_gen9}")
    print()

    # Summary
    print("=" * 80)
    print("SUMMARY")
    print("=" * 80)
    print(f"• Gen 8 (Sword/Shield) has {161} unavailable moves")
    print(f"• BDSP has {314} unavailable moves")
    print(f"• Gen 9 (Scarlet/Violet) has {222} unavailable moves")
    print()
    print(f"• {len(returned_from_gen8)} moves returned in Gen 9 that were missing in Gen 8")
    print(f"• BDSP has {len(fairy_moves_bdsp)} Fairy-type moves available")
    print(f"• BDSP added {len(new_in_bdsp)} post-Gen4 moves (from gens 5-8)")

if __name__ == "__main__":
    main()
