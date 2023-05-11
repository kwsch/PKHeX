using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.SaveBlockAccessor9SV;

namespace PKHeX.Core;

public static class PlayerFashionUnlock9
{
    // All base game clothing options
    private static ReadOnlySpan<ushort> BaseBags => new ushort[]
    {
        3044, 3002, 3033, 3034, 3035, 3036, 3037, 3038, 3039, 3040, 3041, 3042, 3043, 3045, 3046,
        3003, 3004, 3005, 3006, 3007, 3008, 3009, 3010, 3011, 3012, 3013, 3014, 3015, 3016, 3017, 3018, 3019, 3020, 3021, 3022,
        3076, 3077, 3078, 3079, 3080, 3081, 3082, 3083, 3084, 3085, 3086, 3087, 3088, 3089, 3090, 3091, 3092, 3064, 3065, 3066,
        3023, 3024, 3025, 3026, 3027, 3028, 3029, 3030, 3031, 3032, 3073, 3074, 3050, 3051, 3052, 3053, 3054, 3055, 3056, 3057,
        3058, 3059, 3060, 3061, 3062, 3067, 3063, 3068, 3069, 3070, 3071, 3072,
    };

    private static ReadOnlySpan<ushort> BaseCases => new ushort[]
    {
        8005, 8046, 8002, 8003, 8004, 8006, 8007, 8009, 8008, 8010, 8011, 8012, 8013, 8014, 8015,
        8026, 8027, 8028, 8029, 8030, 8031, 8032, 8033, 8034, 8035, 8036, 8037, 8039, 8038, 8040, 8041, 8042, 8043, 8045, 8044,
        8047, 8000, 8001, 8016, 8017, 8018, 8019, 8020, 8021, 8022, 8023, 8024, 8025, 8048, 8049, 8050,
    };

    private static ReadOnlySpan<ushort> BaseEyewear => new ushort[]
    {
        1002, 1114, 1145, 1020, 1021, 1022, 1023, 1024, 1025, 1026, 1027, 1028, 1029, 1030, 1031,
        1032, 1033, 1034, 1035, 1036, 1037, 1038, 1001, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1012, 1013, 1014,
        1015, 1016, 1018, 1017, 1019, 1039, 1040, 1041, 1042, 1043, 1044, 1045, 1046, 1047, 1048, 1049, 1050, 1051, 1052, 1053,
        1054, 1055, 1056, 1057, 1058, 1059, 1060, 1061, 1062, 1063, 1064, 1065, 1066, 1067, 1068, 1069, 1070, 1071, 1072, 1073,
        1074, 1075, 1076, 1077, 1094, 1095, 1096, 1097, 1098, 1099, 1100, 1101, 1102, 1103, 1104, 1105, 1106, 1107, 1108, 1109,
        1110, 1111, 1112, 1113, 1128, 1129, 1130, 1132, 1131, 1133, 1134, 1135, 1136, 1137, 1138, 1139, 1140, 1141, 1142, 1143,
        1144, 1146, 1078, 1079, 1080, 1081, 1082, 1083, 1084, 1085, 1086, 1087, 1088, 1089, 1090, 1091, 1092, 1093, 1115, 1116,
        1117, 1118, 1119, 1120, 1121, 1122, 1123, 1124, 1125, 1126, 1127,
    };

    private static ReadOnlySpan<ushort> BaseFootwear => new ushort[]
    {
        4002, 4001, 4112, 4125, 4123, 4003, 4004, 4005, 4007, 4006, 4008, 4009, 4010, 4011, 4012,
        4013, 4014, 4015, 4019, 4020, 4021, 4022, 4023, 4024, 4025, 4026, 4027, 4028, 4029, 4030, 4031, 4032, 4033, 4034, 4035,
        4036, 4037, 4038, 4039, 4040, 4041, 4071, 4072, 4073, 4081, 4082, 4083, 4064, 4065, 4066, 4067, 4068, 4069, 4070, 4074,
        4075, 4076, 4077, 4078, 4079, 4080, 4084, 4085, 4086, 4087, 4088, 4089, 4090, 4091, 4092, 4093, 4094, 4095, 4096, 4098,
        4097, 4099, 4100, 4101, 4102, 4103, 4104, 4105, 4106, 4107, 4108, 4110, 4109, 4111, 4113, 4114, 4115, 4116, 4117, 4118,
        4119, 4120, 4122, 4121, 4124, 4126, 4127, 4128, 4129, 4130, 4131, 4132, 4133, 4134, 4135, 4136, 4137, 4138, 4139, 4140,
        4141, 4142, 4143, 4016, 4017, 4018, 4042, 4043, 4044, 4051, 4052, 4053, 4054, 4055, 4056, 4057, 4058, 4059, 4060, 4061,
        4062, 4063, 4045, 4046, 4047, 4048, 4049, 4050,
    };

