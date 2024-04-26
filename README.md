
# Morris.Zod
![](./Images/Zod-logo.png)

***Morris.Zod*** 

[![NuGet version (PackageName)](https://img.shields.io/nuget/v/Morris.Zod.svg?style=flat-square)](https://www.nuget.org/packages/Morris.Zod/)

## Overview
Converts .net assembly classes to TypeScript "Zod" validation files.

**NOTE:** Alpha version. Currently has a bug where the consuming csproj won't build inside VS so you'll have to use `dotnet build`.

## Getting started

1. Add a NuGet package reference to Morris.Zod to the project you want to generate Zod TypeScript files for.
2. Edit the csproj file and add the following
```xml
    <PropertyGroup>
        <Morris_Zod_OutputDir>..\ZodGeneratedFiles</Morris_Zod_OutputDir>
    </PropertyGroup>
```
Where the value of `Morris_Zod_OutputDir` is the path to where you want the generated TS files to be written, this path is relative to your csproj file.
3. Build

Your generated files will be in the folder path you specified within your `csproj` file each time that project builds.

## Installation
You can download the latest release / pre-release NuGet packages from the official
***Morris.Zod*** [Nuget page](https://www.nuget.org/packages/Morris.Zod/)

## Release notes
See the [Releases page](./Docs/releases.md) for release history.

# Licence
[MIT](https://opensource.org/licenses/MIT)
