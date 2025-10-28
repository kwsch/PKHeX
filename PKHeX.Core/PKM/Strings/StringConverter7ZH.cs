using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for remapping reserved Chinese characters to and from Unicode, used by Generation 7 3DS games.
/// </summary>
/// <remarks>
/// GameFreak chose to unify both Traditional and Simplified fonts, and thus mapped un-nicknamed species name chars to private use characters.
/// This allowed them to use the same font for all languages, and the display names would show uniquely.
/// Future games use separate fonts for Traditional and Simplified, and thus do not need to display both fonts via private use characters.
/// </remarks>
public static class StringConverter7ZH
{
    internal static bool IsPrivateChar(ushort glyph) => glyph is >= Start and <= End;
    internal static char GetUnicodeChar(char glyph) => Table[glyph - Start];

    public static bool IsTraditional(ushort glyph) => glyph is (>= StartTraditional and <= EndTraditional) or (>= StartTraditionalUSUM and <= EndTraditionalUSUM);
    public static bool IsSimplified(ushort glyph) => glyph is (>= StartSimplified and <= EndSimplified) or (>= StartSimplifiedUSUM and <= EndSimplifiedUSUM);

    public static bool IsTraditional(char glyph) => Traditional.Contains(glyph) || TraditionalUSUM.Contains(glyph);
    public static bool IsSimplified(char glyph) => Simplified.Contains(glyph) || SimplifiedUSUM.Contains(glyph);

    /// <summary>
    /// Converts a Unicode character to Generation 7 in-game Chinese codepoint.
    /// </summary>
    /// <param name="chr">Unicode character.</param>
    /// <param name="traditional">Detection of language for Traditional Chinese check</param>
    /// <returns>In-game Chinese string.</returns>
    internal static char GetPrivateChar(char chr, bool traditional)
    {
        if (traditional)
        {
            var index = Traditional.IndexOf(chr);
            if (index >= 0)
                return (char)(StartTraditional + index);
            index = TraditionalUSUM.IndexOf(chr);
            if (index >= 0)
                return (char)(StartTraditionalUSUM + index);
        }
        else
        {
            var index = Simplified.IndexOf(chr);
            if (index >= 0)
                return (char)(StartSimplified + index);
            index = SimplifiedUSUM.IndexOf(chr);
            if (index >= 0)
                return (char)(StartSimplifiedUSUM + index);
        }
        return chr;
    }

    #region Gen 7 Chinese Character Table
    private const ushort Start = 0xE800;

    // S/M
    private const ushort StartSimplified = 0xE800;
    private const ushort EndSimplified    = 0xEB0E; // 0xE800-0xEB0E
    private const ushort StartTraditional = 0xEB0F;
    private const ushort EndTraditional   = 0xEE1D; // 0xEB0F-0xEE1D
    // US/UM: A few more characters at the end.
    private const ushort StartSimplifiedUSUM  = 0xEE1E;
    private const ushort EndSimplifiedUSUM    = 0xEE21; // 0xEE1E-0xEE21
    private const ushort StartTraditionalUSUM = 0xEE22;
    private const ushort EndTraditionalUSUM   = 0xEE26; // 0xEE22-0xEE26

    private const int SimplifiedCount  = EndSimplified - StartSimplified + 1;
    private const int TraditionalCount = EndTraditional - StartTraditional + 1;
    private const int SimplifiedCountUSUM  = EndSimplifiedUSUM - StartSimplifiedUSUM + 1;
    private const int TraditionalCountUSUM = EndTraditionalUSUM - StartTraditionalUSUM + 1;

    private const ushort End = EndTraditionalUSUM;

