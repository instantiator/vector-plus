using System;
using VectorPlus.Lib;

namespace VectorPlus.Web.Service
{
    public class ModuleConfig
    {
        public Guid ModuleConfigId { get; set; }
        public string UniqueReference { get; set; }
        public DateTime Added { get; set; }
        public byte[] DLL { get; set; }

        public int Release { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool UserEnabled { get; set; }

        public string Author { get; set; }
        public string AuthorEmail { get; set; }
        public string ModuleWebsite { get; set; }

        public static ModuleConfig From(IVectorPlusBehaviourModule module, byte[] dll, bool enabled)
        {
            return new ModuleConfig()
            {
                UniqueReference = module.UniqueReference,
                ModuleConfigId = Guid.NewGuid(),
                Added = DateTime.Now,
                DLL = dll,
                Release = module.Release,
                Name = module.Name,
                Description = module.Description,
                Author = module.Author,
                AuthorEmail = module.AuthorEmail,
                ModuleWebsite = module.ModuleWebsite,
                UserEnabled = enabled
            };
        }
    }
}
