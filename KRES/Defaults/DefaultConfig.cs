using System.Collections.Generic;

namespace KRES.Defaults
{
    public class DefaultConfig
    {
        #region Properties
        private string name = string.Empty;
        public string Name
        {
            get { return this.name; }
        }

        private string description = string.Empty;
        public string Description
        {
            get { return this.description; }
        }

        private List<DefaultResource> resources = new List<DefaultResource>();
        public List<DefaultResource> Resources
        {
            get { return this.resources; }
        }
        #endregion

        #region Initialisation
        public DefaultConfig() { }

        public DefaultConfig(ConfigNode configNode)
        {
            this.name = configNode.GetValue("name");
            this.description = configNode.GetValue("description");
            
            foreach (ConfigNode resourceNode in configNode.GetNodes("KRES_RESOURCE"))
            {
                DefaultResource resource = new DefaultResource();
                resource.Name = resourceNode.GetValue("name");
                resource.Type = resourceNode.GetValue("type");
                if (resourceNode.HasValue("biome"))
                {
                    resource.Biome = resourceNode.GetValue("biome");
                }
                resource.Density = float.Parse(resourceNode.GetValue("density"));
                this.resources.Add(resource);
            }
        }
        #endregion
    }
}
