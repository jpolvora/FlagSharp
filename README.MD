# FlagSharp

FlagSharp is a feature management library for .NET applications. It provides a robust and extensible way to manage feature flags, enabling dynamic control over application behavior without requiring code changes or redeployment.

## Features
- **Feature Stores**: Integrates with observable feature stores to dynamically load and update feature definitions.
- **Feature Filters**: Supports middleware-like filters to evaluate feature flags based on custom logic.
- **Dynamic Updates**: Automatically reloads features when the underlying store changes.
- **Extensibility**: Easily extendable with custom feature stores and filters.

## Installation
To use FlagSharp in your project, add the following NuGet package reference:

	<PackageReference Include="FlagSharp" Version="1.0.0" />

	
## Getting Started

### 1. Define a Feature Store
A feature store is responsible for providing feature definitions. Implement the `IObservableFeatureStore` interface to create a custom store.

```csharp	
public class InMemoryFeatureStore : IObservableFeatureStore 
{ 
    private readonly List<FeatureDefinition> _features;
    public string StoreName => "InMemoryStore";

    public InMemoryFeatureStore()
    {
        _features = new List<FeatureDefinition>
        {
            new FeatureDefinition("FeatureA", "Description for FeatureA", true, StoreName),
            new FeatureDefinition("FeatureB", "Description for FeatureB", false, StoreName)
        };
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task<IEnumerable<FeatureDefinition>> GetFeaturesAsync() => Task.FromResult(_features.AsEnumerable());

    public IDisposable Subscribe(IObserverOf<IObservableFeatureStore, IFeatureManagerObserver> observer) => throw new NotImplementedException();
}
```

### 2. Define a Feature Filter
Feature filters evaluate whether a feature should be enabled based on custom logic. Implement the `IFeatureFilter` interface to create a custom filter.

```csharp
public class TimeBasedFeatureFilter : IFeatureFilter 
{ 
    public string Name => "TimeBasedFilter";
    public bool IsGlobal => false;

    public Task<bool> EvaluateAsync(FilterEvaluationContext context, Func<IPipelineContext?, Task<bool>> next, CancellationToken? cancellationToken = default)
    {
        // Example: Enable the feature only during business hours
        var currentHour = DateTime.Now.Hour;
        return Task.FromResult(currentHour >= 9 && currentHour <= 17);
    }
}

```

### 3. Initialize the FeatureManager
Use the `FeatureManager` class to manage and evaluate feature flags.

```csharp

var featureStores = new List<IObservableFeatureStore> { new InMemoryFeatureStore() }; 
var featureFilters = new List<IFeatureFilter> { new TimeBasedFeatureFilter() };
var featureManager = new FeatureManager(featureStores, featureFilters); 

await featureManager.InitializeAsync();

```

### 4. Check if a Feature is Enabled
Use the `IsFeatureEnabledAsync` method to evaluate whether a feature is enabled.

```csharp

bool isFeatureAEnabled = await featureManager.IsFeatureEnabledAsync("FeatureA"); 

Console.WriteLine($"FeatureA is enabled: {isFeatureAEnabled}");

```


### 5. Retrieve All Features
You can retrieve all loaded features using the `GetFeaturesAsync` method.

```csharp
var features = await featureManager.GetFeaturesAsync(); 
foreach (var feature in features) 
{ 
    Console.WriteLine($"Feature: {feature.Name}, Enabled: {feature.IsEnabled}"); 
}

```



### 6. Handle Dynamic Updates
If a feature store supports updates, the `FeatureManager` will automatically reload features when the store changes.

---

## Project Details
- **Target Framework**: .NET 9
- **Repository**: [GitHub - FlagSharp](https://github.com/jpolvora/FlagSharp)
- **Tags**: Feature, Flags, Feature Management

## Contributing
Contributions are welcome! Feel free to open issues or submit pull requests to improve FlagSharp.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
