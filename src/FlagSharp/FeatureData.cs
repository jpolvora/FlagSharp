namespace FlagSharp
{
    public static class FeatureData
    {
        public static List<FeatureDefinition> GetFeatures(string storeName)
        {
            return
            [
                new FeatureDefinition("FeatureA", string.Empty, true, storeName, EFeatureRequirementType.All) { RequiredFilterNames = new List<string> { "FilterA" }},
                new FeatureDefinition("FeatureB", string.Empty, false, storeName, EFeatureRequirementType.Any) { RequiredFilterNames = new List<string> { "FilterB" }}
            ];
        }
    }
}
