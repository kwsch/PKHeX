using System;

namespace PKHeX.Core;

/// <summary>
/// List of <see cref="PK1"/> prefixed by a count.
/// </summary>
public static class PokeList1
{
    // Structure:
    // u8               Count of slots filled
    // s8[capacity+1]   GB Species ID in Slot (-1 = no species)
    // pkx[capacity]    GB PKM data (no strings)
    // str[capacity]    Trainer Name
    // str[capacity]    Nickname
    // 
    // where,
    // - str has variable size (jp/int)
    // - pkx is different size for Gen1/Gen2

    public const byte SlotEmpty = 0xFF;

    /// <summary>
    /// Gets the species identifier for the slot.
    /// </summary>
    public static byte GetHeaderIdentifierMark(PK1 pk)
    {
        var species = pk.SpeciesInternal;
        if (species == 0)
            return SlotEmpty;
        // Let out-of-bounds species fall through.
        return species;
    }

    /// <summary>
    /// Checks if the slot has content present.
    /// </summary>
    /// <param name="marker">Identifying summary mark from the list header.</param>
    /// <returns>True if present.</returns>
    private static bool IsPresent(byte marker) => marker is not (0 or SlotEmpty);

    /// <summary>
    /// Counts the number of present slots in the list.
    /// </summary>
    /// <param name="input">List header</param>
    /// <param name="capacity">Count of slots allowed in the list</param>
    /// <param name="lerp">Offset jump between slot indexes; used to parse multi-single lists instead of a multi-list.</param>
    public static int CountPresent(ReadOnlySpan<byte> input, int capacity, int lerp = 0)
    {
        var count = 0;
        for (int i = 0; i < capacity; i++)
        {
            var mark = input[(i * lerp) + 1];
            if (IsPresent(mark))
                count++;
        }
        return count;
    }

    private static bool IsJapaneseList(int length) => length == PokeCrypto.SIZE_1JLIST;
    private static bool IsJapaneseString(int length) => length == GBPKML.StringLengthJapanese;
    public static int GetListLengthSingle(bool jp) => jp ? PokeCrypto.SIZE_1JLIST : PokeCrypto.SIZE_1ULIST;
    private static int GetBodyLength(bool party) => party ? PokeCrypto.SIZE_1PARTY : PokeCrypto.SIZE_1STORED;
    private static int GetStringLength(bool jp) => jp ? GBPKML.StringLengthJapanese : GBPKML.StringLengthNotJapan;

