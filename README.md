# Crossoverse.Toolkit.Unity

## Installation
You can install Crossoverse.Toolkit.Unity using the UPM Package.

```
"dev.crossoverse.toolkit.cryptography": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.Cryptography",
"dev.crossoverse.toolkit.dynamicresourcemanager": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.DynamicResourceManager",
"dev.crossoverse.toolkit.imageprocessing": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.ImageProcessing",
"dev.crossoverse.toolkit.resourceprovider": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.ResourceProvider",
"dev.crossoverse.toolkit.scenetransition": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.SceneTransition",
"dev.crossoverse.toolkit.serialization": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.Serialization",
"dev.crossoverse.toolkit.transports": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.Transports/Core",
```

You can install these UPM package via Package Manager in UnityEditor.

- Window -> Package Manager -> + -> Add package from git URL...

| UPM package | git URL |
| ---- | ---- |
| dev.crossoverse.toolkit.cryptography | https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.Cryptography |
| dev.crossoverse.toolkit.dynamicresourcemanager | https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.DynamicResourceManager |
| dev.crossoverse.toolkit.imageprocessing | https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.ImageProcessing |
| dev.crossoverse.toolkit.resourceprovider | https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.ResourceProvider |
| dev.crossoverse.toolkit.scenetransition | https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.SceneTransition |
| dev.crossoverse.toolkit.serialization | https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.Serialization |
| dev.crossoverse.toolkit.transports | https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.Transports/Core |

You can also install via editing Packages/manifest.json directly.

```
// Packages/manifest.json
{
  "dependencies": {
    // ...
    "dev.crossoverse.toolkit.cryptography": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.Cryptography",
    "dev.crossoverse.toolkit.dynamicresourcemanager": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.DynamicResourceManager",
    "dev.crossoverse.toolkit.imageprocessing": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.ImageProcessing",
    "dev.crossoverse.toolkit.resourceprovider": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.ResourceProvider",
    "dev.crossoverse.toolkit.scenetransition": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.SceneTransition",
    "dev.crossoverse.toolkit.serialization": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.Serialization",
    "dev.crossoverse.toolkit.transports": "https://github.com/CrossoverseLabs/Crossoverse.Toolkit.Unity.git?path=src/Crossoverse.Toolkit.Transports/Core",
    // ...
  }
}
```
