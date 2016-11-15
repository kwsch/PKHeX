using Microsoft.VisualStudio.TestTools.UnitTesting;
using PKHeX.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace PKHeX.Tests.Reflection
{

    [TestClass]
    public class StringListEnumAttributeTests
    {
        const string BatchEditCategory = "Batch Editor";

        private class TestUsage
        {
            [StringListEnum(PKHeX.Util.MovesListName)]
            public int Move{ get; set; }

            [StringListEnum(PKHeX.Util.ItemsListName)]
            public int Item { get; set; }
        }

        CultureInfo PreviousCulture;

        [TestInitialize]
        public void TestInit()
        {
            PreviousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Thread.CurrentThread.CurrentCulture = PreviousCulture;
        }

        [TestMethod] [TestCategory(BatchEditCategory)]
        public void TestStringListEnumAttributeGetValue()
        {
            var testClass = new TestUsage();
            testClass.Move = 1;
            testClass.Item = 1;

            Assert.AreEqual("Pound", ReflectUtil.GetValue(testClass, nameof(TestUsage.Move)));
            Assert.AreEqual("Master Ball", ReflectUtil.GetValue(testClass, nameof(TestUsage.Item)));
        }

        [TestMethod]
        [TestCategory(BatchEditCategory)]
        public void TestStringListEnumAttributeSetValueString()
        {
            var testClass = new TestUsage();
            testClass.Move = 1;
            testClass.Item = 1;

            ReflectUtil.SetValue(testClass, nameof(TestUsage.Move), "Karate Chop");
            ReflectUtil.SetValue(testClass, nameof(TestUsage.Item), "Ultra Ball");

            Assert.AreEqual(2, testClass.Move, "Failed to update property.");
            Assert.AreEqual(2, testClass.Item, "Failed to update property.");
        }

        [TestMethod]
        [TestCategory(BatchEditCategory)]
        public void TestStringListEnumAttributeSetValueInteger()
        {
            var testClass = new TestUsage();
            testClass.Move = 1;
            testClass.Item = 1;

            ReflectUtil.SetValue(testClass, nameof(TestUsage.Move), 2);
            ReflectUtil.SetValue(testClass, nameof(TestUsage.Item), 2);

            Assert.AreEqual(2, testClass.Move, "Failed to update property.");
            Assert.AreEqual(2, testClass.Item, "Failed to update property.");
        }

        [TestMethod]
        [TestCategory(BatchEditCategory)]
        public void TestStringListEnumAttributeSetValueStringInteger()
        {
            var testClass = new TestUsage();
            testClass.Move = 1;
            testClass.Item = 1;

            ReflectUtil.SetValue(testClass, nameof(TestUsage.Move), "2");
            ReflectUtil.SetValue(testClass, nameof(TestUsage.Item), "2");

            Assert.AreEqual(2, testClass.Move, "Failed to update property.");
            Assert.AreEqual(2, testClass.Item, "Failed to update property.");
        }

        [TestMethod]
        [TestCategory(BatchEditCategory)]
        public void TestStringListEnumAttributeIsValueEqualString()
        {
            var testClass = new TestUsage();
            testClass.Move = 1;
            testClass.Item = 1;

            Assert.IsTrue(ReflectUtil.GetValueEquals(testClass, nameof(TestUsage.Move), "Pound"), "Failed comparing string");
            Assert.IsTrue(ReflectUtil.GetValueEquals(testClass, nameof(TestUsage.Item), "Master Ball"), "Failed comparing string");
        }

        [TestMethod]
        [TestCategory(BatchEditCategory)]
        public void TestStringListEnumAttributeIsValueEqualInteger()
        {
            var testClass = new TestUsage();
            testClass.Move = 1;
            testClass.Item = 1;

            Assert.IsTrue(ReflectUtil.GetValueEquals(testClass, nameof(TestUsage.Move), 1), "Failed comparing integer");
            Assert.IsTrue(ReflectUtil.GetValueEquals(testClass, nameof(TestUsage.Item), 1), "Failed comparing integer");
        }

        [TestMethod]
        [TestCategory(BatchEditCategory)]
        public void TestStringListEnumAttributeIsValueEqualStringNegative()
        {
            var testClass = new TestUsage();
            testClass.Move = 1;
            testClass.Item = 1;

            Assert.IsFalse(ReflectUtil.GetValueEquals(testClass, nameof(TestUsage.Move), "Karate Chop"), "False positive comparing string");
            Assert.IsFalse(ReflectUtil.GetValueEquals(testClass, nameof(TestUsage.Item), "Ultra Ball"), "False positive comparing string");
        }

        [TestMethod]
        [TestCategory(BatchEditCategory)]
        public void TestStringListEnumAttributeIsValueEqualIntegerNegative()
        {
            var testClass = new TestUsage();
            testClass.Move = 1;
            testClass.Item = 1;

            Assert.IsFalse(ReflectUtil.GetValueEquals(testClass, nameof(TestUsage.Move), 2), "False positive comparing integer");
            Assert.IsFalse(ReflectUtil.GetValueEquals(testClass, nameof(TestUsage.Item), 2), "False positive comparing integer");
        }


    }
}
