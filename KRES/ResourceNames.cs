using System;
using System.Collections.Generic;
using KRES.Extensions;

namespace KRES
{
    public class ResourceNames
    {
        #region Instance
        private static ResourceNames instance;
        public static ResourceNames Instance 
        {
            get
            {
                if (instance == null)
                {
                    instance = new ResourceNames();
                }
                return instance;
            }
        }
        #endregion

        #region Propreties
        private Dictionary<string, string> resources = new Dictionary<string, string>();
        /// <summary>
        /// Contains the resource names as keys and real names as values
        /// </summary>
        public Dictionary<string, string> Resources
        {
            get { return this.resources; }
            set { this.resources = value; }
        }
        #endregion

        #region Initialisation
        private ResourceNames()
        {
            CreateLibrary();
        }
        #endregion

        #region Public methods
        public void CreateLibrary()
        {
            resources.Clear();
            foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("RESOURCE_DEFINITION"))
            {
                if (node.HasValues("name", "realName"))
                {
                    resources.Add(node.GetValue("name"), node.GetValue("realName"));
                }
            }
        }

        /// <summary>
        /// Returns the real name of this resource. Returns the entry if null.
        /// </summary>
        /// <param name="resource">Name of the resource</param>
        /// <returns></returns>
        public string GetRealName(string resource)
        {
            string value = string.Empty;
            if (resources.TryGetValue(resource, out value)) { return value; }
            return resource;
        }

        /// <summary>
        /// Get "Kerbal" name of this resource. Returns the entry if null.
        /// </summary>
        /// <param name="name">Real name of the resource</param>
        /// <returns></returns>
        public string GetResourceName(string name)
        {
            string value = string.Empty;
            foreach (string key in resources.Keys)
            {
                if (resources.TryGetValue(key, out value))
                {
                    if (value == name) { return key; }
                }
            }
            return name;
        }
        #endregion
    }
}
