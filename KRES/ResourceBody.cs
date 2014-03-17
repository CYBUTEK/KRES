using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KRES.Extensions;

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

        private List<ResourceItem> resourceItems = new List<ResourceItem>();
        /// <summary>
        /// Gets the resource items associated with this body
        /// </summary>
        public List<ResourceItem> ResourceItems
        {
            get { return this.resourceItems; }
        }
        #endregion

        #region Initialisation
        public ResourceBody() { }

        public ResourceBody(string body)
        {
            this.name = body;
        }
        #endregion

        #region Methods
        internal IEnumerator<YieldInstruction> LoadItems(ConfigNode settings, System.Random random)
        {
            foreach (string type in KRESUtils.types.Values)
            {
                if (KRESUtils.GetRelevantBodies(type).Any(b => b.bodyName == this.Name))
                {
                    foreach (ConfigNode data in settings.GetNode(type).GetNode(this.Name).GetNodes("KRES_DATA"))
                    {
                        string resourceName = string.Empty;
                        data.TryGetValue("name", ref resourceName);
                        if (!PartResourceLibrary.Instance.resourceDefinitions.Contains(resourceName)) { continue; }
                        if (type == "ore")
                        {
                            string path = Path.Combine(KRESUtils.GetSavePath(), "KRESTextures/" + name + "/" + resourceName + ".png");
                            if (File.Exists(path))
                            {
                                ResourceItem item = new ResourceItem(data, resourceName, this.Name, random);
                                resourceItems.Add(item);
                            }
                        }
                        else if (type == "gas" || type == "liquid")
                        {
                            ResourceItem item = new ResourceItem(data, resourceName, this.Name, type, random);
                            resourceItems.Add(item);
                        }
                        yield return null;
                    }
                }
            }
        }

        internal ResourceItem GetItem(string name, string type)
        {
            return ResourceItems.Find(i => i.Name == name && i.Type == KRESUtils.GetResourceType(type));
        }

        internal List<ResourceItem> GetItemsOfType(string type)
        {
            return ResourceItems.Where(i => i.Type == KRESUtils.GetResourceType(type)).ToList();
        }

        internal List<ResourceItem> GetItemsOfType(ResourceType type)
        {
            return ResourceItems.Where(i => i.Type == type).ToList();
        }

        internal List<ResourceItem> GetItemsOfName(string name)
        {
            return ResourceItems.Where(i => i.Name == name).ToList();
        }
        #endregion
    }
}
