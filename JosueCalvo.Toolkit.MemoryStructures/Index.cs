using System;
using System.Collections;
using System.Collections.Generic;

namespace JosueCalvo.Toolkit.MemoryStructures
{
    public class Index<T> : IEnumerable<KeyValuePair<string, T>>
    {
        const char KeyFinisher = '\n';

        List<int[]> _pages = new List<int[]>();
        List<KeyValuePair<string, List<T>>> _keys = new List<KeyValuePair<string, List<T>>>();

        public string Characters { get; private set; }

        public int Count { get { return _keys.Count; } }

        public Index(string characters)
        {
            Characters = KeyFinisher + characters;
            _pages.Add(new int[characters.Length]);
        }

        public void AddKey(string key, T value)
        {
            lock (_pages)
            {
                var values = AddKeyToPages(key);
                values.Add(value);
            }
        }

        List<T> AddKeyToPages(string key)
        {
            List<T> output = null;

            var pageKey = key + KeyFinisher;
            var page = 0;
            for (var i = 0; i < pageKey.Length; i++)
            {
                var pagePosition = Characters.IndexOf(pageKey.Substring(i, 1));

                if (_pages[page][pagePosition] == 0)
                {
                    output = new List<T>();
                    _keys.Add(new KeyValuePair<string, List<T>>(key, output));
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

        List<T> BranchKey(string key, int startingCharNumber, string foundKey, int foundPointer, int page)
        {
            var output = new List<T>();

            var pageKey = key + KeyFinisher;
            var foundPageKey = foundKey + KeyFinisher;

            var pagePosition = Characters.IndexOf(pageKey.Substring(startingCharNumber, 1));
            _pages.Add(new int[Characters.Length]);
            _pages[page][pagePosition] = _pages.Count - 1;
            page = _pages.Count - 1;

            int i;
            for (i = startingCharNumber + 1; i < pageKey.Length; i++)
            {
                if (foundPageKey.Length >= i && foundPageKey.Substring(i, 1) == pageKey.Substring(i, 1))
                {
                    pagePosition = Characters.IndexOf(pageKey.Substring(i, 1));
                    _pages.Add(new int[Characters.Length]);
                    _pages[page][pagePosition] = _pages.Count - 1;
                    page = _pages.Count - 1;
                }
                else
                {
                    pagePosition = Characters.IndexOf(pageKey.Substring(i, 1));
                    _keys.Add(new KeyValuePair<string, List<T>>(key, output));
                    _pages[page][pagePosition] = _keys.Count * -1;

                    pagePosition = Characters.IndexOf(foundPageKey.Substring(i, 1));
                    _pages[page][pagePosition] = foundPointer;
                    break;
                }
            }

            return output;
        }

        public List<T> GetValues(string key)
        {
            List<T> output = null;

            var pageKey = key + KeyFinisher;
            var page = 0;
            for (var i = 0; i < pageKey.Length; i++)
            {
                var pagePosition = Characters.IndexOf(pageKey.Substring(i, 1));

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
                        output.Add(new KeyValuePair<string, T>(keyValuesPair.Key, value));
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
                        yield return new KeyValuePair<string, T>(keyValuesPair.Key, value);
                    }
                }
            }
        }

        public IEnumerator<KeyValuePair<string, T>> GetRange(string start, string end)
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
                        yield return new KeyValuePair<string, T>(keyValuesPair.Key, value);
                    }
                }
            }
        }
    }
}
