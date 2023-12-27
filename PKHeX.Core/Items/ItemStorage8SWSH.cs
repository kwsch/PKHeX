using System;

namespace PKHeX.Core;

public sealed class ItemStorage8SWSH : IItemStorage
{
    public static readonly ItemStorage8SWSH Instance = new();

    private static ReadOnlySpan<ushort> Pouch_Regular_SWSH =>
    [
        045, 046, 047, 048, 049, 050, 051, 052, 053, 076, 077, 079, 080, 081, 082, 083, 084, 085, 107, 108, 109,
        110, 112, 116, 117, 118, 119, 135, 136, 213, 214, 215, 217, 218, 219, 220, 221, 222, 223, 224, 225, 228,
        229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249,
        250, 251, 252, 253, 254, 255, 257, 258, 259, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276,
        277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297,
        298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318,
        319, 320, 321, 322, 323, 324, 325, 326, 485, 486, 487, 488, 489, 490, 491, 537, 538, 539, 540, 541, 542,
        543, 544, 545, 546, 547, 564, 565, 566, 567, 568, 569, 570, 639, 640, 644, 645, 646, 647, 648, 649, 650,
        846, 849, 879, 880, 881, 882, 883, 884, 904, 905, 906, 907, 908, 909, 910, 911, 912, 913, 914, 915, 916,
        917, 918, 919, 920, 1103, 1104, 1109, 1110, 1111, 1112, 1113, 1114, 1115, 1116, 1117, 1118, 1119, 1120,
        1121, 1122, 1123, 1124, 1125, 1126, 1127, 1128, 1129, 1231, 1232, 1233, 1234, 1235, 1236, 1237, 1238, 1239,
        1240, 1241, 1242, 1243, 1244, 1245, 1246, 1247, 1248, 1249, 1250, 1251, 1252, 1253, 1254,

        1279,
        1280, 1281, 1282, 1283, 1284, 1285, 1286, 1287, 1288, 1289, 1290, 1291, 1292, 1293, 1294, 1295, 1296, 1297,
        1298, 1299, 1300, 1301, 1302, 1303, 1304, 1305, 1306, 1307, 1308, 1309, 1310, 1311, 1312, 1313, 1314, 1315,
        1316, 1317, 1318, 1319, 1320, 1321, 1322, 1323, 1324, 1325, 1326, 1327, 1328, 1329, 1330, 1331, 1332, 1333,
        1334, 1335, 1336, 1337, 1338, 1339, 1340, 1341, 1342, 1343, 1344, 1345, 1346, 1347, 1348, 1349, 1350, 1351,
        1352, 1353, 1354, 1355, 1356, 1357, 1358, 1359, 1360, 1361, 1362, 1363, 1364, 1365, 1366, 1367, 1368, 1369,
        1370, 1371, 1372, 1373, 1374, 1375, 1376, 1377, 1378, 1379, 1380, 1381, 1382, 1383, 1384, 1385, 1386, 1387,
        1388, 1389, 1390, 1391, 1392, 1393, 1394, 1395, 1396, 1397, 1398, 1399, 1400, 1401, 1402, 1403, 1404, 1405,
        1406, 1407, 1408, 1409, 1410, 1411, 1412, 1413, 1414, 1415, 1416, 1417, 1418, 1419, 1420, 1421, 1422, 1423,
        1424, 1425, 1426, 1427, 1428, 1429, 1430, 1431, 1432, 1433, 1434, 1435, 1436, 1437, 1438, 1439, 1440, 1441,
        1442, 1443, 1444, 1445, 1446, 1447, 1448, 1449, 1450, 1451, 1452, 1453, 1454, 1455, 1456, 1457, 1458, 1459,
        1460, 1461, 1462, 1463, 1464, 1465, 1466, 1467, 1468, 1469, 1470, 1471, 1472, 1473, 1474, 1475, 1476, 1477,
        1478, 1479, 1480, 1481, 1482, 1483, 1484, 1485, 1486, 1487, 1488, 1489, 1490, 1491, 1492, 1493, 1494, 1495,
        1496, 1497, 1498, 1499, 1500, 1501, 1502, 1503, 1504, 1505, 1506, 1507, 1508, 1509, 1510, 1511, 1512, 1513,
        1514, 1515, 1516, 1517, 1518, 1519, 1520, 1521, 1522, 1523, 1524, 1525, 1526, 1527, 1528, 1529, 1530, 1531,
        1532, 1533, 1534, 1535, 1536, 1537, 1538, 1539, 1540, 1541, 1542, 1543, 1544, 1545, 1546, 1547, 1548, 1549,
        1550, 1551, 1552, 1553, 1554, 1555, 1556, 1557, 1558, 1559, 1560, 1561, 1562, 1563, 1564, 1565, 1566, 1567,
        1568, 1569, 1570, 1571, 1572, 1573, 1574, 1575, 1576, 1577, 1578, 1581, 1582, 1588,

        // DLC 2
        1592, 1604, 1606,
    ];

