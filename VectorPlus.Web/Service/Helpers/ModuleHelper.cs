using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Extensions.Logging;
using VectorPlus.Lib;
using VectorPlus.Web.Service.Db;

namespace VectorPlus.Web.Service.Helpers
{
    [Obsolete]
    public class ModuleHelper
    {
        public static IEnumerable<IVectorPlusBehaviourModule> ExtractModules(ILogger logger = null)
        {
            logger?.LogDebug("Retrieving behavioural modules.");
            var badModuleConfigs = new List<ModuleConfig>();
            var extractedModules = new List<IVectorPlusBehaviourModule>();

            using (var db = new VectorPlusBackgroundServiceDbContext())
            {
                // fetch all module configs in the database
                var dbModules = db.Modules.OrderBy(m => m.Added).ToList();

                // for each module
                foreach (var dbModule in dbModules)
                {
                    // if the user has enabled it
                    if (dbModule.UserEnabled)
                    {
                        // if the module config has binary
                        if (dbModule.Zip != null)
                        {
                            try
                            {
                                // extract the module
                                var modules = ExtractModules(dbModule.Zip, logger);
                                if (modules != null && modules.Count() > 0)
                                {
                                    extractedModules.AddRange(modules);
                                    logger?.LogInformation("Added modules: " + string.Join(", ", modules.Select(m => m.Name)));
                                }
                                else
                                {
                                    logger?.LogWarning("No modules extracted for: " + dbModule.Name);
                                    badModuleConfigs.Add(dbModule);
                                }
                            }
                            catch (Exception e)
                            {
                                logger?.LogWarning(dbModule.Name + ": " + e.Message);
                                badModuleConfigs.Add(dbModule);
                            }
                        }
                        else
                        {
                            logger?.LogWarning("DLL not stored for module " + dbModule.Name + ".");
                            badModuleConfigs.Add(dbModule);
                        }
                    }
                    else
                    {
                        logger?.LogDebug("Ignored module: " + dbModule.Name);
                    }
                }

                // remove all modules with issues
                db.Modules.RemoveRange(badModuleConfigs);
            }

            logger?.LogDebug("Found " + extractedModules.Count + " modules.");
            logger?.LogDebug("Removed " + badModuleConfigs.Count + " bad modules.");
            return extractedModules;
        }

        public static IEnumerable<IVectorPlusBehaviourModule> ExtractModules(byte[] zipData, ILogger logger = null)
        {
            var modules = new List<IVectorPlusBehaviourModule>();
            var tempZipFile = Path.GetTempFileName();
            var tempDirectory = CreateTemporaryDirectory();
            try
            {
                // extract the ZIP to a temporary folder so all DLLs available to each other
                File.WriteAllBytes(tempZipFile, zipData);
                ZipFile.ExtractToDirectory(tempZipFile, tempDirectory);

                // step through each file in the directory
                foreach (var path in Directory.EnumerateFiles(tempDirectory))
                {
                    // only DLLs
                    if (Path.GetExtension(path.ToLower()) == ".dll")
                    {
                        // Load the DLL, search for modules
                        var loadContext = new ModuleLoadContext(path);
                        var assembly = loadContext.LoadFromAssemblyPath(path);
                        foreach (Type type in assembly.GetExportedTypes())
                        {
                            var ok1 = type.GetInterfaces().Any(i => typeof(IVectorPlusBehaviourModule).IsAssignableFrom(i));
                            var ok2 = type.GetInterfaces().Any(i => i.FullName == typeof(IVectorPlusBehaviourModule).FullName);
                            if (ok2)
                            {
                                IVectorPlusBehaviourModule module = (IVectorPlusBehaviourModule)Activator.CreateInstance(type);
                                modules.Add(module);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger?.LogWarning(e, "Could not load DLL(s) from zip bytes.");
                throw e;
            }
            finally
            {
                if (File.Exists(tempZipFile)) { File.Delete(tempZipFile); }
                if (Directory.Exists(tempDirectory)) { Directory.Delete(tempDirectory, true); }
            }
            return modules;
        }

        public static string CreateTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
