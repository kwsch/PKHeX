using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Drawing.Text;

namespace PKHeX
{
    public partial class PKX
    {
        // C# PKX Function Library
        // No WinForm object related code, only to calculate information.
        // Relies on Util for some common operations.

        // Data
        public static uint LCRNG(uint seed)
        {
            uint a = 0x41C64E6D;
            uint c = 0x00006073;

            seed = (seed * a + c) & 0xFFFFFFFF;
            return seed;
        }
        public static DataTable MovePPTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Move", typeof(ushort));
            table.Columns.Add("PP", typeof(byte));
            table.Rows.Add(0, 0);
            table.Rows.Add(1, 35);
            table.Rows.Add(2, 25);
            table.Rows.Add(3, 10);
            table.Rows.Add(4, 15);
            table.Rows.Add(5, 20);
            table.Rows.Add(6, 20);
            table.Rows.Add(7, 15);
            table.Rows.Add(8, 15);
            table.Rows.Add(9, 15);
            table.Rows.Add(10, 35);
            table.Rows.Add(11, 30);
            table.Rows.Add(12, 5);
            table.Rows.Add(13, 10);
            table.Rows.Add(14, 20);
            table.Rows.Add(15, 30);
            table.Rows.Add(16, 35);
            table.Rows.Add(17, 35);
            table.Rows.Add(18, 20);
            table.Rows.Add(19, 15);
            table.Rows.Add(20, 20);
            table.Rows.Add(21, 20);
            table.Rows.Add(22, 25);
            table.Rows.Add(23, 20);
            table.Rows.Add(24, 30);
            table.Rows.Add(25, 5);
            table.Rows.Add(26, 10);
            table.Rows.Add(27, 15);
            table.Rows.Add(28, 15);
            table.Rows.Add(29, 15);
            table.Rows.Add(30, 25);
            table.Rows.Add(31, 20);
            table.Rows.Add(32, 5);
            table.Rows.Add(33, 35);
            table.Rows.Add(34, 15);
            table.Rows.Add(35, 20);
            table.Rows.Add(36, 20);
            table.Rows.Add(37, 10);
            table.Rows.Add(38, 15);
            table.Rows.Add(39, 30);
            table.Rows.Add(40, 35);
            table.Rows.Add(41, 20);
            table.Rows.Add(42, 20);
            table.Rows.Add(43, 30);
            table.Rows.Add(44, 25);
            table.Rows.Add(45, 40);
            table.Rows.Add(46, 20);
            table.Rows.Add(47, 15);
            table.Rows.Add(48, 20);
            table.Rows.Add(49, 20);
            table.Rows.Add(50, 20);
            table.Rows.Add(51, 30);
            table.Rows.Add(52, 25);
            table.Rows.Add(53, 15);
            table.Rows.Add(54, 30);
            table.Rows.Add(55, 25);
            table.Rows.Add(56, 5);
            table.Rows.Add(57, 15);
            table.Rows.Add(58, 10);
            table.Rows.Add(59, 5);
            table.Rows.Add(60, 20);
            table.Rows.Add(61, 20);
            table.Rows.Add(62, 20);
            table.Rows.Add(63, 5);
            table.Rows.Add(64, 35);
            table.Rows.Add(65, 20);
            table.Rows.Add(66, 25);
            table.Rows.Add(67, 20);
            table.Rows.Add(68, 20);
            table.Rows.Add(69, 20);
            table.Rows.Add(70, 15);
            table.Rows.Add(71, 25);
            table.Rows.Add(72, 15);
            table.Rows.Add(73, 10);
            table.Rows.Add(74, 20);
            table.Rows.Add(75, 25);
            table.Rows.Add(76, 10);
            table.Rows.Add(77, 35);
            table.Rows.Add(78, 30);
            table.Rows.Add(79, 15);
            table.Rows.Add(80, 10);
            table.Rows.Add(81, 40);
            table.Rows.Add(82, 10);
            table.Rows.Add(83, 15);
            table.Rows.Add(84, 30);
            table.Rows.Add(85, 15);
            table.Rows.Add(86, 20);
            table.Rows.Add(87, 10);
            table.Rows.Add(88, 15);
            table.Rows.Add(89, 10);
            table.Rows.Add(90, 5);
            table.Rows.Add(91, 10);
            table.Rows.Add(92, 10);
            table.Rows.Add(93, 25);
            table.Rows.Add(94, 10);
            table.Rows.Add(95, 20);
            table.Rows.Add(96, 40);
            table.Rows.Add(97, 30);
            table.Rows.Add(98, 30);
            table.Rows.Add(99, 20);
            table.Rows.Add(100, 20);
            table.Rows.Add(101, 15);
            table.Rows.Add(102, 10);
            table.Rows.Add(103, 40);
            table.Rows.Add(104, 15);
            table.Rows.Add(105, 10);
            table.Rows.Add(106, 30);
            table.Rows.Add(107, 10);
            table.Rows.Add(108, 20);
            table.Rows.Add(109, 10);
            table.Rows.Add(110, 40);
            table.Rows.Add(111, 40);
            table.Rows.Add(112, 20);
            table.Rows.Add(113, 30);
            table.Rows.Add(114, 30);
            table.Rows.Add(115, 20);
            table.Rows.Add(116, 30);
            table.Rows.Add(117, 10);
            table.Rows.Add(118, 10);
            table.Rows.Add(119, 20);
            table.Rows.Add(120, 5);
            table.Rows.Add(121, 10);
            table.Rows.Add(122, 30);
            table.Rows.Add(123, 20);
            table.Rows.Add(124, 20);
            table.Rows.Add(125, 20);
            table.Rows.Add(126, 5);
            table.Rows.Add(127, 15);
            table.Rows.Add(128, 10);
            table.Rows.Add(129, 20);
            table.Rows.Add(130, 10);
            table.Rows.Add(131, 15);
            table.Rows.Add(132, 35);
            table.Rows.Add(133, 20);
            table.Rows.Add(134, 15);
            table.Rows.Add(135, 10);
            table.Rows.Add(136, 10);
            table.Rows.Add(137, 30);
            table.Rows.Add(138, 15);
            table.Rows.Add(139, 40);
            table.Rows.Add(140, 20);
            table.Rows.Add(141, 15);
            table.Rows.Add(142, 10);
            table.Rows.Add(143, 5);
            table.Rows.Add(144, 10);
            table.Rows.Add(145, 30);
            table.Rows.Add(146, 10);
            table.Rows.Add(147, 15);
            table.Rows.Add(148, 20);
            table.Rows.Add(149, 15);
            table.Rows.Add(150, 40);
            table.Rows.Add(151, 20);
            table.Rows.Add(152, 10);
            table.Rows.Add(153, 5);
            table.Rows.Add(154, 15);
            table.Rows.Add(155, 10);
            table.Rows.Add(156, 10);
            table.Rows.Add(157, 10);
            table.Rows.Add(158, 15);
            table.Rows.Add(159, 30);
            table.Rows.Add(160, 30);
            table.Rows.Add(161, 10);
            table.Rows.Add(162, 10);
            table.Rows.Add(163, 20);
            table.Rows.Add(164, 10);
            table.Rows.Add(165, 1);
            table.Rows.Add(166, 1);
            table.Rows.Add(167, 10);
            table.Rows.Add(168, 25);
            table.Rows.Add(169, 10);
            table.Rows.Add(170, 5);
            table.Rows.Add(171, 15);
            table.Rows.Add(172, 25);
            table.Rows.Add(173, 15);
            table.Rows.Add(174, 10);
            table.Rows.Add(175, 15);
            table.Rows.Add(176, 30);
            table.Rows.Add(177, 5);
            table.Rows.Add(178, 40);
            table.Rows.Add(179, 15);
            table.Rows.Add(180, 10);
            table.Rows.Add(181, 25);
            table.Rows.Add(182, 10);
            table.Rows.Add(183, 30);
            table.Rows.Add(184, 10);
            table.Rows.Add(185, 20);
            table.Rows.Add(186, 10);
            table.Rows.Add(187, 10);
            table.Rows.Add(188, 10);
            table.Rows.Add(189, 10);
            table.Rows.Add(190, 10);
            table.Rows.Add(191, 20);
            table.Rows.Add(192, 5);
            table.Rows.Add(193, 40);
            table.Rows.Add(194, 5);
            table.Rows.Add(195, 5);
            table.Rows.Add(196, 15);
            table.Rows.Add(197, 5);
            table.Rows.Add(198, 10);
            table.Rows.Add(199, 5);
            table.Rows.Add(200, 10);
            table.Rows.Add(201, 10);
            table.Rows.Add(202, 10);
            table.Rows.Add(203, 10);
            table.Rows.Add(204, 20);
            table.Rows.Add(205, 20);
            table.Rows.Add(206, 40);
            table.Rows.Add(207, 15);
            table.Rows.Add(208, 10);
            table.Rows.Add(209, 20);
            table.Rows.Add(210, 20);
            table.Rows.Add(211, 25);
            table.Rows.Add(212, 5);
            table.Rows.Add(213, 15);
            table.Rows.Add(214, 10);
            table.Rows.Add(215, 5);
            table.Rows.Add(216, 20);
            table.Rows.Add(217, 15);
            table.Rows.Add(218, 20);
            table.Rows.Add(219, 25);
            table.Rows.Add(220, 20);
            table.Rows.Add(221, 5);
            table.Rows.Add(222, 30);
            table.Rows.Add(223, 5);
            table.Rows.Add(224, 10);
            table.Rows.Add(225, 20);
            table.Rows.Add(226, 40);
            table.Rows.Add(227, 5);
            table.Rows.Add(228, 20);
            table.Rows.Add(229, 40);
            table.Rows.Add(230, 20);
            table.Rows.Add(231, 15);
            table.Rows.Add(232, 35);
            table.Rows.Add(233, 10);
            table.Rows.Add(234, 5);
            table.Rows.Add(235, 5);
            table.Rows.Add(236, 5);
            table.Rows.Add(237, 15);
            table.Rows.Add(238, 5);
            table.Rows.Add(239, 20);
            table.Rows.Add(240, 5);
            table.Rows.Add(241, 5);
            table.Rows.Add(242, 15);
            table.Rows.Add(243, 20);
            table.Rows.Add(244, 10);
            table.Rows.Add(245, 5);
            table.Rows.Add(246, 5);
            table.Rows.Add(247, 15);
            table.Rows.Add(248, 10);
            table.Rows.Add(249, 15);
            table.Rows.Add(250, 15);
            table.Rows.Add(251, 10);
            table.Rows.Add(252, 10);
            table.Rows.Add(253, 10);
            table.Rows.Add(254, 20);
            table.Rows.Add(255, 10);
            table.Rows.Add(256, 10);
            table.Rows.Add(257, 10);
            table.Rows.Add(258, 10);
            table.Rows.Add(259, 15);
            table.Rows.Add(260, 15);
            table.Rows.Add(261, 15);
            table.Rows.Add(262, 10);
            table.Rows.Add(263, 20);
            table.Rows.Add(264, 20);
            table.Rows.Add(265, 10);
            table.Rows.Add(266, 20);
            table.Rows.Add(267, 20);
            table.Rows.Add(268, 20);
            table.Rows.Add(269, 20);
            table.Rows.Add(270, 20);
            table.Rows.Add(271, 10);
            table.Rows.Add(272, 10);
            table.Rows.Add(273, 10);
            table.Rows.Add(274, 20);
            table.Rows.Add(275, 20);
            table.Rows.Add(276, 5);
            table.Rows.Add(277, 15);
            table.Rows.Add(278, 10);
            table.Rows.Add(279, 10);
            table.Rows.Add(280, 15);
            table.Rows.Add(281, 10);
            table.Rows.Add(282, 20);
            table.Rows.Add(283, 5);
            table.Rows.Add(284, 5);
            table.Rows.Add(285, 10);
            table.Rows.Add(286, 10);
            table.Rows.Add(287, 20);
            table.Rows.Add(288, 5);
            table.Rows.Add(289, 10);
            table.Rows.Add(290, 20);
            table.Rows.Add(291, 10);
            table.Rows.Add(292, 20);
            table.Rows.Add(293, 20);
            table.Rows.Add(294, 20);
            table.Rows.Add(295, 5);
            table.Rows.Add(296, 5);
            table.Rows.Add(297, 15);
            table.Rows.Add(298, 20);
            table.Rows.Add(299, 10);
            table.Rows.Add(300, 15);
            table.Rows.Add(301, 20);
            table.Rows.Add(302, 15);
            table.Rows.Add(303, 10);
            table.Rows.Add(304, 10);
            table.Rows.Add(305, 15);
            table.Rows.Add(306, 10);
            table.Rows.Add(307, 5);
            table.Rows.Add(308, 5);
            table.Rows.Add(309, 10);
            table.Rows.Add(310, 15);
            table.Rows.Add(311, 10);
            table.Rows.Add(312, 5);
            table.Rows.Add(313, 20);
            table.Rows.Add(314, 25);
            table.Rows.Add(315, 5);
            table.Rows.Add(316, 40);
            table.Rows.Add(317, 15);
            table.Rows.Add(318, 5);
            table.Rows.Add(319, 40);
            table.Rows.Add(320, 15);
            table.Rows.Add(321, 20);
            table.Rows.Add(322, 20);
            table.Rows.Add(323, 5);
            table.Rows.Add(324, 15);
            table.Rows.Add(325, 20);
            table.Rows.Add(326, 20);
            table.Rows.Add(327, 15);
            table.Rows.Add(328, 15);
            table.Rows.Add(329, 5);
            table.Rows.Add(330, 10);
            table.Rows.Add(331, 30);
            table.Rows.Add(332, 20);
            table.Rows.Add(333, 30);
            table.Rows.Add(334, 15);
            table.Rows.Add(335, 5);
            table.Rows.Add(336, 40);
            table.Rows.Add(337, 15);
            table.Rows.Add(338, 5);
            table.Rows.Add(339, 20);
            table.Rows.Add(340, 5);
            table.Rows.Add(341, 15);
            table.Rows.Add(342, 25);
            table.Rows.Add(343, 25);
            table.Rows.Add(344, 15);
            table.Rows.Add(345, 20);
            table.Rows.Add(346, 15);
            table.Rows.Add(347, 20);
            table.Rows.Add(348, 15);
            table.Rows.Add(349, 20);
            table.Rows.Add(350, 10);
            table.Rows.Add(351, 20);
            table.Rows.Add(352, 20);
            table.Rows.Add(353, 5);
            table.Rows.Add(354, 5);
            table.Rows.Add(355, 10);
            table.Rows.Add(356, 5);
            table.Rows.Add(357, 40);
            table.Rows.Add(358, 10);
            table.Rows.Add(359, 10);
            table.Rows.Add(360, 5);
            table.Rows.Add(361, 10);
            table.Rows.Add(362, 10);
            table.Rows.Add(363, 15);
            table.Rows.Add(364, 10);
            table.Rows.Add(365, 20);
            table.Rows.Add(366, 15);
            table.Rows.Add(367, 30);
            table.Rows.Add(368, 10);
            table.Rows.Add(369, 20);
            table.Rows.Add(370, 5);
            table.Rows.Add(371, 10);
            table.Rows.Add(372, 10);
            table.Rows.Add(373, 15);
            table.Rows.Add(374, 10);
            table.Rows.Add(375, 10);
            table.Rows.Add(376, 5);
            table.Rows.Add(377, 15);
            table.Rows.Add(378, 5);
            table.Rows.Add(379, 10);
            table.Rows.Add(380, 10);
            table.Rows.Add(381, 30);
            table.Rows.Add(382, 20);
            table.Rows.Add(383, 20);
            table.Rows.Add(384, 10);
            table.Rows.Add(385, 10);
            table.Rows.Add(386, 5);
            table.Rows.Add(387, 5);
            table.Rows.Add(388, 10);
            table.Rows.Add(389, 5);
            table.Rows.Add(390, 20);
            table.Rows.Add(391, 10);
            table.Rows.Add(392, 20);
            table.Rows.Add(393, 10);
            table.Rows.Add(394, 15);
            table.Rows.Add(395, 10);
            table.Rows.Add(396, 20);
            table.Rows.Add(397, 20);
            table.Rows.Add(398, 20);
            table.Rows.Add(399, 15);
            table.Rows.Add(400, 15);
            table.Rows.Add(401, 10);
            table.Rows.Add(402, 15);
            table.Rows.Add(403, 15);
            table.Rows.Add(404, 15);
            table.Rows.Add(405, 10);
            table.Rows.Add(406, 10);
            table.Rows.Add(407, 10);
            table.Rows.Add(408, 20);
            table.Rows.Add(409, 10);
            table.Rows.Add(410, 30);
            table.Rows.Add(411, 5);
            table.Rows.Add(412, 10);
            table.Rows.Add(413, 15);
            table.Rows.Add(414, 10);
            table.Rows.Add(415, 10);
            table.Rows.Add(416, 5);
            table.Rows.Add(417, 20);
            table.Rows.Add(418, 30);
            table.Rows.Add(419, 10);
            table.Rows.Add(420, 30);
            table.Rows.Add(421, 15);
            table.Rows.Add(422, 15);
            table.Rows.Add(423, 15);
            table.Rows.Add(424, 15);
            table.Rows.Add(425, 30);
            table.Rows.Add(426, 10);
            table.Rows.Add(427, 20);
            table.Rows.Add(428, 15);
            table.Rows.Add(429, 10);
            table.Rows.Add(430, 10);
            table.Rows.Add(431, 20);
            table.Rows.Add(432, 15);
            table.Rows.Add(433, 5);
            table.Rows.Add(434, 5);
            table.Rows.Add(435, 15);
            table.Rows.Add(436, 15);
            table.Rows.Add(437, 5);
            table.Rows.Add(438, 10);
            table.Rows.Add(439, 5);
            table.Rows.Add(440, 20);
            table.Rows.Add(441, 5);
            table.Rows.Add(442, 15);
            table.Rows.Add(443, 20);
            table.Rows.Add(444, 5);
            table.Rows.Add(445, 20);
            table.Rows.Add(446, 20);
            table.Rows.Add(447, 20);
            table.Rows.Add(448, 20);
            table.Rows.Add(449, 10);
            table.Rows.Add(450, 20);
            table.Rows.Add(451, 10);
            table.Rows.Add(452, 15);
            table.Rows.Add(453, 20);
            table.Rows.Add(454, 15);
            table.Rows.Add(455, 10);
            table.Rows.Add(456, 10);
            table.Rows.Add(457, 5);
            table.Rows.Add(458, 10);
            table.Rows.Add(459, 5);
            table.Rows.Add(460, 5);
            table.Rows.Add(461, 10);
            table.Rows.Add(462, 5);
            table.Rows.Add(463, 5);
            table.Rows.Add(464, 10);
            table.Rows.Add(465, 5);
            table.Rows.Add(466, 5);
            table.Rows.Add(467, 5);
            table.Rows.Add(468, 15);
            table.Rows.Add(469, 10);
            table.Rows.Add(470, 10);
            table.Rows.Add(471, 10);
            table.Rows.Add(472, 10);
            table.Rows.Add(473, 10);
            table.Rows.Add(474, 10);
            table.Rows.Add(475, 15);
            table.Rows.Add(476, 20);
            table.Rows.Add(477, 15);
            table.Rows.Add(478, 10);
            table.Rows.Add(479, 15);
            table.Rows.Add(480, 10);
            table.Rows.Add(481, 15);
            table.Rows.Add(482, 10);
            table.Rows.Add(483, 20);
            table.Rows.Add(484, 10);
            table.Rows.Add(485, 15);
            table.Rows.Add(486, 10);
            table.Rows.Add(487, 20);
            table.Rows.Add(488, 20);
            table.Rows.Add(489, 20);
            table.Rows.Add(490, 20);
            table.Rows.Add(491, 20);
            table.Rows.Add(492, 15);
            table.Rows.Add(493, 15);
            table.Rows.Add(494, 15);
            table.Rows.Add(495, 15);
            table.Rows.Add(496, 15);
            table.Rows.Add(497, 15);
            table.Rows.Add(498, 20);
            table.Rows.Add(499, 15);
            table.Rows.Add(500, 10);
            table.Rows.Add(501, 15);
            table.Rows.Add(502, 15);
            table.Rows.Add(503, 15);
            table.Rows.Add(504, 15);
            table.Rows.Add(505, 10);
            table.Rows.Add(506, 10);
            table.Rows.Add(507, 10);
            table.Rows.Add(508, 10);
            table.Rows.Add(509, 10);
            table.Rows.Add(510, 15);
            table.Rows.Add(511, 15);
            table.Rows.Add(512, 15);
            table.Rows.Add(513, 15);
            table.Rows.Add(514, 5);
            table.Rows.Add(515, 5);
            table.Rows.Add(516, 15);
            table.Rows.Add(517, 5);
            table.Rows.Add(518, 10);
            table.Rows.Add(519, 10);
            table.Rows.Add(520, 10);
            table.Rows.Add(521, 20);
            table.Rows.Add(522, 20);
            table.Rows.Add(523, 20);
            table.Rows.Add(524, 10);
            table.Rows.Add(525, 10);
            table.Rows.Add(526, 30);
            table.Rows.Add(527, 15);
            table.Rows.Add(528, 15);
            table.Rows.Add(529, 10);
            table.Rows.Add(530, 15);
            table.Rows.Add(531, 25);
            table.Rows.Add(532, 10);
            table.Rows.Add(533, 15);
            table.Rows.Add(534, 10);
            table.Rows.Add(535, 10);
            table.Rows.Add(536, 10);
            table.Rows.Add(537, 20);
            table.Rows.Add(538, 10);
            table.Rows.Add(539, 10);
            table.Rows.Add(540, 10);
            table.Rows.Add(541, 10);
            table.Rows.Add(542, 10);
            table.Rows.Add(543, 15);
            table.Rows.Add(544, 15);
            table.Rows.Add(545, 5);
            table.Rows.Add(546, 5);
            table.Rows.Add(547, 10);
            table.Rows.Add(548, 10);
            table.Rows.Add(549, 10);
            table.Rows.Add(550, 5);
            table.Rows.Add(551, 5);
            table.Rows.Add(552, 10);
            table.Rows.Add(553, 5);
            table.Rows.Add(554, 5);
            table.Rows.Add(555, 15);
            table.Rows.Add(556, 10);
            table.Rows.Add(557, 5);
            table.Rows.Add(558, 5);
            table.Rows.Add(559, 5);
            table.Rows.Add(560, 10);
            table.Rows.Add(561, 10);
            table.Rows.Add(562, 10);
            table.Rows.Add(563, 10);
            table.Rows.Add(564, 20);
            table.Rows.Add(565, 25);
            table.Rows.Add(566, 10);
            table.Rows.Add(567, 20);
            table.Rows.Add(568, 30);
            table.Rows.Add(569, 25);
            table.Rows.Add(570, 20);
            table.Rows.Add(571, 20);
            table.Rows.Add(572, 15);
            table.Rows.Add(573, 20);
            table.Rows.Add(574, 15);
            table.Rows.Add(575, 20);
            table.Rows.Add(576, 20);
            table.Rows.Add(577, 10);
            table.Rows.Add(578, 10);
            table.Rows.Add(579, 10);
            table.Rows.Add(580, 10);
            table.Rows.Add(581, 10);
            table.Rows.Add(582, 20);
            table.Rows.Add(583, 10);
            table.Rows.Add(584, 30);
            table.Rows.Add(585, 15);
            table.Rows.Add(586, 10);
            table.Rows.Add(587, 10);
            table.Rows.Add(588, 10);
            table.Rows.Add(589, 20);
            table.Rows.Add(590, 20);
            table.Rows.Add(591, 5);
            table.Rows.Add(592, 5);
            table.Rows.Add(593, 5);
            table.Rows.Add(594, 20);
            table.Rows.Add(595, 10);
            table.Rows.Add(596, 10);
            table.Rows.Add(597, 20);
            table.Rows.Add(598, 15);
            table.Rows.Add(599, 20);
            table.Rows.Add(600, 20);
            table.Rows.Add(601, 10);
            table.Rows.Add(602, 20);
            table.Rows.Add(603, 30);
            table.Rows.Add(604, 10);
            table.Rows.Add(605, 10);
            table.Rows.Add(606, 40);
            table.Rows.Add(607, 40);
            table.Rows.Add(608, 30);
            table.Rows.Add(609, 20);
            table.Rows.Add(610, 40);
            table.Rows.Add(611, 20);
            table.Rows.Add(612, 20);
            table.Rows.Add(613, 10);
            table.Rows.Add(614, 10);
            table.Rows.Add(615, 10);
            table.Rows.Add(616, 10);
            table.Rows.Add(617, 5);
            table.Rows.Add(618, 10);
            table.Rows.Add(619, 10);
            table.Rows.Add(620, 5);
            table.Rows.Add(621, 5);

