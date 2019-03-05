using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class BoxManipModify : IBoxManip
    {
        public BoxManipType Type { get; }
        public Func<SaveFile, bool> Usable { get; set; }

        public string GetPrompt(bool all) => null;
        public string GetFail(bool all) => null;
        public string GetSuccess(bool all) => null;

        private readonly Action<PKM> Action;
        private readonly Action<PKM, SaveFile> ActionComplex;

        public int Execute(SaveFile SAV, BoxManipParam param)
        {
            var method = Action ?? (px => ActionComplex(px, SAV));
            return SAV.ModifyBoxes(method, param.Start, param.Stop);
        }

        private BoxManipModify(BoxManipType type, Action<PKM> action, Func<SaveFile, bool> usable = null)
        {
            Type = type;
            Action = action;
            Usable = usable;
        }

        private BoxManipModify(BoxManipType type, Action<PKM, SaveFile> action, Func<SaveFile, bool> usable = null)
        {
            Type = type;
            ActionComplex = action;
            Usable = usable;
        }

        public static readonly IReadOnlyList<BoxManipModify> Common = new List<BoxManipModify>
        {
            new BoxManipModify(BoxManipType.ModifyHatchEggs, pk => pk.ForceHatchPKM(), s => s.Generation >= 2),
            new BoxManipModify(BoxManipType.ModifyMaxFriendship, pk => pk.MaximizeFriendship()),
            new BoxManipModify(BoxManipType.ModifyMaxLevel, pk => pk.MaximizeLevel()),
            new BoxManipModify(BoxManipType.ModifyResetMoves, pk => pk.SetMoves(pk.GetMoveSet()), s => s.Generation >= 3),
            new BoxManipModify(BoxManipType.ModifyRandomMoves, pk => pk.SetMoves(pk.GetMoveSet(true))),
            new BoxManipModify(BoxManipType.ModifyHyperTrain,pk => pk.SetSuggestedHyperTrainingData(), s => s.Generation >= 7),
            new BoxManipModify(BoxManipType.ModifyRemoveNicknames, pk => pk.SetDefaultNickname()),
            new BoxManipModify(BoxManipType.ModifyRemoveItem, pk => pk.HeldItem = 0, s => s.Generation >= 2),
        };
    }
}