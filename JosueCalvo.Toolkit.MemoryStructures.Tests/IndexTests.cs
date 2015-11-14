using NUnit.Framework;
using System.Collections.Generic;

namespace JosueCalvo.Toolkit.MemoryStructures.Tests
{
    public class IndexTests
    {
        [Test]
        public void Can_create_Index()
        {
            var index = new Index<string>(" .01234567890abcdefghijklmnopqrstuvwxyz");
        }

        [Test]
        public void Can_add_element_to_index()
        {
            var index = new Index<string>(" .01234567890abcdefghijklmnopqrstuvwxyz");
            index.AddKey("lala", "value1");
            index.AddKey("lala", "value2");
            index.AddKey("pepe", "value3");
            index.AddKey("la", "value4");
            index.AddKey("lalala", "value5");

            var values = index.GetValues("lala");
            Assert.AreEqual(2, values.Count);

            values = index.GetValues("pepe");
            Assert.AreEqual(1, values.Count);
            Assert.AreEqual("value3", values[0]);

            values = index.GetValues("la");
            Assert.AreEqual(1, values.Count);
            Assert.AreEqual("value4", values[0]);

            values = index.GetValues("lalala");
            Assert.AreEqual(1, values.Count);
            Assert.AreEqual("value5", values[0]);
        }

        [Test]
        public void Can_get_sorted_elements_from_index()
        {
            var index = new Index<string>(" .01234567890abcdefghijklmnopqrstuvwxyz");
            index.AddKey("4", "value4");
            index.AddKey("5", "value5");
            index.AddKey("1", "value1");
            index.AddKey("2", "value2");
            index.AddKey("3", "value3");

            var values = index.GetAll();

            var lastKey = string.Empty;
            foreach (var keyValuePair in index)
            {
                Assert.GreaterOrEqual(keyValuePair.Key, lastKey);
                lastKey = keyValuePair.Key;
            }
        }
    }
}
