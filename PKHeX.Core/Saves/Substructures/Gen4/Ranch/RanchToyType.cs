namespace PKHeX.Core;

/// <summary>
/// Toys used in My Pokémon Ranch save files.
/// </summary>
public enum RanchToyType : byte
{
    None = 0,
    Poke_Balloons = 1,
    Slippery_Peel = 2,
    Poke_Bell = 3,
    BounceBack_Ball = 4,
    Poke_Rocket = 5,
    Poke_Cushion = 6,
    Parade_Drum = 7,
    Bonfire = 8,
    Leader_Flag = 9,
    Fountain = 10,
    Ice_Block = 11,
    Poke_Microphone = 12,
    Burst_Ball = 13,
    Poke_Palette = 14,
    Poke_Pendulum = 15,
    Pitfall = 16,
    Training_Bag = 17,
    Stinky_Ball = 18,
    Snowman = 19,
    Round_Rock = 20,
    Spin_Ride = 21,
    Sprint_Stand = 22,
    Attractor = 23,
    Challenger = 24,
    Toy_Box = 25, // Supports metadata - Metadata value is the index of the contained toy.
    Poke_Ball = 26, // Normally unused

    // Pt Update:
    Birthday_Cake = 27,
    Apple_Box_S = 28, // Small Apple Box
    Apple_Box_L = 29, // Large Apple Box
    Dice = 30,
    Picture_Frame_S = 31, // Small Picture Frame
    Picture_Frame_L = 32, // Large Picture Frame
    Flower = 33,
    Lamp = 34,
    Magnet = 35,
    Twirler = 36,
    Bound_Mat = 37,
    Tree = 38,
    Water = 39, // Normally unobtainable; creates a massive plane of water in the sky featured in the "Surfing Jump" event
}
