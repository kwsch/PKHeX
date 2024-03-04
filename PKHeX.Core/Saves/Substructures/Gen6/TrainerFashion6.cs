using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public abstract class TrainerFashion6(ReadOnlySpan<byte> span)
{
    public const int SIZE = 16;

    protected uint data0 = ReadUInt32LittleEndian(span);
    protected uint data1 = ReadUInt32LittleEndian(span[04..]);
    protected uint data2 = ReadUInt32LittleEndian(span[08..]);
    protected uint data3 = ReadUInt32LittleEndian(span[12..]);

    public static TrainerFashion6 GetFashion(ReadOnlySpan<byte> data, byte gender)
    {
        if (gender == 0) // m
            return new Fashion6Male(data);
        return new Fashion6Female(data);
    }

    public void Write(Span<byte> data)
    {
        WriteUInt32LittleEndian(data, data0);
        WriteUInt32LittleEndian(data[04..], data1);
        WriteUInt32LittleEndian(data[08..], data2);
        WriteUInt32LittleEndian(data[12..], data3);
    }

    protected static uint GetBits(uint value, int startPos, int bits)
    {
        uint mask = ((1u << bits) - 1) << startPos;
        return (value & mask) >> startPos;
    }

    protected static uint SetBits(uint value, int startPos, int bits, uint bitValue)
    {
        uint mask = ((1u << bits) - 1) << startPos;
        bitValue &= mask >> startPos;
        return (value & ~mask) | (bitValue << startPos);
    }

    public enum F6HairColor
    {
        Black,
        Brown,
        Honey,
        Orange,
        Blond,
    }

    public enum F6ContactLens
    {
        Brown,
        Hazel,
        None,
        Green,
        Blue,
    }

    public enum F6Skin
    {
        White,
        Light,
        Tan,
        Dark,
    }
}

public sealed class Fashion6Male(ReadOnlySpan<byte> data) : TrainerFashion6(data)
{
    public uint Version  { get => GetBits(data0,  0, 3); set => data0 = SetBits(data0,  0, 3, value); }
    public uint Model    { get => GetBits(data0,  3, 3); set => data0 = SetBits(data0,  3, 3, value); }
    public F6Skin Skin   { get => (F6Skin)GetBits(data0,  6, 2); set => data0 = SetBits(data0,  6, 2, (uint)value); }
    public F6HairColor HairColor { get => (F6HairColor)GetBits(data0,  8, 3); set => data0 = SetBits(data0,  8, 3, (uint)value); }
    public F6Hat Hat     { get => (F6Hat)GetBits(data0, 11, 5); set => data0 = SetBits(data0, 11, 5, (uint)value); }
    public F6HairStyleFront Front    { get => (F6HairStyleFront)GetBits(data0, 16, 3); set => data0 = SetBits(data0, 16, 3, (uint)value); }
    public F6HairStyle Hair { get => (F6HairStyle)GetBits(data0, 19, 4); set => data0 = SetBits(data0, 19, 4, (uint)value); }
    public uint Face     { get => GetBits(data0, 23, 3); set => data0 = SetBits(data0, 23, 3, value); }
    public uint Arms     { get => GetBits(data0, 26, 2); set => data0 = SetBits(data0, 26, 2, value); }
    public uint Unknown0 { get => GetBits(data0, 28, 2); set => data0 = SetBits(data0, 28, 2, value); }
    public uint Unused0  { get => GetBits(data0, 30, 2); set => data0 = SetBits(data0, 30, 2, value); }

    public F6Top Top      { get => (F6Top)GetBits(data1,  0, 6); set => data1 = SetBits(data1,  0, 6, (uint)value); }
    public F6Bottoms Legs { get => (F6Bottoms)GetBits(data1,  6, 5); set => data1 = SetBits(data1,  6, 5, (uint)value); }
    public F6Socks Socks  { get => (F6Socks)GetBits(data1, 11, 3); set => data1 = SetBits(data1, 11, 3, (uint)value); }
    public F6Shoes Shoes  { get => (F6Shoes)GetBits(data1, 14, 5); set => data1 = SetBits(data1, 14, 5, (uint)value); }
    public F6Bag Bag      { get => (F6Bag)GetBits(data1, 19, 4); set => data1 = SetBits(data1, 19, 4, (uint)value); }
    public F6Accessory AHat { get => (F6Accessory)GetBits(data1, 23, 4); set => data1 = SetBits(data1, 23, 4, (uint)value); }
    public uint Unknown1  { get => GetBits(data1, 27, 2); set => data1 = SetBits(data1, 27, 2, value); }
    public uint Unused1   { get => GetBits(data1, 29, 3); set => data1 = SetBits(data1, 29, 3, value); }