    private static ReadOnlySpan<char> Table =>
    [
        '蛋', '妙', '蛙', '种', '子', '草', '花', '小', '火', '龙', '恐', '喷', '杰', '尼', '龟', '卡', // E800-E80F
        '咪', '水', '箭', '绿', '毛', '虫', '铁', '甲', '蛹', '巴', '大', '蝶', '独', '角', '壳', '针', // E810-E81F
        '蜂', '波', '比', '鸟', '拉', '达', '烈', '雀', '嘴', '阿', '柏', '蛇', '怪', '皮', '丘', '雷', // E820-E82F
        '穿', '山', '鼠', '王', '多', '兰', '娜', '后', '朗', '力', '诺', '可', '西', '六', '尾', '九', // E830-E83F
        '胖', '丁', '超', '音', '蝠', '走', '路', '臭', '霸', '派', '斯', '特', '球', '摩', '鲁', '蛾', // E840-E84F
        '地', '三', '喵', '猫', '老', '鸭', '哥', '猴', '暴', '蒂', '狗', '风', '速', '蚊', '香', '蝌', // E850-E85F
        '蚪', '君', '泳', '士', '凯', '勇', '基', '胡', '腕', '豪', '喇', '叭', '芽', '口', '呆', '食', // E860-E86F
        '玛', '瑙', '母', '毒', '刺', '拳', '石', '隆', '岩', '马', '焰', '兽', '磁', '合', '一', '葱', // E870-E87F
        '嘟', '利', '海', '狮', '白', '泥', '舌', '贝', '鬼', '通', '耿', '催', '眠', '貘', '引', '梦', // E880-E88F
        '人', '钳', '蟹', '巨', '霹', '雳', '电', '顽', '弹', '椰', '树', '嘎', '啦', '飞', '腿', '郎', // E890-E89F
        '快', '头', '瓦', '双', '犀', '牛', '钻', '吉', '蔓', '藤', '袋', '墨', '金', '鱼', '星', '宝', // E8A0-E8AF
        '魔', '墙', '偶', '天', '螳', '螂', '迷', '唇', '姐', '击', '罗', '肯', '泰', '鲤', '普', '百', // E8B0-E8BF
        '变', '伊', '布', '边', '菊', '化', '盔', '镰', '刀', '翼', '急', '冻', '闪', '你', '哈', '克', // E8C0-E8CF
        '幻', '叶', '月', '桂', '竺', '葵', '锯', '鳄', '蓝', '立', '咕', '夜', '鹰', '芭', '瓢', '安', // E8D0-E8DF
        '圆', '丝', '蛛', '叉', '字', '灯', '笼', '古', '然', '咩', '羊', '茸', '美', '丽', '露', '才', // E8E0-E8EF
        '皇', '毽', '棉', '长', '手', '向', '日', '蜻', '蜓', '乌', '沼', '太', '阳', '亮', '黑', '暗', // E8F0-E8FF
        '鸦', '妖', '未', '知', '图', '腾', '果', '翁', '麒', '麟', '奇', '榛', '佛', '托', '土', '弟', // E900-E90F
        '蝎', '钢', '千', '壶', '赫', '狃', '熊', '圈', '熔', '蜗', '猪', '珊', '瑚', '炮', '章', '桶', // E910-E91F
        '信', '使', '翅', '戴', '加', '象', '顿', 'Ⅱ', '惊', '鹿', '犬', '无', '畏', '战', '舞', '娃', // E920-E92F
        '奶', '罐', '幸', '福', '公', '炎', '帝', '幼', '沙', '班', '洛', '亚', '凤', '时', '木', '守', // E930-E93F
        '宫', '森', '林', '蜥', '蜴', '稚', '鸡', '壮', '跃', '狼', '纹', '直', '冲', '茧', '狩', '猎', // E940-E94F
        '盾', '粉', '莲', '童', '帽', '乐', '河', '橡', '实', '鼻', '狡', '猾', '傲', '骨', '燕', '鸥', // E950-E95F
        '莉', '奈', '朵', '溜', '糖', '雨', '蘑', '菇', '斗', '笠', '懒', '獭', '过', '动', '猿', '请', // E960-E96F
        '假', '居', '忍', '面', '者', '脱', '妞', '吼', '爆', '幕', '下', '掌', '朝', '北', '优', '雅', // E970-E97F
        '勾', '魂', '眼', '那', '恰', '姆', '落', '正', '拍', '负', '萤', '甜', '蔷', '薇', '溶', '吞', // E980-E98F
        '牙', '鲨', '鲸', '驼', '煤', '炭', '跳', '噗', '晃', '斑', '颚', '蚁', '漠', '仙', '歌', '青', // E990-E99F
        '绵', '七', '夕', '鼬', '斩', '饭', '匙', '鳅', '鲶', '虾', '兵', '螯', '秤', '念', '触', '摇', // E9A0-E9AF
        '篮', '羽', '丑', '纳', '飘', '浮', '泡', '隐', '怨', '影', '诅', '咒', '巡', '灵', '彷', '徨', // E9B0-E9BF
        '热', '带', '铃', '勃', '梭', '雪', '冰', '护', '豹', '珍', '珠', '樱', '空', '棘', '爱', '心', // E9C0-E9CF
        '哑', '属', '艾', '欧', '盖', '固', '坐', '祈', '代', '希', '苗', '台', '猛', '曼', '拿', '儿', // E9D0-E9DF
        '狸', '法', '师', '箱', '蟀', '勒', '伦', '琴', '含', '羞', '苞', '槌', '城', '结', '贵', '妇', // E9E0-E9EF
        '绅', '蜜', '女', '帕', '兹', '潜', '兔', '随', '卷', '耳', '魅', '东', '施', '铛', '响', '坦', // E9F0-E9FF
        '铜', '镜', '钟', '盆', '聒', '噪', '陆', '尖', '咬', '不', '良', '骷', '荧', '光', '霓', '虹', // EA00-EA0F
        '自', '舔', '狂', '远', 'Ｚ', '由', '卢', '席', '恩', '骑', '色', '霏', '莱', '谢', '米', '尔', // EA10-EA1F
        '宙', '提', '主', '暖', '炒', '武', '刃', '丸', '剑', '探', '步', '哨', '约', '扒', '酷', '冷', // EA20-EA2F
        '蚀', '豆', '鸽', '高', '雉', '幔', '庞', '滚', '蝙', '螺', '钉', '差', '搬', '运', '匠', '修', // EA30-EA3F
        '建', '蟾', '蜍', '投', '摔', '打', '包', '保', '足', '蜈', '蚣', '车', '轮', '精', '根', '裙', // EA40-EA4F
        '野', '蛮', '鲈', '混', '流', '氓', '红', '倒', '狒', '殿', '滑', '巾', '征', '哭', '具', '死', // EA50-EA5F
        '神', '棺', '原', '肋', '始', '祖', '破', '灰', '尘', '索', '沫', '栗', '德', '单', '卵', '细', // EA60-EA6F
        '胞', '造', '鹅', '倍', '四', '季', '萌', '哎', '呀', '败', '轻', '蜘', '坚', '齿', '组', '麻', // EA70-EA7F
        '鳗', '宇', '烛', '幽', '晶', '斧', '嚏', '几', '何', '敏', '捷', '功', '夫', '父', '赤', '驹', // EA80-EA8F
        '劈', '司', '令', '炸', '雄', '秃', '丫', '首', '恶', '燃', '烧', '毕', '云', '酋', '迪', '耶', // EA90-EA9F
        '塔', '赛', '里', '狐', '呱', '贺', '掘', '彩', '蓓', '洁', '能', '鞘', '芳', '芙', '妮', '好', // EAA0-EAAF
        '鱿', '贼', '脚', '铠', '垃', '藻', '臂', '枪', '伞', '咚', '碎', '黏', '钥', '朽', '南', '瓜', // EAB0-EABF
        '嗡', '哲', '裴', '格', '枭', '狙', '射', '炽', '咆', '哮', '虎', '漾', '壬', '笃', '啄', '铳', // EAC0-EACF
        '少', '强', '锹', '农', '胜', '虻', '鬃', '弱', '坏', '驴', '仔', '重', '挽', '滴', '伪', '睡', // EAD0-EADF
        '罩', '盗', '着', '竹', '疗', '环', '智', '挥', '猩', '掷', '胆', '噬', '堡', '爷', '参', '性', // EAE0-EAEF
        '：', '银', '伴', '陨', '枕', '戈', '谜', '拟', 'Ｑ', '磨', '舵', '鳞', '杖', '璞', '・', '鸣', // EAF0-EAFF
        '哞', '鳍', '科', '莫', '迦', '虚', '吾', '肌', '费', '束', '辉', '纸', '御', '机', '夏', '蛋', // EB00-EB0F
        '妙', '蛙', '種', '子', '草', '花', '小', '火', '龍', '恐', '噴', '傑', '尼', '龜', '卡', '咪', // EB10-EB1F
        '水', '箭', '綠', '毛', '蟲', '鐵', '甲', '蛹', '巴', '大', '蝶', '獨', '角', '殼', '針', '蜂', // EB20-EB2F
        '波', '比', '鳥', '拉', '達', '烈', '雀', '嘴', '阿', '柏', '蛇', '怪', '皮', '丘', '雷', '穿', // EB30-EB3F
        '山', '鼠', '王', '多', '蘭', '娜', '后', '朗', '力', '諾', '可', '西', '六', '尾', '九', '胖', // EB40-EB4F
        '丁', '超', '音', '蝠', '走', '路', '臭', '霸', '派', '斯', '特', '球', '摩', '魯', '蛾', '地', // EB50-EB5F
        '三', '喵', '貓', '老', '鴨', '哥', '猴', '爆', '蒂', '狗', '風', '速', '蚊', '香', '蝌', '蚪', // EB60-EB6F
        '君', '泳', '士', '凱', '勇', '基', '胡', '腕', '豪', '喇', '叭', '芽', '口', '呆', '食', '瑪', // EB70-EB7F
        '瑙', '母', '毒', '刺', '拳', '石', '隆', '岩', '馬', '焰', '獸', '磁', '合', '一', '蔥', '嘟', // EB80-EB8F
        '利', '海', '獅', '白', '泥', '舌', '貝', '鬼', '通', '耿', '催', '眠', '貘', '引', '夢', '人', // EB90-EB9F
        '鉗', '蟹', '巨', '霹', '靂', '電', '頑', '彈', '椰', '樹', '嘎', '啦', '飛', '腿', '郎', '快', // EBA0-EBAF
        '頭', '瓦', '雙', '犀', '牛', '鑽', '吉', '蔓', '藤', '袋', '墨', '金', '魚', '星', '寶', '魔', // EBB0-EBBF
        '牆', '偶', '天', '螳', '螂', '迷', '唇', '姐', '擊', '羅', '肯', '泰', '鯉', '暴', '普', '百', // EBC0-EBCF
        '變', '伊', '布', '邊', '菊', '化', '盔', '鐮', '刀', '翼', '急', '凍', '閃', '你', '哈', '克', // EBD0-EBDF
        '幻', '葉', '月', '桂', '竺', '葵', '鋸', '鱷', '藍', '立', '咕', '夜', '鷹', '芭', '瓢', '安', // EBE0-EBEF
        '圓', '絲', '蛛', '叉', '字', '燈', '籠', '古', '然', '咩', '羊', '茸', '美', '麗', '露', '才', // EBF0-EBFF
        '皇', '毽', '棉', '長', '手', '向', '日', '蜻', '蜓', '烏', '沼', '太', '陽', '亮', '黑', '暗', // EC00-EC0F
        '鴉', '妖', '未', '知', '圖', '騰', '果', '翁', '麒', '麟', '奇', '榛', '佛', '托', '土', '弟', // EC10-EC1F
        '蠍', '鋼', '千', '壺', '赫', '狃', '熊', '圈', '熔', '蝸', '豬', '珊', '瑚', '炮', '章', '桶', // EC20-EC2F
        '信', '使', '翅', '戴', '加', '象', '頓', 'Ⅱ', '驚', '鹿', '犬', '無', '畏', '戰', '舞', '娃', // EC30-EC3F
        '奶', '罐', '幸', '福', '公', '炎', '帝', '幼', '沙', '班', '洛', '亞', '鳳', '時', '木', '守', // EC40-EC4F
        '宮', '森', '林', '蜥', '蜴', '稚', '雞', '壯', '躍', '狼', '紋', '直', '衝', '繭', '狩', '獵', // EC50-EC5F
        '盾', '粉', '蓮', '童', '帽', '樂', '河', '橡', '實', '鼻', '狡', '猾', '傲', '骨', '燕', '鷗', // EC60-EC6F
        '莉', '奈', '朵', '溜', '糖', '雨', '蘑', '菇', '斗', '笠', '懶', '獺', '過', '動', '猿', '請', // EC70-EC7F
        '假', '居', '忍', '面', '者', '脫', '妞', '吼', '幕', '下', '掌', '朝', '北', '優', '雅', '勾', // EC80-EC8F
        '魂', '眼', '那', '恰', '姆', '落', '正', '拍', '負', '螢', '甜', '薔', '薇', '溶', '吞', '牙', // EC90-EC9F
        '鯊', '鯨', '駝', '煤', '炭', '跳', '噗', '晃', '斑', '顎', '蟻', '漠', '仙', '歌', '青', '綿', // ECA0-ECAF
        '七', '夕', '鼬', '斬', '飯', '匙', '鰍', '鯰', '蝦', '兵', '螯', '秤', '念', '觸', '搖', '籃', // ECB0-ECBF
        '羽', '醜', '納', '飄', '浮', '泡', '隱', '怨', '影', '詛', '咒', '巡', '靈', '彷', '徨', '熱', // ECC0-ECCF
        '帶', '鈴', '勃', '梭', '雪', '冰', '護', '豹', '珍', '珠', '櫻', '空', '棘', '愛', '心', '啞', // ECD0-ECDF
        '屬', '艾', '歐', '蓋', '固', '坐', '祈', '代', '希', '苗', '台', '猛', '曼', '拿', '兒', '狸', // ECE0-ECEF
        '法', '師', '箱', '蟀', '勒', '倫', '琴', '含', '羞', '苞', '槌', '城', '結', '貴', '婦', '紳', // ECF0-ECFF
        '蜜', '女', '帕', '茲', '潛', '兔', '隨', '捲', '耳', '魅', '東', '施', '鐺', '響', '坦', '銅', // ED00-ED0F
        '鏡', '鐘', '盆', '聒', '噪', '陸', '尖', '咬', '不', '良', '骷', '光', '霓', '虹', '自', '舔', // ED10-ED1F
        '狂', '遠', 'Ｚ', '由', '盧', '席', '恩', '騎', '色', '霏', '萊', '謝', '米', '爾', '宙', '提', // ED20-ED2F
        '主', '暖', '炒', '武', '刃', '丸', '劍', '探', '步', '哨', '約', '扒', '酷', '冷', '蝕', '豆', // ED30-ED3F
        '鴿', '高', '雉', '幔', '龐', '滾', '蝙', '螺', '釘', '差', '搬', '運', '匠', '修', '建', '蟾', // ED40-ED4F
        '蜍', '投', '摔', '打', '包', '保', '足', '蜈', '蚣', '車', '輪', '毬', '精', '根', '裙', '野', // ED50-ED5F
        '蠻', '鱸', '混', '流', '氓', '紅', '倒', '狒', '殿', '滑', '巾', '徵', '哭', '具', '死', '神', // ED60-ED6F
        '棺', '原', '肋', '始', '祖', '破', '灰', '塵', '索', '沫', '栗', '德', '單', '卵', '細', '胞', // ED70-ED7F
        '造', '鵝', '倍', '四', '季', '萌', '哎', '呀', '敗', '輕', '蜘', '堅', '齒', '組', '麻', '鰻', // ED80-ED8F
        '宇', '燭', '幽', '晶', '斧', '嚏', '幾', '何', '敏', '捷', '功', '夫', '父', '赤', '駒', '劈', // ED90-ED9F
        '司', '令', '炸', '雄', '禿', '丫', '首', '惡', '燃', '燒', '畢', '雲', '酋', '迪', '耶', '塔', // EDA0-EDAF
        '賽', '里', '狐', '呱', '賀', '掘', '彩', '蓓', '潔', '能', '鞘', '芳', '芙', '妮', '好', '魷', // EDB0-EDBF
        '賊', '腳', '鎧', '垃', '藻', '臂', '槍', '傘', '咚', '碎', '黏', '鑰', '朽', '南', '瓜', '嗡', // EDC0-EDCF
        '哲', '裴', '格', '梟', '狙', '射', '熾', '咆', '哮', '虎', '漾', '壬', '篤', '啄', '銃', '少', // EDD0-EDDF
        '強', '鍬', '農', '勝', '虻', '鬃', '弱', '壞', '驢', '仔', '重', '挽', '滴', '偽', '睡', '罩', // EDE0-EDEF
        '盜', '著', '竹', '療', '環', '智', '揮', '猩', '擲', '膽', '噬', '堡', '爺', '參', '性', '：', // EDF0-EDFF
        '銀', '伴', '隕', '枕', '戈', '謎', '擬', 'Ｑ', '磨', '舵', '鱗', '杖', '璞', '・', '鳴', '哞', // EE00-EE0F
        '鰭', '科', '莫', '迦', '虛', '吾', '肌', '費', '束', '輝', '紙', '御', '機', '夏', '垒', '磊', // EE10-EE1F
        '砰', '奥', '壘', '磊', '砰', '丑', '奧', // EE20-EE26
    ];

    private static ReadOnlySpan<char> Simplified => Table[..SimplifiedCount];
    private static ReadOnlySpan<char> Traditional => Table.Slice(SimplifiedCount, TraditionalCount);
    private static ReadOnlySpan<char> SimplifiedUSUM => Table.Slice(SimplifiedCount + TraditionalCount, SimplifiedCountUSUM);
    private static ReadOnlySpan<char> TraditionalUSUM => Table.Slice(SimplifiedCount + TraditionalCount + SimplifiedCountUSUM, TraditionalCountUSUM);
    #endregion
}
