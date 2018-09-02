namespace PKHeX.Core
{
    public abstract class BoxManipulator
    {
        protected abstract SaveFile SAV { get; }

        public bool Execute(IBoxManip manip, int box, bool allBoxes, bool reverse = false)
        {
            bool usable = manip.Usable?.Invoke(SAV) ?? true;
            if (!usable)
                return false;

            // determine start/stop
            int start = allBoxes ? 0 : box;
            int stop = allBoxes ? SAV.BoxCount - 1 : box;

            var prompt = manip.GetPrompt(allBoxes);
            var fail = manip.GetFail(allBoxes);
            if (!CanManipulateRegion(start, stop, prompt, fail))
                return false;

            var param = new BoxManipParam
            {
                Reverse = reverse,
                Start = start,
                Stop = stop,
            };

            var result = manip.Execute(SAV, param);
            if (!result)
                return false;
            var success = manip.GetSuccess(allBoxes);
            FinishBoxManipulation(success, allBoxes);
            return true;
        }

        protected virtual bool CanManipulateRegion(int start, int end, string prompt, string fail) => !SAV.IsAnySlotLockedInBox(start, end);
        protected abstract void FinishBoxManipulation(string message, bool all);
    }
}
