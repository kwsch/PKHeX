namespace PKHeX.Core
{
    public struct BoxManipParam
    {
        public int Start { get; set; }
        public int Stop { get; set; }
        public bool Reverse { get; set; }

        public override bool Equals(object obj) => obj is BoxManipParam p && p.Start == Start && p.Stop == Stop && p.Reverse == Reverse;
        public override int GetHashCode() => -1;
        public static bool operator ==(BoxManipParam left, BoxManipParam right) => left.Equals(right);
        public static bool operator !=(BoxManipParam left, BoxManipParam right) => !(left == right);
    }
}