#if !NET8_0_OR_GREATER

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Indicates that the specified method parameter expects a constant.
    /// </summary>
    /// <remarks>
    /// This can be used to inform tooling that a constant should be used as an argument for the annotated parameter.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class ConstantExpectedAttribute : Attribute
    {
        /// <summary>
        /// Indicates the minimum bound of the expected constant, inclusive.
        /// </summary>
        public object? Min { get; set; }
        /// <summary>
        /// Indicates the maximum bound of the expected constant, inclusive.
        /// </summary>
        public object? Max { get; set; }
    }
}

#endif