            return table;
        }
        public static DataTable ExpTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Level", typeof(int));

            table.Columns.Add("0 - Erratic", typeof(uint));
            table.Columns.Add("1 - Fast", typeof(uint));
            table.Columns.Add("2 - MF", typeof(uint));
            table.Columns.Add("3 - MS", typeof(uint));
            table.Columns.Add("4 - Slow", typeof(uint));
            table.Columns.Add("5 - Fluctuating", typeof(uint));
            table.Rows.Add(0, 0, 0, 0, 0, 0, 0);
            table.Rows.Add(1, 0, 0, 0, 0, 0, 0);
            table.Rows.Add(2, 15, 6, 8, 9, 10, 4);
            table.Rows.Add(3, 52, 21, 27, 57, 33, 13);
            table.Rows.Add(4, 122, 51, 64, 96, 80, 32);
            table.Rows.Add(5, 237, 100, 125, 135, 156, 65);
            table.Rows.Add(6, 406, 172, 216, 179, 270, 112);
            table.Rows.Add(7, 637, 274, 343, 236, 428, 178);
            table.Rows.Add(8, 942, 409, 512, 314, 640, 276);
            table.Rows.Add(9, 1326, 583, 729, 419, 911, 393);
            table.Rows.Add(10, 1800, 800, 1000, 560, 1250, 540);
            table.Rows.Add(11, 2369, 1064, 1331, 742, 1663, 745);
            table.Rows.Add(12, 3041, 1382, 1728, 973, 2160, 967);
            table.Rows.Add(13, 3822, 1757, 2197, 1261, 2746, 1230);
            table.Rows.Add(14, 4719, 2195, 2744, 1612, 3430, 1591);
            table.Rows.Add(15, 5737, 2700, 3375, 2035, 4218, 1957);
            table.Rows.Add(16, 6881, 3276, 4096, 2535, 5120, 2457);
            table.Rows.Add(17, 8155, 3930, 4913, 3120, 6141, 3046);
            table.Rows.Add(18, 9564, 4665, 5832, 3798, 7290, 3732);
            table.Rows.Add(19, 11111, 5487, 6859, 4575, 8573, 4526);
            table.Rows.Add(20, 12800, 6400, 8000, 5460, 10000, 5440);
            table.Rows.Add(21, 14632, 7408, 9261, 6458, 11576, 6482);
            table.Rows.Add(22, 16610, 8518, 10648, 7577, 13310, 7666);
            table.Rows.Add(23, 18737, 9733, 12167, 8825, 15208, 9003);
            table.Rows.Add(24, 21012, 11059, 13824, 10208, 17280, 10506);
            table.Rows.Add(25, 23437, 12500, 15625, 11735, 19531, 12187);
            table.Rows.Add(26, 26012, 14060, 17576, 13411, 21970, 14060);
            table.Rows.Add(27, 28737, 15746, 19683, 15244, 24603, 16140);
            table.Rows.Add(28, 31610, 17561, 21952, 17242, 27440, 18439);
            table.Rows.Add(29, 34632, 19511, 24389, 19411, 30486, 20974);
            table.Rows.Add(30, 37800, 21600, 27000, 21760, 33750, 23760);
            table.Rows.Add(31, 41111, 23832, 29791, 24294, 37238, 26811);
            table.Rows.Add(32, 44564, 26214, 32768, 27021, 40960, 30146);
            table.Rows.Add(33, 48155, 28749, 35937, 29949, 44921, 33780);
            table.Rows.Add(34, 51881, 31443, 39304, 33084, 49130, 37731);
            table.Rows.Add(35, 55737, 34300, 42875, 36435, 53593, 42017);
            table.Rows.Add(36, 59719, 37324, 46656, 40007, 58320, 46656);
            table.Rows.Add(37, 63822, 40522, 50653, 43808, 63316, 50653);
            table.Rows.Add(38, 68041, 43897, 54872, 47846, 68590, 55969);
            table.Rows.Add(39, 72369, 47455, 59319, 52127, 74148, 60505);
            table.Rows.Add(40, 76800, 51200, 64000, 56660, 80000, 66560);
            table.Rows.Add(41, 81326, 55136, 68921, 61450, 86151, 71677);
            table.Rows.Add(42, 85942, 59270, 74088, 66505, 92610, 78533);
            table.Rows.Add(43, 90637, 63605, 79507, 71833, 99383, 84277);
            table.Rows.Add(44, 95406, 68147, 85184, 77440, 106480, 91998);
            table.Rows.Add(45, 100237, 72900, 91125, 83335, 113906, 98415);
            table.Rows.Add(46, 105122, 77868, 97336, 89523, 121670, 107069);
            table.Rows.Add(47, 110052, 83058, 103823, 96012, 129778, 114205);
            table.Rows.Add(48, 115015, 88473, 110592, 102810, 138240, 123863);
            table.Rows.Add(49, 120001, 94119, 117649, 109923, 147061, 131766);
            table.Rows.Add(50, 125000, 100000, 125000, 117360, 156250, 142500);
            table.Rows.Add(51, 131324, 106120, 132651, 125126, 165813, 151222);
            table.Rows.Add(52, 137795, 112486, 140608, 133229, 175760, 163105);
            table.Rows.Add(53, 144410, 119101, 148877, 141677, 186096, 172697);
            table.Rows.Add(54, 151165, 125971, 157464, 150476, 196830, 185807);
            table.Rows.Add(55, 158056, 133100, 166375, 159635, 207968, 196322);
            table.Rows.Add(56, 165079, 140492, 175616, 169159, 219520, 210739);
            table.Rows.Add(57, 172229, 148154, 185193, 179056, 231491, 222231);
            table.Rows.Add(58, 179503, 156089, 195112, 189334, 243890, 238036);
            table.Rows.Add(59, 186894, 164303, 205379, 199999, 256723, 250562);
            table.Rows.Add(60, 194400, 172800, 216000, 211060, 270000, 267840);
            table.Rows.Add(61, 202013, 181584, 226981, 222522, 283726, 281456);
            table.Rows.Add(62, 209728, 190662, 238328, 234393, 297910, 300293);
            table.Rows.Add(63, 217540, 200037, 250047, 246681, 312558, 315059);
            table.Rows.Add(64, 225443, 209715, 262144, 259392, 327680, 335544);
            table.Rows.Add(65, 233431, 219700, 274625, 272535, 343281, 351520);
            table.Rows.Add(66, 241496, 229996, 287496, 286115, 359370, 373744);
            table.Rows.Add(67, 249633, 240610, 300763, 300140, 375953, 390991);
            table.Rows.Add(68, 257834, 251545, 314432, 314618, 393040, 415050);
            table.Rows.Add(69, 267406, 262807, 328509, 329555, 410636, 433631);
            table.Rows.Add(70, 276458, 274400, 343000, 344960, 428750, 459620);
            table.Rows.Add(71, 286328, 286328, 357911, 360838, 447388, 479600);
            table.Rows.Add(72, 296358, 298598, 373248, 377197, 466560, 507617);
            table.Rows.Add(73, 305767, 311213, 389017, 394045, 486271, 529063);
            table.Rows.Add(74, 316074, 324179, 405224, 411388, 506530, 559209);
            table.Rows.Add(75, 326531, 337500, 421875, 429235, 527343, 582187);
            table.Rows.Add(76, 336255, 351180, 438976, 447591, 548720, 614566);
            table.Rows.Add(77, 346965, 365226, 456533, 466464, 570666, 639146);
            table.Rows.Add(78, 357812, 379641, 474552, 485862, 593190, 673863);
            table.Rows.Add(79, 367807, 394431, 493039, 505791, 616298, 700115);
            table.Rows.Add(80, 378880, 409600, 512000, 526260, 640000, 737280);
            table.Rows.Add(81, 390077, 425152, 531441, 547274, 664301, 765275);
            table.Rows.Add(82, 400293, 441094, 551368, 568841, 689210, 804997);
            table.Rows.Add(83, 411686, 457429, 571787, 590969, 714733, 834809);
            table.Rows.Add(84, 423190, 474163, 592704, 613664, 740880, 877201);
            table.Rows.Add(85, 433572, 491300, 614125, 636935, 767656, 908905);
            table.Rows.Add(86, 445239, 508844, 636056, 660787, 795070, 954084);
            table.Rows.Add(87, 457001, 526802, 658503, 685228, 823128, 987754);
            table.Rows.Add(88, 467489, 545177, 681472, 710266, 851840, 1035837);
            table.Rows.Add(89, 479378, 563975, 704969, 735907, 881211, 1071552);
            table.Rows.Add(90, 491346, 583200, 729000, 762160, 911250, 1122660);
            table.Rows.Add(91, 501878, 602856, 753571, 789030, 941963, 1160499);
            table.Rows.Add(92, 513934, 622950, 778688, 816525, 973360, 1214753);
            table.Rows.Add(93, 526049, 643485, 804357, 844653, 1005446, 1254796);
            table.Rows.Add(94, 536557, 664467, 830584, 873420, 1038230, 1312322);
            table.Rows.Add(95, 548720, 685900, 857375, 902835, 1071718, 1354652);
            table.Rows.Add(96, 560922, 707788, 884736, 932903, 1105920, 1415577);
            table.Rows.Add(97, 571333, 730138, 912673, 963632, 1140841, 1460276);
            table.Rows.Add(98, 583539, 752953, 941192, 995030, 1176490, 1524731);
            table.Rows.Add(99, 591882, 776239, 970299, 1027103, 1212873, 1571884);
            table.Rows.Add(100, 600000, 800000, 1000000, 1059860, 1250000, 1640000);
            return table;
        }

        public static PersonalParser PersonalGetter = new PersonalParser();

        // Stat Fetching
        public static int getMovePP(int move, int ppup)
        {
            return (getBasePP(move) * (5 + ppup) / 5);
        }
        public static int getBasePP(int move)
        {
            if (move < 0) { move = 0; }
            DataTable movepptable = MovePPTable();
            return (byte)movepptable.Rows[move][1];
        }
        public static byte[] getRandomEVs()
        {
            byte[] evs = new byte[6];
          start:
            evs[0] = (byte)Math.Min(Util.rnd32() % 300, 252); // bias two to get maybe 252
            evs[1] = (byte)Math.Min(Util.rnd32() % 300, 252);
            evs[2] = (byte)Math.Min(((Util.rnd32()) % (510 - evs[0] - evs[1])), 252);
            evs[3] = (byte)Math.Min(((Util.rnd32()) % (510 - evs[0] - evs[1] - evs[2])), 252);
            evs[4] = (byte)Math.Min(((Util.rnd32()) % (510 - evs[0] - evs[1] - evs[2] - evs[3])), 252);
            evs[5] = (byte)Math.Min((510 - evs[0] - evs[1] - evs[2] - evs[3] - evs[4]), 252);
            Util.Shuffle(evs);
            if (evs.Sum(b => (ushort)b) > 510) goto start; // try again!
            return evs;
        }
        public static byte getBaseFriendship(int species)
        {
            PersonalParser.Personal Mon = PersonalGetter.GetPersonal(species);
            return Mon.BaseFriendship;
        }
        public static int getLevel(int species, ref uint exp)
        {
            if (exp == 0) { return 1; }
            int tl = 1; // Initial Level

            PersonalParser.Personal MonData = PersonalGetter.GetPersonal(species);
            DataTable table = PKX.ExpTable();

            int growth = MonData.EXPGrowth;

            if ((uint)table.Rows[tl][growth + 1] < exp)
            {
                while ((uint)table.Rows[tl][growth + 1] < exp)
                {
                    // While EXP for guessed level is below our current exp
                    tl += 1;
                    if (tl == 100)
                    {
                        exp = getEXP(100, species);
                        return tl;
                    }
                    // when calcexp exceeds our exp, we exit loop
                }
                if ((uint)table.Rows[tl][growth + 1] == exp) // Matches level threshold
                    return tl;
                else return (tl - 1);
            }
            else return tl;
        }
        public static uint getEXP(int level, int species)
        {
            // Fetch Growth
            if ((level == 0) || (level == 1))
                return 0;
            if (level > 100) level = 100;

            PersonalParser.Personal MonData = PersonalGetter.GetPersonal(species);
            int growth = MonData.EXPGrowth;

            uint exp = (uint)PKX.ExpTable().Rows[level][growth + 1];
            return exp;
        }
        public static int[] getAbilities(int species, int formnum)
        {
            if (formnum > 0) // For species with form-specific abilities.
            {
                // Previous Games
                if (species == 492 && formnum == 1) { species = 727; } // Shaymin Sky
                else if (species == 487 && formnum == 1) { species = 728; } // Giratina-O
                else if (species == 550 && formnum == 1) { species = 738; } // Basculin Blue
                else if (species == 646 && formnum == 1) { species = 741; } // Kyurem White
                else if (species == 646 && formnum == 2) { species = 742; } // Kyurem Black
                else if (species == 641 && formnum == 1) { species = 744; } // Tornadus-T
                else if (species == 642 && formnum == 1) { species = 745; } // Thundurus-T
                else if (species == 645 && formnum == 1) { species = 746; } // Landorus-T

                // XY
                else if (species == 678 && formnum == 1) { species = 748; } // Meowstic Female
                else if (species == 094 && formnum == 1) { species = 747; } // Mega Gengar
                else if (species == 282 && formnum == 1) { species = 758; } // Mega Gardevoir
                else if (species == 181 && formnum == 1) { species = 759; } // Mega Ampharos
                else if (species == 003 && formnum == 1) { species = 760; } // Mega Venusaur
                else if (species == 006 && formnum == 1) { species = 761; } // Mega Charizard X
                else if (species == 006 && formnum == 2) { species = 762; } // Mega Charizard Y
                else if (species == 150 && formnum == 1) { species = 763; } // Mega MewtwoX
                else if (species == 150 && formnum == 2) { species = 764; } // Mega MewtwoY
                else if (species == 257 && formnum == 1) { species = 765; } // Mega Blaziken
                else if (species == 308 && formnum == 1) { species = 766; } // Mega Medicham
                else if (species == 229 && formnum == 1) { species = 767; } // Mega Houndoom
                else if (species == 306 && formnum == 1) { species = 768; } // Mega Aggron
                else if (species == 354 && formnum == 1) { species = 769; } // Mega Banette
                else if (species == 248 && formnum == 1) { species = 770; } // Mega Tyranitar
                else if (species == 212 && formnum == 1) { species = 771; } // Mega Scizor
                else if (species == 127 && formnum == 1) { species = 772; } // Mega Pinsir
                else if (species == 142 && formnum == 1) { species = 773; } // Mega Aerodactyl
                else if (species == 448 && formnum == 1) { species = 774; } // Mega Lucario
                else if (species == 460 && formnum == 1) { species = 775; } // Mega Abomasnow
                else if (species == 009 && formnum == 1) { species = 777; } // Mega Blastoise
                else if (species == 115 && formnum == 1) { species = 778; } // Mega Kangaskhan
                else if (species == 130 && formnum == 1) { species = 779; } // Mega Gyarados
                else if (species == 359 && formnum == 1) { species = 780; } // Mega Absol
                else if (species == 065 && formnum == 1) { species = 781; } // Mega Alakazam
                else if (species == 214 && formnum == 1) { species = 782; } // Mega Heracross
                else if (species == 303 && formnum == 1) { species = 783; } // Mega Mawile
                else if (species == 310 && formnum == 1) { species = 784; } // Mega Manectric
                else if (species == 445 && formnum == 1) { species = 785; } // Mega Garchomp
                else if (species == 381 && formnum == 1) { species = 786; } // Mega Latios
                else if (species == 380 && formnum == 1) { species = 787; } // Mega Latias

                // ORAS
                else if (species == 382 && formnum == 1) { species = 812; } // Primal Kyogre
                else if (species == 383 && formnum == 1) { species = 813; } // Primal Groudon
                else if (species == 720 && formnum == 1) { species = 821; } // Hoopa Unbound
                else if (species == 015 && formnum == 1) { species = 825; } // Mega Beedrill
                else if (species == 018 && formnum == 1) { species = 808; } // Mega Pidgeot
                else if (species == 080 && formnum == 1) { species = 806; } // Mega Slowbro
                else if (species == 208 && formnum == 1) { species = 807; } // Mega Steelix
                else if (species == 254 && formnum == 1) { species = 800; } // Mega Sceptile
                else if (species == 260 && formnum == 1) { species = 799; } // Mega Swampert
                else if (species == 302 && formnum == 1) { species = 801; } // Mega Sableye
                else if (species == 319 && formnum == 1) { species = 805; } // Mega Sharpedo
                else if (species == 323 && formnum == 1) { species = 822; } // Mega Camerupt
                else if (species == 334 && formnum == 1) { species = 802; } // Mega Altaria
                else if (species == 362 && formnum == 1) { species = 809; } // Mega Glalie
                else if (species == 373 && formnum == 1) { species = 824; } // Mega Salamence
                else if (species == 376 && formnum == 1) { species = 811; } // Mega Metagross
                else if (species == 384 && formnum == 1) { species = 814; } // Mega Rayquaza
                else if (species == 428 && formnum == 1) { species = 823; } // Mega Lopunny
                else if (species == 475 && formnum == 1) { species = 803; } // Mega Gallade
                else if (species == 531 && formnum == 1) { species = 804; } // Mega Audino
                else if (species == 719 && formnum == 1) { species = 810; } // Mega Diancie
            }

            int[] abils = { 0, 0, 0 };
            Array.Copy(speciesability, species * 4 + 1, abils, 0, 3);
            return abils;
        }
        public static int getGender(string s)
        {
            if (s == "♂" || s == "M")
                return 0;
            else if (s == "♀" || s == "F")
                return 1;
            else return 2;
        }
        public static ushort[] getStats(int species, int level, int nature, int form,
                                        int HP_EV, int ATK_EV, int DEF_EV, int SPA_EV, int SPD_EV, int SPE_EV,
                                        int HP_IV, int ATK_IV, int DEF_IV, int SPA_IV, int SPD_IV, int SPE_IV)
        {
            PersonalParser.Personal MonData = PersonalGetter.GetPersonal(species, form);
            int HP_B = MonData.BaseStats[0];
            int ATK_B = MonData.BaseStats[1];
            int DEF_B = MonData.BaseStats[2];
            int SPE_B = MonData.BaseStats[3];
            int SPA_B = MonData.BaseStats[4];
            int SPD_B = MonData.BaseStats[5];

            // Calculate Stats
            ushort[] stats = new ushort[6]; // Stats are stored as ushorts in the PKX structure. We'll cap them as such.
            stats[0] = (ushort)((((HP_IV + (2 * HP_B) + (HP_EV / 4) + 100) * level) / 100) + 10);
            stats[1] = (ushort)((((ATK_IV + (2 * ATK_B) + (ATK_EV / 4)) * level) / 100) + 5);
            stats[2] = (ushort)((((DEF_IV + (2 * DEF_B) + (DEF_EV / 4)) * level) / 100) + 5);
            stats[4] = (ushort)((((SPA_IV + (2 * SPA_B) + (SPA_EV / 4)) * level) / 100) + 5);
            stats[5] = (ushort)((((SPD_IV + (2 * SPD_B) + (SPD_EV / 4)) * level) / 100) + 5);
            stats[3] = (ushort)((((SPE_IV + (2 * SPE_B) + (SPE_EV / 4)) * level) / 100) + 5);

            // Account for nature
            int incr = nature / 5 + 1;
            int decr = nature % 5 + 1;
            if (incr != decr)
            {
                stats[incr] *= 11; stats[incr] /= 10;
                stats[decr] *= 9; stats[decr] /= 10;
            }

            // Return Result
            return stats;
        }

        // Manipulation
        public static byte[] shuffleArray(byte[] pkx, uint sv)
        {
            byte[] ekx = new byte[260];
            Array.Copy(pkx, ekx, 8);

            // Now to shuffle the blocks

            // Define Shuffle Order Structure
            byte[] aloc = new byte[] { 0, 0, 0, 0, 0, 0, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3 };
            byte[] bloc = new byte[] { 1, 1, 2, 3, 2, 3, 0, 0, 0, 0, 0, 0, 2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2 };
            byte[] cloc = new byte[] { 2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2, 0, 0, 0, 0, 0, 0, 3, 2, 3, 2, 1, 1 };
            byte[] dloc = new byte[] { 3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 0, 0, 0, 0, 0, 0 };

            // Get Shuffle Order
            byte[] shlog = new byte[] { aloc[sv], bloc[sv], cloc[sv], dloc[sv] };

            // UnShuffle Away!
            for (int b = 0; b < 4; b++)
                Array.Copy(pkx, 8 + 56 * shlog[b], ekx, 8 + 56 * b, 56);

            // Fill the Battle Stats back
            if (pkx.Length > 232)
                Array.Copy(pkx, 232, ekx, 232, 28);

            return ekx;
        }
        public static byte[] decryptArray(byte[] ekx)
        {
            byte[] pkx = new byte[0x104];
            Array.Copy(ekx, pkx, ekx.Length);
            uint pv = BitConverter.ToUInt32(pkx, 0);
            uint sv = (((pv & 0x3E000) >> 0xD) % 24);

            uint seed = pv;

            // Decrypt Blocks with RNG Seed
            for (int i = 8; i < 232; i += 2)
            {
                int pre = pkx[i] + ((pkx[i + 1]) << 8);
                seed = PKX.LCRNG(seed);
                int seedxor = (int)((seed) >> 16);
                int post = (pre ^ seedxor);
                pkx[i] = (byte)((post) & 0xFF);
                pkx[i + 1] = (byte)(((post) >> 8) & 0xFF);
            }

            // Deshuffle
            pkx = shuffleArray(pkx, sv);

            // Decrypt the Party Stats
            seed = pv;
            for (int i = 232; i < 260; i += 2)
            {
                int pre = pkx[i] + ((pkx[i + 1]) << 8);
                seed = PKX.LCRNG(seed);
                int seedxor = (int)((seed) >> 16);
                int post = (pre ^ seedxor);
                pkx[i] = (byte)((post) & 0xFF);
                pkx[i + 1] = (byte)(((post) >> 8) & 0xFF);
            }

            return pkx;
        }
        public static byte[] encryptArray(byte[] pkx)
        {
            // Shuffle
            uint pv = BitConverter.ToUInt32(pkx, 0);
            uint sv = (((pv & 0x3E000) >> 0xD) % 24);

            byte[] ekxdata = new byte[pkx.Length];
            Array.Copy(pkx, ekxdata, pkx.Length);

            // If I unshuffle 11 times, the 12th (decryption) will always decrypt to ABCD.
            // 2 x 3 x 4 = 12 (possible unshuffle loops -> total iterations)
            for (int i = 0; i < 11; i++)
                ekxdata = shuffleArray(ekxdata, sv);

            uint seed = pv;
            // Encrypt Blocks with RNG Seed
            for (int i = 8; i < 232; i += 2)
            {
                int pre = ekxdata[i] + ((ekxdata[i + 1]) << 8);
                seed = PKX.LCRNG(seed);
                int seedxor = (int)((seed) >> 16);
                int post = (pre ^ seedxor);
                ekxdata[i] = (byte)((post) & 0xFF);
                ekxdata[i + 1] = (byte)(((post) >> 8) & 0xFF);
            }

            // Encrypt the Party Stats
            seed = pv;
            for (int i = 232; i < 260; i += 2)
            {
                int pre = ekxdata[i] + ((ekxdata[i + 1]) << 8);
                seed = PKX.LCRNG(seed);
                int seedxor = (int)((seed) >> 16);
                int post = (pre ^ seedxor);
                ekxdata[i] = (byte)((post) & 0xFF);
                ekxdata[i + 1] = (byte)(((post) >> 8) & 0xFF);
            }

            // Done
            return ekxdata;
        }
        public static ushort getCHK(byte[] data)
        {
            ushort chk = 0;
            for (int i = 8; i < 232; i += 2) // Loop through the entire PKX
                chk += BitConverter.ToUInt16(data, i);

            return chk;
        }
        public static bool verifychk(byte[] input)
        {
            ushort checksum = 0;
            if (input.Length == 100 || input.Length == 80)  // Gen 3 Files
            {
                for (int i = 32; i < 80; i += 2)
                    checksum += BitConverter.ToUInt16(input, i);

                return (checksum == BitConverter.ToUInt16(input, 28));
            }
            else
            {
                if (input.Length == 236 || input.Length == 220 || input.Length == 136) // Gen 4/5
                    Array.Resize(ref input, 136);                   
                else if (input.Length == 232 || input.Length == 260) // Gen 6
                    Array.Resize(ref input, 232);                    
                else throw new Exception("Wrong sized input array to verifychecksum");

                ushort chk = 0;
                for (int i = 8; i < input.Length; i += 2)
                    chk += BitConverter.ToUInt16(input, i);

                return (chk == BitConverter.ToUInt16(input, 0x6));
            }
        }
        public static UInt16 getPSV(UInt32 PID)
        {
            return Convert.ToUInt16(((PID >> 16) ^ (PID & 0xFFFF)) >> 4);
        }
        public static UInt16 getTSV(UInt16 TID, UInt16 SID)
        {
            return Convert.ToUInt16((TID ^ SID) >> 4);
        }
        public static uint getRandomPID(int species, int cg)
        {
            PersonalParser.Personal MonData = PersonalGetter.GetPersonal(species);
            int gt = MonData.GenderRatio;
            uint pid = (uint)Util.rnd32();
            if (gt == 255) //Genderless
                return pid;
            if (gt == 254) //Female Only
                gt++;
            while (true) // Loop until we find a suitable PID
            {
                uint gv = (pid & 0xFF);
                if (cg == 2) // Genderless
                    break;  // PID Passes
                else if ((cg == 1) && (gv <= gt)) // Female
                    break;  // PID Passes
                else if ((cg == 0) && (gv > gt))
                    break;  // PID Passes
                pid = (uint)Util.rnd32();
            }
            return pid;
        }

        // Object
        #region PKX Object
        private Image pksprite;
        private uint mEC, mPID, mIV32,

            mexp,
            mHP_EV, mATK_EV, mDEF_EV, mSPA_EV, mSPD_EV, mSPE_EV,
            mHP_IV, mATK_IV, mDEF_IV, mSPE_IV, mSPA_IV, mSPD_IV,
            mcnt_cool, mcnt_beauty, mcnt_cute, mcnt_smart, mcnt_tough, mcnt_sheen,
            mmarkings, mhptype;

        private string
            mnicknamestr, mgenderstring, mnotOT, mot, mSpeciesName, mNatureName, mHPName, mAbilityName,
            mMove1N, mMove2N, mMove3N, mMove4N;

        private int
            mability, mabilitynum, mnature, mfeflag, mgenderflag, maltforms, mPKRS_Strain, mPKRS_Duration,
            mmetlevel, motgender;

        private bool
            misegg, misnick, misshiny;

        private ushort
            mspecies, mhelditem, mTID, mSID, mTSV, mESV,
            mmove1, mmove2, mmove3, mmove4,
            mmove1_pp, mmove2_pp, mmove3_pp, mmove4_pp,
            mmove1_ppu, mmove2_ppu, mmove3_ppu, mmove4_ppu,
            meggmove1, meggmove2, meggmove3, meggmove4,
            mchk,

            mOTfriendship, mOTaffection,
            megg_year, megg_month, megg_day,
            mmet_year, mmet_month, mmet_day,
            meggloc, mmetloc,
            mball, mencountertype,
            mgamevers, mcountryID, mregionID, mdsregID, motlang;

        public Image pkimg { get { return pksprite; } }
        public string Nickname { get { return mnicknamestr; } }
        public string Species { get { return mSpeciesName; } }
        public string Nature { get { return mNatureName; } }
        public string Gender { get { return mgenderstring; } }
        public string ESV { get { return mESV.ToString("0000"); } }
        public string HP_Type { get { return mHPName; } }
        public string Ability { get { return mAbilityName; } }
        public string Move1 { get { return mMove1N; } }
        public string Move2 { get { return mMove2N; } }
        public string Move3 { get { return mMove3N; } }
        public string Move4 { get { return mMove4N; } }

        #region Extraneous
        public string EC { get { return mEC.ToString("X8"); } }
        public string PID { get { return mPID.ToString("X8"); } }
        public uint HP_IV { get { return mHP_IV; } }
        public uint ATK_IV { get { return mATK_IV; } }
        public uint DEF_IV { get { return mDEF_IV; } }
        public uint SPA_IV { get { return mSPA_IV; } }
        public uint SPD_IV { get { return mSPD_IV; } }
        public uint SPE_IV { get { return mSPE_IV; } }
        public uint EXP { get { return mexp; } }
        public uint HP_EV { get { return mHP_EV; } }
        public uint ATK_EV { get { return mATK_EV; } }
        public uint DEF_EV { get { return mDEF_EV; } }
        public uint SPA_EV { get { return mSPA_EV; } }
        public uint SPD_EV { get { return mSPD_EV; } }
        public uint SPE_EV { get { return mSPE_EV; } }
        public uint Cool { get { return mcnt_cool; } }
        public uint Beauty { get { return mcnt_beauty; } }
        public uint Cute { get { return mcnt_cute; } }
        public uint Smart { get { return mcnt_smart; } }
        public uint Tough { get { return mcnt_tough; } }
        public uint Sheen { get { return mcnt_sheen; } }
        public uint Markings { get { return mmarkings; } }

        public string NotOT { get { return mnotOT; } }
        public string OT { get { return mot; } }

        public int AbilityNum { get { return mabilitynum; } }
        public int FatefulFlag { get { return mfeflag; } }
        public int GenderFlag { get { return mgenderflag; } }
        public int AltForms { get { return maltforms; } }
        public int PKRS_Strain { get { return mPKRS_Strain; } }
        public int PKRS_Days { get { return mPKRS_Duration; } }
        public int MetLevel { get { return mmetlevel; } }
        public int OT_Gender { get { return motgender; } }

        public bool IsEgg { get { return misegg; } }
        public bool IsNicknamed { get { return misnick; } }
        public bool IsShiny { get { return misshiny; } }

        public ushort HeldItem { get { return mhelditem; } }
        public ushort TID { get { return mTID; } }
        public ushort SID { get { return mSID; } }
        public ushort TSV { get { return mTSV; } }
        public ushort Move1_PP { get { return mmove1_pp; } }
        public ushort Move2_PP { get { return mmove2_pp; } }
        public ushort Move3_PP { get { return mmove3_pp; } }
        public ushort Move4_PP { get { return mmove4_pp; } }
        public ushort Move1_PPUp { get { return mmove1_ppu; } }
        public ushort Move2_PPUp { get { return mmove2_ppu; } }
        public ushort Move3_PPUp { get { return mmove3_ppu; } }
        public ushort Move4_PPUp { get { return mmove4_ppu; } }
        public ushort EggMove1 { get { return meggmove1; } }
        public ushort EggMove2 { get { return meggmove2; } }
        public ushort EggMove3 { get { return meggmove3; } }
        public ushort EggMove4 { get { return meggmove4; } }
        public ushort Checksum { get { return mchk; } }
        public ushort mFriendship { get { return mOTfriendship; } }
        public ushort OT_Affection { get { return mOTaffection; } }
        public ushort Egg_Year { get { return megg_year; } }
        public ushort Egg_Day { get { return megg_month; } }
        public ushort Egg_Month { get { return megg_day; } }
        public ushort Met_Year { get { return mmet_year; } }
        public ushort Met_Day { get { return mmet_month; } }
        public ushort Met_Month { get { return mmet_day; } }
        public ushort Egg_Location { get { return meggloc; } }
        public ushort Met_Location { get { return mmetloc; } }
        public ushort Ball { get { return mball; } }
        public ushort Encounter { get { return mencountertype; } }
        public ushort GameVersion { get { return mgamevers; } }
        public ushort CountryID { get { return mcountryID; } }
        public ushort RegionID { get { return mregionID; } }
        public ushort DSRegionID { get { return mdsregID; } }
        public ushort OTLang { get { return motlang; } }

        #endregion
        public PKX(byte[] pkx)
        {
            mnicknamestr = "";
            mnotOT = "";
            mot = "";
            mEC = BitConverter.ToUInt32(pkx, 0);
            mchk = BitConverter.ToUInt16(pkx, 6);
            mspecies = BitConverter.ToUInt16(pkx, 0x08);
            mhelditem = BitConverter.ToUInt16(pkx, 0x0A);
            mTID = BitConverter.ToUInt16(pkx, 0x0C);
            mSID = BitConverter.ToUInt16(pkx, 0x0E);
            mexp = BitConverter.ToUInt32(pkx, 0x10);
            mability = pkx[0x14];
            mabilitynum = pkx[0x15];
            // 0x16, 0x17 - unknown
            mPID = BitConverter.ToUInt32(pkx, 0x18);
            mnature = pkx[0x1C];
            mfeflag = pkx[0x1D] % 2;
            mgenderflag = (pkx[0x1D] >> 1) & 0x3;
            maltforms = (pkx[0x1D] >> 3);
            mHP_EV = pkx[0x1E];
            mATK_EV = pkx[0x1F];
            mDEF_EV = pkx[0x20];
            mSPA_EV = pkx[0x22];
            mSPD_EV = pkx[0x23];
            mSPE_EV = pkx[0x21];
            mcnt_cool = pkx[0x24];
            mcnt_beauty = pkx[0x25];
            mcnt_cute = pkx[0x26];
            mcnt_smart = pkx[0x27];
            mcnt_tough = pkx[0x28];
            mcnt_sheen = pkx[0x29];
            mmarkings = pkx[0x2A];
            mPKRS_Strain = pkx[0x2B] >> 4;
            mPKRS_Duration = pkx[0x2B] % 0x10;

            // Block B
            mnicknamestr = Util.TrimFromZero(Encoding.Unicode.GetString(pkx, 0x40, 24));
            // 0x58, 0x59 - unused
            mmove1 = BitConverter.ToUInt16(pkx, 0x5A);
            mmove2 = BitConverter.ToUInt16(pkx, 0x5C);
            mmove3 = BitConverter.ToUInt16(pkx, 0x5E);
            mmove4 = BitConverter.ToUInt16(pkx, 0x60);
            mmove1_pp = pkx[0x62];
            mmove2_pp = pkx[0x63];
            mmove3_pp = pkx[0x64];
            mmove4_pp = pkx[0x65];
            mmove1_ppu = pkx[0x66];
            mmove2_ppu = pkx[0x67];
            mmove3_ppu = pkx[0x68];
            mmove4_ppu = pkx[0x69];
            meggmove1 = BitConverter.ToUInt16(pkx, 0x6A);
            meggmove2 = BitConverter.ToUInt16(pkx, 0x6C);
            meggmove3 = BitConverter.ToUInt16(pkx, 0x6E);
            meggmove4 = BitConverter.ToUInt16(pkx, 0x70);

            // 0x72 - Super Training Flag - Passed with pkx to new form

            // 0x73 - unused/unknown
            mIV32 = BitConverter.ToUInt32(pkx, 0x74);
            mHP_IV = mIV32 & 0x1F;
            mATK_IV = (mIV32 >> 5) & 0x1F;
            mDEF_IV = (mIV32 >> 10) & 0x1F;
            mSPE_IV = (mIV32 >> 15) & 0x1F;
            mSPA_IV = (mIV32 >> 20) & 0x1F;
            mSPD_IV = (mIV32 >> 25) & 0x1F;
            misegg = Convert.ToBoolean((mIV32 >> 30) & 1);
            misnick = Convert.ToBoolean((mIV32 >> 31));

            // Block C
            mnotOT = Util.TrimFromZero(Encoding.Unicode.GetString(pkx, 0x78, 24));
            bool notOTG = Convert.ToBoolean(pkx[0x92]);
            // Memory Editor edits everything else with pkx in a new form

            // Block D
            mot = Util.TrimFromZero(Encoding.Unicode.GetString(pkx, 0xB0, 24));
            // 0xC8, 0xC9 - unused
            mOTfriendship = pkx[0xCA];
            mOTaffection = pkx[0xCB]; // Handled by Memory Editor
            // 0xCC, 0xCD, 0xCE, 0xCF, 0xD0
            megg_year = pkx[0xD1];
            megg_month = pkx[0xD2];
            megg_day = pkx[0xD3];
            mmet_year = pkx[0xD4];
            mmet_month = pkx[0xD5];
            mmet_day = pkx[0xD6];
            // 0xD7 - unused
            meggloc = BitConverter.ToUInt16(pkx, 0xD8);
            mmetloc = BitConverter.ToUInt16(pkx, 0xDA);
            mball = pkx[0xDC];
            mmetlevel = pkx[0xDD] & 0x7F;
            motgender = (pkx[0xDD]) >> 7;
            mencountertype = pkx[0xDE];
            mgamevers = pkx[0xDF];
            mcountryID = pkx[0xE0];
            mregionID = pkx[0xE1];
            mdsregID = pkx[0xE2];
            motlang = pkx[0xE3];

            if (mgenderflag == 0)
                mgenderstring = "♂";
            else if (mgenderflag == 1)
                mgenderstring = "♀";
            else 
                mgenderstring = "-";

            mhptype = (15 * ((mHP_IV & 1) + 2 * (mATK_IV & 1) + 4 * (mDEF_IV & 1) + 8 * (mSPE_IV & 1) + 16 * (mSPA_IV & 1) + 32 * (mSPD_IV & 1))) / 63 + 1;

            mTSV = (ushort)((mTID ^ mSID) >> 4);
            mESV = (ushort)(((mPID >> 16) ^ (mPID & 0xFFFF)) >> 4);

            misshiny = (mTSV == mESV);
            // Nidoran Gender Fixing Text
            if (!Convert.ToBoolean(misnick))
            {
                if (mnicknamestr.Contains((char)0xE08F))
                    mnicknamestr = Regex.Replace(mnicknamestr, "\uE08F", "\u2640");
                else if (mnicknamestr.Contains((char)0xE08E))
                    mnicknamestr = Regex.Replace(mnicknamestr, "\uE08E", "\u2642");
            }
            {
                int species = BitConverter.ToInt16(pkx, 0x08); // Get Species
                uint isegg = (BitConverter.ToUInt32(pkx, 0x74) >> 30) & 1;

                int altforms = (pkx[0x1D] >> 3);
                int gender = (pkx[0x1D] >> 1) & 0x3;

                string file;
                if (isegg == 1)
                { file = "egg"; }
                else
                {
                    file = "_" + species.ToString();
                    if (altforms > 0) // Alt Form Handling
                        file = file + "_" + altforms.ToString();
                    else if ((species == 521) && (gender == 1))   // Unfezant
                        file = "_" + species.ToString() + "f";
                }
                if (species == 0)
                    file = "_0";

                pksprite = (Image)Properties.Resources.ResourceManager.GetObject(file);
            }
            try
            {
                mSpeciesName = Form1.specieslist[mspecies];
                mNatureName = Form1.natures[mnature];
                mHPName = Form1.types[mhptype];
                mAbilityName = Form1.abilitylist[mability];
                mMove1N = Form1.movelist[mmove1];
                mMove2N = Form1.movelist[mmove2];
                mMove3N = Form1.movelist[mmove3];
                mMove4N = Form1.movelist[mmove4];
            }
            catch { return; }
        }
        #endregion

        // Save File Related
        internal static int detectSAVIndex(byte[] data, ref int savindex)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            {
                byte[] difihash1 = new byte[0x12C];
                byte[] difihash2 = new byte[0x12C];
                Array.Copy(data, 0x330, difihash1, 0, 0x12C);
                Array.Copy(data, 0x200, difihash2, 0, 0x12C);
                byte[] hashValue1 = mySHA256.ComputeHash(difihash1);
                byte[] hashValue2 = mySHA256.ComputeHash(difihash2);
                byte[] actualhash = new byte[0x20];
                Array.Copy(data, 0x16C, actualhash, 0, 0x20);
                if (hashValue1.SequenceEqual(actualhash))
                {
                    Console.WriteLine("Active DIFI 2 - Save 1.");
                    savindex = 0;
                }
                else if (hashValue2.SequenceEqual(actualhash))
                {
                    Console.WriteLine("Active DIFI 1 - Save 2.");
                    savindex = 1;
                }
                else
                {
                    Console.WriteLine("ERROR: NO ACTIVE DIFI HASH MATCH");
                    savindex = 2;
                }
            }
            if ((data[0x168] ^ 1) != savindex && savindex != 2)
            {
                Console.WriteLine("ERROR: ACTIVE BLOCK MISMATCH");
                savindex = 2;
            }
            return savindex;
        }
        internal static ushort ccitt16(byte[] data)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= (ushort)(data[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }
            return crc;
        }

        // Font Related
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
        internal static Font getPKXFont(float fontsize)
        {
            byte[] fontData = Properties.Resources.PGLDings_NormalRegular;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);            
            PrivateFontCollection fonts = new PrivateFontCollection();
            fonts.AddMemoryFont(fontPtr, Properties.Resources.PGLDings_NormalRegular.Length);                   uint dummy = 0;
            AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.PGLDings_NormalRegular.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);

            return new Font(fonts.Families[0], fontsize);
        }

        // Table Related
        #region Species/Form Ability Table: ORAS Personal Data
        internal static int[] speciesability = new int[] 
        { 
                                000,	000,	000,	000,
                                001,	065,	065,	034,
                                002,	065,	065,	034,
                                003,	065,	065,	034,
                                004,	066,	066,	094,
                                005,	066,	066,	094,
                                006,	066,	066,	094,
                                007,	067,	067,	044,
                                008,	067,	067,	044,
                                009,	067,	067,	044,
                                010,	019,	019,	050,
                                011,	061,	061,	061,
                                012,	014,	014,	110,
                                013,	019,	019,	050,
                                014,	061,	061,	061,
                                015,	068,	068,	097,
                                016,	051,	077,	145,
                                017,	051,	077,	145,
                                018,	051,	077,	145,
                                019,	050,	062,	055,
                                020,	050,	062,	055,
                                021,	051,	051,	097,
                                022,	051,	051,	097,
                                023,	022,	061,	127,
                                024,	022,	061,	127,
                                025,	009,	009,	031,
                                026,	009,	009,	031,
                                027,	008,	008,	146,
                                028,	008,	008,	146,
                                029,	038,	079,	055,
                                030,	038,	079,	055,
                                031,	038,	079,	125,
                                032,	038,	079,	055,
                                033,	038,	079,	055,
                                034,	038,	079,	125,
                                035,	056,	098,	132,
                                036,	056,	098,	109,
                                037,	018,	018,	070,
                                038,	018,	018,	070,
                                039,	056,	172,	132,
                                040,	056,	172,	119,
                                041,	039,	039,	151,
                                042,	039,	039,	151,
                                043,	034,	034,	050,
                                044,	034,	034,	001,
                                045,	034,	034,	027,
                                046,	027,	087,	006,
                                047,	027,	087,	006,
                                048,	014,	110,	050,
                                049,	019,	110,	147,
                                050,	008,	071,	159,
                                051,	008,	071,	159,
                                052,	053,	101,	127,
                                053,	007,	101,	127,
                                054,	006,	013,	033,
                                055,	006,	013,	033,
                                056,	072,	083,	128,
                                057,	072,	083,	128,
                                058,	022,	018,	154,
                                059,	022,	018,	154,
                                060,	011,	006,	033,
                                061,	011,	006,	033,
                                062,	011,	006,	033,
                                063,	028,	039,	098,
                                064,	028,	039,	098,
                                065,	028,	039,	098,
                                066,	062,	099,	080,
                                067,	062,	099,	080,
                                068,	062,	099,	080,
                                069,	034,	034,	082,
                                070,	034,	034,	082,
                                071,	034,	034,	082,
                                072,	029,	064,	044,
                                073,	029,	064,	044,
                                074,	069,	005,	008,
                                075,	069,	005,	008,
                                076,	069,	005,	008,
                                077,	050,	018,	049,
                                078,	050,	018,	049,
                                079,	012,	020,	144,
                                080,	012,	020,	144,
                                081,	042,	005,	148,
                                082,	042,	005,	148,
                                083,	051,	039,	128,
                                084,	050,	048,	077,
                                085,	050,	048,	077,
                                086,	047,	093,	115,
                                087,	047,	093,	115,
                                088,	001,	060,	143,
                                089,	001,	060,	143,
                                090,	075,	092,	142,
                                091,	075,	092,	142,
                                092,	026,	026,	026,
                                093,	026,	026,	026,
                                094,	026,	026,	026,
                                095,	069,	005,	133,
                                096,	015,	108,	039,
                                097,	015,	108,	039,
                                098,	052,	075,	125,
                                099,	052,	075,	125,
                                100,	043,	009,	106,
                                101,	043,	009,	106,
                                102,	034,	034,	139,
                                103,	034,	034,	139,
                                104,	069,	031,	004,
                                105,	069,	031,	004,
                                106,	007,	120,	084,
                                107,	051,	089,	039,
                                108,	020,	012,	013,
                                109,	026,	026,	026,
                                110,	026,	026,	026,
                                111,	031,	069,	120,
                                112,	031,	069,	120,
                                113,	030,	032,	131,
                                114,	034,	102,	144,
                                115,	048,	113,	039,
                                116,	033,	097,	006,
                                117,	038,	097,	006,
                                118,	033,	041,	031,
                                119,	033,	041,	031,
                                120,	035,	030,	148,
                                121,	035,	030,	148,
                                122,	043,	111,	101,
                                123,	068,	101,	080,
                                124,	012,	108,	087,
                                125,	009,	009,	072,
                                126,	049,	049,	072,
                                127,	052,	104,	153,
                                128,	022,	083,	125,
                                129,	033,	033,	155,
                                130,	022,	022,	153,
                                131,	011,	075,	093,
                                132,	007,	007,	150,
                                133,	050,	091,	107,
                                134,	011,	011,	093,
                                135,	010,	010,	095,
                                136,	018,	018,	062,
                                137,	036,	088,	148,
                                138,	033,	075,	133,
                                139,	033,	075,	133,
                                140,	033,	004,	133,
                                141,	033,	004,	133,
                                142,	069,	046,	127,
                                143,	017,	047,	082,
                                144,	046,	046,	081,
                                145,	046,	046,	009,
                                146,	046,	046,	049,
                                147,	061,	061,	063,
                                148,	061,	061,	063,
                                149,	039,	039,	136,
                                150,	046,	046,	127,
                                151,	028,	028,	028,
                                152,	065,	065,	102,
                                153,	065,	065,	102,
                                154,	065,	065,	102,
                                155,	066,	066,	018,
                                156,	066,	066,	018,
                                157,	066,	066,	018,
                                158,	067,	067,	125,
                                159,	067,	067,	125,
                                160,	067,	067,	125,
                                161,	050,	051,	119,
                                162,	050,	051,	119,
                                163,	015,	051,	110,
                                164,	015,	051,	110,
                                165,	068,	048,	155,
                                166,	068,	048,	089,
                                167,	068,	015,	097,
                                168,	068,	015,	097,
                                169,	039,	039,	151,
                                170,	010,	035,	011,
                                171,	010,	035,	011,
                                172,	009,	009,	031,
                                173,	056,	098,	132,
                                174,	056,	172,	132,
                                175,	055,	032,	105,
                                176,	055,	032,	105,
                                177,	028,	048,	156,
                                178,	028,	048,	156,
                                179,	009,	009,	057,
                                180,	009,	009,	057,
                                181,	009,	009,	057,
                                182,	034,	034,	131,
                                183,	047,	037,	157,
                                184,	047,	037,	157,
                                185,	005,	069,	155,
                                186,	011,	006,	002,
                                187,	034,	102,	151,
                                188,	034,	102,	151,
                                189,	034,	102,	151,
                                190,	050,	053,	092,
                                191,	034,	094,	048,
                                192,	034,	094,	048,
                                193,	003,	014,	119,
                                194,	006,	011,	109,
                                195,	006,	011,	109,
                                196,	028,	028,	156,
                                197,	028,	028,	039,
                                198,	015,	105,	158,
                                199,	012,	020,	144,
                                200,	026,	026,	026,
                                201,	026,	026,	026,
                                202,	023,	023,	140,
                                203,	039,	048,	157,
                                204,	005,	005,	142,
                                205,	005,	005,	142,
                                206,	032,	050,	155,
                                207,	052,	008,	017,
                                208,	069,	005,	125,
                                209,	022,	050,	155,
                                210,	022,	095,	155,
                                211,	038,	033,	022,
                                212,	068,	101,	135,
                                213,	005,	082,	126,
                                214,	068,	062,	153,
                                215,	039,	051,	124,
                                216,	053,	095,	118,
                                217,	062,	095,	127,
                                218,	040,	049,	133,
                                219,	040,	049,	133,
                                220,	012,	081,	047,
                                221,	012,	081,	047,
                                222,	055,	030,	144,
                                223,	055,	097,	141,
                                224,	021,	097,	141,
                                225,	072,	055,	015,
                                226,	033,	011,	041,
                                227,	051,	005,	133,
                                228,	048,	018,	127,
                                229,	048,	018,	127,
                                230,	033,	097,	006,
                                231,	053,	053,	008,
                                232,	005,	005,	008,
                                233,	036,	088,	148,
                                234,	022,	119,	157,
                                235,	020,	101,	141,
                                236,	062,	080,	072,
                                237,	022,	101,	080,
                                238,	012,	108,	093,
                                239,	009,	009,	072,
                                240,	049,	049,	072,
                                241,	047,	113,	157,
                                242,	030,	032,	131,
                                243,	046,	046,	010,
                                244,	046,	046,	018,
                                245,	046,	046,	011,
                                246,	062,	062,	008,
                                247,	061,	061,	061,
                                248,	045,	045,	127,
                                249,	046,	046,	136,
                                250,	046,	046,	144,
                                251,	030,	030,	030,
                                252,	065,	065,	084,
                                253,	065,	065,	084,
                                254,	065,	065,	084,
                                255,	066,	066,	003,
                                256,	066,	066,	003,
                                257,	066,	066,	003,
                                258,	067,	067,	006,
                                259,	067,	067,	006,
                                260,	067,	067,	006,
                                261,	050,	095,	155,
                                262,	022,	095,	153,
                                263,	053,	082,	095,
                                264,	053,	082,	095,
                                265,	019,	019,	050,
                                266,	061,	061,	061,
                                267,	068,	068,	079,
                                268,	061,	061,	061,
                                269,	019,	019,	014,
                                270,	033,	044,	020,
                                271,	033,	044,	020,
                                272,	033,	044,	020,
                                273,	034,	048,	124,
                                274,	034,	048,	124,
                                275,	034,	048,	124,
                                276,	062,	062,	113,
                                277,	062,	062,	113,
                                278,	051,	051,	044,
                                279,	051,	051,	044,
                                280,	028,	036,	140,
                                281,	028,	036,	140,
                                282,	028,	036,	140,
                                283,	033,	033,	044,
                                284,	022,	022,	127,
                                285,	027,	090,	095,
                                286,	027,	090,	101,
                                287,	054,	054,	054,
                                288,	072,	072,	072,
                                289,	054,	054,	054,
                                290,	014,	014,	050,
                                291,	003,	003,	151,
                                292,	025,	025,	025,
                                293,	043,	043,	155,
                                294,	043,	043,	113,
                                295,	043,	043,	113,
                                296,	047,	062,	125,
                                297,	047,	062,	125,
                                298,	047,	037,	157,
                                299,	005,	042,	159,
                                300,	056,	096,	147,
                                301,	056,	096,	147,
                                302,	051,	100,	158,
                                303,	052,	022,	125,
                                304,	005,	069,	134,
                                305,	005,	069,	134,
                                306,	005,	069,	134,
                                307,	074,	074,	140,
                                308,	074,	074,	140,
                                309,	009,	031,	058,
                                310,	009,	031,	058,
                                311,	057,	057,	031,
                                312,	058,	058,	010,
                                313,	035,	068,	158,
                                314,	012,	110,	158,
                                315,	030,	038,	102,
                                316,	064,	060,	082,
                                317,	064,	060,	082,
                                318,	024,	024,	003,
                                319,	024,	024,	003,
                                320,	041,	012,	046,
                                321,	041,	012,	046,
                                322,	012,	086,	020,
                                323,	040,	116,	083,
                                324,	073,	073,	075,
                                325,	047,	020,	082,
                                326,	047,	020,	082,
                                327,	020,	077,	126,
                                328,	052,	071,	125,
                                329,	026,	026,	026,
                                330,	026,	026,	026,
                                331,	008,	008,	011,
                                332,	008,	008,	011,
                                333,	030,	030,	013,
                                334,	030,	030,	013,
                                335,	017,	017,	137,
                                336,	061,	061,	151,
                                337,	026,	026,	026,
                                338,	026,	026,	026,
                                339,	012,	107,	093,
                                340,	012,	107,	093,
                                341,	052,	075,	091,
                                342,	052,	075,	091,
                                343,	026,	026,	026,
                                344,	026,	026,	026,
                                345,	021,	021,	114,
                                346,	021,	021,	114,
                                347,	004,	004,	033,
                                348,	004,	004,	033,
                                349,	033,	012,	091,
                                350,	063,	172,	056,
                                351,	059,	059,	059,
                                352,	016,	016,	168,
                                353,	015,	119,	130,
                                354,	015,	119,	130,
                                355,	026,	026,	119,
                                356,	046,	046,	119,
                                357,	034,	094,	139,
                                358,	026,	026,	026,
                                359,	046,	105,	154,
                                360,	023,	023,	140,
                                361,	039,	115,	141,
                                362,	039,	115,	141,
                                363,	047,	115,	012,
                                364,	047,	115,	012,
                                365,	047,	115,	012,
                                366,	075,	075,	155,
                                367,	033,	033,	041,
                                368,	033,	033,	093,
                                369,	033,	069,	005,
                                370,	033,	033,	093,
                                371,	069,	069,	125,
                                372,	069,	069,	142,
                                373,	022,	022,	153,
                                374,	029,	029,	135,
                                375,	029,	029,	135,
                                376,	029,	029,	135,
                                377,	029,	029,	005,
                                378,	029,	029,	115,
                                379,	029,	029,	135,
                                380,	026,	026,	026,
                                381,	026,	026,	026,
                                382,	002,	002,	002,
                                383,	070,	070,	070,
                                384,	076,	076,	076,
                                385,	032,	032,	032,
                                386,	046,	046,	046,
                                387,	065,	065,	075,
                                388,	065,	065,	075,
                                389,	065,	065,	075,
                                390,	066,	066,	089,
                                391,	066,	066,	089,
                                392,	066,	066,	089,
                                393,	067,	067,	128,
                                394,	067,	067,	128,
                                395,	067,	067,	128,
                                396,	051,	051,	120,
                                397,	022,	022,	120,
                                398,	022,	022,	120,
                                399,	086,	109,	141,
                                400,	086,	109,	141,
                                401,	061,	061,	050,
                                402,	068,	068,	101,
                                403,	079,	022,	062,
                                404,	079,	022,	062,
                                405,	079,	022,	062,
                                406,	030,	038,	102,
                                407,	030,	038,	101,
                                408,	104,	104,	125,
                                409,	104,	104,	125,
                                410,	005,	005,	043,
                                411,	005,	005,	043,
                                412,	061,	061,	142,
                                413,	107,	107,	142,
                                414,	068,	068,	110,
                                415,	118,	118,	055,
                                416,	046,	046,	127,
                                417,	050,	053,	010,
                                418,	033,	033,	041,
                                419,	033,	033,	041,
                                420,	034,	034,	034,
                                421,	122,	122,	122,
                                422,	060,	114,	159,
                                423,	060,	114,	159,
                                424,	101,	053,	092,
                                425,	106,	084,	138,
                                426,	106,	084,	138,
                                427,	050,	103,	007,
                                428,	056,	103,	007,
                                429,	026,	026,	026,
                                430,	015,	105,	153,
                                431,	007,	020,	051,
                                432,	047,	020,	128,
                                433,	026,	026,	026,
                                434,	001,	106,	051,
                                435,	001,	106,	051,
                                436,	026,	085,	134,
                                437,	026,	085,	134,
                                438,	005,	069,	155,
                                439,	043,	111,	101,
                                440,	030,	032,	132,
                                441,	051,	077,	145,
                                442,	046,	046,	151,
                                443,	008,	008,	024,
                                444,	008,	008,	024,
                                445,	008,	008,	024,
                                446,	053,	047,	082,
                                447,	080,	039,	158,
                                448,	080,	039,	154,
                                449,	045,	045,	159,
                                450,	045,	045,	159,
                                451,	004,	097,	051,
                                452,	004,	097,	051,
                                453,	107,	087,	143,
                                454,	107,	087,	143,
                                455,	026,	026,	026,
                                456,	033,	114,	041,
                                457,	033,	114,	041,
                                458,	033,	011,	041,
                                459,	117,	117,	043,
                                460,	117,	117,	043,
                                461,	046,	046,	124,
                                462,	042,	005,	148,
                                463,	020,	012,	013,
                                464,	031,	116,	120,
                                465,	034,	102,	144,
                                466,	078,	078,	072,
                                467,	049,	049,	072,
                                468,	055,	032,	105,
                                469,	003,	110,	119,
                                470,	102,	102,	034,
                                471,	081,	081,	115,
                                472,	052,	008,	090,
                                473,	012,	081,	047,
                                474,	091,	088,	148,
                                475,	080,	080,	154,
                                476,	005,	042,	159,
                                477,	046,	046,	119,
                                478,	081,	081,	130,
                                479,	026,	026,	026,
                                480,	026,	026,	026,
                                481,	026,	026,	026,
                                482,	026,	026,	026,
                                483,	046,	046,	140,
                                484,	046,	046,	140,
                                485,	018,	018,	049,
                                486,	112,	112,	112,
                                487,	046,	046,	140,
                                488,	026,	026,	026,
                                489,	093,	093,	093,
                                490,	093,	093,	093,
                                491,	123,	123,	123,
                                492,	030,	030,	030,
                                493,	121,	121,	121,
                                494,	162,	162,	162,
                                495,	065,	065,	126,
                                496,	065,	065,	126,
                                497,	065,	065,	126,
                                498,	066,	066,	047,
                                499,	066,	066,	047,
                                500,	066,	066,	120,
                                501,	067,	067,	075,
                                502,	067,	067,	075,
                                503,	067,	067,	075,
                                504,	050,	051,	148,
                                505,	035,	051,	148,
                                506,	072,	053,	050,
                                507,	022,	146,	113,
                                508,	022,	146,	113,
                                509,	007,	084,	158,
                                510,	007,	084,	158,
                                511,	082,	082,	065,
                                512,	082,	082,	065,
                                513,	082,	082,	066,
                                514,	082,	082,	066,
                                515,	082,	082,	067,
                                516,	082,	082,	067,
                                517,	108,	028,	140,
                                518,	108,	028,	140,
                                519,	145,	105,	079,
                                520,	145,	105,	079,
                                521,	145,	105,	079,
                                522,	031,	078,	157,
                                523,	031,	078,	157,
                                524,	005,	005,	159,
                                525,	005,	005,	159,
                                526,	005,	005,	159,
                                527,	109,	103,	086,
                                528,	109,	103,	086,
                                529,	146,	159,	104,
                                530,	146,	159,	104,
                                531,	131,	144,	103,
                                532,	062,	125,	089,
                                533,	062,	125,	089,
                                534,	062,	125,	089,
                                535,	033,	093,	011,
                                536,	033,	093,	011,
                                537,	033,	143,	011,
                                538,	062,	039,	104,
                                539,	005,	039,	104,
                                540,	068,	034,	142,
                                541,	102,	034,	142,
                                542,	068,	034,	142,
                                543,	038,	068,	003,
                                544,	038,	068,	003,
                                545,	038,	068,	003,
                                546,	158,	151,	034,
                                547,	158,	151,	034,
                                548,	034,	020,	102,
                                549,	034,	020,	102,
                                550,	120,	091,	104,
                                551,	022,	153,	083,
                                552,	022,	153,	083,
                                553,	022,	153,	083,
                                554,	055,	055,	039,
                                555,	125,	125,	161,
                                556,	011,	034,	114,
                                557,	005,	075,	133,
                                558,	005,	075,	133,
                                559,	061,	153,	022,
                                560,	061,	153,	022,
                                561,	147,	098,	110,
                                562,	152,	152,	152,
                                563,	152,	152,	152,
                                564,	116,	005,	033,
                                565,	116,	005,	033,
                                566,	129,	129,	129,
                                567,	129,	129,	129,
                                568,	001,	060,	106,
                                569,	001,	133,	106,
                                570,	149,	149,	149,
                                571,	149,	149,	149,
                                572,	056,	101,	092,
                                573,	056,	101,	092,
                                574,	119,	172,	023,
                                575,	119,	172,	023,
                                576,	119,	172,	023,
                                577,	142,	098,	144,
                                578,	142,	098,	144,
                                579,	142,	098,	144,
                                580,	051,	145,	093,
                                581,	051,	145,	093,
                                582,	115,	115,	133,
                                583,	115,	115,	133,
                                584,	115,	115,	133,
                                585,	034,	157,	032,
                                586,	034,	157,	032,
                                587,	009,	009,	078,
                                588,	068,	061,	099,
                                589,	068,	075,	142,
                                590,	027,	027,	144,
                                591,	027,	027,	144,
                                592,	011,	130,	006,
                                593,	011,	130,	006,
                                594,	131,	093,	144,
                                595,	014,	127,	068,
                                596,	014,	127,	068,
                                597,	160,	160,	160,
                                598,	160,	160,	107,
                                599,	057,	058,	029,
                                600,	057,	058,	029,
                                601,	057,	058,	029,
                                602,	026,	026,	026,
                                603,	026,	026,	026,
                                604,	026,	026,	026,
                                605,	140,	028,	148,
                                606,	140,	028,	148,
                                607,	018,	049,	151,
                                608,	018,	049,	151,
                                609,	018,	049,	151,
                                610,	079,	104,	127,
                                611,	079,	104,	127,
                                612,	079,	104,	127,
                                613,	081,	081,	155,
                                614,	081,	081,	033,
                                615,	026,	026,	026,
                                616,	093,	075,	142,
                                617,	093,	060,	084,
                                618,	009,	007,	008,
                                619,	039,	144,	120,
                                620,	039,	144,	120,
                                621,	024,	125,	104,
                                622,	089,	103,	099,
                                623,	089,	103,	099,
                                624,	128,	039,	046,
                                625,	128,	039,	046,
                                626,	120,	157,	043,
                                627,	051,	125,	055,
                                628,	051,	125,	128,
                                629,	145,	142,	133,
                                630,	145,	142,	133,
                                631,	082,	018,	073,
                                632,	068,	055,	054,
                                633,	055,	055,	055,
                                634,	055,	055,	055,
                                635,	026,	026,	026,
                                636,	049,	049,	068,
                                637,	049,	049,	068,
                                638,	154,	154,	154,
                                639,	154,	154,	154,
                                640,	154,	154,	154,
                                641,	158,	158,	128,
                                642,	158,	158,	128,
                                643,	163,	163,	163,
                                644,	164,	164,	164,
                                645,	159,	159,	125,
                                646,	046,	046,	046,
                                647,	154,	154,	154,
                                648,	032,	032,	032,
                                649,	088,	088,	088,
                                650,	065,	065,	171,
                                651,	065,	065,	171,
                                652,	065,	065,	171,
                                653,	066,	066,	170,
                                654,	066,	066,	170,
                                655,	066,	066,	170,
                                656,	067,	067,	168,
                                657,	067,	067,	168,
                                658,	067,	067,	168,
                                659,	053,	167,	037,
                                660,	053,	167,	037,
                                661,	145,	145,	177,
                                662,	049,	049,	177,
                                663,	049,	049,	177,
                                664,	019,	014,	132,
                                665,	061,	061,	132,
                                666,	019,	014,	132,
                                667,	079,	127,	153,
                                668,	079,	127,	153,
                                669,	166,	166,	180,
                                670,	166,	166,	180,
                                671,	166,	166,	180,
                                672,	157,	157,	179,
                                673,	157,	157,	179,
                                674,	089,	104,	113,
                                675,	089,	104,	113,
                                676,	169,	169,	169,
                                677,	051,	151,	020,
                                678,	051,	151,	158,
                                679,	099,	099,	099,
                                680,	099,	099,	099,
                                681,	176,	176,	176,
                                682,	131,	131,	165,
                                683,	131,	131,	165,
                                684,	175,	175,	084,
                                685,	175,	175,	084,
                                686,	126,	021,	151,
                                687,	126,	021,	151,
                                688,	181,	097,	124,
                                689,	181,	097,	124,
                                690,	038,	143,	091,
                                691,	038,	143,	091,
                                692,	178,	178,	178,
                                693,	178,	178,	178,
                                694,	087,	008,	094,
                                695,	087,	008,	094,
                                696,	173,	173,	005,
                                697,	173,	173,	069,
                                698,	174,	174,	117,
                                699,	174,	174,	117,
                                700,	056,	056,	182,
                                701,	007,	084,	104,
                                702,	167,	053,	057,
                                703,	029,	029,	005,
                                704,	157,	093,	183,
                                705,	157,	093,	183,
                                706,	157,	093,	183,
                                707,	158,	158,	170,
                                708,	030,	119,	139,
                                709,	030,	119,	139,
                                710,	053,	119,	015,
                                711,	053,	119,	015,
                                712,	020,	115,	005,
                                713,	020,	115,	005,
                                714,	119,	151,	140,
                                715,	119,	151,	140,
                                716,	187,	187,	187,
                                717,	186,	186,	186,
                                718,	188,	188,	188,
                                719,	029,	029,	029,
                                720,	170,	170,	170,
                                721,	011,	011,	011,
                                722,	046,	046,	046,
                                723,	046,	046,	046,
                                724,	046,	046,	046,
                                725,	107,	107,	142,
                                726,	107,	107,	142,
                                727,	032,	032,	032,
                                728,	026,	026,	026,
                                729,	026,	026,	026,
                                730,	026,	026,	026,
                                731,	026,	026,	026,
                                732,	026,	026,	026,
                                733,	026,	026,	026,
                                734,	059,	059,	059,
                                735,	059,	059,	059,
                                736,	059,	059,	059,
                                737,	122,	122,	122,
                                738,	069,	091,	104,
                                739,	125,	125,	161,
                                740,	032,	032,	032,
                                741,	163,	163,	163,
                                742,	164,	164,	164,
                                743,	154,	154,	154,
                                744,	144,	144,	144,
                                745,	010,	010,	010,
                                746,	022,	022,	022,
                                747,	023,	023,	023,
                                748,	051,	151,	172,
                                749,	169,	169,	169,
                                750,	169,	169,	169,
                                751,	169,	169,	169,
                                752,	169,	169,	169,
                                753,	169,	169,	169,
                                754,	169,	169,	169,
                                755,	169,	169,	169,
                                756,	169,	169,	169,
                                757,	169,	169,	169,
                                758,	182,	182,	182,
                                759,	104,	104,	104,
                                760,	047,	047,	047,
                                761,	181,	181,	181,
                                762,	070,	070,	070,
                                763,	080,	080,	080,
                                764,	015,	015,	015,
                                765,	003,	003,	003,
                                766,	074,	074,	074,
                                767,	094,	094,	094,
                                768,	111,	111,	111,
                                769,	158,	158,	158,
                                770,	045,	045,	045,
                                771,	101,	101,	101,
                                772,	184,	184,	184,
                                773,	181,	181,	181,
                                774,	091,	091,	091,
                                775,	117,	117,	117,
                                776,	176,	176,	176,
                                777,	178,	178,	178,
                                778,	185,	185,	185,
                                779,	104,	104,	104,
                                780,	156,	156,	156,
                                781,	036,	036,	036,
                                782,	092,	092,	092,
                                783,	037,	037,	037,
                                784,	022,	022,	022,
                                785,	159,	159,	159,
                                786,	026,	026,	026,
                                787,	026,	026,	026,
                                788,	053,	119,	015,
                                789,	053,	119,	015,
                                790,	053,	119,	015,
                                791,	053,	119,	015,
                                792,	053,	119,	015,
                                793,	053,	119,	015,
                                794,	166,	166,	180,
                                795,	166,	166,	180,
                                796,	166,	166,	180,
                                797,	166,	166,	180,
                                798,	166,	166,	180,
                                799,	033,	033,	033,
                                800,	031,	031,	031,
                                801,	156,	156,	156,
                                802,	182,	182,	182,
                                803,	039,	039,	039,
                                804,	131,	131,	131,
                                805,	173,	173,	173,
                                806,	075,	075,	075,
                                807,	159,	159,	159,
                                808,	099,	099,	099,
                                809,	174,	174,	174,
                                810,	156,	156,	156,
                                811,	181,	181,	181,
                                812,	189,	189,	189,
                                813,	190,	190,	190,
                                814,	191,	191,	191,
                                815,	009,	009,	031,
                                816,	009,	009,	031,
                                817,	009,	009,	031,
                                818,	009,	009,	031,
                                819,	009,	009,	031,
                                820,	009,	009,	031,
                                821,	170,	170,	170,
                                822,	125,	125,	125,
                                823,	113,	113,	113,
                                824,	184,	184,	184,
                                825,	091,	091,	091,
                };
        #endregion

        // Classes
        public class PersonalParser
        {
            public byte[] file = (byte[])Properties.Resources.ResourceManager.GetObject("personal");
            public int EntryLength = 0xE;
            public struct Personal
            {
                public byte[] BaseStats;
                public byte[] Abilities;
                public byte BaseFriendship;
                public byte GenderRatio;
                public byte EXPGrowth;
                public byte AltFormCount;
                public byte FormPointer; //721+FormPointer+(FormID-1)=SpeciesIndex           
            }

            public Personal GetPersonal(int species)
            {
                Personal data = new Personal();
                byte[] MonData = new byte[EntryLength];
                data.BaseStats = new byte[6];
                data.Abilities = new byte[3];
                Array.Copy(file, species * EntryLength, MonData, 0, EntryLength);
                Array.Copy(MonData, data.BaseStats, 6);
                Array.Copy(MonData, 6, data.Abilities, 0, 3);
                data.BaseFriendship = MonData[0x9];
                data.GenderRatio = MonData[0xA];
                data.EXPGrowth = MonData[0xB];
                data.AltFormCount = MonData[0xC];
                data.FormPointer = MonData[0xD];
                return data;
            }

            public Personal GetPersonal(int species, int formID)
            {
                Personal data = GetPersonal(species);
                if (formID > 0 && formID <= data.AltFormCount && data.AltFormCount > 0 && data.FormPointer > 0) //Working with an Alt Forme with a base stat change
                {
                    formID--;
                    data = GetPersonal(721 + formID + data.FormPointer);
                }
                return data;
            }
        }
    }
}
