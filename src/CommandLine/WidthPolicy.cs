namespace CommandLine
{
    /// <summary>
    /// Policy that controls how to configure the width of help text
    /// </summary>
    public enum WidthPolicy
    {
        /// <summary>
        /// Use the supplied width regardless of whether it will fit on screen (recommended if you are writing to custom stream)
        /// </summary>
        ForceUse,
        /// <summary>
        /// Use the screen width if smaller than the supplied width
        /// </summary>
        FitToScreen
    }
}