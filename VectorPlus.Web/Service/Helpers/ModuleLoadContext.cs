using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace VectorPlus.Web.Service.Helpers
{
    [Obsolete]
    public class ModuleLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver pluginAssemblyResolver;
        private AssemblyDependencyResolver applicationAssemblyResolver;

        public ModuleLoadContext(string pluginPath)
        {
            pluginAssemblyResolver = new AssemblyDependencyResolver(pluginPath);

            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            var applicationPath = Path.GetDirectoryName(path);

            applicationAssemblyResolver = new AssemblyDependencyResolver(applicationPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string applicationAssemblyPath = applicationAssemblyResolver.ResolveAssemblyToPath(assemblyName);
            if (applicationAssemblyPath != null)
            {
                return LoadFromAssemblyPath(applicationAssemblyPath);
            }

            string pluginAssemblyPath = pluginAssemblyResolver.ResolveAssemblyToPath(assemblyName);
            if (pluginAssemblyPath != null)
            {
                return LoadFromAssemblyPath(pluginAssemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string applicationLibraryPath = applicationAssemblyResolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (applicationLibraryPath != null)
            {
                return LoadUnmanagedDllFromPath(applicationLibraryPath);
            }

            string pluginLibraryPath = pluginAssemblyResolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (pluginLibraryPath != null)
            {
                return LoadUnmanagedDllFromPath(pluginLibraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