    private static ReadOnlySpan<ushort> BaseGloves => new ushort[]
    {
        2001, 2020, 2023, 2068, 2069, 2070, 2071, 2072, 2073, 2074, 2075, 2076, 2077, 2078, 2079,
        2080, 2081, 2094, 2095, 2096, 2002, 2003, 2004, 2005, 2006, 2007, 2008, 2009, 2011, 2010, 2012, 2013, 2014, 2015, 2058,
        2059, 2060, 2061, 2062, 2082, 2083, 2084, 2086, 2085, 2087, 2030, 2031, 2032, 2033, 2034, 2035, 2036, 2037, 2038, 2039,
        2040, 2041, 2042, 2044, 2043, 2045, 2046, 2047, 2049, 2048, 2050, 2051, 2052, 2053, 2054, 2055, 2056, 2057, 2063, 2064,
        2065, 2066, 2067, 2088, 2089, 2090, 2091, 2092, 2093, 2016, 2017, 2018, 2019, 2021, 2022, 2024, 2025, 2026, 2027, 2028,
        2029,
    };

    private static ReadOnlySpan<ushort> BaseHeadwear => new ushort[]
    {
        5002, 5001, 5244, 5003, 5004, 5005, 5006, 5007, 5008, 5009, 5010, 5011, 5012, 5013, 5014,
        5015, 5016, 5017, 5018, 5019, 5020, 5021, 5022, 5023, 5024, 5025, 5026, 5027, 5028, 5029, 5030, 5031, 5032, 5033, 5040,
        5041, 5042, 5043, 5044, 5045, 5046, 5047, 5048, 5049, 5050, 5051, 5052, 5053, 5054, 5055, 5056, 5057, 5074, 5075, 5076,
        5077, 5078, 5079, 5080, 5081, 5082, 5083, 5084, 5085, 5086, 5087, 5088, 5089, 5090, 5058, 5059, 5060, 5061, 5062, 5063,
        5064, 5065, 5066, 5067, 5068, 5069, 5070, 5091, 5092, 5093, 5094, 5095, 5096, 5097, 5098, 5099, 5100, 5101, 5102, 5103,
        5104, 5105, 5106, 5107, 5142, 5143, 5144, 5145, 5146, 5147, 5148, 5149, 5150, 5151, 5152, 5153, 5154, 5155, 5156, 5157,
        5158, 5108, 5109, 5110, 5111, 5112, 5113, 5114, 5115, 5116, 5117, 5118, 5119, 5120, 5121, 5122, 5123, 5124, 5210, 5211,
        5212, 5213, 5214, 5215, 5216, 5217, 5218, 5219, 5220, 5221, 5222, 5223, 5224, 5225, 5226, 5227, 5228, 5229, 5232, 5230,
        5231, 5233, 5234, 5235, 5236, 5237, 5239, 5238, 5240, 5241, 5242, 5243, 5245, 5246, 5247, 5248, 5249, 5250, 5251, 5252,
        5253, 5254, 5255, 5256, 5257, 5258, 5259, 5260, 5261, 5262, 5263, 5264, 5265, 5266, 5267, 5268, 5269, 5270, 5271, 5272,
        5273, 5274, 5275, 5276, 5277, 5278, 5279, 5280, 5281, 5282, 5283, 5284, 5285, 5286, 5287, 5288, 5289, 5290, 5291, 5292,
        5293, 5361, 5362, 5363, 5364, 5366, 5365, 5367, 5368, 5369, 5370, 5371, 5372, 5373, 5374, 5375, 5376, 5377, 5378, 5379,
        5380, 5381, 5382, 5384, 5383, 5385, 5386, 5387, 5388, 5389, 5390, 5391, 5392, 5393, 5394, 5395, 5396, 5397, 5398, 5399,
        5400, 5401, 5402, 5403, 5404, 5405, 5406, 5407, 5408, 5409, 5410, 5411, 5412, 5034, 5035, 5036, 5125, 5126, 5127, 5128,
        5129, 5130, 5131, 5132, 5133, 5134, 5135, 5136, 5137, 5138, 5139, 5140, 5141, 5159, 5160, 5161, 5162, 5163, 5164, 5165,
        5166, 5167, 5168, 5169, 5170, 5171, 5172, 5173, 5174, 5175, 5193, 5194, 5195, 5196, 5197, 5198, 5199, 5200, 5201, 5202,
        5203, 5204, 5205, 5206, 5207, 5208, 5209, 5176, 5177, 5178, 5179, 5180, 5181, 5182, 5183, 5184, 5185, 5186, 5187, 5188,
        5189, 5190, 5191, 5192, 5071, 5072, 5073, 5294, 5295, 5296, 5297, 5298, 5299, 5300, 5301, 5302, 5303, 5304, 5305, 5306,
        5307, 5308, 5309, 5310, 5311, 5312, 5313, 5314, 5315, 5316, 5317, 5318, 5319, 5320, 5321, 5322, 5323, 5324, 5325, 5326,
        5327, 5328, 5329, 5330, 5331, 5332, 5333, 5334, 5335, 5336, 5337, 5338, 5339, 5340, 5341, 5342, 5343, 5344, 5345, 5346,
        5347, 5348, 5349, 5350, 5351, 5352, 5353, 5354, 5355, 5356, 5357, 5358, 5359, 5360,
    };