    private static ReadOnlySpan<ushort> Pouch_Ball_SWSH =>
    [
        001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016,
        492, 493, 494, 495, 496, 497, 498, 499, 500,
        576,
        851,
    ];

    private static ReadOnlySpan<ushort> Pouch_Battle_SWSH =>
    [
        055, 056, 057, 058, 059, 060, 061, 062, 063, 1580,
    ];

    private static ReadOnlySpan<ushort> Pouch_Key_SWSH =>
    [
        078,
        628, 629, 631, 632, 638,
        703,
        847,
        943, 944, 945, 946,
        1074, 1075, 1076, 1077, 1080, 1081, 1100, 1255, 1266, 1267,
        1269, 1270, 1271, 1278, 1583, 1584, 1585, 1586, 1587, 1589,

        // DLC 2
        1590, 1591, 1593, 1594, 1595, 1596, 1597, 1598, 1599, 1600, 1601, 1602, 1603, 1605, 1607,
    ];

    private static ReadOnlySpan<ushort> Pouch_TMTR_SWSH =>
    [
        328, 329, 330, 331, 332, 333, 334, 335, 336, 337,
        338, 339, 340, 341, 342, 343, 344, 345, 346, 347,
        348, 349, 350, 351, 352, 353, 354, 355, 356, 357,
        358, 359, 360, 361, 362, 363, 364, 365, 366, 367,
        368, 369, 370, 371, 372, 373, 374, 375, 376, 377,
        378, 379, 380, 381, 382, 383, 384, 385, 386, 387,
        388, 389, 390, 391, 392, 393, 394, 395, 396, 397,
        398, 399, 400, 401, 402, 403, 404, 405, 406, 407,
        408, 409, 410, 411, 412, 413, 414, 415, 416, 417,
        418, 419, // 01-92

        618, 619, 620, // 93-95
        690, 691, 692, 693, // 96-99

        // TR
        1130, 1131, 1132, 1133, 1134, 1135, 1136, 1137, 1138, 1139,
        1140, 1141, 1142, 1143, 1144, 1145, 1146, 1147, 1148, 1149,
        1150, 1151, 1152, 1153, 1154, 1155, 1156, 1157, 1158, 1159,
        1160, 1161, 1162, 1163, 1164, 1165, 1166, 1167, 1168, 1169,
        1170, 1171, 1172, 1173, 1174, 1175, 1176, 1177, 1178, 1179,
        1180, 1181, 1182, 1183, 1184, 1185, 1186, 1187, 1188, 1189,
        1190, 1191, 1192, 1193, 1194, 1195, 1196, 1197, 1198, 1199,
        1200, 1201, 1202, 1203, 1204, 1205, 1206, 1207, 1208, 1209,
        1210, 1211, 1212, 1213, 1214, 1215, 1216, 1217, 1218, 1219,
        1220, 1221, 1222, 1223, 1224, 1225, 1226, 1227, 1228, 1229,

        1230, // TM00
    ];

    private const int COUNT_TR = 100;
    private static ReadOnlySpan<ushort> Pouch_TR_SWSH => Pouch_TMTR_SWSH.Slice(99, COUNT_TR);
    public static bool IsTechRecord(ushort itemID) => itemID - 1130u < COUNT_TR;

    private static ReadOnlySpan<ushort> Pouch_Medicine_SWSH =>
    [
        017, 018, 019, 020, 021, 022, 023, 024, 025, 026,
        027, 028, 029, 030, 031, 032, 033, 034, 035, 036,
        037, 038, 039, 040, 041, 042, 043, 054,
        134,
        504, 591,
        708, 709,
        852, 903,
        1579,
    ];

