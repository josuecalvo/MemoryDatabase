using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JosueCalvo.Toolkit.MemoryStructures
{
    public class Index<T> : IEnumerable<KeyValuePair<string, T>>
    {
        List<int[]> _pages = new List<int[]>();
        KeySet<T> _keys = new KeySet<T>();

        public CharSet CharSet { get; private set; }

        public int Count { get; private set; }

        public Index(CharSet charSet)
        {
            CharSet = charSet;
            _pages.Add(new int[CharSet.CharacterSortOrder.Length]);
        }

        public void AddKey(string key, T value)
        {
            var sortingKey = GetSortingKey(key);
            Count++;

            var pageKey = key + CharSet.KeyFinisher;
            var page = 0;
            for (var i = 0; i < pageKey.Length; i++)
            {
                var pagePosition = GetPagePosition(pageKey.Substring(i, 1));

                if (_pages[page][pagePosition] == 0)
                {
                    _pages[page][pagePosition] = (_keys.AddValue(sortingKey, key, value) * -1) - 1;
                    break;
                }
                else if (_pages[page][pagePosition] > 0)
                {
                    page = _pages[page][pagePosition];
                }
                else
                {
                    var keysPointer = (_pages[page][pagePosition] * -1) - 1;
                    if (_keys.GetSortingKey(keysPointer) == sortingKey)
                    {
                        _keys.AppendValue(keysPointer, key, value);
                        break;
                    }
                    else
                    {
                        BranchKey(sortingKey, key, i, _keys.GetSortingKey(keysPointer), _pages[page][pagePosition], page, value);
                        break;
                    }
                }
            }
        }

        void BranchKey(string sortingKey, string key, int startingCharNumber, string foundKey, int foundPointer, int page, T value)
        {
            var pageKey = key + CharSet.KeyFinisher;
            var foundPageKey = foundKey + CharSet.KeyFinisher;

            var pagePosition = GetPagePosition(pageKey.Substring(startingCharNumber, 1));
            _pages.Add(new int[CharSet.CharacterSortOrder.Length]);
            _pages[page][pagePosition] = _pages.Count - 1;
            page = _pages.Count - 1;

            int i;
            for (i = startingCharNumber + 1; i < pageKey.Length; i++)
            {
                if (foundPageKey.Length >= i && foundPageKey.Substring(i, 1) == pageKey.Substring(i, 1))
                {
                    pagePosition = GetPagePosition(pageKey.Substring(i, 1));
                    _pages.Add(new int[CharSet.CharacterSortOrder.Length]);
                    _pages[page][pagePosition] = _pages.Count - 1;
                    page = _pages.Count - 1;
                }
                else
                {
                    pagePosition = GetPagePosition(pageKey.Substring(i, 1));

                    _pages[page][pagePosition] = (_keys.AddValue(sortingKey, key, value) * -1) - 1;

                    pagePosition = GetPagePosition(foundPageKey.Substring(i, 1));
                    _pages[page][pagePosition] = foundPointer;
                    break;
                }
            }
        }

        public List<KeyValuePair<string, T>> GetValues(string key)
        {
            List<KeyValuePair<string, T>> output = null;

            var pageKey = key + CharSet.KeyFinisher;
            var page = 0;
            for (var i = 0; i < pageKey.Length; i++)
            {
                var pagePosition = GetPagePosition(pageKey.Substring(i, 1));

                if (_pages[page][pagePosition] == 0)
                {
                    break;
                }
                else if (_pages[page][pagePosition] > 0)
                {
                    page = _pages[page][pagePosition];
                }
                else
                {
                    var pointer = (_pages[page][pagePosition] * -1) - 1;
                    output = _keys.GetValues(pointer);
                    break;
                }
            }

            return output;
        }

        public List<KeyValuePair<string, T>> GetAll(int page = 0)
        {
            var output = new List<KeyValuePair<string, T>>();

            foreach (var pointer in _pages[page])
            {
                if (pointer > 0)
                {
                    output.AddRange(GetAll(pointer));
                }
                else if (pointer < 0)
                {
                    var keyValuesPair = _keys.GetValues((pointer * -1) - 1);
                    output.AddRange(keyValuesPair);
                }
            }

            return output;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            foreach (var pointer in _pages[0])
            {
                if (pointer > 0)
                {
                    foreach (var value in GetAll(pointer))
                    {
                        yield return value;
                    }
                }
                else if (pointer < 0)
                {
                    var keyValuesPair = _keys.GetValues((pointer * -1) - 1);
                    foreach (var pair in keyValuesPair)
                    {
                        yield return new KeyValuePair<string, T>(pair.Key, pair.Value);
                    }
                }
            }
        }

        int GetPagePosition(string keyCharString)
        {
            var keyChar = keyCharString[0];

            if (CharSet.CharConversion.Any())
            {
                char foundKeyChar;
                if (CharSet.CharConversion.TryGetValue(keyChar, out foundKeyChar))
                {
                    keyChar = foundKeyChar;
                }
            }

            var output = CharSet.CharacterSortOrder.IndexOf(keyChar);

            if (output == -1)
            {
                output = CharSet.CharacterSortOrder.IndexOf(CharSet.UnknownChar);
            }

            return output;
        }

        string GetSortingKey(string key)
        {
            key = key.ToLower();
            if (!CharSet.CharConversion.Any())
            {
                return key;
            }
            else
            {
                var sortingKey = new StringBuilder();

                foreach (char keyChar in key)
                {
                    char foundKeyChar;
                    if (CharSet.CharConversion.TryGetValue(keyChar, out foundKeyChar))
                    {
                        sortingKey.Append(foundKeyChar);
                    }
                    else
                    {
                        sortingKey.Append(keyChar);
                    }
                }

                return sortingKey.ToString();
            }
        }

    }
}
