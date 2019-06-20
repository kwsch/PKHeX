namespace PKHeX.Core
{
    public static class PIDTypeInfo
    {
        public static bool IsReversedPID(this PIDType type)
        {
            switch (type)
            {
                case PIDType.BACD_R:
                case PIDType.BACD_R_A:
                case PIDType.BACD_R_S:
                case PIDType.BACD_U:
                case PIDType.BACD_U_A:
                case PIDType.BACD_U_S:
                case PIDType.CXD:
                case PIDType.CXDAnti:
                case PIDType.Method_1_Unown:
                case PIDType.Method_2_Unown:
                case PIDType.Method_3_Unown:
                case PIDType.Method_4_Unown:
                    return true;
                default:
                    return false;
            }
        }
    }
}
