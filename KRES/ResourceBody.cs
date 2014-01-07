using System.Collections.Generic;
using UnityEngine;

namespace KRES
{
    public class ResourceBody
    {
        #region Properties
        private string name = string.Empty;
        /// <summary>
        /// Gets the name of the celestial body.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        private List<ResourceMap> resourceMaps = new List<ResourceMap>();
        /// <summary>
        /// Gets and sets the resources associated with the celestial body.
        /// </summary>
        public List<ResourceMap> ResourceMaps
        {
            get { return this.resourceMaps; }
            set { this.resourceMaps = value; }
        }
        #endregion

        #region Initialisation
        public ResourceBody(string name)
        {
            this.name = name;
        }
        public ResourceBody(string name, ResourceMap[] resourceMaps)
        {
            this.name = name;
            this.resourceMaps.AddRange(resourceMaps);
        }
        #endregion
    }
}
