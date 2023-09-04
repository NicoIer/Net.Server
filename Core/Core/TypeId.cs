namespace Moba
{
    /// <summary>
    /// 比dict更快
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class TypeId<T>
    {
        public static ushort id = typeof(T).FullName.GetStableHash();
    }
}