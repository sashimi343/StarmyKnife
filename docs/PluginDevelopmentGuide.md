# Plugin development guide

## Overview

1. Create a new .NET 8.0 class library project
2. Add `StarmyKnife.Core.dll` to your project dependencies
3. Create a class that implements the following:
  * Inherit `StarmyKnife.Core.Plugins.PluginBase` class
  * Implement the interface (*) according to the function to be developed
  * Add `StarmyKnifePlugins.Plugins.StarmyKnifePlugins` attribute to your class
4. After implementation, store the built DLL in the `(StarmyKnifePath)\Plugins`

## Plugin interfaces

|Function | Interface                                                |
|---------|----------------------------------------------------------|
|ChainConverter|`StarmyKnife.Core.Plugins.IChainConverter`|
|Generator|`StarmyKnife.Core.Plugins.IGenerator`|
|PrettyValidator|`StarmyKnife.Core.Plugins.IPrettyValidator`|

## Plugin parameters

TODO: Write document