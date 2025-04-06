namespace FlagSharp
{
    // Ensure the FeatureDefinition class is included in the namespace
    public record FeatureDefinition(string Name, string Description, bool IsEnabled, string StoreName, EFeatureRequirementType RequirementType = EFeatureRequirementType.Any)
    {
        public IEnumerable<string> RequiredFilterNames { get; set; } = [];

    }
}
