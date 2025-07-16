namespace VendixPos
{
    public static class EntityExtensions
    {

        public static List<string> GetNullProperties<T>(this T entity) where T : class
        {
            var nullProperties = new List<string>();
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.GetValue(entity) == null)
                {
                    nullProperties.Add(property.Name);
                }
            }
            return nullProperties;
        }

    }
}
