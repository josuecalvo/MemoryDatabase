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
        List<KeyValuePair<string, List<KeyValuePair<string, T>>>> _keys = new List<KeyValuePair<string, List<KeyValuePair<string, T>>>>();

        public CharSet CharSet { get; private set; }

        public int Count { get { return _keys.Count; } }

        public Index(CharSet charSet)
        {
            CharSet = charSet;
            _pages.Add(new int[CharSet.CharacterSortOrder.Length]);
        }

        public void AddKey(string key, T value)
        {
            var sortingKey = GetSortingKey(key);
            lock (_pages)
            {
                var values = AddKeyToPages(sortingKey);
                values.Add(new KeyValuePair<string, T>(key, value));
            }
        }

        List<KeyValuePair<string, T>> AddKeyToPages(string key)
        {
            List<KeyValuePair<string, T>> output = null;

            var pageKey = key + CharSet.KeyFinisher;
            var page = 0;
            for (var i = 0; i < pageKey.Length; i++)
            {
                var pagePosition = GetPagePosition(pageKey.Substring(i, 1));

                if (_pages[page][pagePosition] == 0)
                {
                    output = new List<KeyValuePair<string, T>>();
                    _keys.Add(new KeyValuePair<string, List<KeyValuePair<string, T>>>(key, output));
                    _pages[page][pagePosition] = _keys.Count * -1;
                    break;
                }
                else if (_pages[page][pagePosition] > 0)
                {
                    page = _pages[page][pagePosition];
                }
                else
                {
                    var keyValuesPair = _keys[(_pages[page][pagePosition] * -1) - 1];
                    if (keyValuesPair.Key == key)
                    {
                        output = keyValuesPair.Value;
                        break;
                    }
                    else
                    {
                        output = BranchKey(key, i, keyValuesPair.Key, _pages[page][pagePosition], page);
                        break;
                    }
                }
            }

            return output;
        }

        List<KeyValuePair<string, T>> BranchKey(string key, int startingCharNumber, string foundKey, int foundPointer, int page)
        {
            var output = new List<KeyValuePair<string, T>>();

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
                    _keys.Add(new  KeyValuePair<string, List<KeyValuePair<string, T>>>(key, output));
                    _pages[page][pagePosition] = _keys.Count * -1;

                    pagePosition = GetPagePosition(foundPageKey.Substring(i, 1));
                    _pages[page][pagePosition] = foundPointer;
                    break;
                }
            }

            return output;
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
                    var keyValuesPair = _keys[(_pages[page][pagePosition] * -1) - 1];
                    if (keyValuesPair.Key == key)
                    {
                        output = keyValuesPair.Value;
                        break;
                    }
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
                    var keyValuesPair = _keys[(pointer * -1) - 1];
                    foreach (var value in keyValuesPair.Value)
                    {
                        output.Add(new KeyValuePair<string, T>(value.Key, value.Value));
                    }
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
                    var keyValuesPair = _keys[(pointer * -1) - 1];
                    foreach (var value in keyValuesPair.Value)
                    {
                        yield return new KeyValuePair<string, T>(value.Key, value.Value);
                    }
                }
            }
        }

        int GetPagePosition(string keyCharString)
        {
            var keyChar = keyCharString[0];

            if (CharSet.CharConvertion.Any())
            {
                char foundKeyChar;
                if (CharSet.CharConvertion.TryGetValue(keyChar, out foundKeyChar))
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
            if (!CharSet.CharConvertion.Any())
            {
                return key;
            }
            else
            {
                var sortingKey = new StringBuilder();

                foreach (char keyChar in key)
                {
                    char foundKeyChar;
                    if (CharSet.CharConvertion.TryGetValue(keyChar, out foundKeyChar))
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