    /// <summary>
    /// Checks if the header is sane.
    /// </summary>
    /// <param name="input">List header</param>
    /// <param name="capacity">Count of slots allowed in the list</param>
    public static bool IsValidHeader(ReadOnlySpan<byte> input, int capacity)
    {
        if (input.Length < 2)
            return false;

        var count = input[0];
        if (count > capacity)
            return false;

        var expectLength = 1 + (capacity + 1);
        if (input.Length < expectLength)
            return false;

        for (int i = 0; i < capacity; i++)
        {
            var mark = input[1 + i];
            bool present = IsPresent(mark);
            if (present != (i < count))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Reads an entity from a single-slot list.
    /// </summary>
    public static PK1 ReadFromSingle(ReadOnlySpan<byte> input)
    {
        bool jp = IsJapaneseList(input.Length);
        var stringLength = GetStringLength(jp);
        return ReadFromList(input, stringLength);
    }

    /// <summary>
    /// Reads an entity from a list.
    /// </summary>
    /// <param name="input">List data</param>
    /// <param name="stringLength">Trainer and Nickname string length</param>
    /// <param name="capacity">Count of slots allowed in the list</param>
    /// <param name="isParty">List stores party stats for each entity</param>
    /// <param name="index">Entity index to read</param>
    public static PK1 ReadFromList(ReadOnlySpan<byte> input, int stringLength, int capacity = 1, bool isParty = true, int index = 0)
    {
        var start = 1 + (capacity + 1);
        int sizeBody = GetBodyLength(isParty);

        var ofsBody = start + (sizeBody * index);
        var ofsStr1 = start + (sizeBody * capacity) + (stringLength * index);
        var ofsStr2 = ofsStr1 + (capacity * stringLength);

        var body = input.Slice(ofsBody, sizeBody);
        var stringSection1 = input.Slice(ofsStr1, stringLength);
        var stringSection2 = input.Slice(ofsStr2, stringLength);

        return new PK1(body, stringSection1, stringSection2);
    }

    /// <summary>
    /// Writes an entity to a list.
    /// </summary>
    /// <param name="output">List data</param>
    /// <param name="pk">Entity to write</param>
    /// <param name="capacity">Count of slots allowed in the list</param>
    /// <param name="isParty">List stores party stats for each entity</param>
    /// <param name="index">Entity index to write</param>
    public static void WriteToList(Span<byte> output, PK1 pk, int capacity = 1, bool isParty = true, int index = 0)
    {
        var start = 1 + (capacity + 1);
        var sizeBody = GetBodyLength(isParty);
        var stringLength = pk.OriginalTrainerTrash.Length;

        var ofsBody = start + (sizeBody * index);
        var ofsStr1 = start + (sizeBody * capacity) + (stringLength * index);
        var ofsStr2 = ofsStr1 + (capacity * stringLength);

        var body = output.Slice(ofsBody, sizeBody);
        var stringSection1 = output.Slice(ofsStr1, stringLength);
        var stringSection2 = output.Slice(ofsStr2, stringLength);

        pk.Data.AsSpan()[..sizeBody].CopyTo(body);
        pk.OriginalTrainerTrash.CopyTo(stringSection1);
        pk.NicknameTrash.CopyTo(stringSection2);

        output[1 + index] = GetHeaderIdentifierMark(pk);
        output[0] = (byte)CountPresent(output, capacity);
        output[1 + capacity] = SlotEmpty; // cap off the list
    }

    /// <summary>
    /// Unpacks a multi-slot list into many single-slot lists.
    /// </summary>
    /// <param name="input">Multi-slot list</param>
    /// <param name="output">Destination to write each single-slot list</param>
    /// <param name="stringLength">Trainer and Nickname string length</param>
    /// <param name="capacity">Count of slots allowed in the list</param>
    /// <param name="isParty">List stores party stats for each entity</param>
    public static void Unpack(ReadOnlySpan<byte> input, Span<byte> output, int stringLength, int capacity, bool isParty)
    {
        var lengthBody = GetBodyLength(isParty);
        var lengthParty = GetBodyLength(true);

        var ofsBody = 1 + (capacity + 1);
        var ofsStr1 = ofsBody + (capacity * lengthBody);
        var ofsStr2 = ofsStr1 + (capacity * stringLength);

        var count = Math.Min(capacity, input[0]);
        for (int i = 0; i < count; i++)
        {
            var species = input[1 + i];
            var body = input.Slice(ofsBody, lengthBody);
            var stringSection1 = input.Slice(ofsStr1, stringLength);
            var stringSection2 = input.Slice(ofsStr2, stringLength);

            output[0] = 1;
            output[1] = species;
            output = output[3..];
            body.CopyTo(output);
            output = output[lengthParty..];
            stringSection1.CopyTo(output);
            output = output[stringLength..];
            stringSection2.CopyTo(output);
            output = output[stringLength..];

            ofsBody += lengthBody;
            ofsStr1 += stringLength;
            ofsStr2 += stringLength;
        }
    }

    /// <summary>
    /// Packs many single-slot lists into a multi-slot list.
    /// </summary>
    /// <param name="input">Source single-slot lists</param>
    /// <param name="output">Destination multi-slot list</param>
    /// <param name="stringLength">Trainer and Nickname string length</param>
    /// <param name="capacity">Count of slots allowed in the list</param>
    /// <param name="isParty">List stores party stats for each entity</param>
    /// <param name="isDestInitialized">True if the destination list is initialized</param>
    public static bool MergeSingles(ReadOnlySpan<byte> input, Span<byte> output, int stringLength, int capacity, bool isParty, bool isDestInitialized = true)
    {
        // Collect the count of set slots
        var jp = IsJapaneseString(stringLength);
        var size = GetListLengthSingle(jp);

        int count = CountPresent(input, capacity, size);
        if (count == 0 && (!isDestInitialized || !output.ContainsAnyExcept<byte>(0)))
            return false; // No need to merge if all empty and dest is not initialized.

        output[0] = (byte)count; // ensure written list is valid
        // Put non-zero species first, then empty slots
        var ctr = 0;
        var emptyIndex = count;

        var lengthBody = GetBodyLength(isParty);
        var lengthParty = GetBodyLength(true);
        for (int i = 0; i < capacity; i++)
        {
            var single = input.Slice(i * size, size);
            var marker = single[1]; // assume correct, don't look in body data.
            if (marker is 0) // Ensure deleted (zeroed) slots act as an Empty (FF) slot.
                marker = SlotEmpty;

            var index = IsPresent(marker) ? ctr++ : emptyIndex++;
            output[1 + index] = marker;

            // Source Properties
            var body = single.Slice(3, lengthBody);
            var str1 = single.Slice(3 + lengthParty, stringLength);
            var str2 = single.Slice(3 + lengthParty + stringLength, stringLength);

            // Destination Properties
            var ofsStart = 1 + (capacity + 1);
            var ofsBody = ofsStart + (index * lengthBody);
            var ofsStr1 = ofsStart + (capacity * lengthBody) + (index * stringLength);
            var ofsStr2 = ofsStr1 + (capacity * stringLength);
            var destBody = output.Slice(ofsBody, lengthBody);
            var destStr1 = output.Slice(ofsStr1, stringLength);
            var destStr2 = output.Slice(ofsStr2, stringLength);

            body.CopyTo(destBody);
            str1.CopyTo(destStr1);
            str2.CopyTo(destStr2);
        }

        output[1 + capacity] = SlotEmpty; // cap off the list
        return true;
    }

    /// <inheritdoc cref="WrapSingle(PK1,Span{byte})"/>
    public static byte[] WrapSingle(PK1 pk)
    {
        var length = GetListLengthSingle(pk.Japanese);
        var data = new byte[length];
        WrapSingle(pk, data);
        return data;
    }

    /// <summary>
    /// Wraps a single entity into a single-slot list.
    /// </summary>
    /// <param name="pk">Entity to wrap</param>
    /// <param name="output">Destination to write the single-slot list</param>
    public static void WrapSingle(PK1 pk, Span<byte> output) => WriteToList(output, pk);

    public static void UnpackNOB(ReadOnlySpan<byte> input, Span<byte> output, int stringLength, bool isParty = false)
    {
        // Nickname, OT, Data
        var lengthBody = GetBodyLength(isParty);
        var lengthParty = GetBodyLength(true);
        var nick = input[..stringLength];
        var trainer = input.Slice(stringLength, stringLength);
        var box = input.Slice(stringLength * 2, lengthBody);

        var marker = box[0];
        if (marker is 0) // Ensure deleted (zeroed) slots act as an Empty (FF) slot.
            marker = SlotEmpty;

        output[0] = 1;
        output[1] = marker;
        output = output[3..];

        // Data, OT, Nickname
        box.CopyTo(output);
        output = output[lengthParty..];
        trainer.CopyTo(output);
        output = output[stringLength..];
        nick.CopyTo(output);
    }

    public static void PackNOB(ReadOnlySpan<byte> input, Span<byte> output, int stringLength, bool isParty = false)
    {
        var lengthBody = GetBodyLength(isParty);
        var lengthParty = GetBodyLength(true);

        input = input[3..]; // Skip header.
        var box = input[..lengthBody];
        var trainer = input.Slice(lengthParty, stringLength);
        var nick = input.Slice(lengthParty + stringLength, stringLength);

        nick.CopyTo(output);
        output = output[stringLength..];
        trainer.CopyTo(output);
        output = output[stringLength..];
        box.CopyTo(output);
    }
}
