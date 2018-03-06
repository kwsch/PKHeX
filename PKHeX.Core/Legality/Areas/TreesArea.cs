using System.Linq;

namespace PKHeX.Core
{
    // Pokemon Crystal Headbutt tree encounters by trainer id, based on mechanics described in
    // https://bulbapedia.bulbagarden.net/wiki/Headbutt_tree#Mechanics

    /// <summary> Indicates the Availability of the Headbutt Tree </summary>
    public enum TreeEncounterAvailable
    {
        /// <summary> Encounter is possible a reachable tree </summary>
        ValidTree,
        /// <summary> Encounter is only possible a tree reachable only with walk-through walls cheats </summary>
        InvalidTree,
        /// <summary> Encounter is not possible in any tree </summary>
        Impossible
    }

    /// <summary> Coordinate / Index Relationship for a Headbutt Tree </summary>
    internal class TreeCoordinates
    {
        private int X { get; }
        private int Y { get; }
        internal int Index => (X*Y + X+Y) / 5 % 10;

        public TreeCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary> Trees on a given map </summary>
    public class TreesArea
    {
        private const int PivotCount = 10;
        private static int[][] TrainerModerateTreeIndex { get; } = GenerateTrainersTreeIndex();
        private static int[][] GenerateTrainersTreeIndex()
        {
            // A tree have a low encounter or moderate encounter base on the TID Pivot Index (TID % 10)
            // Calculate for every Trainer Pivot Index the 5 tree index for low encounters
            int[][] TrainersIndex = new int[PivotCount][];
            for (int i = 0; i < PivotCount; i++)
            {
                int[] ModerateEncounterTreeIndex = new int[5];
                for (int j = 0; j <= 4; j++)
                    ModerateEncounterTreeIndex[j] = (i + j) % PivotCount;
                TrainersIndex[i] = ModerateEncounterTreeIndex.OrderBy(x => x).ToArray();
            }
            return TrainersIndex;
        }
        internal static TreesArea[] GetArray(byte[][] entries) => entries.Select(z => new TreesArea(z)).ToArray();

        public int Location { get; private set; }
        public TreeEncounterAvailable[] GetTrees(SlotType t) => t == SlotType.Headbutt
            ? TrainerModerateEncounterTree
            : TrainerLowEncounterTree;

        private TreeEncounterAvailable[] TrainerModerateEncounterTree { get; set; }
        private TreeEncounterAvailable[] TrainerLowEncounterTree { get; set; }
        private int[] ValidTreeIndex { get; set; }
        private int[] InvalidTreeIndex { get; set; }
        private TreeCoordinates[] ValidTrees { get; set; }
        private TreeCoordinates[] InvalidTrees { get; set; }

        private TreesArea(byte[] entry)
        {
            ReadAreaRawData(entry);
            GenerateAreaTreeIndex();
            GenerateAreaTrainerEncounters();
        }

        private void ReadAreaRawData(byte[] entry)
        {
            // Coordinates of trees for every are obtained with the program G2Map
            // ValidTrees are those accessible by the player
            Location = entry[0];
            ValidTrees = new TreeCoordinates[entry[1]];
            var ofs = 2;
            for (int i = 0; i < ValidTrees.Length; i++, ofs += 2)
                ValidTrees[i] = new TreeCoordinates(entry[ofs], entry[ofs + 1]);

            // Invalid tress are trees that the player can not reach without cheating devices, like a tree beyond other trees
            InvalidTrees = new TreeCoordinates[entry[ofs]];
            ofs += 1;
            for (int i = 0; i < InvalidTrees.Length; i++, ofs += 2)
                InvalidTrees[i] = new TreeCoordinates(entry[ofs], entry[ofs + 1]);
        }

        private void GenerateAreaTreeIndex()
        {
            // For legality purposes, only the tree index is needed.
            // Group the trees data by their index; trees that share indexes are indistinguishable from one another
            ValidTreeIndex = ValidTrees.Select(t => t.Index).Distinct().OrderBy(i => i).ToArray();
            InvalidTreeIndex = InvalidTrees.Select(t => t.Index).Distinct().OrderBy(i => i).Except(ValidTreeIndex).ToArray();
        }

        private void GenerateAreaTrainerEncounters()
        {
            // Check for every trainer pivot index if there are trees with moderate encounter and low encounter available in the area
            TrainerModerateEncounterTree = new TreeEncounterAvailable[PivotCount];
            TrainerLowEncounterTree = new TreeEncounterAvailable[PivotCount];
            for (int i = 0; i < PivotCount; i++)
            {
                var TrainerModerateTrees = TrainerModerateTreeIndex[i];
                TrainerModerateEncounterTree[i] = GetAvailableModerate(TrainerModerateTrees);
                TrainerLowEncounterTree[i] = GetAvailableLow(TrainerModerateTrees);
            }
        }

        private TreeEncounterAvailable GetAvailableModerate(int[] moderate)
        {
            if (ValidTreeIndex.Any(moderate.Contains))
                return TreeEncounterAvailable.ValidTree;
            if (InvalidTreeIndex.Any(moderate.Contains))
                return TreeEncounterAvailable.InvalidTree;
            return TreeEncounterAvailable.Impossible;
        }
        private TreeEncounterAvailable GetAvailableLow(int[] moderate)
        {
            if (ValidTreeIndex.Except(moderate).Any())
                return TreeEncounterAvailable.ValidTree;
            if (InvalidTreeIndex.Except(moderate).Any())
                return TreeEncounterAvailable.InvalidTree;
            return TreeEncounterAvailable.Impossible;
        }
    }
}
