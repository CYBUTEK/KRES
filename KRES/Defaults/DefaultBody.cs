using System.Collections.Generic;
using UnityEngine;
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
        public DefaultBody(ConfigNode configNode)
        {
            configNode.TryGetValue("name", ref this.name);

            foreach (ConfigNode resourceNode in configNode.GetNodes("KRES_RESOURCE"))
            {
                this.resources.Add(new DefaultResource(resourceNode));
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

        public ConfigNode CreateConfigNode()
        {
            ConfigNode configNode = new ConfigNode("KRES_BODY");
            configNode.AddValue("name", this.name);

            foreach (DefaultResource resource in this.resources)
            {
                configNode.AddNode(resource.CreateConfigNode());
            }

            return configNode;
        }
        #endregion 
    }
}
