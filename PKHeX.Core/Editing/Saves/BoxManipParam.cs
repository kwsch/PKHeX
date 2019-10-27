namespace PKHeX.Core
{
    public readonly struct BoxManipParam
    {
        public readonly int Start;
        public readonly int Stop;
        public readonly bool Reverse;

        public BoxManipParam(int start, int stop, bool reverse = false)
        {
            Start = start;
            Stop = stop;
            Reverse = reverse;
        }

        public bool Equals(BoxManipParam p) => p.Start == Start && p.Stop == Stop && p.Reverse == Reverse;
        public override bool Equals(object obj) => obj is BoxManipParam p && Equals(p);
        public override int GetHashCode() => -1;
        public static bool operator ==(BoxManipParam left, BoxManipParam right) => left.Equals(right);
        public static bool operator !=(BoxManipParam left, BoxManipParam right) => !(left == right);
    }
}