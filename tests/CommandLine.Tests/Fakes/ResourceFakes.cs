namespace CommandLine.Tests.Fakes
{
    public static class StaticResource
    {
        public static string HelpText { get { return "Localized HelpText"; } }
        public static TypeWithImplicitCast ImplicitCastHelpText => new TypeWithImplicitCast("Localized HelpText");
        public static TypeWithExplicitCast ExplicitCastHelpText => new TypeWithExplicitCast("Localized HelpText");
        public static TypeWithWrongImplicitCast WrongImplicitCastHelpText => new TypeWithWrongImplicitCast();
        public static TypeWithWrongExplicitCast WrongExplicitCastHelpText => new TypeWithWrongExplicitCast();
    }

    public class NonStaticResource
    {
        public static string HelpText { get { return "Localized HelpText"; } }
        public static string WriteOnlyText { set { value?.ToString(); } }
        private static string PrivateHelpText { get { return "Localized HelpText"; } }
        public static TypeWithImplicitCast ImplicitCastHelpText => new TypeWithImplicitCast("Localized HelpText");
        public static TypeWithExplicitCast ExplicitCastHelpText => new TypeWithExplicitCast("Localized HelpText");
        public static TypeWithWrongImplicitCast WrongImplicitCastHelpText => new TypeWithWrongImplicitCast();
        public static TypeWithWrongExplicitCast WrongExplicitCastHelpText => new TypeWithWrongExplicitCast();
    }

    public class NonStaticResource_WithNonStaticProperty
    {
        public string HelpText { get { return "Localized HelpText"; } }
    }

    internal class InternalResource
    {
        public static string HelpText { get { return "Localized HelpText"; } }
    }

    public class TypeWithImplicitCast
    {
        private string value;

        public TypeWithImplicitCast(string value)
        {
            this.value = value;
        }

        public static implicit operator string(TypeWithImplicitCast obj)
        {
            return obj.value;
        }

        public static implicit operator int(TypeWithImplicitCast obj)
        {
            return 0;
        }
    }

    public class TypeWithWrongImplicitCast
    {
        public static implicit operator int(TypeWithWrongImplicitCast obj)
        {
            return 0;
        }
    }

    public class TypeWithExplicitCast
    {
        private string value;

        public TypeWithExplicitCast(string value)
        {
            this.value = value;
        }

        public static explicit operator string(TypeWithExplicitCast obj)
        {
            return obj.value;
        }

        public static explicit operator int(TypeWithExplicitCast obj)
        {
            return 0;
        }
    }

    public class TypeWithWrongExplicitCast
    {
        public static explicit operator int(TypeWithWrongExplicitCast obj)
        {
            return 0;
        }
    }
}
