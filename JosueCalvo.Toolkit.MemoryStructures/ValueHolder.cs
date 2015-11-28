namespace JosueCalvo.Toolkit.MemoryStructures
{
    public class ValueHolder<T>
    {
        public string OriginalKey { get; set; }
        public T Value { get; set; }
        public int? NextValueHolder { get; set; }
    }
}