    public bool Contacts      { get => GetBits(data2,  0, 1) == 1; set => data2 = SetBits(data2,  0, 1, value ? 1u : 0); }
    public uint FacialHair    { get => GetBits(data2,  1, 3);      set => data2 = SetBits(data2,  1, 3, value); }
    public F6ContactLens ColorContacts { get => (F6ContactLens)GetBits(data2,  4, 3);  set => data2 = SetBits(data2,  4, 3, (uint)value); }
    public uint FacialColor   { get => GetBits(data2,  7, 3);      set => data2 = SetBits(data2,  7, 3, value); }
    public uint PaintLeft     { get => GetBits(data2, 10, 4);      set => data2 = SetBits(data2, 10, 4, value); }
    public uint PaintRight    { get => GetBits(data2, 14, 4);      set => data2 = SetBits(data2, 14, 4, value); }
    public uint PaintLeftC    { get => GetBits(data2, 18, 3);      set => data2 = SetBits(data2, 18, 3, value); }
    public uint PaintRightC   { get => GetBits(data2, 21, 3);      set => data2 = SetBits(data2, 21, 3, value); }
    public bool Freckles      { get => GetBits(data2, 24, 2) == 1; set => data2 = SetBits(data2, 24, 2, value ? 1u : 0); }
    public uint ColorFreckles { get => GetBits(data2, 26, 3);      set => data2 = SetBits(data2, 26, 3, value); }
    public uint Unused2       { get => GetBits(data2, 29, 3);      set => data2 = SetBits(data2, 29, 3, value); }

    public enum F6Top
    {
        _0,
        Zipped_Jacket_Blue,
        Zipped_Jacket_Red,
        Zipped_Jacket_Green,
        Zipped_Jacket_Black,
        Zipped_Jacket_Navy_Blue,
        Zipped_Jacket_Orange,
        Down_Jacket_Black,
        Down_Jacket_Red,
        Down_Jacket_Aqua,
        Pajama_Top,
        Striped_Shirt_Combo_Purple,
        Striped_Shirt_Combo_Red,
        Striped_Shirt_Combo_Aqua,
        Striped_Shirt_Combo_Pale_Pink,
        Plaid_Shirt_Combo_Red,
        Plaid_Shirt_Combo_Gray,
        Zipped_Shirt_Combo_Black,
        Zipped_Shirt_Combo_White,
        Hoodie_Olive,
        Hoodie_Aqua,
        Hoodie_Yellow,
        V_Neck_T_Shirt_Black,
        V_Neck_T_Shirt_White,
        V_Neck_T_Shirt_Pink,
        V_Neck_T_Shirt_Aqua,
        Logo_T_Shirt_White,
        Logo_T_Shirt_Orange,
        Logo_T_Shirt_Green,
        Logo_T_Shirt_Blue,
        Logo_T_Shirt_Yellow,
        Splatter_Paint_T_Shirt_Black,
        Splatter_Paint_T_Shirt_Red,
        Splatter_Paint_T_Shirt_Purple,
        King_T_Shirt,
        Twin_T_Shirt,
    }

    public enum F6Socks
    {
        None,
        Ankle_Socks_Black,
        Ankle_Socks_Red,
        Ankle_Socks_Green,
        Ankle_Socks_Purple,
    }

    public enum F6Shoes
    {
        _0,
        Barefoot,
        _2,
        Short_Boots_Black,
        Short_Boots_Red,
        Short_Boots_Brown,
        _6,
        Loafers_Brown,
        Loafers_Black,
        Sneakers_Black,
        Sneakers_White,
        Sneakers_Red,
        Sneakers_Blue,
        Sneakers_Yellow,
    }

