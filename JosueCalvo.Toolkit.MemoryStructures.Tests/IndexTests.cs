using NUnit.Framework;

namespace JosueCalvo.Toolkit.MemoryStructures.Tests
{
    public class IndexTests
    {
        [Test]
        public void Can_create_Index()
        {
            var index = new Index<string>(new CharSet());
        }

        [Test]
        public void Can_add_element_to_index()
        {
            var index = new Index<string>(new CharSet());
            index.AddKey("lala", "value1");
            index.AddKey("lala", "value2");
            index.AddKey("pepe", "value3");
            index.AddKey("la", "value4");
            index.AddKey("lalala", "value5");

            var values = index.GetValues("lala");
            Assert.AreEqual(2, values.Count);

            values = index.GetValues("pepe");
            Assert.AreEqual(1, values.Count);
            Assert.AreEqual("value3", values[0].Value);

            values = index.GetValues("la");
            Assert.AreEqual(1, values.Count);
            Assert.AreEqual("value4", values[0].Value);

            values = index.GetValues("lalala");
            Assert.AreEqual(1, values.Count);
            Assert.AreEqual("value5", values[0].Value);
        }

        [Test]
        public void Can_get_sorted_elements_from_index()
        {
            var index = new Index<string>(new CharSet());
            index.AddKey("4", "value4");
            index.AddKey("5", "value5");
            index.AddKey("1", "value1");
            index.AddKey("2", "value2");
            index.AddKey("3", "value3");

            var lastKey = string.Empty;
            foreach (var keyValuePair in index)
            {
                Assert.GreaterOrEqual(keyValuePair.Key, lastKey);
                lastKey = keyValuePair.Key;
            }
        }

        [Test]
        public void Can_get_spanish_traditional_sorted_elements_from_index()
        {
            var index = new Index<string>(new CharSet(" .01234567890abcdefghijklmnñopqrstuvwxyz", '.'));
            index.AddKey("01 OOO", "value3");
            index.AddKey("01 ooo", "value1");
            index.AddKey("01 ñ", "value1");
            index.AddKey("01 a", "value4");
            index.AddKey("01 n", "value5");
            index.AddKey("01 Mama", "value2");

            var values = index.GetAll();
            Assert.AreEqual("01 ñ", values[3].Key);
            Assert.AreEqual("01 OOO", values[4].Key);
            Assert.AreEqual("01 ooo", values[5].Key);
        }

        [Test]
        public void Can_get_spanish_traditional_accent_insensitive_sorted_elements_from_index()
        {
            var index = new Index<string>(new CharSet(" .01234567890abcdefghijklmnñopqrstuvwxyz", '.', new System.Collections.Generic.Dictionary<char, char>
            {
                {'á', 'a'},
                {'é', 'e'},
                {'í', 'i'},
                {'ó', 'o'},
                {'ú', 'u'},
            }
            ));

            index.AddKey("01 o", "value5");
            index.AddKey("01 u", "value2");
            index.AddKey("01 í", "value4");
            index.AddKey("01 i", "value4");
            index.AddKey("01 a", "value3");
            index.AddKey("01 e", "value1");

            var values = index.GetAll();
            Assert.AreEqual("01 í", values[2].Key);
        }
    }
}
