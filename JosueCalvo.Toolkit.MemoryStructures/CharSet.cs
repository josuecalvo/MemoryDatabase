using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JosueCalvo.Toolkit.MemoryStructures
{
    public class CharSet
    {
        public string CharacterSortOrder { get; private set; }
        public char UnknownChar { get; private set; }
        public char KeyFinisher { get { return '\n'; } }

        public ReadOnlyDictionary<char, char> CharConvertion { get; }

        public CharSet(string characterSortOrder, char unknownChar, Dictionary<char, char> charConversion)
        {
            if (characterSortOrder.IndexOf(unknownChar) == -1)
            {
                throw new ArgumentOutOfRangeException("The unkown char must be part of the character sort order");
            }

            UnknownChar = unknownChar;
            CharacterSortOrder = KeyFinisher + characterSortOrder.Replace("\n", "");
            CharConvertion = new ReadOnlyDictionary<char, char>(charConversion);
        }

        public CharSet(string characterSortOrder, char unknownChar) : this(characterSortOrder, unknownChar, new Dictionary<char, char>())
        { }

        public CharSet() : this(" .01234567890abcdefghijklmnopqrstuvwxyz", '.')
        { }
    }
}