    public enum F6Hat
    {
        None,
        Logo_Cap_Glitched, // hacked
        Logo_Cap_Black,
        Logo_Cap_Blue,
        Logo_Cap_Red,
        Logo_Cap_Green,
        Fedora_Glitched, // hacked
        Fedora_Red,
        Fedora_Gray,
        Fedora_Black,
        Checkered_Fedora_Black,
        Outdoors_Cap_Glitched, // hacked
        Outdoors_Cap_Red,
        Outdoors_Cap_Black,
        Outdoors_Cap_Olive,
        Outdoors_Cap_Beige,
        Knit_Cap_Glitched, // hacked
        Knit_Cap_White,
        Knit_Cap_Black,
        Knit_Cap_Orange,
        Knit_Cap_Purple,
        Camo_Cap_Olive,
        Camo_Cap_Aqua,
        Bamboo_Sprig_Hat,
    }

    public enum F6HairStyle
    {
        _0,
        Medium,
        Medium_Perm,
        Short,
        Very_Short,
    }

    public enum F6Bottoms
    {
        _0,
        Chinos_Black,
        Chinos_Beige,
        Checked_Pants_Red,
        Checked_Pants_Gray,
        Cuffed_Jeans,
        Damaged_Jeans,
        Vinyl_Pants,
        Pajama_Pants,
        Short_Cargo_Pants_Black,
        Short_Cargo_Pants_Olive,
        Short_Cargo_Pants_Purple,
        Skinny_Jeans_Blue,
        Skinny_Jeans_Brown,
        Skinny_Jeans_Beige,
        Skinny_Jeans_Red,
        Camo_Pants_Green,
        Camo_Pants_Gray,
    }

    public enum F6Bag
    {
        None,
        Two_Tone_Bag_Black,
        Two_Tone_Bag_Red,
        Two_Tone_Bag_Olive,
        Two_Tone_Bag_Aqua,
        Two_Tone_Bag_Orange,
        Vinyl_Messenger_Bag_Black,
        Vinyl_Messenger_Bag_Brown,
    }

    public enum F6Accessory
    {
        None,
        Button_Accessory_Gray,
        Button_Accessory_Pink,
        Button_Accessory_Purple,
        Button_Accessory_Yellow,
        Button_Accessory_Lime_Green,
        Wide_Frame_Sunglasses_Black,
        Wide_Frame_Sunglasses_Yellow,
        Wide_Frame_Sunglasses_Red,
        Wide_Frame_Sunglasses_White,
        Feather_Accessory_Black,
        Feather_Accessory_Red,
        Feather_Accessory_Green,
    }

    public enum F6HairStyleFront
    {
        _0,
        Default,
    }
}

public sealed class Fashion6Female(ReadOnlySpan<byte> data) : TrainerFashion6(data)
{
    public uint Version  { get => GetBits(data0,  0, 3); set => data0 = SetBits(data0,  0, 3, value); }
    public uint Model    { get => GetBits(data0,  3, 3); set => data0 = SetBits(data0,  3, 3, value); }
    public F6Skin Skin   { get => (F6Skin)GetBits(data0,  6, 2); set => data0 = SetBits(data0,  6, 2, (uint)value); }
    public F6HairColor HairColor{ get => (F6HairColor)GetBits(data0,  8, 3); set => data0 = SetBits(data0,  8, 3, (uint)value); }
    public F6Hat Hat     { get => (F6Hat)GetBits(data0, 11, 6); set => data0 = SetBits(data0, 11, 6, (uint)value); }
    public F6HairStyleFront Front { get => (F6HairStyleFront)GetBits(data0, 17, 3); set => data0 = SetBits(data0, 17, 3, (uint)value); }
    public F6HairStyle Hair { get => (F6HairStyle)GetBits(data0, 20, 4); set => data0 = SetBits(data0, 20, 4, (uint)value); }
    public uint Face     { get => GetBits(data0, 24, 3); set => data0 = SetBits(data0, 24, 3, value); }
    public uint Arms     { get => GetBits(data0, 27, 2); set => data0 = SetBits(data0, 27, 2, value); }
    public uint Unknown0 { get => GetBits(data0, 29, 2); set => data0 = SetBits(data0, 29, 2, value); }
    public uint Unused0  { get => GetBits(data0, 31, 1); set => data0 = SetBits(data0, 31, 1, value); }

