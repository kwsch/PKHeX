using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX
{
    public partial class Util
    {
        public static string[] getStringList(string f)
        {
            object txt = Properties.Resources.ResourceManager.GetObject(f); // Fetch File, \n to list.
            if (txt == null) return new string[0];
            string[] rawlist = ((string)txt).Split('\n');
            for (int i = 0; i < rawlist.Length; i++)
                rawlist[i] = rawlist[i].Trim();
            return rawlist;
        }
        public static string[] getStringList(string f, string l)
        {
            object txt = Properties.Resources.ResourceManager.GetObject("text_" + f + "_" + l); // Fetch File, \n to list.
            if (txt == null) return new string[0];
            string[] rawlist = ((string)txt).Split('\n');
            for (int i = 0; i < rawlist.Length; i++)
                rawlist[i] = rawlist[i].Trim();
            return rawlist;
        }
        internal static string[] getNulledStringArray(string[] SimpleStringList)
        {
            try
            {
                string[] newlist = new string[ToInt32(SimpleStringList[SimpleStringList.Length - 1].Split(',')[0]) + 1];
                for (int i = 1; i < SimpleStringList.Length; i++)
                    newlist[ToInt32(SimpleStringList[i].Split(',')[0])] = SimpleStringList[i].Split(',')[1];
                return newlist;
            }
            catch { return null; }
        }
        
        // DataSource Providing
        internal static List<ComboItem> getCBList(string textfile, string lang)
        {
            // Set up
            string[] inputCSV = getStringList(textfile);

            // Get Language we're fetching for
            int index = Array.IndexOf(new[] { "ja", "en", "fr", "de", "it", "es", "ko", "zh", }, lang);

            // Set up our Temporary Storage
            string[] unsortedList = new string[inputCSV.Length - 1];
            int[] indexes = new int[inputCSV.Length - 1];

            // Gather our data from the input file
            for (int i = 1; i < inputCSV.Length; i++)
            {
                string[] countryData = inputCSV[i].Split(',');
                indexes[i - 1] = Convert.ToInt32(countryData[0]);
                unsortedList[i - 1] = countryData[index + 1];
            }

            // Sort our input data
            string[] sortedList = new string[inputCSV.Length - 1];
            Array.Copy(unsortedList, sortedList, unsortedList.Length);
            Array.Sort(sortedList);

            // Arrange the input data based on original number
            return sortedList.Select(s => new ComboItem
            {
                Text = s,
                Value = indexes[Array.IndexOf(unsortedList, s)]
            }).ToList();
        }
        internal static List<ComboItem> getCBList(string[] inStrings, params int[][] allowed)
        {
            List<ComboItem> cbList = new List<ComboItem>();
            if (allowed?.First() == null)
                allowed = new[] { Enumerable.Range(0, inStrings.Length).ToArray() };

            foreach (int[] list in allowed)
            {
                // Sort the Rest based on String Name
                string[] unsortedChoices = new string[list.Length];
                for (int i = 0; i < list.Length; i++)
                    unsortedChoices[i] = inStrings[list[i]];

                string[] sortedChoices = new string[unsortedChoices.Length];
                Array.Copy(unsortedChoices, sortedChoices, unsortedChoices.Length);
                Array.Sort(sortedChoices);

                // Add the rest of the items
                cbList.AddRange(sortedChoices.Select(s => new ComboItem
                {
                    Text = s,
                    Value = list[Array.IndexOf(unsortedChoices, s)]
                }));
            }
            return cbList;
        }
        internal static List<ComboItem> getOffsetCBList(List<ComboItem> cbList, string[] inStrings, int offset, int[] allowed)
        {
            if (allowed == null)
                allowed = Enumerable.Range(0, inStrings.Length).ToArray();

            int[] list = (int[])allowed.Clone();
            for (int i = 0; i < list.Length; i++)
                list[i] -= offset;

            // Sort the Rest based on String Name
            string[] unsortedChoices = new string[allowed.Length];
            for (int i = 0; i < allowed.Length; i++)
                unsortedChoices[i] = inStrings[list[i]];

            string[] sortedChoices = new string[unsortedChoices.Length];
            Array.Copy(unsortedChoices, sortedChoices, unsortedChoices.Length);
            Array.Sort(sortedChoices);

            // Add the rest of the items
            cbList.AddRange(sortedChoices.Select(s => new ComboItem
            {
                Text = s,
                Value = allowed[Array.IndexOf(unsortedChoices, s)]
            }));
            return cbList;
        }
        internal static List<ComboItem> getVariedCBList(string[] inStrings, int[] stringNum, int[] stringVal)
        {
            // Set up
            List<ComboItem> newlist = new List<ComboItem>();

            for (int i = 4; i > 1; i--) // add 4,3,2
            {
                // First 3 Balls are always first
                ComboItem ncbi = new ComboItem
                {
                    Text = inStrings[i],
                    Value = i
                };
                newlist.Add(ncbi);
            }

            // Sort the Rest based on String Name
            string[] ballnames = new string[stringNum.Length];
            for (int i = 0; i < stringNum.Length; i++)
                ballnames[i] = inStrings[stringNum[i]];

            string[] sortedballs = new string[stringNum.Length];
            Array.Copy(ballnames, sortedballs, ballnames.Length);
            Array.Sort(sortedballs);

            // Add the rest of the balls
            newlist.AddRange(sortedballs.Select(s => new ComboItem
            {
                Text = s,
                Value = stringVal[Array.IndexOf(ballnames, s)]
            }));
            return newlist;
        }
        internal static List<ComboItem> getUnsortedCBList(string textfile)
        {
            // Set up
            List<ComboItem> cbList = new List<ComboItem>();
            string[] inputCSV = getStringList(textfile);

            // Gather our data from the input file
            for (int i = 1; i < inputCSV.Length; i++)
            {
                string[] inputData = inputCSV[i].Split(',');
                ComboItem ncbi = new ComboItem
                {
                    Text = inputData[1],
                    Value = Convert.ToInt32(inputData[0])
                };
                cbList.Add(ncbi);
            }
            return cbList;
        }
    }
}
