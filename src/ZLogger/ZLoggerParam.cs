namespace ZLogger
{
    // The helper class used directly with logger 
    public static partial class ZLogger
    {
        public static ZLoggerNamedParam<T> Param<T>(string name, T value) => new(name, value);
    }

    public readonly struct ZLoggerNamedParam<T>
    {
        public readonly string Name;
        public readonly T Value;

        public ZLoggerNamedParam(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}