    private static ReadOnlySpan<ushort> BaseLegwear => new ushort[]
    {
        6045, 6000, 6001, 6002, 6003, 6004, 6005, 6006, 6007, 6008, 6009, 6010, 6011, 6012, 6013,
        6014, 6015, 6016, 6017, 6018, 6020, 6019, 6021, 6022, 6023, 6024, 6025, 6026, 6027, 6028, 6029, 6030, 6031, 6032, 6033,
        6034, 6097, 6098, 6099, 6049, 6050, 6051, 6052, 6053, 6103, 6104, 6105, 6035, 6036, 6037, 6038, 6039, 6040, 6041, 6042,
        6043, 6044, 6046, 6047, 6054, 6048, 6055, 6056, 6057, 6058, 6059, 6060, 6061, 6062, 6063, 6064, 6065, 6066, 6067, 6068,
        6069, 6070, 6071, 6072, 6073, 6074, 6075, 6076, 6077, 6078, 6079, 6080, 6081, 6082, 6083, 6084, 6085, 6086, 6087, 6088,
        6089, 6090, 6091, 6092, 6093, 6094, 6095, 6096, 6100, 6101, 6102,
    };

    private static ReadOnlySpan<ushort> BaseUniformsMale => new ushort[] { 7000, 7001, 7002, 7003 };
    private static ReadOnlySpan<ushort> BaseUniformsFemale => new ushort[] { 7004, 7005, 7006, 7007 };

    // Not purchasable base clothing options
    private static ReadOnlySpan<ushort> LeagueStarHeadwear => new ushort[] { 5037, 5038 };

    // Previous games phone cases
    private static ReadOnlySpan<ushort> CasesRotometery => new ushort[] { 8051, 8052, 8053, 8054 };

