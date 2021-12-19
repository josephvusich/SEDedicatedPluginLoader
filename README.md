# SEDedicatedPluginLoader
A lightweight Space Engineers plugin loader aimed at dedicated servers.

# Building
If you do not have Visual Studio, you can download a pre-built DLL from the releases page and skip to [Installation and usage](#installation-and-usage).
1. Clone this repository.
2. Open `SEDedicatedPluginLoader.sln` in Visual Studio.
3. If your Steam or Space Engineers install paths are not in the default `C:\Program Files (x86)\Steam`, update the missing library references to the correct game install path.
4. Ensure the solution Platform is set to `x64`, and not `AnyCPU`, to avoid mismatch warnings.
5. Click Build > Build Solution.
6. Create a new installation directory.
7. In the installation directory, add `DedicatedPluginLoader.dll` from the build output, and the `Dependencies` directory from the root of this repository.

# Installation and usage
1. Download the latest version from the releases page and unzip locally. If building yourself, skip to step 3.
2. Right-click the unzipped DLLs, click Properties, select Unblock, and then click OK, or Space Engineers may not be able to load the DLL. Skip this step if Unblock does not appear in the Properties window.
3. Add your plugin's dependencies to the `Dependencies` folder.
4. Add your plugin libraries to the `Plugins` folder.
5. Open Space Engineers' Dedicated Server Manager and navigate to the Plugins page for your server.
6. Under Assemblies, click Add, and select `DedicatedPluginLoader.dll`
7. Run the server.

```
DedicatedPluginLoader.dll
Dependencies/
  0Harmony.dll
  # Add dependency libraries here
Plugins/
  # Add plugins here
```

# Uninstallation
1. Open Space Engineers' Dedicated Server Manager and navigate to the Plugins page for your server.
2. Under Assemblies, select `DedicatedPluginLoader`, and click Remove.
3. Delete the folder containing `DedicatedPluginLoader.dll`
