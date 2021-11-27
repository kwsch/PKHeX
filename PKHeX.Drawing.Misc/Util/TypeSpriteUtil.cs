using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;

namespace PKHeX.Drawing.Misc
{
    public static class TypeSpriteUtil
    {
        public static Image? GetTypeSprite(int type, int generation = PKX.Generation)
        {
            if (generation <= 2)
                type = (int)((MoveType)type).GetMoveTypeGeneration(generation);
            return (Bitmap?)Resources.ResourceManager.GetObject($"type_icon_{type:00}");
        }
    }
}
