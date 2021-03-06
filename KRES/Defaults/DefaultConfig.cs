﻿using System;
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
            Random random = new Random();
            foreach (ConfigNode bodyNode in configNode.GetNodes("KRES_BODY"))
            {
                if (KRESUtils.IsCelestialBody(bodyNode.GetValue("name")))
                {
                    this.bodies.Add(new DefaultBody(bodyNode, random));
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
            ConfigNode configNode = null;
            if (addRootNode)
            {
                configNode = new ConfigNode("KRES");
            }
            else
            {
                configNode = new ConfigNode();
            }

            configNode.AddValue("name", this.name);
            configNode.AddValue("description", this.description);
            configNode.AddValue("generated", false);
            configNode.AddValue("WARNING", "SPOILERS BELOW, PROCEED AT YOUR OWN RISK");
            foreach (string type in KRESUtils.types.Values)
            {
                UnityEngine.Debug.Log("[KRES]: Creating " + type + " node");
                ConfigNode t = configNode.AddNode(type);
                foreach (CelestialBody body in KRESUtils.GetRelevantBodies(type))
                {
                    if (HasBody(body.bodyName))
                    {
                        t.AddNode(this.bodies.Find(b => b.Name == body.name).CreateConfigNode(type));
                    }
                    else
                    {
                        t.AddNode(body.bodyName);
                        UnityEngine.Debug.LogWarning("[KRES]: The " + this.name + " defaults file does not contain a definition of " + type + " for " + body.bodyName);
                    }
                }
            }

            return configNode;
        }
        #endregion
    }
}
