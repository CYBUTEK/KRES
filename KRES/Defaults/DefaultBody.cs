using System.Collections.Generic;
using UnityEngine;

namespace KRES.Defaults
{
    public class DefaultBody
    {
        #region Properties
        private string name = string.Empty;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private List<DefaultResource> resources = new List<DefaultResource>();
        public List<DefaultResource> Resources
        {
            get { return this.resources; }
            set { this.resources = value; }
        }
        #endregion

        #region Initialisation
        public DefaultBody(ConfigNode configNode)
        {
            this.name = configNode.GetValue("name");

            foreach (ConfigNode resourceNode in configNode.GetNodes("KRES_RESOURCE"))
            {
                DefaultResource resource = new DefaultResource();
                resource.Name = resourceNode.GetValue("name");
                resource.Type = resourceNode.GetValue("type");
                resource.Density = double.Parse(resourceNode.GetValue("density"));
                resource.Octaves = double.Parse(resourceNode.GetValue("octaves"));
                resource.Persistence = double.Parse(resourceNode.GetValue("persistence"));
                resource.Frequency = double.Parse(resourceNode.GetValue("frequency"));
                if (resourceNode.HasValue("biome"))
                {
                    resource.Biome = resourceNode.GetValue("biome");
                }
                this.resources.Add(resource);
            }
        }
        #endregion
        
        #region Public Methods
        public DefaultResource GetResource(string name)
        {
            foreach (DefaultResource resource in this.resources)
            {
                if (resource.Name == name)
                {
                    return resource;
                }
            }
            return null;
        }

        public bool HasResource(string name)
        {
            foreach (DefaultResource resource in this.resources)
            {
                if (resource.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion 
    }
}
