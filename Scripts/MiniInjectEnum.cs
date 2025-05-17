namespace MiniDi
{
    public enum Scope
    {
        Project,
        Scene,
        GameObject
    }

    public enum Lifetime
    {
        Singleton,
        Transient,
        Scoped,
        Pooled,
        Weak,
        Lazy
    }
}