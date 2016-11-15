using Microsoft.VisualStudio.TestTools.UnitTesting;
using PKHeX.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PKHeX.Tests.Reflection
{
    [TestClass]
    public class BatchEditAttributeTests
    {
        const string BatchEditCategory = "Batch Editor";

        [TestMethod] [TestCategory(BatchEditCategory)]
        public void CheckPKHeXUsage()
        {
            var badProperties = new List<PropertyInfo>();

            // Check every type in PKHeX to ensure attributes are only on properties of compatible types
            foreach (var t in typeof(PKHeX.Main).Assembly.GetTypes())
            {
                foreach (var p in t.GetProperties())
                {
                    foreach (BatchEditAttribute attribute in p.GetCustomAttributes(typeof(BatchEditAttribute), true))
                    {
                        if (!attribute.SupportedTypes.Contains(p.PropertyType))
                        {
                            badProperties.Add(p);
                            break;
                        }                        
                    }
                }
            }
            Assert.AreEqual(0, badProperties.Count, $"{badProperties.Count} incorrect attribute usages in PKHeX.exe.  Number may be inflated due to inheritance.");
        }
    }
}
