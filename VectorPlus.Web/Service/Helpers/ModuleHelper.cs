using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using VectorPlus.Lib;
using VectorPlus.Web.Service.Db;

namespace VectorPlus.Web.Service.Helpers
{
    public class ModuleHelper
    {

        public static IEnumerable<IVectorPlusBehaviourModule> ExtractModules(ILogger logger = null)
        {
            logger?.LogDebug("Retrieving behavioural modules.");
            var extractedModules = new List<IVectorPlusBehaviourModule>();
            using (var db = new VectorPlusBackgroundServiceDbContext())
            {
                var dbModules = db.Modules.OrderBy(m => m.Added).ToList();

                foreach (var dbModule in dbModules)
                {
                    if (dbModule.UserEnabled)
                    {
                        if (dbModule.DLL != null)
                        {
                            var modules = ExtractModules(dbModule.DLL, logger);
                            if (modules != null)
                            {
                                extractedModules.AddRange(modules);
                                foreach (var mod in modules)
                                {
                                    logger?.LogDebug("Added module: " + mod.Name);
                                }
                            }
                        }
                        else
                        {
                            logger?.LogWarning("DLL not stored for module " + dbModule.Name + ".");
                        }
                    }
                }
            }
            logger?.LogDebug("Found " + extractedModules.Count + " modules.");
            return extractedModules;
        }

        public static IEnumerable<IVectorPlusBehaviourModule> ExtractModules(byte[] dll, ILogger logger = null)
        {
            try
            {
                string path = Path.GetTempFileName();
                File.WriteAllBytes(path, dll);
                var loadContext = new ModuleLoadContext(path);
                var assembly = loadContext.LoadFromAssemblyPath(path);

                var modules = new List<IVectorPlusBehaviourModule>();
                foreach (Type type in assembly.GetExportedTypes())
                {
                    if (typeof(IVectorPlusBehaviourModule).IsAssignableFrom(type))
                    {
                        IVectorPlusBehaviourModule module = (IVectorPlusBehaviourModule)Activator.CreateInstance(type);
                        modules.Add(module);
                    }
                }
                return modules;
            }
            catch (Exception e)
            {
                logger?.LogWarning("Could not load DLL.", e);
                return null;
            }
        }
    }
}
