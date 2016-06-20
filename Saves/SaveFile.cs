using System;
using System.Linq;

namespace PKHeX
{
    // Base Class for Save Files
    public abstract class SaveFile
    {
        internal static bool SetUpdateDex = true;
        internal static bool SetUpdatePKM = true;

        // General Object Properties
        public byte[] Data;
        public bool Edited;
        public string FileName, FilePath;
        public abstract string BAKName { get; }
        public abstract byte[] BAK { get; }
        public abstract bool Exportable { get; }
        public abstract SaveFile Clone();

        // General PKM Properties
        protected abstract Type PKMType { get; }
        public abstract PKM getPKM(byte[] data);
        public abstract PKM BlankPKM { get; }
        public abstract byte[] decryptPKM(byte[] data);
        public abstract int SIZE_STORED { get; }
        public abstract int SIZE_PARTY { get; }
        public abstract int MaxEV { get; }
        public ushort[] HeldItems { get; protected set; }

        // General SAV Properties
        public byte[] Write()
        {
            setChecksums();
            return Data;
        }
        public virtual string MiscSaveChecks() { return ""; }
        public virtual string MiscSaveInfo() { return ""; }
        protected abstract GameVersion Version { get; }
        public abstract bool ChecksumsValid { get; }
        public abstract string ChecksumInfo { get; }
        public abstract int Generation { get; }

        public bool ORASDEMO => Data.Length == SaveUtil.SIZE_G6ORASDEMO;
        public bool ORAS => Version == GameVersion.OR || Version == GameVersion.AS;
        public bool XY => Version == GameVersion.X || Version == GameVersion.Y;

        // Flags
        public bool HasWondercards => WondercardData > -1;
        public bool HasSuperTrain => SuperTrain > -1;
        public bool HasBerryField => BerryField > -1;
        public bool HasHoF => HoF > -1;
        public bool HasSecretBase => SecretBase > -1;
        public bool HasPuff => Puff > -1;
        public bool HasPSS => PSS > -1;
        public bool HasOPower => OPower > -1;
        public bool HasJPEG => JPEGData != null;
        public bool HasBox => Box > -1;
        public bool HasParty => Party > -1;
        public bool HasBattleBox => BattleBox > -1;
        public bool HasFused => Fused > -1;
        public bool HasGTS => GTS > -1;
        public bool HasDaycare => Daycare > -1;
        public bool HasPokeDex => PokeDex > -1;
        public virtual bool HasBoxWallpapers => PCLayout > -1;
        public virtual bool HasSUBE => SUBE > -1 && !ORAS;
        public virtual bool HasGeolocation => false;
        public bool HasPokeBlock => ORAS && !ORASDEMO;
        public bool HasEvents => EventFlags != null;

        // Counts
        protected virtual int GiftCountMax { get; } = int.MinValue;
        protected virtual int GiftFlagMax { get; } = 0x800;
        protected virtual int EventFlagMax { get; } = int.MinValue;
        protected virtual int EventConstMax { get; } = int.MinValue;
        public abstract int DaycareSeedSize { get; }
        public abstract int OTLength { get; }
        public abstract int NickLength { get; }

        // Offsets
        protected int Box { get; set; } = int.MinValue;
        protected int Party { get; set; } = int.MinValue;
        protected int Trainer1 { get; set; } = int.MinValue;
        protected int Daycare { get; set; } = int.MinValue;
        protected int WondercardData { get; set; } = int.MinValue;
        protected int PCLayout { get; set; } = int.MinValue;
        protected int EventFlag { get; set; } = int.MinValue;
        protected int EventConst { get; set; } = int.MinValue;

        public int GTS { get; protected set; } = int.MinValue;
        public int BattleBox { get; protected set; } = int.MinValue;
        public int Fused { get; protected set; } = int.MinValue;
        public int SUBE { get; protected set; } = int.MinValue;
        public int PokeDex { get; protected set; } = int.MinValue;
        public int SuperTrain { get; protected set; } = int.MinValue;
        public int SecretBase { get; protected set; } = int.MinValue;
        public int Puff { get; protected set; } = int.MinValue;
        public int PSS { get; protected set; } = int.MinValue;
        public int BerryField { get; protected set; } = int.MinValue;
        public int OPower { get; protected set; } = int.MinValue;
        public int HoF { get; protected set; } = int.MinValue;