    private static ReadOnlySpan<ushort> Pouch_Berries_SWSH =>
    [
                                                     149,
        150, 151, 152, 153, 154, 155, 156, 157, 158, 159,
        160, 161, 162, 163,                          169,
        170, 171, 172, 173, 174,
                            184, 185, 186, 187, 188, 189,
        190, 191, 192, 193, 194, 195, 196, 197, 198, 199,
        200, 201, 202, 203, 204, 205, 206, 207, 208, 209,
        210, 211, 212,
        686, 687, 688,
    ];

    private static ReadOnlySpan<ushort> Pouch_Ingredients_SWSH =>
    [
        1084, 1085, 1086, 1087, 1088, 1089, 1090, 1091, 1092, 1093,
        1094, 1095, 1096, 1097, 1098, 1099, 1256, 1257, 1258, 1259,
        1260, 1261, 1262, 1263, 1264,
    ];

    private static ReadOnlySpan<ushort> Pouch_Treasure_SWSH =>
    [
        086, 087, 088, 089, 090, 091, 092, 094, 106,
        571, 580, 581, 582, 583,
        795, 796,
        1105, 1106, 1107, 1108,
    ];

    internal static ReadOnlySpan<ushort> Unreleased =>
    [
        016, // Cherish Ball
        298, // Flame Plate
        299, // Splash Plate
        300, // Zap Plate
        301, // Meadow Plate
        302, // Icicle Plate
        303, // Fist Plate
        304, // Toxic Plate
        305, // Earth Plate
        306, // Sky Plate
        307, // Mind Plate
        308, // Insect Plate
        309, // Stone Plate
        310, // Spooky Plate
        311, // Draco Plate
        312, // Dread Plate
        313, // Iron Plate

        500, // Park Ball
        // 644, // Pixie Plate
    ];

    internal static Range DynamaxCrystalBCAT => new(DMAX_START, DMAX_END + 1);

    // 1279, // ★And458 (Jangmo-o)
    // 1280, // ★And15 (Larvitar)
    // 1281, // ★And337 (Corviknight)
    // 1282, // ★And603 (Eiscue)
    // 1283, // ★And390 (Stonjourner)
    // 1284, // ★Sgr6879 (Copperajah)
    // 1285, // ★Sgr6859 (Centiskorch)
    // 1286, // ★Sgr6913 (Flapple/Appletun)
    // 1287, // ★Sgr7348 (Sandaconda)
    // 1288, // ★Sgr7121 (Duraludon)
    // 1289, // ★Sgr6746 (Pikachu)
    // 1290, // ★Sgr7194 (Eevee)
    private const int DMAX_START = 1279;
    private const int DMAX_END = 1578;
    private const int DMAX_LEGAL_END = 1290; // ★Sgr7194 (Eevee)
    public static bool IsDynamaxCrystal(ushort item) => item is >= DMAX_START and <= DMAX_END;
    public static bool IsDynamaxCrystalAvailable(ushort item) => item is >= DMAX_START and <= DMAX_LEGAL_END;

    public static bool IsItemLegal(ushort item)
    {
        if (IsDynamaxCrystal(item))
            return IsDynamaxCrystalAvailable(item);
        return Unreleased.BinarySearch(item) < 0;
    }

    public static ushort[] GetAllHeld() => [..Pouch_Regular_SWSH, ..Pouch_Ball_SWSH, ..Pouch_Battle_SWSH, ..Pouch_Berries_SWSH, ..Pouch_Medicine_SWSH, ..Pouch_TR_SWSH, ..Pouch_Treasure_SWSH, ..Pouch_Ingredients_SWSH];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount)
    {
        if (type is InventoryType.KeyItems)
            return true;
        return IsItemLegal((ushort)itemIndex);
    }

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Medicine => Pouch_Medicine_SWSH,
        InventoryType.Balls => Pouch_Ball_SWSH,
        InventoryType.BattleItems => Pouch_Battle_SWSH,
        InventoryType.Berries => Pouch_Berries_SWSH,
        InventoryType.Items => Pouch_Regular_SWSH,
        InventoryType.TMHMs => Pouch_TMTR_SWSH,
        InventoryType.Treasure => Pouch_Treasure_SWSH,
        InventoryType.Candy => Pouch_Ingredients_SWSH,
        InventoryType.KeyItems => Pouch_Key_SWSH,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