    public F6Top Top     { get => (F6Top)GetBits(data1,  0, 6); set => data1 = SetBits(data1,  0, 6, (uint)value); }
    public F6Bottom Legs { get => (F6Bottom)GetBits(data1,  6, 7); set => data1 = SetBits(data1,  6, 7, (uint)value); }
    public F6Dress Dress { get => (F6Dress)GetBits(data1, 13, 4); set => data1 = SetBits(data1, 13, 4, (uint)value); }
    public F6Socks Socks { get => (F6Socks)GetBits(data1, 17, 5); set => data1 = SetBits(data1, 17, 5, (uint)value); }
    public F6Shoes Shoes { get => (F6Shoes)GetBits(data1, 22, 6); set => data1 = SetBits(data1, 22, 6, (uint)value); }
    public uint Unknown1 { get => GetBits(data1, 28, 2); set => data1 = SetBits(data1, 28, 2, value); }
    public uint Unused1  { get => GetBits(data1, 30, 2); set => data1 = SetBits(data1, 30, 2, value); }

    public F6Bag Bag          { get => (F6Bag)GetBits(data2,  0, 5); set => data2 = SetBits(data2,  0, 5, (uint)value); }
    public F6Accessory AHat   { get => (F6Accessory)GetBits(data2,  5, 5); set => data2 = SetBits(data2,  5, 5, (uint)value); }
    public bool Contacts      { get => GetBits(data2, 10, 1) == 1; set => data2 = SetBits(data2, 10, 1, value ? 1u : 0); }
    public uint MascaraType   { get => GetBits(data2, 11, 2); set => data2 = SetBits(data2, 11, 2, value); }
    public bool Eyeliner      { get => GetBits(data2, 13, 2) == 1; set => data2 = SetBits(data2, 13, 2, value ? 1u : 0); }
    public bool Cheek         { get => GetBits(data2, 15, 2) == 1; set => data2 = SetBits(data2, 15, 2, value ? 1u : 0); }
    public bool Lips          { get => GetBits(data2, 17, 2) == 1; set => data2 = SetBits(data2, 17, 2, value ? 1u : 0); }
    public F6ContactLens ColorContacts { get => (F6ContactLens)GetBits(data2, 19, 3); set => data2 = SetBits(data2, 19, 3, (uint)value); }
    public bool Mascara       { get => GetBits(data2, 22, 3) == 1; set => data2 = SetBits(data2, 22, 3, value ? 1u : 0); }
    public uint ColorEyeliner { get => GetBits(data2, 25, 3);      set => data2 = SetBits(data2, 25, 3, value); }
    public uint ColorCheek    { get => GetBits(data2, 28, 3);      set => data2 = SetBits(data2, 28, 3, value); }
    public uint Unused2       { get => GetBits(data2, 31, 1);      set => data2 = SetBits(data2, 31, 1, value); }

    public uint ColorLips     { get => GetBits(data3, 0, 2); set => data3 = SetBits(data3, 0, 2, value); }
    public uint ColorFreckles { get => GetBits(data3, 2, 3); set => data3 = SetBits(data3, 2, 3, value); }
    public bool Freckles      { get => GetBits(data3, 5, 3) == 1; set => data3 = SetBits(data3, 5, 3, value ? 1u : 0); }
    public uint Unused3       { get => GetBits(data3, 8, 24); set => data3 = SetBits(data3, 8, 24, value); }