        // SAV Properties
        public PKM[] BoxData
        {
            get
            {
                PKM[] data = new PKM[BoxCount*30];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = getStoredSlot(getBoxOffset(i/30) + SIZE_STORED*(i%30));
                    data[i].Identifier = $"{getBoxName(i/30)}:{(i%30 + 1).ToString("00")}";
                }
                return data;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length != BoxCount*30)
                    throw new ArgumentException($"Expected {BoxCount*30}, got {value.Length}");
                if (value.Any(pk => PKMType != pk.GetType()))
                    throw new ArgumentException($"Not {PKMType} array.");

                for (int i = 0; i < value.Length; i++)
                    setStoredSlot(value[i], getBoxOffset(i/30) + SIZE_STORED*(i%30));
            }
        }
        public PKM[] PartyData
        {
            get
            {
                PKM[] data = new PKM[PartyCount];
                for (int i = 0; i < data.Length; i++)
                    data[i] = getPartySlot(Party + SIZE_PARTY * i);
                return data;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length == 0 || value.Length > 6)
                    throw new ArgumentException("Expected 1-6, got " + value.Length);
                if (value.Any(pk => PKMType != pk.GetType()))
                    throw new ArgumentException($"Not {PKMType} array.");
                if (value[0].Species == 0)
                    throw new ArgumentException("Can't have an empty first slot." + value.Length);

                PKM[] newParty = value.Where(pk => pk.Species != 0).ToArray();

                PartyCount = newParty.Length;
                Array.Resize(ref newParty, 6);

                for (int i = PartyCount; i < newParty.Length; i++)
                    newParty[i] = BlankPKM;
                for (int i = 0; i < newParty.Length; i++)
                    setPartySlot(newParty[i], getPartyOffset(i));
            }
        }
        public PKM[] BattleBoxData
        {
            get
            {
                if (Generation < 5)
                    return new PKM[0];

                PKM[] data = new PKM[6];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = getStoredSlot(BattleBox + SIZE_STORED * i);
                    if (data[i].Species == 0)
                        return data.Take(i).ToArray();
                }
                return data;
            }
        }

        public bool[] EventFlags
        {
            get
            {
                if (EventFlagMax < 0)
                    return null;

                bool[] Flags = new bool[EventFlagMax];
                for (int i = 0; i < Flags.Length; i++)
                    Flags[i] = (Data[EventFlag + i / 8] >> i % 8 & 0x1) == 1;
                return Flags;
            }
            set
            {
                if (EventFlagMax < 0)
                    return;
                if (value.Length != EventFlagMax)
                    return;

                byte[] data = new byte[value.Length / 8];
                for (int i = 0; i < value.Length; i++)
                    if (value[i])
                        data[i >> 3] |= (byte)(1 << (i & 7));

                data.CopyTo(Data, EventFlag);
            }
        }
        public ushort[] EventConsts
        {
            get
            {
                if (EventConstMax < 0)
                    return null;

                ushort[] Constants = new ushort[EventConstMax];
                for (int i = 0; i < Constants.Length; i++)
                    Constants[i] = BitConverter.ToUInt16(Data, EventConst + i);
                return Constants;
            }
            set
            {
                if (EventConstMax < 0)
                    return;
                if (value.Length != EventConstMax)
                    return;

                for (int i = 0; i < value.Length; i++)
                    BitConverter.GetBytes(value[i]).CopyTo(Data, EventConst + i * 2);
            }
        }

        // Inventory
        public abstract InventoryPouch[] Inventory { get; set; }
        protected int OFS_PouchHeldItem { get; set; } = int.MinValue;
        protected int OFS_PouchKeyItem { get; set; } = int.MinValue;
        protected int OFS_PouchMedicine { get; set; } = int.MinValue;
        protected int OFS_PouchTMHM { get; set; } = int.MinValue;
        protected int OFS_PouchBerry { get; set; } = int.MinValue;
        protected int OFS_PouchBalls { get; set; } = int.MinValue;
        protected int OFS_BattleItems { get; set; } = int.MinValue;
        protected int OFS_MailItems { get; set; } = int.MinValue;

        // Mystery Gift
        protected abstract bool[] MysteryGiftReceivedFlags { get; set; }
        protected abstract MysteryGift[] MysteryGiftCards { get; set; }
        public virtual MysteryGiftAlbum GiftAlbum
        {
            get
            {
                return new MysteryGiftAlbum
                {
                    Flags = MysteryGiftReceivedFlags,
                    Gifts = MysteryGiftCards
                };
            }
            set
            {
                MysteryGiftReceivedFlags = value.Flags;
                MysteryGiftCards = value.Gifts;
            }
        }

        public virtual bool BattleBoxLocked { get { return false; } set { } }
        public virtual string JPEGTitle => null;
        public virtual byte[] JPEGData => null;
        public virtual int Country { get { return -1; } set { } }
        public virtual int ConsoleRegion { get { return -1; } set { } }
        public virtual int SubRegion { get { return -1; } set { } }

        // Trainer Info
        public abstract int Gender { get; set; }
        public abstract int Language { get; set; }
        public virtual int Game { get { return -1; } set { } }
        public abstract ushort TID { get; set; }
        public abstract ushort SID { get; set; }
        public abstract string OT { get; set; }
        public abstract int BoxCount { get; }
        public abstract int PartyCount { get; protected set; }
        public virtual int CurrentBox { get { return 0; } set { } }
        
        // Varied Methods
        protected abstract void setChecksums();
        public abstract int getBoxOffset(int box);
        public abstract int getPartyOffset(int slot);
        public abstract int getBoxWallpaper(int box);
        public abstract string getBoxName(int box);
        public abstract void setBoxName(int box, string val);

        // Daycare
        public int DaycareIndex = 0;
        public abstract int getDaycareSlotOffset(int loc, int slot);
        public abstract uint? getDaycareEXP(int loc, int slot);
        public virtual ulong? getDaycareRNGSeed(int loc) { return null; }
        public virtual bool? getDaycareHasEgg(int loc) { return false; }
        public abstract bool? getDaycareOccupied(int loc, int slot);
        
        public abstract void setDaycareEXP(int loc, int slot, uint EXP);
        public virtual void setDaycareRNGSeed(int loc, ulong seed) { }
        public virtual void setDaycareHasEgg(int loc, bool hasEgg) { }
        public abstract void setDaycareOccupied(int loc, int slot, bool occupied);

        // Storage
        public PKM getPartySlot(int offset)
        {
            return getPKM(decryptPKM(getData(offset, SIZE_PARTY)));
        }
        public PKM getStoredSlot(int offset)
        {
            return getPKM(decryptPKM(getData(offset, SIZE_STORED)));
        }
        public void setPartySlot(PKM pkm, int offset, bool? trade = null, bool? dex = null)
        {
            if (pkm == null) return;
            if (pkm.GetType() != PKMType)
                throw new InvalidCastException($"PKM Format needs to be {PKMType} when setting to a Gen{Generation} Save File.");
            if (trade ?? SetUpdatePKM)
                setPKM(pkm);
            if (dex ?? SetUpdateDex)
                setDex(pkm);

            setData(pkm.EncryptedPartyData, offset);
            Console.WriteLine("");
            Edited = true;
        }
        public void setStoredSlot(PKM pkm, int offset, bool? trade = null, bool? dex = null)
        {
            if (pkm == null) return;
            if (pkm.GetType() != PKMType)
                throw new InvalidCastException($"PKM Format needs to be {PKMType} when setting to a Gen{Generation} Save File.");
            if (trade ?? SetUpdatePKM)
                setPKM(pkm);
            if (dex ?? SetUpdateDex)
                setDex(pkm);

            setData(pkm.EncryptedBoxData, offset);
            Edited = true;
        }
        public void setPartySlot(byte[] data, int offset, bool? trade = null, bool? dex = null)
        {
            if (data == null) return;
            PKM pkm = getPKM(decryptPKM(data));
            if (trade ?? SetUpdatePKM)
                setPKM(pkm);
            if (dex ?? SetUpdateDex)
                setDex(pkm);

            setData(pkm.EncryptedPartyData, offset);
            Edited = true;
        }
        public void setStoredSlot(byte[] data, int offset, bool? trade = null, bool? dex = null)
        {
            if (data == null) return;
            PKM pkm = getPKM(decryptPKM(data));
            if (trade ?? SetUpdatePKM)
                setPKM(pkm);
            if (dex ?? SetUpdateDex)
                setDex(pkm);

            setData(pkm.EncryptedBoxData, offset);
            Edited = true;
        }

        public void sortBoxes(int BoxStart = 0, int BoxEnd = -1)
        {
            PKM[] BD = BoxData;
            var Section = BD.Skip(BoxStart*30);
            if (BoxEnd > BoxStart)
                Section = Section.Take(30*(BoxEnd - BoxStart));

            var Sorted = Section
                .OrderBy(p => p.Species == 0) // empty slots at end
                .ThenBy(p => p.IsEgg) // eggs to the end
                .ThenBy(p => p.Species) // species sorted
                .ThenBy(p => p.IsNicknamed).ToArray();

            Array.Copy(Sorted, 0, BD, BoxStart*30, Sorted.Length);
            BoxData = BD;
        }
        public void resetBoxes(int BoxStart = 0, int BoxEnd = -1)
        {
            if (BoxEnd < 0)
                BoxEnd = BoxCount;
            for (int i = BoxStart; i < BoxEnd; i++)
            {
                int offset = getBoxOffset(i);
                for (int p = 0; p < 30; p++)
                    setStoredSlot(BlankPKM, offset + SIZE_STORED * p);
            }
        }

        public byte[] getPCBin() { return BoxData.SelectMany(pk => pk.EncryptedBoxData).ToArray(); }
        public byte[] getBoxBin(int box) { return BoxData.Skip(box*30).Take(30).SelectMany(pk => pk.EncryptedBoxData).ToArray(); }
        public bool setPCBin(byte[] data)
        {
            if (data.Length != getPCBin().Length)
                return false;

            // split up data to individual pkm
            byte[][] pkdata = new byte[data.Length/SIZE_STORED][];
            for (int i = 0; i < data.Length; i += SIZE_STORED)
                pkdata[i] = data.Skip(i).Take(SIZE_STORED).ToArray();
            
            PKM[] pkms = BoxData;
            for (int i = 0; i < pkms.Length; i++)
                pkms[i].Data = decryptPKM(pkdata[i]);
            BoxData = pkms;
            return true;
        }
        public bool setBoxBin(byte[] data, int box)
        {
            if (data.Length != getBoxBin(box).Length)
                return false;

            byte[][] pkdata = new byte[data.Length / SIZE_STORED][];
            for (int i = 0; i < data.Length; i += SIZE_STORED)
                pkdata[i] = data.Skip(i).Take(SIZE_STORED).ToArray();

            PKM[] pkms = BoxData;
            for (int i = 0; i < 30; i++)
                pkms[box*30 + i].Data = decryptPKM(pkdata[i]);
            BoxData = pkms;
            return true;
        }

        protected virtual void setPKM(PKM pkm) { }
        protected virtual void setDex(PKM pkm) { }
        
        public byte[] getData(int Offset, int Length)
        {
            return Data.Skip(Offset).Take(Length).ToArray();
        }
        public void setData(byte[] input, int Offset)
        {
            input.CopyTo(Data, Offset);
            Edited = true;
        }
    }
}