    // DLCs preorder clothing options
    private static ReadOnlySpan<ushort> PreorderBags => new ushort[] { 3093 };
    private static ReadOnlySpan<ushort> PreorderFootwear => new ushort[] { 4144 };
    private static ReadOnlySpan<ushort> PreorderGloves => new ushort[] { 2097 };
    private static ReadOnlySpan<ushort> PreorderHeadwear => new ushort[] { 5413 };
    private static ReadOnlySpan<ushort> PreorderLegwear => new ushort[] { 6107, 6108 };
    private static ReadOnlySpan<ushort> PreorderUniformsFemale => new ushort[] { 7008, 7009, 7010, 7011 };
    private static ReadOnlySpan<ushort> PreorderUniformsMale => new ushort[] { 7012, 7013, 7014, 7015 };

    // PokéPortal special clothing options
    private static ReadOnlySpan<ushort> GiftBags => new ushort[]
    {
        3047, // canvas_pokeball
        3048, // canvas_megaball
        3049, // canvas_ultraball

        3075, // floral_print
    };

    public static int Add(SCBlockAccessor acc, uint key, ReadOnlySpan<ushort> add)
    {
        var block = acc.GetBlock(key);
        var data = block.Data;
        return Add(data, add);
    }

    private const int entrySize = 8;

    public static int Add(Span<byte> data, ReadOnlySpan<ushort> add)
    {
        var missing = new HashSet<ushort>(add.Length);
        foreach (var item in add)
            missing.Add(item);

        while (data.Length >= entrySize)
        {
            var item = ReadUInt16LittleEndian(data);
            if (item == ushort.MaxValue)
                break;
            missing.Remove(item);
            data = data[entrySize..];
        }
        if (missing.Count == 0)
            return 0;
        return AddItems(data, missing);
    }

    private static int AddItems(Span<byte> data, IEnumerable<ushort> missing)
    {
        int added = 0;
        foreach (var item in missing)
        {
            if (data.Length < entrySize)
                break;

            WriteUInt16LittleEndian(data, item);
            WriteUInt16LittleEndian(data[2..], 0);
            WriteUInt16LittleEndian(data[4..], 1);
            WriteUInt16LittleEndian(data[6..], 0);
            added++;
            data = data[entrySize..];
        }
        return added;
    }

    public static int UnlockBase(SCBlockAccessor acc, int playerGender)
    {
        int count = 0;
        count += Add(acc, KFashionUnlockedBag, BaseBags);
        count += Add(acc, KFashionUnlockedPhoneCase, BaseCases);
        count += Add(acc, KFashionUnlockedEyewear, BaseEyewear);
        count += Add(acc, KFashionUnlockedFootwear, BaseFootwear);
        count += Add(acc, KFashionUnlockedGloves, BaseGloves);
        count += Add(acc, KFashionUnlockedHeadwear, BaseHeadwear);
        count += Add(acc, KFashionUnlockedLegwear, BaseLegwear);

        if (playerGender == 0)
        {
            count += Add(acc, KFashionUnlockedUniform, BaseUniformsMale);
        }
        else if (playerGender == 1)
        {
            count += Add(acc, KFashionUnlockedUniform, BaseUniformsFemale);
        }

        return count;
    }

    public static int UnlockExtras(SCBlockAccessor acc)
    {
        int count = 0;
        count += Add(acc, KFashionUnlockedPhoneCase, CasesRotometery);
        count += Add(acc, KFashionUnlockedHeadwear, LeagueStarHeadwear);

        return count;
    }

    public static int UnlockPortal(SCBlockAccessor acc)
    {
        int count = 0;
        count += Add(acc, KFashionUnlockedBag, GiftBags);

        return count;
    }

    public static int UnlockPreorder(SCBlockAccessor acc, int playerGender)
    {
        int count = 0;
        count += Add(acc, KFashionUnlockedBag, PreorderBags);
        count += Add(acc, KFashionUnlockedFootwear, PreorderFootwear);
        count += Add(acc, KFashionUnlockedGloves, PreorderGloves);
        count += Add(acc, KFashionUnlockedHeadwear, PreorderHeadwear);
        count += Add(acc, KFashionUnlockedLegwear, PreorderLegwear);

        if (playerGender == 0)
        {
            count += Add(acc, KFashionUnlockedUniform, PreorderUniformsMale);
        }
        else if (playerGender == 1)
        {
            count += Add(acc, KFashionUnlockedUniform, PreorderUniformsFemale);
        }

        return count;
    }
}
