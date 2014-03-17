using System;
using System.Collections.Generic;
using System.Linq;
using KRES.Extensions;

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
        public DefaultBody(ConfigNode configNode, Random random)
        {
            configNode.TryGetValue("name", ref this.name);
            foreach (ConfigNode resourceNode in configNode.GetNodes("KRES_RESOURCE"))
            {
                this.resources.Add(new DefaultResource(resourceNode, random));
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

        public DefaultResource GetResourceOfType(string name, string type)
        {
            return this.Resources.Find(r => r.Name == name && r.Type == type);
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

        public ConfigNode CreateConfigNode(string type)
        {
            ConfigNode configNode = new ConfigNode(this.Name);
            foreach (DefaultResource resource in this.Resources.Where(r => r.Type == type))
            {
                configNode.AddNode(resource.CreateConfigNode());
            }
            return configNode;
        }
        #endregion 
    }
}
