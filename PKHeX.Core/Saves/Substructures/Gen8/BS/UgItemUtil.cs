using System.Collections.Generic;

namespace PKHeX.Core;

public static class UgItemUtil
{
    public static UgItemType GetType(int ugItemID)
    {
        if ((uint)ugItemID >= Items.Count)
            return UgItemType.None;
        return Items[ugItemID].Type;
    }

    public static int GetMax(int ugItemID) => GetType(ugItemID) switch
    {
        UgItemType.Sphere => UndergroundItemList8b.ItemMaxCount,
        UgItemType.Statue => UndergroundItemList8b.StatueMaxCount,
        UgItemType.Pedestal => UndergroundItemList8b.StatueMaxCount,
        _ => UndergroundItemList8b.ItemMaxCount,
    };

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private readonly record struct UgItemDef(ushort UgItemID, short ItemID, sbyte SphereID, sbyte PedestalID, short StatueID)
    {
        private bool IsSphere => SphereID > 0;
        private bool IsItem => ItemID > 0;
        private bool IsStatue => StatueID > 0;
        private bool IsPedestal => PedestalID > 0;

        public UgItemType Type =>
            IsSphere ? UgItemType.Sphere :
            IsItem ? UgItemType.Item :
            IsStatue ? UgItemType.Statue :
            IsPedestal ? UgItemType.Pedestal : UgItemType.None;
    }

