namespace OpenDeepSpace.NetCore.Autofacastle
{
    /// <summary>
    /// 筛选器
    /// </summary>
    public class NamedTypeSelector
    {
        /// <summary>
        /// 筛选器名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 断言
        /// </summary>
        public Func<Type, bool> Predicate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="predicate"></param>
        public NamedTypeSelector(string name, Func<Type, bool> predicate)
        {
            Name = name;
            Predicate = predicate;
        }

        public NamedTypeSelector(Func<Type, bool> predicate)
        {
            Predicate = predicate;
        }
    }
}