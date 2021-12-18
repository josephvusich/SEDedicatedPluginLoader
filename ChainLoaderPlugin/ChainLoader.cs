using Sandbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using VRage.Plugins;

namespace DedicatedPluginLoader {

  public class ChainLoader : IPlugin {

    private readonly string preloadDir, pluginDir;
    private readonly Dictionary<string, Assembly> dependencies = new Dictionary<string, Assembly>();
    private readonly List<IPlugin> plugins = new List<IPlugin>();

    public ChainLoader() {
      Log("Starting up...");

      var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      Log($"Using directory {baseDir}");

      preloadDir = MkDir(baseDir, "Dependencies");
      pluginDir = MkDir(baseDir, "Plugins");

      Log("Loading dependencies...");
      PreloadDependencies();

      Log("Loading plugins...");
      LoadPlugins();

      Log($"Loaded {plugins.Count} plugins and {dependencies.Count} dependency libraries.");
    }

    private static void Log(string msg) {
      MySandboxGame.Log.WriteLineAndConsole($"[DedicatedPluginLoader] {msg}");
    }

    public void Init(object gameInstance) {
      foreach (var p in plugins) {
        p.Init(gameInstance);
      }
    }

    public void Update() {
      foreach (var p in plugins) {
        p.Update();
      }
    }

    public void Dispose() {
      foreach (var p in plugins) {
        p.Dispose();
      }
    }

    private void PreloadDependencies() {
      AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
      foreach (var a in LoadAssemblies(preloadDir)) {
        Log($"Preloaded assembly {a.Key}");
        dependencies.Add(a.Key, a.Value);
      }
    }

    private static IEnumerable<KeyValuePair<string, Assembly>> LoadAssemblies(string directory) {
      var files = Directory.GetFiles(directory, "*.dll");
      Array.Sort(files);
      foreach (var dll in files) {
        Log($"Loading {dll}...");
        var key = Path.GetFileNameWithoutExtension(dll);
        var assembly = Assembly.LoadFile(dll);

        yield return new KeyValuePair<string, Assembly>(key, assembly);
      }
    }

    private void LoadPlugins() {
      foreach (var a in LoadAssemblies(pluginDir)) {
        var pluginClasses = a.Value.GetTypes()
          .Where(t => typeof(IPlugin).IsAssignableFrom(t))
          .ToList();

        if (pluginClasses.Count == 0) {
          throw new EntryPointNotFoundException($"{a.Value.FullName} does not contain any IPlugin implementations");
        }

        foreach (var t in pluginClasses) {
          IPlugin plugin;

          try {
            plugin = (IPlugin)Activator.CreateInstance(t);
          } catch (Exception e) {
            plugin = null;

            // SE ignores errors thrown during plugin instantiation
            Log($"Exception: {e.Message}");
            while (e.InnerException != null) {
              Log($"Exception: {e.InnerException.Message}");
              e = e.InnerException;
            }
          }

          if (plugin != null) {
            plugins.Add(plugin);
          }
        }
      }
    }

    private Assembly ResolveAssembly(object sender, ResolveEventArgs args) {
      foreach (var kv in dependencies) {
        if (args.Name.Contains(kv.Key)) {
          Log($"Resolved assembly {args.Name}");
          return kv.Value;
        }
      }
      return null;
    }

    private static string MkDir(string baseDir, string subDir) {
      var dir = Path.Combine(baseDir, subDir);
      if (!Directory.Exists(dir)) {
        Directory.CreateDirectory(dir);
      }
      return dir;
    }

  }
}
