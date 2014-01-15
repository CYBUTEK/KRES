using System.Collections.Generic;
using KRES.Extensions;

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

        private List<DefaultBody> bodies = new List<DefaultBody>();
        public List<DefaultBody> Bodies
        {
            get { return this.bodies; }
        }
        #endregion

        #region Initialisation
        public DefaultConfig() { }

        public DefaultConfig(ConfigNode configNode)
        {
            configNode.TryGetValue("name", ref this.name);
            configNode.TryGetValue("description", ref this.description);

            foreach (ConfigNode bodyNode in configNode.GetNodes("KRES_BODY"))
            {
                if (KRESUtils.IsCelestialBody(bodyNode.GetValue("name")))
                {
                    this.bodies.Add(new DefaultBody(bodyNode));
                }
            }
        }
        #endregion

        #region Public Methods
        public DefaultBody GetBody(string name)
        {
            foreach (DefaultBody body in this.bodies)
            {
                if (body.Name == name)
                {
                    return body;
                }
            }
            return null;
        }

        public bool HasBody(string name)
        {
            foreach (DefaultBody body in this.bodies)
            {
                if (body.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public ConfigNode CreateConfigNode(bool addRootNode = false)
        {
            ConfigNode configNode;
            if (addRootNode)
            {
                configNode = new ConfigNode("KRES_DEFAULTS");
            }
            else
            {
                configNode = new ConfigNode();
            }

            configNode.AddValue("name", this.name);
            configNode.AddValue("description", this.description);

            foreach (DefaultBody body in this.bodies)
            {
                configNode.AddNode(body.CreateConfigNode());
            }

            return configNode;
        }
        #endregion
    }
}
