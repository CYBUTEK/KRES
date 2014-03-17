using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KRES.Extensions;

namespace KRES
{
    public class ResourceInfoLibrary
    {
        public class ResourceInfo
        {
            #region Propreties
            private PartResourceDefinition resource = new PartResourceDefinition();
            public PartResourceDefinition Resource
            {
                get { return this.resource; }
            }

            private string name = string.Empty;
            public string Name
            {
                get { return this.name; }
            }

            private string realName = string.Empty;
            public string RealName
            {
                get { return this.realName; }
            }

            private Color colour = KRESUtils.BlankColour;
            public Color Colour
            {
                get { return this.colour; }
            }
            #endregion

            #region Constructor
            public ResourceInfo(ConfigNode node)
            {
                node.TryGetValue("name", ref this.name);
                resource = PartResourceLibrary.Instance.GetDefinition(this.Name);
                node.TryGetValue("realName", ref this.realName);
                node.TryGetValue("colour", ref this.colour);
            }
            #endregion
        }

        #region Instance
        private static ResourceInfoLibrary instance;
        public static ResourceInfoLibrary Instance 
        {
            get
            {
                if (instance == null)
                {
                    instance = new ResourceInfoLibrary();
                }
                return instance;
            }
        }
        #endregion

        #region Propreties
        private List<ResourceInfo> resources = new List<ResourceInfo>();
        /// <summary>
        /// Contains the resources info
        /// </summary>
        public List<ResourceInfo> Resources
        {
            get { return this.resources; }
            set { this.resources = value; }
        }
        #endregion

        #region Initialisation
        private ResourceInfoLibrary()
        {
            resources.Clear();
            resources.AddRange(GameDatabase.Instance.GetConfigNodes("RESOURCE_DEFINITION").Select(n => new ResourceInfo(n)));
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the ResourceInfo of the given name
        /// </summary>
        /// <param name="name">Name of the resource to find</param>
        public ResourceInfo GetResource(string name)
        {
            return resources.Find(r => r.Name == name);
        }
        #endregion
    }
}
