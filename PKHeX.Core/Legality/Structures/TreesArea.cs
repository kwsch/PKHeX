using System.Linq;

namespace PKHeX.Core
{
    // Pokemon Crystal Headbutt tree encounters by trainer id, based on mechanics described in
    // https://bulbapedia.bulbagarden.net/wiki/Headbutt_tree#Mechanics

    public enum TreeEncounterAvailable
    {
        ValidTree, // Encounter is possible a reachable tree
        InvalidTree, // Encounter is only possible a tree reachable only with walk-trough walls cheats
        Impossible // Encounter is not possible in any tree
    }

    internal class TreeCoordinates
    {
        internal int X { get; set; }
        internal int Y { get; set; }
        internal int Index => ((X * Y + X + Y) / 5) % 10;
    }

    public class TreesArea
    {
        public int Location { get; private set; }
        public TreeEncounterAvailable[] TrainerModerateEncounterTree { get; private set; }
        public TreeEncounterAvailable[] TrainerLowEncounterTree { get; private set; }

        private int[] ValidTreeIndex { get; set; }
        private int[] InvalidTreeIndex { get; set; }

        private TreeCoordinates[] ValidTrees { get; set; }
        private TreeCoordinates[] InvalidTrees { get; set; }
        private static int[][] TrainerModerateTreeIndex { get; set; }

        internal static TreesArea[] GetArray(byte[][] entries)
        {
            if (entries == null)
                return null;
            TrainerModerateTreeIndex = GenerateTrainersTreeIndex();
            var Areas = new TreesArea[entries.Length];
            for(int i = 0; i < entries.Length; i++)
            {
                Areas[i] = GetArea(entries[i]);
            }
            return Areas;
        }

        private static int[][] GenerateTrainersTreeIndex()
        {
            // A tree have a low encounter or moderate encounter base on the TID Pivot Index ( TID % 10)
            // Calculate for every Trainer Pivot Index the 5 tree index for low encounters
            int[][] TrainersIndex = new int[10][];
            for (int pivotindex = 0; pivotindex < 10; pivotindex++)
            {
                int[] ModerateEncounterTreeIndex = new int[5];
                for(int index = 0; index <= 4; index++)
                    ModerateEncounterTreeIndex[index] = (pivotindex + index) % 10;
                TrainersIndex[pivotindex] = ModerateEncounterTreeIndex.OrderBy(x => x).ToArray();
            }
            return TrainersIndex;
        }

        private static TreesArea GetArea(byte[] entrie)
        {
            var Area = new TreesArea();

            Area.ReadAreaRawData(entrie);
            Area.GenerateAreaTreeIndex();
            Area.GenerateAreaTrainerEncounters();
            return Area;
        }

        private void ReadAreaRawData(byte[] entrie)
        {
            // Coordinates of trees for every are obtained with the program G2Map
            // ValidTrees are those accessible by the player
            // Invalid tress are trees that the player can not reach without cheating devices, like a tree beyond other trees
            Location = entrie[0];
            ValidTrees = new TreeCoordinates[entrie[1]];

            var ofs = 2;
            for (int i = 0; i < ValidTrees.Length; i++)
            {
                ValidTrees[i] = new TreeCoordinates()
                {
                    X = entrie[ofs],
                    Y = entrie[ofs + 1]
                };
                ofs += 2;
            }
            InvalidTrees = new TreeCoordinates[entrie[ofs]];
            ofs += 1;
            for (int i = 0; i < InvalidTrees.Length; i++)
            {
                InvalidTrees[i] = new TreeCoordinates()
                {
                    X = entrie[ofs],
                    Y = entrie[ofs + 1]
                };
                ofs += 2;
            }
        }

        private void GenerateAreaTreeIndex()
        {
            // For legality purpose only the tree index is needed, group the trees data by their index, trees with the same index are indistinguishable
            ValidTreeIndex = ValidTrees.Select(t => t.Index).Distinct().OrderBy(i => i).ToArray();
            InvalidTreeIndex = InvalidTrees.Select(t => t.Index).Distinct().OrderBy(i => i).Except(ValidTreeIndex).ToArray();
        }

        private void GenerateAreaTrainerEncounters()
        {
            // Check for every trainer pivot index if there are trees with low encounter and moderate encounter available in the area
            TrainerModerateEncounterTree = new TreeEncounterAvailable[10];
            TrainerLowEncounterTree = new TreeEncounterAvailable[10];
            for (int pivotindex = 0; pivotindex < 10; pivotindex++)
            {
                var TrainerModerateTrees = TrainerModerateTreeIndex[pivotindex];

                var ModerateValid = ValidTreeIndex.Any(t => TrainerModerateTrees.Contains(t));
                var ModerateInvalid = InvalidTreeIndex.Any(t => TrainerModerateTrees.Contains(t));
                if (ModerateValid)
                    // There is a valid tree with an index for moderate encounters
                    TrainerModerateEncounterTree[pivotindex] = TreeEncounterAvailable.ValidTree;
                else if (ModerateInvalid)
                    // There is a tree with an index for moderate encounters but is invalid
                    TrainerModerateEncounterTree[pivotindex] = TreeEncounterAvailable.InvalidTree;
                else
                    // No trees for moderate encounters
                    TrainerModerateEncounterTree[pivotindex] = TreeEncounterAvailable.Impossible;

                var LowValid = ValidTreeIndex.Except(TrainerModerateTrees).Any();
                var LowInvalid = InvalidTreeIndex.Except(TrainerModerateTrees).Any();
                if (LowValid)
                    // There is a valid tree with an index for low encounters
                    TrainerLowEncounterTree[pivotindex] = TreeEncounterAvailable.ValidTree;
                else if (LowInvalid)
                    // There is a tree with an index for low encounters but is invalid
                    TrainerLowEncounterTree[pivotindex] = TreeEncounterAvailable.InvalidTree;
                else
                    // No trees for low encounters
                    TrainerLowEncounterTree[pivotindex] = TreeEncounterAvailable.Impossible;
            }
        }
    }
}
