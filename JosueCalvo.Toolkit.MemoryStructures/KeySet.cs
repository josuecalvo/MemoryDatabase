using System.Collections.Generic;

namespace JosueCalvo.Toolkit.MemoryStructures
{
    public class KeySet<T>
    {
        List<KeyHolder> _keys = new List<KeyHolder>();
        List<ValueHolder<T>> _values = new List<ValueHolder<T>>();

        public int AddValue(string sortingKey, string originalKey, T value)
        {
            _values.Add(new ValueHolder<T>
            {
                OriginalKey = originalKey,
                Value = value,
                NextValueHolder = null
            });

            _keys.Add(new KeyHolder
            {
                SortingKey = sortingKey,
                FirstValueHolder = _values.Count - 1,
                LastValueHolder = _values.Count - 1
            });

            return _keys.Count - 1;
        }

        public void AppendValue(int keyPointer, string originalKey, T value)
        {
            _values.Add(new ValueHolder<T>
            {
                OriginalKey = originalKey,
                Value = value,
                NextValueHolder = null
            });

            _values[_keys[keyPointer].LastValueHolder].NextValueHolder = _values.Count - 1;
            _keys[keyPointer].LastValueHolder = _values.Count - 1;
        }

        public string GetSortingKey(int pointer)
        {
            return _keys[pointer].SortingKey;
        }

        public List<KeyValuePair<string, T>> GetValues(int keyPointer)
        {
            var output = new List<KeyValuePair<string, T>>();

            int? valuePointer = _keys[keyPointer].FirstValueHolder;
            while (valuePointer != null)
            {
                output.Add(new KeyValuePair<string, T>(_values[(int)valuePointer].OriginalKey, _values[(int)valuePointer].Value));
                valuePointer = _values[(int)valuePointer].NextValueHolder;
            }

            return output;
        }
    }
}