using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public static partial class Legal
{
    internal const int MaxSpeciesID_8a = (int)Species.Enamorus;
    internal const int MaxMoveID_8a = (int)Move.TakeHeart;
    internal const int MaxItemID_8a = 1828; // Legend Plate
    internal const int MaxBallID_8a = (int)Ball.LAOrigin;
    internal const int MaxGameID_8a = (int)GameVersion.SP;
    internal const int MaxAbilityID_8a = MaxAbilityID_8_R2;

    internal static readonly ushort[] HeldItems_LA = Array.Empty<ushort>();

    internal static readonly ushort[] Pouch_Items_LA =
    {
        017, 023, 024, 025, 026, 027, 028, 029, 039, 041,
        050, 054, 072, 073, 075, 080, 081, 082, 083, 084,
        085, 090, 091, 092, 107, 108, 109, 110, 149, 150,
        151, 152, 153, 154, 155, 157, 158, 159, 160, 161,
        162, 163, 164, 166, 168, 233, 252, 321, 322, 323,
        324, 325, 326, 327, 583,      849,

        1125, 1126, 1127, 1128, 1231, 1232, 1233, 1234, 1235, 1236,
        1237, 1238, 1239, 1240, 1241, 1242, 1243, 1244, 1245, 1246,
        1247, 1248, 1249, 1250, 1251,

        1611, 1613, 1614, 1615, 1616, 1617, 1618, 1619, 1620, 1621,
        1628, 1630, 1631, 1632, 1633, 1634, 1635, 1636, 1637, 1638,
        1651, 1679, 1681, 1682, 1684, 1686, 1687, 1688, 1689, 1690,
        1691, 1692, 1693, 1694, 1695, 1696, 1699, 1700, 1701, 1702,
        1703, 1704, 1705, 1706, 1707, 1708, 1709, 1710, 1711, 1712,
        1713, 1716, 1717, 1720, 1724, 1725, 1726, 1727, 1728, 1732,
        1733, 1734, 1735, 1736, 1738, 1739, 1740, 1741, 1742, 1746,
        1747, 1748, 1749, 1750, 1754, 1755, 1756, 1757, 1758, 1759,
        1760, 1761, 1762, 1764, 1785,
    };

    internal static readonly ushort[] Pouch_Recipe_LA =
    {
        1640, 1641, 1642, 1643, 1644,       1646, 1647, 1648, 1649,
        1650,       1652, 1653, 1654, 1655, 1656, 1657, 1658, 1659,
        1660, 1661, 1662, 1663, 1664, 1665, 1666, 1667, 1668, 1669,
        1670, 1671,       1673, 1674, 1675, 1676, 1677,

        1729,
        1730, 1731,

        1751, 1752, 1753,

        1783, 1784,
    };

    internal static readonly ushort[] Pouch_Key_LA =
    {
        111,
        298, 299,
        300, 301, 302, 303, 304, 305, 306, 307, 308, 309,
        310, 311, 312, 313,
        441, 455, 466,
        632, 638, 644,
        1608, 1609, 1610, 1612, 1622, 1624, 1625, 1626, 1627, 1629,
        1639, 1678, 1721, 1722, 1723, 1737, 1743, 1744, 1745, 1763,
        1765, 1766, 1767, 1768, 1769, 1771, 1776, 1777, 1778, 1779,
        1780, 1782, 1786, 1787, 1788, 1789, 1790, 1792, 1793, 1794,
        1795, 1796, 1797, 1798, 1799, 1800, 1801, 1802, 1803, 1804,
        1805, 1806, 1807,
        1828,
    };

    #region Moves

    internal static readonly ushort[] MoveShop8_LA =
    {
        (int)Move.FalseSwipe,
        (int)Move.FireFang,
        (int)Move.ThunderFang,
        (int)Move.IceFang,
        (int)Move.IceBall,
        (int)Move.RockSmash,
        (int)Move.Spikes,
        (int)Move.Bulldoze,
        (int)Move.AerialAce,
        (int)Move.StealthRock,
        (int)Move.Swift,
        (int)Move.TriAttack,
        (int)Move.MagicalLeaf,
        (int)Move.OminousWind,
        (int)Move.PowerShift,
        (int)Move.FocusEnergy,
        (int)Move.BulkUp,
        (int)Move.CalmMind,
        (int)Move.Rest,
        (int)Move.BabyDollEyes,
        (int)Move.FirePunch,
        (int)Move.ThunderPunch,
        (int)Move.IcePunch,
        (int)Move.DrainPunch,
        (int)Move.PoisonJab,
        (int)Move.PsychoCut,
        (int)Move.ZenHeadbutt,
        (int)Move.LeechLife,
        (int)Move.XScissor,
        (int)Move.RockSlide,
        (int)Move.ShadowClaw,
        (int)Move.IronHead,
        (int)Move.IronTail,
        (int)Move.MysticalFire,
        (int)Move.WaterPulse,
        (int)Move.ChargeBeam,
        (int)Move.EnergyBall,
        (int)Move.IcyWind,
        (int)Move.SludgeBomb,
        (int)Move.EarthPower,
        (int)Move.ShadowBall,
        (int)Move.Snarl,
        (int)Move.FlashCannon,
        (int)Move.DazzlingGleam,
        (int)Move.GigaImpact,
        (int)Move.AquaTail,
        (int)Move.WildCharge,
        (int)Move.HighHorsepower,
        (int)Move.Megahorn,
        (int)Move.StoneEdge,
        (int)Move.Outrage,
        (int)Move.PlayRough,
        (int)Move.HyperBeam,
        (int)Move.Flamethrower,
        (int)Move.Thunderbolt,
        (int)Move.IceBeam,
        (int)Move.Psychic,
        (int)Move.DarkPulse,
        (int)Move.DracoMeteor,
        (int)Move.SteelBeam,
        (int)Move.VoltTackle,
    };

    #endregion
}
