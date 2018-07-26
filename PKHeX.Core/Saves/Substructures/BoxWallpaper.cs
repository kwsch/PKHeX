namespace PKHeX.Core
{
    public static class BoxWallpaper
    {
        public static string GetWallpaper(SaveFile SAV, int index)
        {
            index++;
            string s = $"box_wp{index:00}";
            switch (SAV.Generation)
            {
                case 7: s += "xy";
                    break;
                case 6: s += SAV.ORAS && index > 16 ? "ao" : "xy";
                    break;
                case 5: s += SAV.B2W2 && index > 16 ? "b2w2" : "bw";
                    break;
                case 4:
                    if (SAV.Pt && index > 16)
                        s += "pt";
                    else if (SAV.HGSS && index > 16)
                        s += "hgss";
                    else
                        s += "dp";
                    break;
                case 3:
                    if (SAV.E)
                        s += "e";
                    else if (SAV.FRLG && index > 12)
                        s += "frlg";
                    else
                        s += "rs";
                    break;
            }
            return s;
        }
        public static bool IsWallpaperRed(SaveFile SAV, int box)
        {
            switch (SAV.Generation)
            {
                case 3:
                    if (SAV.GameCube)
                        return box == 7 && SAV is SAV3XD; // flame pattern in XD
                    switch (SAV.GetBoxWallpaper(box))
                    {
                        case 5: // Volcano
                            return true;
                        case 13: // PokéCenter
                            return SAV.E;
                    }
                    break;
                case 4:
                    switch (SAV.GetBoxWallpaper(box))
                    {
                        case 5: // Volcano
                        case 12: // Checks
                        case 13: // PokéCenter
                        case 22: // Special
                            return true;
                    }
                    break;
                case 5:
                    switch (SAV.GetBoxWallpaper(box))
                    {
                        case 5: // Volcano
                        case 12: // Checks
                            return true;
                        case 19: // PWT
                        case 22: // Reshiram
                            return SAV.B2W2;
                        case 21: // Zoroark
                        case 23: // Musical
                            return SAV.BW;
                    }
                    break;
                case 6:
                case 7:
                    switch (SAV.GetBoxWallpaper(box))
                    {
                        case 5: // Volcano
                        case 12: // PokéCenter
                        case 20: // Special5 Flare/Magma
                            return true;
                    }
                    break;
            }
            return false;
        }
    }
}
