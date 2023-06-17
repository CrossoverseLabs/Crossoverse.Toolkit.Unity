# Crossoverse.Toolkit.Unity

## Installation
You can install Crossoverse.Toolkit.Unity using the UPM Package.

```
"dev.crossoverse.toolkit.resourceprovider": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=Packages/Crossoverse.Toolkit.ResourceProvider"
```

You can install these UPM package via Package Manager in UnityEditor.

- Window -> Package Manager -> + -> Add package from git URL...

| UPM package | git URL |
| ---- | ---- |
| dev.crossoverse.toolkit.resourceprovider | https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=Packages/Crossoverse.Toolkit.ResourceProvider |

You can also install via editing Packages/manifest.json directly.

```
// Packages/manifest.json
{
  "dependencies": {
    // ...
    "dev.crossoverse.toolkit.resourceprovider": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=Packages/Crossoverse.Toolkit.ResourceProvider",
    // ...
  }
}
```