    #region Table
    private static readonly IReadOnlyList<UgItemDef> Items =
    [
        new(000,000,000,000,000), // None
        new(001, -1,001, -1, -1), // Red Sphere S
        new(002, -1,002, -1, -1), // Blue Sphere S
        new(003, -1,003, -1, -1), // Green Sphere S
        new(004, -1,004, -1, -1), // Prism Sphere S
        new(005, -1,005, -1, -1), // Pale Sphere S
        new(006, -1,006, -1, -1), // Red Sphere L
        new(007, -1,007, -1, -1), // Blue Sphere L
        new(008, -1,008, -1, -1), // Green Sphere L
        new(009, -1,009, -1, -1), // Prism Sphere L
        new(010, -1,010, -1, -1), // Pale Sphere L
        new(011,028, -1, -1, -1), // Revive
        new(012,029, -1, -1, -1), // Max Revive
        new(013,072, -1, -1, -1), // Red Shard
        new(014,073, -1, -1, -1), // Blue Shard
        new(015,074, -1, -1, -1), // Yellow Shard
        new(016,075, -1, -1, -1), // Green Shard
        new(017,080, -1, -1, -1), // Sun Stone
        new(018,081, -1, -1, -1), // Moon Stone
        new(019,082, -1, -1, -1), // Fire Stone
        new(020,083, -1, -1, -1), // Thunder Stone
        new(021,084, -1, -1, -1), // Water Stone
        new(022,085, -1, -1, -1), // Leaf Stone
        new(023,110, -1, -1, -1), // Oval Stone
        new(024,229, -1, -1, -1), // Everstone
        new(025,111, -1, -1, -1), // Odd Keystone
        new(026,091, -1, -1, -1), // Star Piece
        new(027,093, -1, -1, -1), // Heart Scale
        new(028,099, -1, -1, -1), // Root Fossil
        new(029,100, -1, -1, -1), // Claw Fossil
        new(030,101, -1, -1, -1), // Helix Fossil
        new(031,102, -1, -1, -1), // Dome Fossil
        new(032,103, -1, -1, -1), // Old Amber
        new(033,104, -1, -1, -1), // Armor Fossil
        new(034,105, -1, -1, -1), // Skull Fossil
        new(035,106, -1, -1, -1), // Rare Bone
        new(036,269, -1, -1, -1), // Light Clay
        new(037,278, -1, -1, -1), // Iron Ball
        new(038,282, -1, -1, -1), // Icy Rock
        new(039,283, -1, -1, -1), // Smooth Rock
        new(040,284, -1, -1, -1), // Heat Rock
        new(041,285, -1, -1, -1), // Damp Rock
        new(042,1808, -1, -1, -1), // Mysterious Shard S
        new(043,1809, -1, -1, -1), // Mysterious Shard L
        new(044,238, -1, -1, -1), // Hard Stone
        new(045,298, -1, -1, -1), // Flame Plate
        new(046,299, -1, -1, -1), // Splash Plate
        new(047,300, -1, -1, -1), // Zap Plate
        new(048,301, -1, -1, -1), // Meadow Plate
        new(049,302, -1, -1, -1), // Icicle Plate
        new(050,303, -1, -1, -1), // Fist Plate
        new(051,304, -1, -1, -1), // Toxic Plate
        new(052,305, -1, -1, -1), // Earth Plate
        new(053,306, -1, -1, -1), // Sky Plate
        new(054,307, -1, -1, -1), // Mind Plate
        new(055,308, -1, -1, -1), // Insect Plate
        new(056,309, -1, -1, -1), // Stone Plate
        new(057,310, -1, -1, -1), // Spooky Plate
        new(058,311, -1, -1, -1), // Draco Plate
        new(059,312, -1, -1, -1), // Dread Plate
        new(060,313, -1, -1, -1), // Iron Plate
        new(061,644, -1, -1, -1), // Pixie Plate
        new(062, -1, -1, -1,000), // 
        new(063, -1, -1, -1,000), // 
        new(064, -1, -1, -1,000), // 
        new(065, -1, -1, -1,000), // 
        new(066, -1, -1, -1,000), // 
        new(067, -1, -1, -1,000), // 
        new(068, -1, -1, -1,000), // 
        new(069, -1, -1, -1,000), // 
        new(070, -1, -1, -1,000), // 
        new(071, -1, -1, -1,000), // 
        new(072, -1, -1, -1,000), // 
        new(073, -1, -1, -1,000), // 
        new(074, -1, -1, -1,000), // 
        new(075, -1, -1, -1,000), // 
        new(076, -1, -1, -1,000), // 
        new(077, -1, -1, -1,000), // 
        new(078, -1, -1, -1,000), // 
        new(079, -1, -1, -1,000), // 
        new(080, -1, -1, -1,000), // 
        new(081, -1, -1, -1,000), // 
        new(082, -1, -1, -1,000), // 
        new(083, -1, -1, -1,000), // 
        new(084, -1, -1, -1,000), // 
        new(085, -1, -1, -1,000), // 
        new(086, -1, -1, -1,000), // 
        new(087, -1, -1, -1,000), // 
        new(088, -1, -1, -1,000), // 
        new(089, -1, -1, -1,000), // 
        new(090, -1, -1, -1,000), // 
        new(091, -1, -1, -1,000), // 
        new(092, -1, -1, -1,000), // 
        new(093, -1, -1, -1,000), // 
        new(094, -1, -1, -1,000), // 
        new(095, -1, -1, -1,000), // 
        new(096, -1, -1, -1,000), // 
        new(097, -1, -1, -1,000), // 
        new(098, -1, -1, -1,000), // 
        new(099, -1, -1, -1,000), // 
        new(100, -1, -1, -1,000), // 
        new(101, -1, -1, -1,000), // 
        new(102, -1, -1, -1,000), // 
        new(103, -1, -1, -1,000), // 
        new(104, -1, -1, -1,000), // 
        new(105, -1, -1, -1,000), // 
        new(106, -1, -1, -1,000), // 
        new(107, -1, -1, -1,000), // 
        new(108, -1, -1, -1,000), // 
        new(109, -1, -1, -1,000), // 
        new(110, -1, -1, -1,000), // 
        new(111, -1, -1, -1,000), // 
        new(112, -1, -1, -1,000), // 
        new(113, -1, -1, -1,000), // 
        new(114, -1, -1, -1,000), // 
        new(115, -1, -1, -1,000), // 
        new(116, -1, -1, -1,000), // 
        new(117, -1, -1, -1,000), // 
        new(118, -1, -1, -1,000), // 
        new(119, -1, -1, -1,000), // 
        new(120, -1, -1, -1,000), // 
        new(121, -1, -1, -1,000), // 
        new(122, -1, -1, -1,000), // 
        new(123, -1, -1, -1,000), // 
        new(124, -1, -1, -1,000), // 
        new(125, -1, -1, -1,000), // 
        new(126, -1, -1, -1,000), // 
        new(127, -1, -1, -1,000), // 
        new(128, -1, -1, -1,000), // 
        new(129, -1, -1, -1,000), // 
        new(130, -1, -1, -1,000), // 
        new(131, -1, -1, -1,000), // 
        new(132, -1, -1, -1,000), // 
        new(133, -1, -1, -1,000), // 
        new(134, -1, -1, -1,000), // 
        new(135, -1, -1, -1,000), // 
        new(136, -1, -1, -1,000), // 
        new(137, -1, -1, -1,000), // 
        new(138, -1, -1, -1,000), // 
        new(139, -1, -1, -1,000), // 
        new(140, -1, -1, -1,000), // 
        new(141, -1, -1, -1,000), // 
        new(142, -1, -1, -1,000), // 
        new(143, -1, -1, -1,000), // 
        new(144, -1, -1, -1,000), // 
        new(145, -1, -1, -1,000), // 
        new(146, -1, -1, -1,000), // 
        new(147, -1, -1, -1,000), // 
        new(148, -1, -1, -1,000), // 
        new(149, -1, -1, -1,000), // 
        new(150, -1, -1, -1,000), // 
        new(151, -1, -1, -1,000), // 
        new(152, -1, -1, -1,000), // 
        new(153, -1, -1, -1,000), // 
        new(154, -1, -1, -1,000), // 
        new(155, -1, -1, -1,000), // 
        new(156, -1, -1, -1,000), // 
        new(157, -1, -1, -1,000), // 
        new(158, -1, -1, -1,000), // 
        new(159, -1, -1, -1,000), // 
        new(160, -1, -1, -1,000), // 
        new(161, -1, -1, -1,000), // 
        new(162, -1, -1, -1,000), // 
        new(163, -1, -1, -1,000), // 
        new(164, -1, -1, -1,000), // 
        new(165, -1, -1, -1,000), // 
        new(166, -1, -1, -1,000), // 
        new(167, -1, -1, -1,000), // 
        new(168, -1, -1, -1,000), // 
        new(169, -1, -1, -1,000), // 
        new(170, -1, -1, -1,000), // 
        new(171, -1, -1, -1,000), // 
        new(172, -1, -1, -1,000), // 
        new(173, -1, -1, -1,000), // 
        new(174, -1, -1, -1,000), // 
        new(175, -1, -1, -1,000), // 
        new(176, -1, -1, -1,000), // 
        new(177, -1, -1, -1,000), // 
        new(178, -1, -1, -1,000), // 
        new(179, -1, -1, -1,000), // 
        new(180, -1, -1, -1,000), // 
        new(181, -1, -1, -1,000), // 
        new(182, -1, -1, -1,000), // 
        new(183, -1, -1, -1,000), // 
        new(184, -1, -1, -1,000), // 
        new(185, -1, -1, -1,000), // 
        new(186, -1, -1, -1,000), // 
        new(187, -1, -1, -1,000), // 
        new(188, -1, -1, -1,000), // 
        new(189, -1, -1, -1,000), // 
        new(190, -1, -1, -1,000), // 
        new(191, -1, -1, -1,000), // 
        new(192, -1, -1, -1,000), // 
        new(193, -1, -1, -1,000), // 
        new(194, -1, -1, -1,000), // 
        new(195, -1, -1, -1,000), // 
        new(196, -1, -1, -1,000), // 
        new(197, -1, -1, -1,000), // 
        new(198, -1, -1, -1,000), // 
        new(199, -1, -1, -1,000), // 
        new(200, -1, -1, -1,000), // 
        new(201, -1, -1, -1,000), // 
        new(202, -1, -1, -1,000), // 
        new(203, -1, -1, -1,000), // 
        new(204, -1, -1, -1,000), // 
        new(205, -1, -1, -1,000), // 
        new(206, -1, -1, -1,000), // 
        new(207, -1, -1, -1,000), // 
        new(208, -1, -1, -1,000), // 
        new(209, -1, -1, -1,000), // 
        new(210, -1, -1, -1,000), // 
        new(211, -1, -1, -1,000), // 
        new(212, -1, -1, -1,000), // 
        new(213, -1, -1, -1,000), // 
        new(214, -1, -1, -1,000), // 
        new(215, -1, -1, -1,000), // 
        new(216, -1, -1, -1,000), // 
        new(217, -1, -1, -1,000), // 
        new(218, -1, -1, -1,000), // 
        new(219, -1, -1, -1,000), // 
        new(220, -1, -1, -1,000), // 
        new(221, -1, -1, -1,000), // 
        new(222, -1, -1, -1,000), // 
        new(223, -1, -1, -1,000), // 
        new(224, -1, -1, -1,000), // 
        new(225, -1, -1, -1,000), // 
        new(226, -1, -1, -1,000), // 
        new(227, -1, -1, -1,000), // 
        new(228, -1, -1, -1,000), // 
        new(229, -1, -1, -1,000), // 
        new(230, -1, -1, -1,000), // 
        new(231, -1, -1, -1,000), // 
        new(232, -1, -1, -1,000), // 
        new(233, -1, -1, -1,000), // 
        new(234, -1, -1, -1,000), // 
        new(235, -1, -1, -1,000), // 
        new(236, -1, -1, -1,000), // 
        new(237, -1, -1, -1,000), // 
        new(238, -1, -1, -1,000), // 
        new(239, -1, -1, -1,000), // 
        new(240, -1, -1, -1,000), // 
        new(241, -1, -1, -1,000), // 
        new(242, -1, -1, -1,000), // 
        new(243, -1, -1, -1,000), // 
        new(244, -1, -1, -1,000), // 
        new(245, -1, -1, -1,000), // 
        new(246, -1, -1, -1,000), // 
        new(247, -1, -1, -1,000), // 
        new(248, -1, -1, -1,000), // 
        new(249, -1, -1, -1,000), // 
        new(250, -1, -1, -1,000), // 
        new(251, -1, -1, -1,000), // 
        new(252, -1, -1, -1,000), // 
        new(253, -1, -1, -1,000), // 
        new(254, -1, -1, -1,000), // 
        new(255, -1, -1, -1,000), // 
        new(256, -1, -1, -1,000), // 
        new(257, -1, -1, -1,000), // 
        new(258, -1, -1, -1,000), // 
        new(259, -1, -1, -1,000), // 
        new(260, -1, -1, -1,000), // 
        new(261, -1, -1, -1,000), // 
        new(262, -1, -1, -1,000), // 
        new(263, -1, -1, -1,000), // 
        new(264, -1, -1, -1,000), // 
        new(265, -1, -1, -1,000), // 
        new(266, -1, -1, -1,000), // 
        new(267, -1, -1, -1,000), // 
        new(268, -1, -1, -1,000), // 
        new(269, -1, -1, -1,000), // 
        new(270, -1, -1, -1,000), // 
        new(271, -1, -1, -1,000), // 
        new(272, -1, -1, -1,000), // 
        new(273, -1, -1, -1,000), // 
        new(274, -1, -1, -1,000), // 
        new(275, -1, -1, -1,000), // 
        new(276, -1, -1, -1,000), // 
        new(277, -1, -1, -1,000), // 
        new(278, -1, -1, -1,000), // 
        new(279, -1, -1, -1,000), // 
        new(280, -1, -1, -1,000), // 
        new(281, -1, -1, -1,000), // 
        new(282, -1, -1, -1,000), // 
        new(283, -1, -1, -1,000), // 
        new(284, -1, -1, -1,000), // 
        new(285, -1, -1, -1,000), // 
        new(286, -1, -1, -1,000), // 
        new(287, -1, -1, -1,000), // 
        new(288, -1, -1, -1,000), // 
        new(289, -1, -1, -1,000), // 
        new(290, -1, -1, -1,000), // 
        new(291, -1, -1, -1,000), // 
        new(292, -1, -1, -1,000), // 
        new(293, -1, -1, -1,000), // 
        new(294, -1, -1, -1,000), // 
        new(295, -1, -1, -1,000), // 
        new(296, -1, -1, -1,000), // 
        new(297, -1, -1, -1,000), // 
        new(298, -1, -1, -1,000), // 
        new(299, -1, -1, -1,000), // 
        new(300, -1, -1, -1,000), // 
        new(301, -1, -1, -1,000), // 
        new(302, -1, -1, -1,000), // 
        new(303, -1, -1, -1,000), // 
        new(304, -1, -1, -1,000), // 
        new(305, -1, -1, -1,000), // 
        new(306, -1, -1, -1,000), // 
        new(307, -1, -1, -1,000), // 
        new(308, -1, -1, -1,000), // 
        new(309, -1, -1, -1,000), // 
        new(310, -1, -1, -1,000), // 
        new(311, -1, -1, -1,000), // 
        new(312, -1, -1, -1,000), // 
        new(313, -1, -1, -1,000), // 
        new(314, -1, -1, -1,000), // 
        new(315, -1, -1, -1,000), // 
        new(316, -1, -1, -1,000), // 
        new(317, -1, -1, -1,000), // 
        new(318, -1, -1, -1,000), // 
        new(319, -1, -1, -1,000), // 
        new(320, -1, -1, -1,000), // 
        new(321, -1, -1, -1,000), // 
        new(322, -1, -1, -1,000), // 
        new(323, -1, -1, -1,000), // 
        new(324, -1, -1, -1,000), // 
        new(325, -1, -1, -1,000), // 
        new(326, -1, -1, -1,000), // 
        new(327, -1, -1, -1,000), // 
        new(328, -1, -1, -1,000), // 
        new(329, -1, -1, -1,000), // 
        new(330, -1, -1, -1,000), // 
        new(331, -1, -1, -1,000), // 
        new(332, -1, -1, -1,000), // 
        new(333, -1, -1, -1,000), // 
        new(334, -1, -1, -1,000), // 
        new(335, -1, -1, -1,000), // 
        new(336, -1, -1, -1,000), // 
        new(337, -1, -1, -1,000), // 
        new(338, -1, -1, -1,000), // 
        new(339, -1, -1, -1,000), // 
        new(340, -1, -1, -1,000), // 
        new(341, -1, -1, -1,000), // 
        new(342, -1, -1, -1,000), // 
        new(343, -1, -1, -1,000), // 
        new(344, -1, -1, -1,000), // 
        new(345, -1, -1, -1,000), // 
        new(346, -1, -1, -1,000), // 
        new(347, -1, -1, -1,000), // 
        new(348, -1, -1, -1,000), // 
        new(349, -1, -1, -1,000), // 
        new(350, -1, -1, -1,000), // 
        new(351, -1, -1, -1,000), // 
        new(352, -1, -1, -1,000), // 
        new(353, -1, -1, -1,000), // 
        new(354, -1, -1, -1,000), // 
        new(355, -1, -1, -1,000), // 
        new(356, -1, -1, -1,000), // 
        new(357, -1, -1, -1,000), // 
        new(358, -1, -1, -1,000), // 
        new(359, -1, -1, -1,000), // 
        new(360, -1, -1, -1,000), // 
        new(361, -1, -1, -1,000), // 
        new(362, -1, -1, -1,000), // 
        new(363, -1, -1, -1,000), // 
        new(364, -1, -1, -1,000), // 
        new(365, -1, -1, -1,000), // 
        new(366, -1, -1, -1,000), // 
        new(367, -1, -1, -1,000), // 
        new(368, -1, -1, -1,000), // 
        new(369, -1, -1, -1,000), // 
        new(370, -1, -1,001, -1), // Square Pedestal XS
        new(371, -1, -1,002, -1), // Square Pedestal S
        new(372, -1, -1,003, -1), // Square Pedestal M
        new(373, -1, -1,004, -1), // Square Pedestal L
        new(374, -1, -1,005, -1), // Round Pedestal XS
        new(375, -1, -1,006, -1), // Round Pedestal M
        new(376, -1, -1,007, -1), // Round Pedestal L
        new(377, -1, -1,008, -1), // Sturdy Pedestal XS
        new(378, -1, -1,009, -1), // Sturdy Pedestal S
        new(379, -1, -1,010, -1), // Sturdy Pedestal M
        new(380, -1, -1,011, -1), // Sturdy Pedestal L
        new(381, -1, -1,012, -1), // Clear Pedestal XS
        new(382, -1, -1,013, -1), // Clear Pedestal S
        new(383, -1, -1,014, -1), // Clear Pedestal M
        new(384, -1, -1,015, -1), // Clear Pedestal L
        new(385, -1, -1,016, -1), // Dawn Pedestal L
        new(386, -1, -1,017, -1), // Dawn Pedestal XL
        new(387, -1, -1,018, -1), // Night Pedestal L
        new(388, -1, -1,019, -1), // Night Pedestal XL
        new(389, -1, -1,020, -1), // Diamond Pedestal L
        new(390, -1, -1,021, -1), // Diamond Pedestal XL
        new(391, -1, -1,022, -1), // Pearl Pedestal L
        new(392, -1, -1,023, -1), // Pearl Pedestal XL
        new(393, -1, -1,024, -1), // Spin Pedestal XS
        new(394, -1, -1,025, -1), // Spin Pedestal M
        new(395, -1, -1,026, -1), // Spin Pedestal L
        new(396, -1, -1,027, -1), // Spinback Pedestal XS
        new(397, -1, -1,028, -1), // Spinback Pedestal M
        new(398, -1, -1,029, -1), // Spinback Pedestal L
        new(399,1810, -1, -1, -1), // Digger Drill
        new(400,328, -1, -1, -1), // TM01
        new(401,329, -1, -1, -1), // TM02
        new(402,330, -1, -1, -1), // TM03
        new(403,331, -1, -1, -1), // TM04
        new(404,332, -1, -1, -1), // TM05
        new(405,333, -1, -1, -1), // TM06
        new(406,334, -1, -1, -1), // TM07
        new(407,335, -1, -1, -1), // TM08
        new(408,336, -1, -1, -1), // TM09
        new(409,337, -1, -1, -1), // TM10
        new(410,338, -1, -1, -1), // TM11
        new(411,339, -1, -1, -1), // TM12
        new(412,340, -1, -1, -1), // TM13
        new(413,341, -1, -1, -1), // TM14
        new(414,342, -1, -1, -1), // TM15
        new(415,343, -1, -1, -1), // TM16
        new(416,344, -1, -1, -1), // TM17
        new(417,345, -1, -1, -1), // TM18
        new(418,346, -1, -1, -1), // TM19
        new(419,347, -1, -1, -1), // TM20
        new(420,348, -1, -1, -1), // TM21
        new(421,349, -1, -1, -1), // TM22
        new(422,350, -1, -1, -1), // TM23
        new(423,351, -1, -1, -1), // TM24
        new(424,352, -1, -1, -1), // TM25
        new(425,353, -1, -1, -1), // TM26
        new(426,354, -1, -1, -1), // TM27
        new(427,355, -1, -1, -1), // TM28
        new(428,356, -1, -1, -1), // TM29
        new(429,357, -1, -1, -1), // TM30
        new(430,358, -1, -1, -1), // TM31
        new(431,359, -1, -1, -1), // TM32
        new(432,360, -1, -1, -1), // TM33
        new(433,361, -1, -1, -1), // TM34
        new(434,362, -1, -1, -1), // TM35
        new(435,363, -1, -1, -1), // TM36
        new(436,364, -1, -1, -1), // TM37
        new(437,365, -1, -1, -1), // TM38
        new(438,366, -1, -1, -1), // TM39
        new(439,367, -1, -1, -1), // TM40
        new(440,368, -1, -1, -1), // TM41
        new(441,369, -1, -1, -1), // TM42
        new(442,370, -1, -1, -1), // TM43
        new(443,371, -1, -1, -1), // TM44
        new(444,372, -1, -1, -1), // TM45
        new(445,373, -1, -1, -1), // TM46
        new(446,374, -1, -1, -1), // TM47
        new(447,375, -1, -1, -1), // TM48
        new(448,376, -1, -1, -1), // TM49
        new(449,377, -1, -1, -1), // TM50
        new(450,378, -1, -1, -1), // TM51
        new(451,379, -1, -1, -1), // TM52
        new(452,380, -1, -1, -1), // TM53
        new(453,381, -1, -1, -1), // TM54
        new(454,382, -1, -1, -1), // TM55
        new(455,383, -1, -1, -1), // TM56
        new(456,384, -1, -1, -1), // TM57
        new(457,385, -1, -1, -1), // TM58
        new(458,386, -1, -1, -1), // TM59
        new(459,387, -1, -1, -1), // TM60
        new(460,388, -1, -1, -1), // TM61
        new(461,389, -1, -1, -1), // TM62
        new(462,390, -1, -1, -1), // TM63
        new(463,391, -1, -1, -1), // TM64
        new(464,392, -1, -1, -1), // TM65
        new(465,393, -1, -1, -1), // TM66
        new(466,394, -1, -1, -1), // TM67
        new(467,395, -1, -1, -1), // TM68
        new(468,396, -1, -1, -1), // TM69
        new(469,397, -1, -1, -1), // TM70
        new(470,398, -1, -1, -1), // TM71
        new(471,399, -1, -1, -1), // TM72
        new(472,400, -1, -1, -1), // TM73
        new(473,401, -1, -1, -1), // TM74
        new(474,402, -1, -1, -1), // TM75
        new(475,403, -1, -1, -1), // TM76
        new(476,404, -1, -1, -1), // TM77
        new(477,405, -1, -1, -1), // TM78
        new(478,406, -1, -1, -1), // TM79
        new(479,407, -1, -1, -1), // TM80
        new(480,408, -1, -1, -1), // TM81
        new(481,409, -1, -1, -1), // TM82
        new(482,410, -1, -1, -1), // TM83
        new(483,411, -1, -1, -1), // TM84
        new(484,412, -1, -1, -1), // TM85
        new(485,413, -1, -1, -1), // TM86
        new(486,414, -1, -1, -1), // TM87
        new(487,415, -1, -1, -1), // TM88
        new(488,416, -1, -1, -1), // TM89
        new(489,417, -1, -1, -1), // TM90
        new(490,418, -1, -1, -1), // TM91
        new(491,419, -1, -1, -1), // TM92
        new(492, -1, -1, -1,001), // Bulbasaur Statue
        new(493, -1, -1, -1,002), // Ivysaur Statue
        new(494, -1, -1, -1,003), // Venusaur Statue
        new(495, -1, -1, -1,004), // Charmander Statue
        new(496, -1, -1, -1,005), // Charmeleon Statue
        new(497, -1, -1, -1,006), // Charizard Statue
        new(498, -1, -1, -1,007), // Squirtle Statue
        new(499, -1, -1, -1,008), // Wartortle Statue
        new(500, -1, -1, -1,009), // Blastoise Statue
        new(501, -1, -1, -1,010), // Pikachu Statue
        new(502, -1, -1, -1,011), // Nidoqueen Statue
        new(503, -1, -1, -1,012), // Nidoking Statue
        new(504, -1, -1, -1,013), // Clefairy Statue
        new(505, -1, -1, -1,014), // Vulpix Statue
        new(506, -1, -1, -1,015), // Jigglypuff Statue
        new(507, -1, -1, -1,016), // Oddish Statue
        new(508, -1, -1, -1,017), // Meowth Statue
        new(509, -1, -1, -1,018), // Psyduck Statue
        new(510, -1, -1, -1,019), // Arcanine Statue
        new(511, -1, -1, -1,020), // Poliwrath Statue
        new(512, -1, -1, -1,021), // Alakazam Statue
        new(513, -1, -1, -1,022), // Machamp Statue
        new(514, -1, -1, -1,023), // Tentacruel Statue
        new(515, -1, -1, -1,024), // Geodude Statue
        new(516, -1, -1, -1,025), // Rapidash Statue
        new(517, -1, -1, -1,026), // Slowpoke Statue
        new(518, -1, -1, -1,027), // Farfetch’d Statue
        new(519, -1, -1, -1,028), // Gengar Statue
        new(520, -1, -1, -1,029), // Cubone Statue
        new(521, -1, -1, -1,030), // Lickitung Statue
        new(522, -1, -1, -1,031), // Weezing Statue
        new(523, -1, -1, -1,032), // Chansey Statue
        new(524, -1, -1, -1,033), // Kangaskhan Statue
        new(525, -1, -1, -1,034), // Seaking Statue
        new(526, -1, -1, -1,035), // Mr. Mime Statue
        new(527, -1, -1, -1,036), // Tauros Statue
        new(528, -1, -1, -1,037), // Magikarp Statue
        new(529, -1, -1, -1,038), // Gyarados Statue
        new(530, -1, -1, -1,039), // Lapras Statue
        new(531, -1, -1, -1,040), // Ditto Statue
        new(532, -1, -1, -1,041), // Eevee Statue
        new(533, -1, -1, -1,042), // Vaporeon Statue
        new(534, -1, -1, -1,043), // Jolteon Statue
        new(535, -1, -1, -1,044), // Flareon Statue
        new(536, -1, -1, -1,045), // Porygon Statue
        new(537, -1, -1, -1,046), // Omastar Statue
        new(538, -1, -1, -1,047), // Snorlax Statue
        new(539, -1, -1, -1,048), // Dragonite Statue
        new(540, -1, -1, -1,049), // Chikorita Statue
        new(541, -1, -1, -1,050), // Bayleef Statue
        new(542, -1, -1, -1,051), // Meganium Statue
        new(543, -1, -1, -1,052), // Cyndaquil Statue
        new(544, -1, -1, -1,053), // Quilava Statue
        new(545, -1, -1, -1,054), // Typhlosion Statue
        new(546, -1, -1, -1,055), // Totodile Statue
        new(547, -1, -1, -1,056), // Croconaw Statue
        new(548, -1, -1, -1,057), // Feraligatr Statue
        new(549, -1, -1, -1,058), // Furret Statue
        new(550, -1, -1, -1,059), // Noctowl Statue
        new(551, -1, -1, -1,060), // Crobat Statue
        new(552, -1, -1, -1,061), // Togepi Statue
        new(553, -1, -1, -1,062), // Ampharos Statue
        new(554, -1, -1, -1,063), // Bellossom Statue
        new(555, -1, -1, -1,064), // Marill Statue
        new(556, -1, -1, -1,065), // Sudowoodo Statue
        new(557, -1, -1, -1,066), // Aipom Statue
        new(558, -1, -1, -1,067), // Sunkern Statue
        new(559, -1, -1, -1,068), // Quagsire Statue
        new(560, -1, -1, -1,069), // Espeon Statue
        new(561, -1, -1, -1,070), // Umbreon Statue
        new(562, -1, -1, -1,071), // Murkrow Statue
        new(563, -1, -1, -1,072), // Unown Statue
        new(564, -1, -1, -1,073), // Wobbuffet Statue
        new(565, -1, -1, -1,074), // Girafarig Statue
        new(566, -1, -1, -1,075), // Dunsparce Statue
        new(567, -1, -1, -1,076), // Steelix Statue
        new(568, -1, -1, -1,077), // Snubbull Statue
        new(569, -1, -1, -1,078), // Scizor Statue
        new(570, -1, -1, -1,079), // Heracross Statue
        new(571, -1, -1, -1,080), // Magcargo Statue
        new(572, -1, -1, -1,081), // Octillery Statue
        new(573, -1, -1, -1,082), // Mantine Statue
        new(574, -1, -1, -1,083), // Smeargle Statue
        new(575, -1, -1, -1,084), // Miltank Statue
        new(576, -1, -1, -1,085), // Tyranitar Statue
        new(577, -1, -1, -1,086), // Treecko Statue
        new(578, -1, -1, -1,087), // Grovyle Statue
        new(579, -1, -1, -1,088), // Sceptile Statue
        new(580, -1, -1, -1,089), // Torchic Statue
        new(581, -1, -1, -1,090), // Combusken Statue
        new(582, -1, -1, -1,091), // Blaziken Statue
        new(583, -1, -1, -1,092), // Mudkip Statue
        new(584, -1, -1, -1,093), // Marshtomp Statue
        new(585, -1, -1, -1,094), // Swampert Statue
        new(586, -1, -1, -1,095), // Beautifly Statue
        new(587, -1, -1, -1,096), // Dustox Statue
        new(588, -1, -1, -1,097), // Ludicolo Statue
        new(589, -1, -1, -1,098), // Pelipper Statue
        new(590, -1, -1, -1,099), // Gardevoir Statue
        new(591, -1, -1, -1,100), // Shroomish Statue
        new(592, -1, -1, -1,101), // Slaking Statue
        new(593, -1, -1, -1,102), // Ninjask Statue
        new(594, -1, -1, -1,103), // Exploud Statue
        new(595, -1, -1, -1,104), // Skitty Statue
        new(596, -1, -1, -1,105), // Sableye Statue
        new(597, -1, -1, -1,106), // Mawile Statue
        new(598, -1, -1, -1,107), // Aggron Statue
        new(599, -1, -1, -1,108), // Medicham Statue
        new(600, -1, -1, -1,109), // Manectric Statue
        new(601, -1, -1, -1,110), // Plusle Statue
        new(602, -1, -1, -1,111), // Minun Statue
        new(603, -1, -1, -1,112), // Volbeat Statue
        new(604, -1, -1, -1,113), // Illumise Statue
        new(605, -1, -1, -1,114), // Roselia Statue
        new(606, -1, -1, -1,115), // Sharpedo Statue
        new(607, -1, -1, -1,116), // Wailmer Statue
        new(608, -1, -1, -1,117), // Camerupt Statue
        new(609, -1, -1, -1,118), // Torkoal Statue
        new(610, -1, -1, -1,119), // Spinda Statue
        new(611, -1, -1, -1,120), // Flygon Statue
        new(612, -1, -1, -1,121), // Cacturne Statue
        new(613, -1, -1, -1,122), // Altaria Statue
        new(614, -1, -1, -1,123), // Zangoose Statue
        new(615, -1, -1, -1,124), // Seviper Statue
        new(616, -1, -1, -1,125), // Lunatone Statue
        new(617, -1, -1, -1,126), // Solrock Statue
        new(618, -1, -1, -1,127), // Whiscash Statue
        new(619, -1, -1, -1,128), // Claydol Statue
        new(620, -1, -1, -1,129), // Cradily Statue
        new(621, -1, -1, -1,130), // Milotic Statue
        new(622, -1, -1, -1,131), // Castform Statue
        new(623, -1, -1, -1,132), // Kecleon Statue
        new(624, -1, -1, -1,133), // Tropius Statue
        new(625, -1, -1, -1,134), // Chimecho Statue
        new(626, -1, -1, -1,135), // Absol Statue
        new(627, -1, -1, -1,136), // Spheal Statue
        new(628, -1, -1, -1,137), // Clamperl Statue
        new(629, -1, -1, -1,138), // Relicanth Statue
        new(630, -1, -1, -1,139), // Luvdisc Statue
        new(631, -1, -1, -1,140), // Salamence Statue
        new(632, -1, -1, -1,141), // Metagross Statue
        new(633, -1, -1, -1,142), // Turtwig Statue
        new(634, -1, -1, -1,143), // Grotle Statue
        new(635, -1, -1, -1,144), // Torterra Statue
        new(636, -1, -1, -1,145), // Chimchar Statue
        new(637, -1, -1, -1,146), // Monferno Statue
        new(638, -1, -1, -1,147), // Infernape Statue
        new(639, -1, -1, -1,148), // Piplup Statue
        new(640, -1, -1, -1,149), // Prinplup Statue
        new(641, -1, -1, -1,150), // Empoleon Statue
        new(642, -1, -1, -1,151), // Staraptor Statue
        new(643, -1, -1, -1,152), // Bibarel Statue
        new(644, -1, -1, -1,153), // Kricketot Statue
        new(645, -1, -1, -1,154), // Luxray Statue
        new(646, -1, -1, -1,155), // Budew Statue
        new(647, -1, -1, -1,156), // Roserade Statue
        new(648, -1, -1, -1,157), // Cranidos Statue
        new(649, -1, -1, -1,158), // Bastiodon Statue
        new(650, -1, -1, -1,159), // Burmy Statue
        new(651, -1, -1, -1,160), // Wormadam Statue
        new(652, -1, -1, -1,161), // Mothim Statue
        new(653, -1, -1, -1,162), // Vespiquen Statue
        new(654, -1, -1, -1,163), // Pachirisu Statue
        new(655, -1, -1, -1,164), // Floatzel Statue
        new(656, -1, -1, -1,165), // Cherubi Statue
        new(657, -1, -1, -1,166), // Gastrodon Statue
        new(658, -1, -1, -1,167), // Ambipom Statue
        new(659, -1, -1, -1,168), // Drifloon Statue
        new(660, -1, -1, -1,169), // Buneary Statue
        new(661, -1, -1, -1,170), // Mismagius Statue
        new(662, -1, -1, -1,171), // Honchkrow Statue
        new(663, -1, -1, -1,172), // Purugly Statue
        new(664, -1, -1, -1,173), // Skuntank Statue
        new(665, -1, -1, -1,174), // Bronzong Statue
        new(666, -1, -1, -1,175), // Happiny Statue
        new(667, -1, -1, -1,176), // Chatot Statue
        new(668, -1, -1, -1,177), // Spiritomb Statue
        new(669, -1, -1, -1,178), // Garchomp Statue
        new(670, -1, -1, -1,179), // Lucario Statue
        new(671, -1, -1, -1,180), // Hippowdon Statue
        new(672, -1, -1, -1,181), // Drapion Statue
        new(673, -1, -1, -1,182), // Croagunk Statue
        new(674, -1, -1, -1,183), // Carnivine Statue
        new(675, -1, -1, -1,184), // Lumineon Statue
        new(676, -1, -1, -1,185), // Abomasnow Statue
        new(677, -1, -1, -1,186), // Weavile Statue
        new(678, -1, -1, -1,187), // Magnezone Statue
        new(679, -1, -1, -1,188), // Rhyperior Statue
        new(680, -1, -1, -1,189), // Tangrowth Statue
        new(681, -1, -1, -1,190), // Electivire Statue
        new(682, -1, -1, -1,191), // Magmortar Statue
        new(683, -1, -1, -1,192), // Togekiss Statue
        new(684, -1, -1, -1,193), // Yanmega Statue
        new(685, -1, -1, -1,194), // Leafeon Statue
        new(686, -1, -1, -1,195), // Glaceon Statue
        new(687, -1, -1, -1,196), // Gliscor Statue
        new(688, -1, -1, -1,197), // Mamoswine Statue
        new(689, -1, -1, -1,198), // Porygon-Z Statue
        new(690, -1, -1, -1,199), // Gallade Statue
        new(691, -1, -1, -1,200), // Probopass Statue
        new(692, -1, -1, -1,201), // Dusknoir Statue
        new(693, -1, -1, -1,202), // Froslass Statue
        new(694, -1, -1, -1,203), // Rotom Statue
        new(695, -1, -1, -1,204), // Rotom Statue
        new(696, -1, -1, -1,205), // Rotom Statue
        new(697, -1, -1, -1,206), // Rotom Statue
        new(698, -1, -1, -1,207), // Rotom Statue
        new(699, -1, -1, -1,208), // Rotom Statue
        new(700, -1, -1, -1,209), // Articuno Statue
        new(701, -1, -1, -1,210), // Zapdos Statue
        new(702, -1, -1, -1,211), // Moltres Statue
        new(703, -1, -1, -1,212), // Mewtwo Statue
        new(704, -1, -1, -1,213), // Raikou Statue
        new(705, -1, -1, -1,214), // Entei Statue
        new(706, -1, -1, -1,215), // Suicune Statue
        new(707, -1, -1, -1,216), // Lugia Statue
        new(708, -1, -1, -1,217), // Ho-Oh Statue
        new(709, -1, -1, -1,218), // Regirock Statue
        new(710, -1, -1, -1,219), // Regice Statue
        new(711, -1, -1, -1,220), // Registeel Statue
        new(712, -1, -1, -1,221), // Latias Statue
        new(713, -1, -1, -1,222), // Latios Statue
        new(714, -1, -1, -1,223), // Kyogre Statue
        new(715, -1, -1, -1,224), // Groudon Statue
        new(716, -1, -1, -1,225), // Rayquaza Statue
        new(717, -1, -1, -1,226), // Bulbasaur Statue 
        new(718, -1, -1, -1,227), // Ivysaur Statue 
        new(719, -1, -1, -1,228), // Venusaur Statue 
        new(720, -1, -1, -1,229), // Charmander Statue 
        new(721, -1, -1, -1,230), // Charmeleon Statue 
        new(722, -1, -1, -1,231), // Charizard Statue 
        new(723, -1, -1, -1,232), // Squirtle Statue 
        new(724, -1, -1, -1,233), // Wartortle Statue 
        new(725, -1, -1, -1,234), // Blastoise Statue 
        new(726, -1, -1, -1,235), // Pikachu Statue 
        new(727, -1, -1, -1,236), // Nidoqueen Statue 
        new(728, -1, -1, -1,237), // Nidoking Statue 
        new(729, -1, -1, -1,238), // Clefairy Statue 
        new(730, -1, -1, -1,239), // Vulpix Statue 
        new(731, -1, -1, -1,240), // Jigglypuff Statue 
        new(732, -1, -1, -1,241), // Oddish Statue 
        new(733, -1, -1, -1,242), // Meowth Statue 
        new(734, -1, -1, -1,243), // Psyduck Statue 
        new(735, -1, -1, -1,244), // Arcanine Statue 
        new(736, -1, -1, -1,245), // Poliwrath Statue 
        new(737, -1, -1, -1,246), // Alakazam Statue 
        new(738, -1, -1, -1,247), // Machamp Statue 
        new(739, -1, -1, -1,248), // Tentacruel Statue 
        new(740, -1, -1, -1,249), // Geodude Statue 
        new(741, -1, -1, -1,250), // Rapidash Statue 
        new(742, -1, -1, -1,251), // Slowpoke Statue 
        new(743, -1, -1, -1,252), // Farfetch’d Statue 
        new(744, -1, -1, -1,253), // Gengar Statue 
        new(745, -1, -1, -1,254), // Cubone Statue 
        new(746, -1, -1, -1,255), // Lickitung Statue 
        new(747, -1, -1, -1,256), // Weezing Statue 
        new(748, -1, -1, -1,257), // Chansey Statue 
        new(749, -1, -1, -1,258), // Kangaskhan Statue 
        new(750, -1, -1, -1,259), // Seaking Statue 
        new(751, -1, -1, -1,260), // Mr. Mime Statue 
        new(752, -1, -1, -1,261), // Tauros Statue 
        new(753, -1, -1, -1,262), // Magikarp Statue 
        new(754, -1, -1, -1,263), // Gyarados Statue 
        new(755, -1, -1, -1,264), // Lapras Statue 
        new(756, -1, -1, -1,265), // Ditto Statue 
        new(757, -1, -1, -1,266), // Eevee Statue 
        new(758, -1, -1, -1,267), // Vaporeon Statue 
        new(759, -1, -1, -1,268), // Jolteon Statue 
        new(760, -1, -1, -1,269), // Flareon Statue 
        new(761, -1, -1, -1,270), // Porygon Statue 
        new(762, -1, -1, -1,271), // Omastar Statue 
        new(763, -1, -1, -1,272), // Snorlax Statue 
        new(764, -1, -1, -1,273), // Dragonite Statue 
        new(765, -1, -1, -1,274), // Chikorita Statue 
        new(766, -1, -1, -1,275), // Bayleef Statue 
        new(767, -1, -1, -1,276), // Meganium Statue 
        new(768, -1, -1, -1,277), // Cyndaquil Statue 
        new(769, -1, -1, -1,278), // Quilava Statue 
        new(770, -1, -1, -1,279), // Typhlosion Statue 
        new(771, -1, -1, -1,280), // Totodile Statue 
        new(772, -1, -1, -1,281), // Croconaw Statue 
        new(773, -1, -1, -1,282), // Feraligatr Statue 
        new(774, -1, -1, -1,283), // Furret Statue 
        new(775, -1, -1, -1,284), // Noctowl Statue 
        new(776, -1, -1, -1,285), // Crobat Statue 
        new(777, -1, -1, -1,286), // Togepi Statue 
        new(778, -1, -1, -1,287), // Ampharos Statue 
        new(779, -1, -1, -1,288), // Bellossom Statue 
        new(780, -1, -1, -1,289), // Marill Statue 
        new(781, -1, -1, -1,290), // Sudowoodo Statue 
        new(782, -1, -1, -1,291), // Aipom Statue 
        new(783, -1, -1, -1,292), // Sunkern Statue 
        new(784, -1, -1, -1,293), // Quagsire Statue 
        new(785, -1, -1, -1,294), // Espeon Statue 
        new(786, -1, -1, -1,295), // Umbreon Statue 
        new(787, -1, -1, -1,296), // Murkrow Statue 
        new(788, -1, -1, -1,297), // Unown Statue 
        new(789, -1, -1, -1,298), // Wobbuffet Statue 
        new(790, -1, -1, -1,299), // Girafarig Statue 
        new(791, -1, -1, -1,300), // Dunsparce Statue 
        new(792, -1, -1, -1,301), // Steelix Statue 
        new(793, -1, -1, -1,302), // Snubbull Statue 
        new(794, -1, -1, -1,303), // Scizor Statue 
        new(795, -1, -1, -1,304), // Heracross Statue 
        new(796, -1, -1, -1,305), // Magcargo Statue 
        new(797, -1, -1, -1,306), // Octillery Statue 
        new(798, -1, -1, -1,307), // Mantine Statue 
        new(799, -1, -1, -1,308), // Smeargle Statue 
        new(800, -1, -1, -1,309), // Miltank Statue 
        new(801, -1, -1, -1,310), // Tyranitar Statue 
        new(802, -1, -1, -1,311), // Treecko Statue 
        new(803, -1, -1, -1,312), // Grovyle Statue 
        new(804, -1, -1, -1,313), // Sceptile Statue 
        new(805, -1, -1, -1,314), // Torchic Statue 
        new(806, -1, -1, -1,315), // Combusken Statue 
        new(807, -1, -1, -1,316), // Blaziken Statue 
        new(808, -1, -1, -1,317), // Mudkip Statue 
        new(809, -1, -1, -1,318), // Marshtomp Statue 
        new(810, -1, -1, -1,319), // Swampert Statue 
        new(811, -1, -1, -1,320), // Beautifly Statue 
        new(812, -1, -1, -1,321), // Dustox Statue 
        new(813, -1, -1, -1,322), // Ludicolo Statue 
        new(814, -1, -1, -1,323), // Pelipper Statue 
        new(815, -1, -1, -1,324), // Gardevoir Statue 
        new(816, -1, -1, -1,325), // Shroomish Statue 
        new(817, -1, -1, -1,326), // Slaking Statue 
        new(818, -1, -1, -1,327), // Ninjask Statue 
        new(819, -1, -1, -1,328), // Exploud Statue 
        new(820, -1, -1, -1,329), // Skitty Statue 
        new(821, -1, -1, -1,330), // Sableye Statue 
        new(822, -1, -1, -1,331), // Mawile Statue 
        new(823, -1, -1, -1,332), // Aggron Statue 
        new(824, -1, -1, -1,333), // Medicham Statue 
        new(825, -1, -1, -1,334), // Manectric Statue 
        new(826, -1, -1, -1,335), // Plusle Statue 
        new(827, -1, -1, -1,336), // Minun Statue 
        new(828, -1, -1, -1,337), // Volbeat Statue 
        new(829, -1, -1, -1,338), // Illumise Statue 
        new(830, -1, -1, -1,339), // Roselia Statue 
        new(831, -1, -1, -1,340), // Sharpedo Statue 
        new(832, -1, -1, -1,341), // Wailmer Statue 
        new(833, -1, -1, -1,342), // Camerupt Statue 
        new(834, -1, -1, -1,343), // Torkoal Statue 
        new(835, -1, -1, -1,344), // Spinda Statue 
        new(836, -1, -1, -1,345), // Flygon Statue 
        new(837, -1, -1, -1,346), // Cacturne Statue 
        new(838, -1, -1, -1,347), // Altaria Statue 
        new(839, -1, -1, -1,348), // Zangoose Statue 
        new(840, -1, -1, -1,349), // Seviper Statue 
        new(841, -1, -1, -1,350), // Lunatone Statue 
        new(842, -1, -1, -1,351), // Solrock Statue 
        new(843, -1, -1, -1,352), // Whiscash Statue 
        new(844, -1, -1, -1,353), // Claydol Statue 
        new(845, -1, -1, -1,354), // Cradily Statue 
        new(846, -1, -1, -1,355), // Milotic Statue 
        new(847, -1, -1, -1,356), // Castform Statue 
        new(848, -1, -1, -1,357), // Kecleon Statue 
        new(849, -1, -1, -1,358), // Tropius Statue 
        new(850, -1, -1, -1,359), // Chimecho Statue 
        new(851, -1, -1, -1,360), // Absol Statue 
        new(852, -1, -1, -1,361), // Spheal Statue 
        new(853, -1, -1, -1,362), // Clamperl Statue 
        new(854, -1, -1, -1,363), // Relicanth Statue 
        new(855, -1, -1, -1,364), // Luvdisc Statue 
        new(856, -1, -1, -1,365), // Salamence Statue 
        new(857, -1, -1, -1,366), // Metagross Statue 
        new(858, -1, -1, -1,367), // Turtwig Statue 
        new(859, -1, -1, -1,368), // Grotle Statue 
        new(860, -1, -1, -1,369), // Torterra Statue 
        new(861, -1, -1, -1,370), // Chimchar Statue 
        new(862, -1, -1, -1,371), // Monferno Statue 
        new(863, -1, -1, -1,372), // Infernape Statue 
        new(864, -1, -1, -1,373), // Piplup Statue 
        new(865, -1, -1, -1,374), // Prinplup Statue 
        new(866, -1, -1, -1,375), // Empoleon Statue 
        new(867, -1, -1, -1,376), // Staraptor Statue 
        new(868, -1, -1, -1,377), // Bibarel Statue 
        new(869, -1, -1, -1,378), // Kricketot Statue 
        new(870, -1, -1, -1,379), // Luxray Statue 
        new(871, -1, -1, -1,380), // Budew Statue 
        new(872, -1, -1, -1,381), // Roserade Statue 
        new(873, -1, -1, -1,382), // Cranidos Statue 
        new(874, -1, -1, -1,383), // Bastiodon Statue 
        new(875, -1, -1, -1,384), // Burmy Statue 
        new(876, -1, -1, -1,385), // Wormadam Statue 
        new(877, -1, -1, -1,386), // Mothim Statue 
        new(878, -1, -1, -1,387), // Vespiquen Statue 
        new(879, -1, -1, -1,388), // Pachirisu Statue 
        new(880, -1, -1, -1,389), // Floatzel Statue 
        new(881, -1, -1, -1,390), // Cherubi Statue 
        new(882, -1, -1, -1,391), // Gastrodon Statue 
        new(883, -1, -1, -1,392), // Ambipom Statue 
        new(884, -1, -1, -1,393), // Drifloon Statue 
        new(885, -1, -1, -1,394), // Buneary Statue 
        new(886, -1, -1, -1,395), // Mismagius Statue 
        new(887, -1, -1, -1,396), // Honchkrow Statue 
        new(888, -1, -1, -1,397), // Purugly Statue 
        new(889, -1, -1, -1,398), // Skuntank Statue 
        new(890, -1, -1, -1,399), // Bronzong Statue 
        new(891, -1, -1, -1,400), // Happiny Statue 
        new(892, -1, -1, -1,401), // Chatot Statue 
        new(893, -1, -1, -1,402), // Spiritomb Statue 
        new(894, -1, -1, -1,403), // Garchomp Statue 
        new(895, -1, -1, -1,404), // Lucario Statue 
        new(896, -1, -1, -1,405), // Hippowdon Statue 
        new(897, -1, -1, -1,406), // Drapion Statue 
        new(898, -1, -1, -1,407), // Croagunk Statue 
        new(899, -1, -1, -1,408), // Carnivine Statue 
        new(900, -1, -1, -1,409), // Lumineon Statue 
        new(901, -1, -1, -1,410), // Abomasnow Statue 
        new(902, -1, -1, -1,411), // Weavile Statue 
        new(903, -1, -1, -1,412), // Magnezone Statue 
        new(904, -1, -1, -1,413), // Rhyperior Statue 
        new(905, -1, -1, -1,414), // Tangrowth Statue 
        new(906, -1, -1, -1,415), // Electivire Statue 
        new(907, -1, -1, -1,416), // Magmortar Statue 
        new(908, -1, -1, -1,417), // Togekiss Statue 
        new(909, -1, -1, -1,418), // Yanmega Statue 
        new(910, -1, -1, -1,419), // Leafeon Statue 
        new(911, -1, -1, -1,420), // Glaceon Statue 
        new(912, -1, -1, -1,421), // Gliscor Statue 
        new(913, -1, -1, -1,422), // Mamoswine Statue 
        new(914, -1, -1, -1,423), // Porygon-Z Statue 
        new(915, -1, -1, -1,424), // Gallade Statue 
        new(916, -1, -1, -1,425), // Probopass Statue 
        new(917, -1, -1, -1,426), // Dusknoir Statue 
        new(918, -1, -1, -1,427), // Froslass Statue 
        new(919, -1, -1, -1,428), // Rotom Statue 
        new(920, -1, -1, -1,429), // Rotom Statue 
        new(921, -1, -1, -1,430), // Rotom Statue 
        new(922, -1, -1, -1,431), // Rotom Statue 
        new(923, -1, -1, -1,432), // Rotom Statue 
        new(924, -1, -1, -1,433), // Rotom Statue 
        new(925,420, -1, -1, -1), // TM93
        new(926,421, -1, -1, -1), // TM94
        new(927,422, -1, -1, -1), // TM95
        new(928,423, -1, -1, -1), // TM96
        new(929,424, -1, -1, -1), // TM97
        new(930,425, -1, -1, -1), // TM98
        new(931,426, -1, -1, -1), // TM99
        new(932,427, -1, -1, -1), // TM100
        new(933, -1, -1,030, -1), // Square Pedestal XL
        new(934, -1, -1,031, -1), // Round Pedestal XL
        new(935, -1, -1,032, -1), // Sturdy Pedestal XL
        new(936, -1, -1,033, -1), // Clear Pedestal XL
        new(937, -1, -1,034, -1), // Spin Pedestal XL
        new(938, -1, -1,035, -1), // Spinback Pedestal XL
        new(939, -1, -1, -1,434), // Giratina Statue (Origin Form)
        new(940, -1, -1, -1, -1), // 
        new(941, -1, -1, -1, -1), // 
        new(942, -1, -1, -1, -1), // 
        new(943, -1, -1, -1, -1), // 
        new(944, -1, -1, -1, -1), // 
        new(945, -1, -1, -1, -1), // 
        new(946, -1, -1, -1, -1), // 
        new(947, -1, -1, -1, -1), // 
        new(948, -1, -1, -1, -1), // 
        new(949, -1, -1, -1, -1), // 
        new(950, -1, -1, -1, -1), // 
        new(951, -1, -1, -1, -1), // 
        new(952, -1, -1, -1, -1), // 
        new(953, -1, -1, -1, -1), // 
        new(954, -1, -1, -1, -1), // 
        new(955, -1, -1, -1, -1), // 
        new(956, -1, -1, -1, -1), // 
    ];
    #endregion
}
