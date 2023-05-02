namespace LDGame.Attributes
{
    /// <summary>
    /// Attribute for fields with a fmod id.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class FmodIdAttribute : Attribute 
    {
        public readonly FmodIdKind Kind;

        public FmodIdAttribute(FmodIdKind kind) => Kind = kind;
    }

    public enum FmodIdKind
    {
        Event = 1,
        Bus = 2
    }
}
