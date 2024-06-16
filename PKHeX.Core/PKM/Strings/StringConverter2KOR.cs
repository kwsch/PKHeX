using System;
using static PKHeX.Core.StringConverter1;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="string"/> for Generation 2 Korean Gold/Silver.
/// </summary>
public static class StringConverter2KOR
{
    /// <summary>
    /// Checks if any of the characters inside <see cref="str"/> are from the special Korean codepoint pages.
    /// </summary>
    public static bool GetIsKorean(ReadOnlySpan<char> str)
    {
        foreach (var c in str)
        {
            if (!IsKoreanChar(c))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Converts Generation 2 Korean encoded data into a string.
    /// </summary>
    /// <param name="data">Encoded data.</param>
    /// <returns>Decoded string.</returns>
    public static string GetString(ReadOnlySpan<byte> data)
    {
        Span<char> result = stackalloc char[data.Length];
        int length = LoadString(data, result);
        return new string(result[..length]);
    }

    /// <inheritdoc cref="GetString(ReadOnlySpan{byte})"/>
    /// <param name="data">Encoded data</param>
    /// <param name="result">Decoded character result buffer</param>
    /// <returns>Character count loaded.</returns>
    public static int LoadString(ReadOnlySpan<byte> data, Span<char> result)
    {
        if (data.Length == 0)
            return 0;
        if (data[0] == TradeOTCode) // In-game Trade
        {
            result[0] = TradeOT;
            return 1;
        }

        int i = 0;
        int ctr = 0;
        for (; i < data.Length; i++)
        {
            var value = data[i];
            if (value == TableInvalid) // Don't bother.
                break;

            ReadOnlySpan<char> table;
            if (value > TableMax)
            {
                table = Table0;
            }
            else
            {
                if (++i == data.Length)
                    break;
                table = GetTable(value);
                value = data[i];
            }

            var c = table[value];
            if (c == Terminator) // Stop if Terminator
                break;
            result[ctr++] = c;
        }
        return ctr;
    }

    /// <summary>
    /// Gets the count of characters in the data.
    /// </summary>
    public static int GetStringLength(ReadOnlySpan<byte> data)
    {
        int length = 0;
        for (var i = 0; i < data.Length; i++)
        {
            byte value = data[i];
            if (value is TableInvalid or TerminatorCode)
                break;
            if (value <= TableMax)
                i++; // Korean char takes 2 bytes
            length++;
        }
        return length;
    }

    /// <summary>
    /// Gets the index of the first terminator in the data.
    /// </summary>
    public static int GetTerminatorIndex(ReadOnlySpan<byte> data)
    {
        for (var i = 0; i < data.Length; i++)
        {
            byte value = data[i];
            if (value is TableInvalid or TerminatorCode)
                return i;
            if (value <= TableMax)
                i++; // Korean char takes 2 bytes
        }
        return -1;
    }

    /// <summary>
    /// Converts a string to Generation 1 encoded data.
    /// </summary>
    /// <param name="destBuffer">Span of bytes to write encoded string data</param>
    /// <param name="value">Decoded string.</param>
    /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
    /// <param name="option">Buffer pre-formatting option</param>
    /// <returns>Encoded data.</returns>
    public static int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength,
        StringConverterOption option = StringConverterOption.Clear50)
    {
        if (option is StringConverterOption.ClearZero)
            destBuffer.Clear();
        else if (option is StringConverterOption.Clear50)
            destBuffer.Fill(TerminatorCode);

        if (value.Length == 0)
            return 0;
        // Korean games can't trade for Gen1 In-Game trades with this char, but handle it anyway since the game can handle it.
        if (value[0] == TradeOT) // Handle "[TRAINER]"
        {
            destBuffer[0] = TradeOTCode;
            destBuffer[1] = TerminatorCode;
            return 2;
        }

        if (value.Length > maxLength)
            value = value[..maxLength]; // Hard cap

        int ctr = LoadCharacters(destBuffer, value);
        if (ctr < value.Length)
            destBuffer[ctr++] = TerminatorCode;
        return ctr;
    }

    private static int LoadCharacters(Span<byte> destBuffer, ReadOnlySpan<char> value)
    {
        int ctr = 0;
        foreach (var c in value)
        {
            var (table, val) = GetKoreanChar(c);
            if (table != TableInvalid)
            {
                if (ctr + 2 > destBuffer.Length)
                    break; // adding 2 characters will overflow requested buffer cap
                destBuffer[ctr++] = table;
                destBuffer[ctr++] = val;
            }
            else
            {
                var index = Table0.IndexOf(c);
                if (index is -1 or TerminatorCode)
                    break;
                if (ctr + 1 > destBuffer.Length)
                    break; // adding 1 character will overflow requested buffer cap
                destBuffer[ctr++] = (byte)index;
            }
        }
        return ctr;
    }

    private static (byte Table, byte Value) GetKoreanChar(char c)
    {
        for (var t = TableMin; t <= TableMax; t++)
        {
            var table = GetTable(t);
            var index = table.IndexOf(c);
            if (index != -1)
                return (t, (byte)index);
        }
        return default;
    }

    private static bool IsKoreanChar(char c)
    {
        for (var t = TableMin; t <= TableMax; t++)
        {
            var table = GetTable(t);
            var index = table.IndexOf(c);
            if (index != -1)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Localizes a Gen4+ Korean species name to the localization used in Generation 2 Gold/Silver
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <param name="nick">Generation 4 Species Name</param>
    /// <returns>Localized Name for Generation 2</returns>
    public static void LocalizeKOR2(ushort species, ref string nick)
    {
        if (species == 61) // Poliwhirl
            nick = "수륙챙이"; // "슈륙챙이" in future games
        else if (species == 114) // Tangela
            nick = "덩구리"; // "덩쿠리" in future games
    }

    #region Gen 2 Korean Character Tables
    private const char NULL = Terminator;
    private const byte TableInvalid = 0;
    private const byte TableMin = 1;
    private const byte TableMax = 11;

    private static ReadOnlySpan<char> GetTable(byte table) => table switch
    {
        0x01 => Table1,
        0x02 => Table2,
        0x03 => Table3,
        0x04 => Table4,
        0x05 => Table5,
        0x06 => Table6,
        0x07 => Table7,
        0x08 => Table8,
        0x09 => Table9,
        0x0A => TableA,
        0x0B => TableB,
        _ => throw new ArgumentOutOfRangeException(nameof(table)),
    };

    private static ReadOnlySpan<char> Table1 =>
    [
        NULL, '가', '각', '간', '갇', '갈', '갉', '갊', '감', '갑', '값', '갓', '갔', '강', '갖', '갗',
        '같', '갚', '갛', '개', '객', '갠', '갤', '갬', '갭', '갯', '갰', '갱', '갸', '갹', '갼', '걀',
        '걋', '걍', '걔', '걘', '걜', '거', '걱', '건', '걷', '걸', '걺', '검', '겁', '것', '겄', '겅',
        '겆', '겉', '겊', '겋', '게', '겐', '겔', '겜', '겝', '겟', '겠', '겡', '겨', '격', '겪', '견',
        '겯', '결', '겹', '겸', '겻', '겼', '경', '곁', '계', '곈', '곌', '곕', '곗', '고', '곡', '곤',
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        '곧', '골', '곪', '곬', '곯', '곰', '곱', '곳', '공', '곶', '과', '곽', '관', '괄', '괆', NULL,
        NULL, '괌', '괍', '괏', '광', '괘', '괜', '괠', '괩', '괬', '괭', '괴', '괵', '괸', '괼', '괻',
        '굅', '굇', '굉', '교', '굔', '굘', '굡', '굣', '구', '국', '군', '굳', '굴', '굵', '굶', '굻',
        '굼', '굽', '굿', '궁', '궂', '궈', '궉', '권', '궐', '궜', '궝', '궤', '궷', '귀', '귁', '귄',
        '귈', '귐', '귑', '귓', '규', '균', '귤', '그', '극', '근', '귿', '글', '긁', '금', '급', '긋',
        '긍', '긔', '기', '긱', '긴', '긷', '길', '긺', '김', '깁', '깃', '깅', '깆', '깊', '까', '깍',
        '깎', '깐', '깔', '깖', '깜', '깝', '깟', '깠', '깡', '깥', '깨', '깩', '깬', '깰', '깸', NULL,
        NULL, '깹', '깻', '깼', '깽', '꺄', '꺅', '꺌', '꺼', '꺽', '꺾', '껀', '껄', '껌', '껍', '껏',
        '껐', '껑', '께', '껙', '껜', '껨', '껫', '껭', '껴', '껸', '껼', '꼇', '꼈', '꼍', '꼐', '꼬',
        '꼭', '꼰', '꼲', '꼴', '꼼', '꼽', '꼿', '꽁', '꽂', '꽃', '꽈', '꽉', '꽐', '꽜', '꽝', '꽤',
    ];

    private static ReadOnlySpan<char> Table2 =>
    [
        '꽥', '꽹', '꾀', '꾄', '꾈', '꾐', '꾑', '꾕', '꾜', '꾸', '꾹', '꾼', '꿀', '꿇', '꿈', '꿉',
        '꿋', '꿍', '꿎', '꿔', '꿜', '꿨', '꿩', '꿰', '꿱', '꿴', '꿸', '뀀', '뀁', '뀄', '뀌', '뀐',
        '뀔', '뀜', '뀝', '뀨', '끄', '끅', '끈', '끊', '끌', '끎', '끓', '끔', '끕', '끗', '끙', NULL,
        NULL, '끝', '끼', '끽', '낀', '낄', '낌', '낍', '낏', '낑', '나', '낙', '낚', '난', '낟', '날',
        '낡', '낢', '남', '납', '낫', '났', '낭', '낮', '낯', '낱', '낳', '내', '낵', '낸', '낼', '냄',
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        '냅', '냇', '냈', '냉', '냐', '냑', '냔', '냘', '냠', '냥', '너', '넉', '넋', '넌', '널', '넒',
        '넓', '넘', '넙', '넛', '넜', '넝', '넣', '네', '넥', '넨', '넬', '넴', '넵', '넷', '넸', '넹',
        '녀', '녁', '년', '녈', '념', '녑', '녔', '녕', '녘', '녜', '녠', '노', '녹', '논', '놀', '놂',
        '놈', '놉', '놋', '농', '높', '놓', '놔', '놘', '놜', '놨', '뇌', '뇐', '뇔', '뇜', '뇝', NULL,
        NULL, '뇟', '뇨', '뇩', '뇬', '뇰', '뇹', '뇻', '뇽', '누', '눅', '눈', '눋', '눌', '눔', '눕',
        '눗', '눙', '눠', '눴', '눼', '뉘', '뉜', '뉠', '뉨', '뉩', '뉴', '뉵', '뉼', '늄', '늅', '늉',
        '느', '늑', '는', '늘', '늙', '늚', '늠', '늡', '늣', '능', '늦', '늪', '늬', '늰', '늴', '니',
        '닉', '닌', '닐', '닒', '님', '닙', '닛', '닝', '닢', '다', '닥', '닦', '단', '닫', '달', '닭',
        '닮', '닯', '닳', '담', '답', '닷', '닸', '당', '닺', '닻', '닿', '대', '댁', '댄', '댈', '댐',
        '댑', '댓', '댔', '댕', NULL, '더', '덕', '덖', '던', '덛', '덜', '덞', '덟', '덤', '덥', NULL,
    ];

    private static ReadOnlySpan<char> Table3 =>
    [
        NULL, '덧', '덩', '덫', '덮', '데', '덱', '덴', '델', '뎀', '뎁', '뎃', '뎄', '뎅', '뎌', '뎐',
        '뎔', '뎠', '뎡', '뎨', '뎬', '도', '독', '돈', '돋', '돌', '돎', NULL, '돔', '돕', '돗', '동',
        '돛', '돝', '돠', '돤', '돨', '돼', '됐', '되', '된', '될', '됨', '됩', '됫', '됴', '두', '둑',
        '둔', '둘', '둠', '둡', '둣', '둥', '둬', '뒀', '뒈', '뒝', '뒤', '뒨', '뒬', '뒵', '뒷', '뒹',
        '듀', '듄', '듈', '듐', '듕', '드', '득', '든', '듣', '들', '듦', '듬', '듭', '듯', '등', '듸',
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        '디', '딕', '딘', '딛', '딜', '딤', '딥', '딧', '딨', '딩', '딪', '따', '딱', '딴', '딸', NULL,
        NULL, '땀', '땁', '땃', '땄', '땅', '땋', '때', '땍', '땐', '땔', '땜', '땝', '땟', '땠', '땡',
        '떠', '떡', '떤', '떨', '떪', '떫', '떰', '떱', '떳', '떴', '떵', '떻', '떼', '떽', '뗀', '뗄',
        '뗌', '뗍', '뗏', '뗐', '뗑', '뗘', '뗬', '또', '똑', '똔', '똘', '똥', '똬', '똴', '뙈', '뙤',
        '뙨', '뚜', '뚝', '뚠', '뚤', '뚫', '뚬', '뚱', '뛔', '뛰', '뛴', '뛸', '뜀', '뜁', '뜅', '뜨',
        '뜩', '뜬', '뜯', '뜰', '뜸', '뜹', '뜻', '띄', '띈', '띌', '띔', '띕', '띠', '띤', '띨', '띰',
        '띱', '띳', '띵', '라', '락', '란', '랄', '람', '랍', '랏', '랐', '랑', '랒', '랖', '랗', NULL,
        '뢔', '래', '랙', '랜', '랠', '램', '랩', '랫', '랬', '랭', '랴', '략', '랸', '럇', '량', '러',
        '럭', '런', '럴', '럼', '럽', '럿', '렀', '렁', '렇', '레', '렉', '렌', '렐', '렘', '렙', '렛',
        '렝', '려', '력', '련', '렬', '렴', '렵', '렷', '렸', '령', '례', '롄', '롑', '롓', '로', '록',
    ];

    private static ReadOnlySpan<char> Table4 =>
    [
        '론', '롤', '롬', '롭', '롯', '롱', '롸', '롼', '뢍', '뢨', '뢰', '뢴', '뢸', '룀', '룁', '룃',
        '룅', '료', '룐', '룔', '룝', '룟', '룡', '루', '룩', '룬', '룰', '룸', '룹', '룻', '룽', '뤄',
        '뤘', '뤠', '뤼', '뤽', '륀', '륄', '륌', '륏', '륑', '류', '륙', '륜', '률', '륨', '륩', NULL,
        NULL, '륫', '륭', '르', '륵', '른', '를', '름', '릅', '릇', '릉', '릊', '릍', '릎', '리', '릭',
        '린', '릴', '림', '립', '릿', '링', '마', '막', '만', '많', '맏', '말', '맑', '맒', '맘', '맙',
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        '맛', '망', '맞', '맡', '맣', '매', '맥', '맨', '맬', '맴', '맵', '맷', '맸', '맹', '맺', '먀',
        '먁', '먈', '먕', '머', '먹', '먼', '멀', '멂', '멈', '멉', '멋', '멍', '멎', '멓', '메', '멕',
        '멘', '멜', '멤', '멥', '멧', '멨', '멩', '며', '멱', '면', '멸', '몃', '몄', '명', '몇', '몌',
        '모', '목', '몫', '몬', '몰', '몲', '몸', '몹', '못', '몽', '뫄', '뫈', '뫘', '뫙', '뫼', NULL,
        NULL, '묀', '묄', '묍', '묏', '묑', '묘', '묜', '묠', '묩', '묫', '무', '묵', '묶', '문', '묻',
        '물', '묽', '묾', '뭄', '뭅', '뭇', '뭉', '뭍', '뭏', '뭐', '뭔', '뭘', '뭡', '뭣', '뭬', '뮈',
        '뮌', '뮐', '뮤', '뮨', '뮬', '뮴', '뮷', '므', '믄', '믈', '믐', '믓', '미', '믹', '민', '믿',
        '밀', '밂', '밈', '밉', '밋', '밌', '밍', '및', '밑', '바', '박', '밖', '밗', '반', '받', '발',
        '밝', '밞', '밟', '밤', '밥', '밧', '방', '밭', '배', '백', '밴', '밸', '뱀', '뱁', '뱃', '뱄',
        '뱅', '뱉', '뱌', '뱍', '뱐', '뱝', '버', '벅', '번', '벋', '벌', '벎', '범', '법', '벗', NULL,
    ];

    private static ReadOnlySpan<char> Table5 =>
    [
        NULL, '벙', '벚', '베', '벡', '벤', '벧', '벨', '벰', '벱', '벳', '벴', '벵', '벼', '벽', '변',
        '별', '볍', '볏', '볐', '병', '볕', '볘', '볜', '보', '복', '볶', '본', '볼', '봄', '봅', '봇',
        '봉', '봐', '봔', '봤', '봬', '뵀', '뵈', '뵉', '뵌', '뵐', '뵘', '뵙', '뵤', '뵨', '부', '북',
        '분', '붇', '불', '붉', '붊', '붐', '붑', '붓', '붕', '붙', '붚', '붜', '붤', '붰', '붸', '뷔',
        '뷕', '뷘', '뷜', '뷩', '뷰', '뷴', '뷸', '븀', '븃', '븅', '브', '븍', '븐', '블', '븜', '븝',
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        '븟', '비', '빅', '빈', '빌', '빎', '빔', '빕', '빗', '빙', '빚', '빛', '빠', '빡', '빤', NULL,
        NULL, '빨', '빪', '빰', '빱', '빳', '빴', '빵', '빻', '빼', '빽', '뺀', '뺄', '뺌', '뺍', '뺏',
        '뺐', '뺑', '뺘', '뺙', '뺨', '뻐', '뻑', '뻔', '뻗', '뻘', '뻠', '뻣', '뻤', '뻥', '뻬', '뼁',
        '뼈', '뼉', '뼘', '뼙', '뼛', '뼜', '뼝', '뽀', '뽁', '뽄', '뽈', '뽐', '뽑', '뽕', '뾔', '뾰',
        '뿅', '뿌', '뿍', '뿐', '뿔', '뿜', '뿟', '뿡', '쀼', '쁑', '쁘', '쁜', '쁠', '쁨', '쁩', '삐',
        '삑', '삔', '삘', '삠', '삡', '삣', '삥', '사', '삭', '삯', '산', '삳', '살', '삵', '삶', '삼',
        '삽', '삿', '샀', '상', '샅', '새', '색', '샌', '샐', '샘', '샙', '샛', '샜', '생', '샤', NULL,
        NULL, '샥', '샨', '샬', '샴', '샵', '샷', '샹', '섀', '섄', '섈', '섐', '섕', '서', '석', '섞',
        '섟', '선', '섣', '설', '섦', '섧', '섬', '섭', '섯', '섰', '성', '섶', '세', '섹', '센', '셀',
        '셈', '셉', '셋', '셌', '셍', '셔', '셕', '션', '셜', '셤', '셥', '셧', '셨', '셩', '셰', '셴',
    ];

    private static ReadOnlySpan<char> Table6 =>
    [
        '셸', '솅', '소', '속', '솎', '손', '솔', '솖', '솜', '솝', '솟', '송', '솥', '솨', '솩', '솬',
        '솰', '솽', '쇄', '쇈', '쇌', '쇔', '쇗', '쇘', '쇠', '쇤', '쇨', '쇰', '쇱', '쇳', '쇼', '쇽',
        '숀', '숄', '숌', '숍', '숏', '숑', '수', '숙', '순', '숟', '술', '숨', '숩', '숫', '숭', '쌰',
        '쎼', '숯', '숱', '숲', '숴', '쉈', '쉐', '쉑', '쉔', '쉘', '쉠', '쉥', '쉬', '쉭', '쉰', '쉴',
        '쉼', '쉽', '쉿', '슁', '슈', '슉', '슐', '슘', '슛', '슝', '스', '슥', '슨', '슬', '슭', '슴',
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        '습', '슷', '승', '시', '식', '신', '싣', '실', '싫', '심', '십', '싯', '싱', '싶', '싸', '싹',
        '싻', '싼', '쌀', '쌈', '쌉', '쌌', '쌍', '쌓', '쌔', '쌕', '쌘', '쌜', '쌤', '쌥', '쌨', '쌩',
        '썅', '써', '썩', '썬', '썰', '썲', '썸', '썹', '썼', '썽', '쎄', '쎈', '쎌', '쏀', '쏘', '쏙',
        '쏜', '쏟', '쏠', '쏢', '쏨', '쏩', '쏭', '쏴', '쏵', '쏸', '쐈', '쐐', '쐤', '쐬', '쐰', NULL,
        '쓔', '쐴', '쐼', '쐽', '쑈', '쑤', '쑥', '쑨', '쑬', '쑴', '쑵', '쑹', '쒀', '쒔', '쒜', '쒸',
        '쒼', '쓩', '쓰', '쓱', '쓴', '쓸', '쓺', '쓿', '씀', '씁', '씌', '씐', '씔', '씜', '씨', '씩',
        '씬', '씰', '씸', '씹', '씻', '씽', '아', '악', '안', '앉', '않', '알', '앍', '앎', '앓', '암',
        '압', '앗', '았', '앙', '앝', '앞', '애', '액', '앤', '앨', '앰', '앱', '앳', '앴', '앵', '야',
        '약', '얀', '얄', '얇', '얌', '얍', '얏', '양', '얕', '얗', '얘', '얜', '얠', '얩', '어', '억',
        '언', '얹', '얻', '얼', '얽', '얾', '엄', '업', '없', '엇', '었', '엉', '엊', '엌', '엎', NULL,
    ];

    private static ReadOnlySpan<char> Table7 =>
    [
        NULL, '에', '엑', '엔', '엘', '엠', '엡', '엣', '엥', '여', '역', '엮', '연', '열', '엶', '엷',
        '염', '엽', '엾', '엿', '였', '영', '옅', '옆', '옇', '예', '옌', '옐', '옘', '옙', '옛', '옜',
        '오', '옥', '온', '올', '옭', '옮', '옰', '옳', '옴', '옵', '옷', '옹', '옻', '와', '왁', '완',
        '왈', '왐', '왑', '왓', '왔', '왕', '왜', '왝', '왠', '왬', '왯', '왱', '외', '왹', '왼', '욀',
        '욈', '욉', '욋', '욍', '요', '욕', '욘', '욜', '욤', '욥', '욧', '용', '우', '욱', '운', '울',
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        '욹', '욺', '움', '웁', '웃', '웅', '워', '웍', '원', '월', '웜', '웝', '웠', '웡', '웨', NULL,
        NULL, '웩', '웬', '웰', '웸', '웹', '웽', '위', '윅', '윈', '윌', '윔', '윕', '윗', '윙', '유',
        '육', '윤', '율', '윰', '윱', '윳', '융', '윷', '으', '윽', '은', '을', '읆', '음', '읍', '읏',
        '응', '읒', '읓', '읔', '읕', '읖', '읗', '의', '읜', '읠', '읨', '읫', '이', '익', '인', '일',
        '읽', '읾', '잃', '임', '입', '잇', '있', '잉', '잊', '잎', '자', '작', '잔', '잖', '잗', '잘',
        '잚', '잠', '잡', '잣', '잤', '장', '잦', '재', '잭', '잰', '잴', '잼', '잽', '잿', '쟀', '쟁',
        '쟈', '쟉', '쟌', '쟎', '쟐', '쟘', '쟝', '쟤', '쟨', '쟬', '저', '적', '전', '절', '젊', NULL,
        NULL, '점', '접', '젓', '정', '젖', '제', '젝', '젠', '젤', '젬', '젭', '젯', '젱', '져', '젼',
        '졀', '졈', '졉', '졌', '졍', '졔', '조', '족', '존', '졸', '졺', '좀', '좁', '좃', '종', '좆',
        '좇', '좋', '좌', '좍', '좔', '좝', '좟', '좡', '좨', '좼', '좽', '죄', '죈', '죌', '죔', '죕',
    ];

    private static ReadOnlySpan<char> Table8 =>
    [
        '죗', '죙', '죠', '죡', '죤', '죵', '주', '죽', '준', '줄', '줅', '줆', '줌', '줍', '줏', '중',
        '줘', '줬', '줴', '쥐', '쥑', '쥔', '쥘', '쥠', '쥡', '쥣', '쥬', '쥰', '쥴', '쥼', '즈', '즉',
        '즌', '즐', '즘', '즙', '즛', '증', '지', '직', '진', '짇', '질', '짊', '짐', '집', '짓', NULL,
        '쬬', '징', '짖', '짙', '짚', '짜', '짝', '짠', '짢', '짤', '짧', '짬', '짭', '짯', '짰', '짱',
        '째', '짹', '짼', '쨀', '쨈', '쨉', '쨋', '쨌', '쨍', '쨔', '쨘', '쨩', '쩌', '쩍', '쩐', '쩔',
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        '쩜', '쩝', '쩟', '쩠', '쩡', '쩨', '쩽', '쪄', '쪘', '쪼', '쪽', '쫀', '쫄', '쫌', '쫍', '쫏',
        '쫑', '쫓', '쫘', '쫙', '쫠', '쫬', '쫴', '쬈', '쬐', '쬔', '쬘', '쬠', '쬡', '쭁', '쭈', '쭉',
        '쭌', '쭐', '쭘', '쭙', '쭝', '쭤', '쭸', '쭹', '쮜', '쮸', '쯔', '쯤', '쯧', '쯩', '찌', '찍',
        '찐', '찔', '찜', '찝', '찡', '찢', '찧', '차', '착', '찬', '찮', '찰', '참', '찹', '찻', NULL,
        NULL, '찼', '창', '찾', '채', '책', '챈', '챌', '챔', '챕', '챗', '챘', '챙', '챠', '챤', '챦',
        '챨', '챰', '챵', '처', '척', '천', '철', '첨', '첩', '첫', '첬', '청', '체', '첵', '첸', '첼',
        '쳄', '쳅', '쳇', '쳉', '쳐', '쳔', '쳤', '쳬', '쳰', '촁', '초', '촉', '촌', '촐', '촘', '촙',
        '촛', '총', '촤', '촨', '촬', '촹', '최', '쵠', '쵤', '쵬', '쵭', '쵯', '쵱', '쵸', '춈', '추',
        '축', '춘', '출', '춤', '춥', '춧', '충', '춰', '췄', '췌', '췐', '취', '췬', '췰', '췸', '췹',
        '췻', '췽', '츄', '츈', '츌', '츔', '츙', '츠', '측', '츤', '츨', '츰', '츱', '츳', '층', NULL,
    ];

    private static ReadOnlySpan<char> Table9 =>
    [
        NULL, '치', '칙', '친', '칟', '칠', '칡', '침', '칩', '칫', '칭', '카', '칵', '칸', '칼', '캄',
        '캅', '캇', '캉', '캐', '캑', '캔', '캘', '캠', '캡', '캣', '캤', '캥', '캬', '캭', '컁', '커',
        '컥', '컨', '컫', '컬', '컴', '컵', '컷', '컸', '컹', '케', '켁', '켄', '켈', '켐', '켑', '켓',
        '켕', '켜', '켠', '켤', '켬', '켭', '켯', '켰', '켱', '켸', '코', '콕', '콘', '콜', '콤', '콥',
        '콧', '콩', '콰', '콱', '콴', '콸', '쾀', '쾅', '쾌', '쾡', '쾨', '쾰', '쿄', '쿠', '쿡', '쿤',
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        '쿨', '쿰', '쿱', '쿳', '쿵', '쿼', '퀀', '퀄', '퀑', '퀘', '퀭', '퀴', '퀵', '퀸', '퀼', NULL,
        NULL, '큄', '큅', '큇', '큉', '큐', '큔', '큘', '큠', '크', '큭', '큰', '클', '큼', '큽', '킁',
        '키', '킥', '킨', '킬', '킴', '킵', '킷', '킹', '타', '탁', '탄', '탈', '탉', '탐', '탑', '탓',
        '탔', '탕', '태', '택', '탠', '탤', '탬', '탭', '탯', '탰', '탱', '탸', '턍', '터', '턱', '턴',
        '털', '턺', '텀', '텁', '텃', '텄', '텅', '테', '텍', '텐', '텔', '템', '텝', '텟', '텡', '텨',
        '텬', '텼', '톄', '톈', '토', '톡', '톤', '톨', '톰', '톱', '톳', '통', '톺', '톼', '퇀', '퇘',
        '퇴', '퇸', '툇', '툉', '툐', '투', '툭', '툰', '툴', '툼', '툽', '툿', '퉁', '퉈', '퉜', NULL,
        NULL, '퉤', '튀', '튁', '튄', '튈', '튐', '튑', '튕', '튜', '튠', '튤', '튬', '튱', '트', '특',
        '튼', '튿', '틀', '틂', '틈', '틉', '틋', '틔', '틘', '틜', '틤', '틥', '티', '틱', '틴', '틸',
        '팀', '팁', '팃', '팅', '파', '팍', '팎', '판', '팔', '팖', '팜', '팝', '팟', '팠', '팡', '팥',
    ];

    private static ReadOnlySpan<char> TableA =>
    [
        '패', '팩', '팬', '팰', '팸', '팹', '팻', '팼', '팽', '퍄', '퍅', '퍼', '퍽', '펀', '펄', '펌',
        '펍', '펏', '펐', '펑', '페', '펙', '펜', '펠', '펨', '펩', '펫', '펭', '펴', '편', '펼', '폄',
        '폅', '폈', '평', '폐', '폘', '폡', '폣', '포', '폭', '폰', '폴', '폼', '폽', '폿', '퐁', NULL,
        NULL, '퐈', '퐝', '푀', '푄', '표', '푠', '푤', '푭', '푯', '푸', '푹', '푼', '푿', '풀', '풂',
        '품', '풉', '풋', '풍', '풔', '풩', '퓌', '퓐', '퓔', '퓜', '퓟', '퓨', '퓬', '퓰', '퓸', '퓻',
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        '퓽', '프', '픈', '플', '픔', '픕', '픗', '피', '픽', '핀', '필', '핌', '핍', '핏', '핑', '하',
        '학', '한', '할', '핥', '함', '합', '핫', '항', '해', '핵', '핸', '핼', '햄', '햅', '햇', '했',
        '행', '햐', '향', '허', '헉', '헌', '헐', '헒', '험', '헙', '헛', '헝', '헤', '헥', '헨', '헬',
        '헴', '헵', '헷', '헹', '혀', '혁', '현', '혈', '혐', '협', '혓', '혔', '형', '혜', '혠', NULL,
        NULL, '혤', '혭', '호', '혹', '혼', '홀', '홅', '홈', '홉', '홋', '홍', '홑', '화', '확', '환',
        '활', '홧', '황', '홰', '홱', '홴', '횃', '횅', '회', '획', '횐', '횔', '횝', '횟', '횡', '효',
        '횬', '횰', '횹', '횻', '후', '훅', '훈', '훌', '훑', '훔', '훗', '훙', '훠', '훤', '훨', '훰',
        '훵', '훼', '훽', '휀', '휄', '휑', '휘', '휙', '휜', '휠', '휨', '휩', '휫', '휭', '휴', '휵',
        '휸', '휼', '흄', '흇', '흉', '흐', '흑', '흔', '흖', '흗', '흘', '흙', '흠', '흡', '흣', '흥',
        '흩', '희', '흰', '흴', '흼', '흽', '힁', '히', '힉', '힌', '힐', '힘', '힙', '힛', '힝', NULL,
    ];

    private static ReadOnlySpan<char> TableB =>
    [
        'ㄱ', 'ㄴ', 'ㄷ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅅ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ', 'ㄲ', 'ㄸ',
        'ㅃ', 'ㅆ', 'ㅉ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        'ㅏ', 'ㅑ', 'ㅓ', 'ㅕ', 'ㅗ', 'ㅛ', 'ㅜ', 'ㅠ', 'ㅡ', 'ㅣ', 'ㅐ', 'ㅒ', 'ㅔ', 'ㅖ', 'ㅘ', 'ㅙ',
        'ㅚ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅢ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '_', '—',
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        '「', '」', '『', '』', '(', ')', '!', '?', '-', '~', '…', ',', '.', NULL, NULL, NULL,
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
        '０', '１', '２', '３', '４', '５', '６', '７', '８', '９', NULL, NULL, NULL, NULL, NULL, '　',
    ];

    // In transporter's code, none of these glyphs are legitimately accessible via keyboard.
    private const char NUL = NULL;
    private static ReadOnlySpan<char> Table0 =>
    [
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL,
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL,
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL,
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL,
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL,
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL,
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL,
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, ' ',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
        'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', NUL, NUL, NUL, NUL, NUL, NUL,
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
        'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', NUL, NUL, NUL, NUL, NUL, NUL,
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL,
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL,
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, '♂',
        NUL, '×', NUL, '/', NUL, '♀', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
    ];

    #endregion
}