    public enum F6Top
    {
        None,
        Ruffled_Camisole_Pale_Pink,
        Ruffled_Camisole_Aqua,
        Ruffled_Camisole_Yellow,
        Ruffled_Tank_Top_Black,
        Striped_Tank_Top_Black,
        Striped_Tank_Top_Blue,
        Striped_Tank_Top_Pink,
        Midriff_Halter_Top_Aqua,
        Midriff_Halter_Top_Orange,
        Sleeveless_Turtleneck_Black,
        Sleeveless_Turtleneck_White,
        Pajama_Top,
        Short_Parka_Red,
        Short_Parka_Pink,
        Short_Parka_Lime_Green,
        Short_Parka_Blue,
        Tie_Neck_Blouse_Red,
        Tie_Neck_Blouse_Gray,
        Ribbon_Smock_Top_Brown,
        Ribbon_Smock_Top_Pale_Pink,
        Ribbon_Smock_Top_Yellow,
        Ribbon_Smock_Top_Purple,
        Exotic_Top_Lime_Green,
        Exotic_Top_Orange,
        Scarf_Top_Yellow,
        Scarf_Top_Pale_Pink,
        Scarf_Top_Purple,
        Glitzy_Scarf_Top,
        Shirt_and_Tie_Gray,
        Shirt_and_Tie_Green,
        Shirt_and_Tie_Blue,
        Poke_Ball_Baby_Doll_Tee_Aqua,
        Poke_Ball_Baby_Doll_Tee_Purple,
        Poke_Ball_Baby_Doll_Tee_Yellow,
        Poke_Ball_Baby_Doll_Tee_Green,
    }

    public enum F6Socks
    {
        None,
        Knee_Socks_Black,
        Knee_Socks_White,
        Knee_Socks_Red,
        Knee_Socks_Blue,
        Knee_Socks_Green,
        Knee_Socks_Pink,
        Knee_Socks_Yellow,
        No_Socks,
        OTK_Socks_Black,
        OTK_Socks_White,
        OTK_Socks_Green,
        OTK_Socks_Gray,
        OTK_Socks_Pink,
        OTK_Socks_Red,
        OTK_Socks_Brown,
        Single_Stripe_OTK_Socks,
        Wide_Stripe_OTK_Socks_Black,
        Wide_Stripe_OTK_Socks_Pale_Pink,
        Punk_OTK_Socks,
        Camo_OTK_Socks,
        Leggings,
        Tights_Black,
        Tights_Orange,
        Tights_Pale_Pink,
        Tights_Navy_Blue,
        Tights_Pink,
        Tights_Purple,
    }

    public enum F6Shoes
    {
        None,
        Riding_Boots_Pink,
        Riding_Boots_Black,
        Riding_Boots_White,
        Riding_Boots_Gray,
        Riding_Boots_Brown,
        Riding_Boots_Beige,
        Laced_Boots_Brown,
        Laced_Boots_Black,
        Zipped_Boots,
        Barefoot,
        _11,
        High_Tops_Black,
        High_Tops_Pink,
        High_Tops_Yellow,
        High_Tops_Purple,
        Bow_Shoes_Black,
        Bow_Shoes_Brown,
        Saddle_Shoes_White,
        Saddle_Shoes_Brown,
        Saddle_Shoes_Navy_Blue,
        Mary_Janes_Black,
        Mary_Janes_Red,
        Mary_Janes_Purple,
        Mary_Janes_White,
        Mary_Janes_Pale_Pink,
        Mary_Janes_Aqua,
    }

    public enum F6Hat
    {
        None,
        Felt_Hat_Glitched, // hacked
        Felt_Hat_Black,
        Felt_Hat_White,
        Felt_Hat_Gray,
        Felt_Hat_Navy_Blue,
        Felt_Hat_Brown,
        Felt_Hat_Pink,
        Felt_Hat_Pale_Pink,
        Felt_Hat_Beige,
        Felt_Hat_Aqua,
        Boater_Red,
        Boater_Blue,
        Sports_Cap_Glitched, // hacked
        Sports_Cap_Aqua,
        Sports_Cap_Yellow,
        Sports_Cap_Green,
        Logo_Cap_Pink,
        Logo_Cap_Black,
        Cycling_Cap_Glitched, // hacked
        Cycling_Cap_Blue,
        Cycling_Cap_White,
        Cycling_Cap_Beige,
        Exotic_Cap_Brown,
        Exotic_Cap_Purple,
        Fedora_Glitched, // hacked
        Fedora_White,
        Fedora_Brown,
        Fedora_Red,
        Fedora_Yellow,
        Fedora_Green,
        Fedora_Purple,
    }

    public enum F6HairStyle
    {
        None,
        Bob,
        Long,
        Medium,
        Ponytail,
        Short,
        Pigtails,
    }

    public enum F6HairStyleFront
    {
        None,
        Bangs,
        Sideswept,
    }

