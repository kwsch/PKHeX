using System.Linq;

namespace PKHeX.Core
{
    // Pokemon Crystal Headbutt tree encounters by trainer id, based on mechanics described in
    // https://bulbapedia.bulbagarden.net/wiki/Headbutt_tree#Mechanics

    /// <summary>
    /// Generation 2 Headbutt Trees on a given map
    /// </summary>
    public sealed class TreesArea
    {
        private const int PivotCount = 10;
        private static readonly int[][] TrainerModerateTreeIndex = GenerateTrainersTreeIndex();

        private static int[][] GenerateTrainersTreeIndex()
        {
            // A tree has a low encounter or moderate encounter base on the TID Pivot Index (TID % 10)
            // For every Trainer Pivot Index, calculate the low encounter trees (total of 5)
            int[][] TrainersIndex = new int[PivotCount][];
            for (int i = 0; i < PivotCount; i++)
            {
                int[] ModerateEncounterTreeIndex = new int[5];
                for (int j = 0; j < ModerateEncounterTreeIndex.Length; j++)
                    ModerateEncounterTreeIndex[j] = (i + j) % PivotCount;
                TrainersIndex[i] = ModerateEncounterTreeIndex.OrderBy(x => x).ToArray();
            }
            return TrainersIndex;
        }

        internal static TreesArea[] GetArray(byte[][] entries) => entries.Select(z => new TreesArea(z)).ToArray();

        public readonly int Location;
        private readonly TreeEncounterAvailable[] TrainerModerateEncounterTree;
        private readonly TreeEncounterAvailable[] TrainerLowEncounterTree;
        private readonly int[] ValidTreeIndex;
        private readonly int[] InvalidTreeIndex;
        private readonly TreeCoordinates[] ValidTrees;
        private readonly TreeCoordinates[] InvalidTrees;

        public TreeEncounterAvailable[] GetTrees(SlotType t) => t == SlotType.Headbutt
            ? TrainerModerateEncounterTree
            : TrainerLowEncounterTree;

        private TreesArea(byte[] entry)
        {
            // Coordinates of trees were obtained with the program G2Map
            // ValidTrees are those accessible by the player
            Location = entry[0];
            ValidTrees = new TreeCoordinates[entry[1]];
            var ofs = 2;
            for (int i = 0; i < ValidTrees.Length; i++, ofs += 2)
                ValidTrees[i] = new TreeCoordinates(entry[ofs], entry[ofs + 1]);

            // Invalid tress are trees that the player can not reach without cheating devices, like a tree beyond other trees
            InvalidTrees = new TreeCoordinates[entry[ofs]];
            ofs++;
            for (int i = 0; i < InvalidTrees.Length; i++, ofs += 2)
                InvalidTrees[i] = new TreeCoordinates(entry[ofs], entry[ofs + 1]);

            // For legality purposes, only the tree index is needed.
            // Group the trees data by their index; trees that share indexes are indistinguishable from one another
            ValidTreeIndex = ValidTrees.Select(t => t.Index).Distinct().OrderBy(i => i).ToArray();
            InvalidTreeIndex = InvalidTrees.Select(t => t.Index).Distinct().OrderBy(i => i).Except(ValidTreeIndex).ToArray();

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

        #if DEBUG
        public void DumpLocation(string[] locationNames)
        {
            string loc = locationNames[Location];
            System.Console.WriteLine($"Location: {loc}");
            System.Console.WriteLine("Valid:");
            foreach (var tree in ValidTrees)
                System.Console.WriteLine($"{tree.Index} @ ({tree.X:D2},{tree.Y:D2})");
            System.Console.WriteLine("Invalid:");
            foreach (var tree in InvalidTrees)
                System.Console.WriteLine($"{tree.Index} @ ({tree.X:D2},{tree.Y:D2})");
            System.Console.WriteLine("===");
        }
        #endif
    }
}