    public enum F6Dress
    {
        None,
        Single_Front_Coat_Dress,
        Double_Front_Coat_Dress,
        Trench_Coat_Beige,
        Trench_Coat_Black,
        Sundae_Dress,
        Frilly_Dress,
        Sparkly_Bolero_Dress,
        Little_Black_Dress,
        High_Waisted_Outfit_Black,
        High_Waisted_Outfit_White,
    }

    public enum F6Bottom
    {
        None,
        Denim_Miniskirt_Blue,
        Denim_Miniskirt_Olive,
        Denim_Miniskirt_Black,
        Scalloped_Skirt_Orange,
        Scalloped_Skirt_Red,
        Skinny_Jeans_Aqua,
        Skinny_Jeans_White,
        Skinny_Jeans_Olive,
        Skinny_Jeans_Blue,
        Skinny_Jeans_Beige,
        Skinny_Jeans_Black,
        Skinny_Jeans_Red,
        Damaged_Skinny_Jeans,
        Accented_Jeans_Blue,
        Accented_Jeans_Yellow,
        Accented_Jeans_Red,
        Accented_Jeans_Lime_Green,
        Bold_Striped_Pants_Gray,
        Bold_Striped_Pants_Green,
        Bold_Striped_Pants_Blue,
        Pajama_Pants,
        Pleated_Skirt_Black,
        Pleated_Skirt_White,
        Pleated_Skirt_Red,
        Pleated_Skirt_Blue,
        Striped_Pleated_Skirt_Pink,
        Striped_Pleated_Skirt_Aqua,
        Striped_Pleated_Skirt_Yellow,
        Pleated_Kilt_Skirt_Gray,
        Pleated_Kilt_Skirt_Red,
        Scalloped_Tiered_Skirt_Pink,
        Scalloped_Tiered_Skirt_Yellow,
        Scalloped_Tiered_Skirt_White,
        Tiered_Satin_Skirt_Purple,
        Tiered_Satin_Skirt_White,
        Jean_Shorts_Gray,
        Jean_Shorts_Aqua,
        Jean_Shorts_White,
        Jean_Shorts_Brown,
        Jean_Shorts_Black,
        Jean_Shorts_Pink,
        Jean_Shorts_Orange,
        Damaged_Jean_Shorts,
        Cross_Laced_Shorts_Olive,
        Cross_Laced_Shorts_Brown,
    }

    public enum F6Bag
    {
        None,
        Tote_Bag_Pink,
        Tote_Bag_Red,
        Tote_Bag_Orange,
        Tote_Bag_Yellow,
        Tote_Bag_White,
        Enamel_Striped_Purse_Red,
        Enamel_Striped_Purse_Blue,
        Ribbon_Purse_Pale_Pink,
        Ribbon_Purse_Aqua,
        Strappy_Purse_Black,
        Strappy_Purse_Brown,
        Strappy_Purse_Beige,
        Strappy_Purse_Purple,
        Strappy_Purse_White,
        Tassel_Purse_Purple,
        Tassel_Purse_Green,
    }

    public enum F6Accessory
    {
        None,
        Button_Accessory_Gray,
        Button_Accessory_Pink,
        Button_Accessory_Purple,
        Button_Accessory_Yellow,
        Button_Accessory_Lime_Green,
        Artificial_Flower_Pin_Pink,
        Artificial_Flower_Pin_Pale_Pink,
        Artificial_Flower_Pin_Yellow,
        Artificial_Flower_Pin_Aqua,
        Wide_Frame_Sunglasses_White,
        Wide_Frame_Sunglasses_Red,
        Wide_Frame_Sunglasses_Blue,
        Wide_Frame_Sunglasses_Yellow,
        Metal_Pin_Gold,
        Metal_Pin_Silver,
        Metal_Pin_Black,
        Hat_Ribbon_Accessory_Black,
        Hat_Ribbon_Accessory_Red,
        Hat_Ribbon_Accessory_White,
        Hat_Ribbon_Accessory_Blue,
        Hat_Ribbon_Accessory_Pale_Pink,
        Feather_Accessory_Black,
        Feather_Accessory_Red,
        Feather_Accessory_Green,
    }
}
