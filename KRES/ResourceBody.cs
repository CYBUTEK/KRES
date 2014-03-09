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
        /// <summary>
        /// Initiates with a list of ResourceMaps from all the textures in the directory
        /// </summary>
        /// <param name="name">Name of the body</param>
        public ResourceBody(ConfigNode body)
        {
            this.name = body.name;
            foreach(string path in Directory.GetFiles(Path.Combine(KRESUtils.GetSavePath(), "KRESTextures/" + name)))
            {
                if (Path.GetExtension(path) == ".png")
                {
                    string resourceName = Path.GetFileNameWithoutExtension(path);
                    ConfigNode resource = body.GetNodes("KRES_RESOURCE").First(n => n.HasValue("name") && n.GetValue("name") == resourceName && n.HasValue("type") && n.GetValue("type") == "ore");
                    Texture2D texture = new Texture2D(1440, 720, TextureFormat.ARGB32, false);
                    texture.LoadImage(File.ReadAllBytes(path));
                    ResourceMap map = new ResourceMap(resourceName, texture, resource);
                    resourceMaps.Add(map);
                }
            }
        }

        /// <summary>
        /// Initiates using a provided list of ResourceMaps
        /// </summary>
        /// <param name="name">Name of the body</param>
        /// <param name="resourceMaps">List of ResourceMaps</param>
        public ResourceBody(string name, ResourceMap[] resourceMaps)
        {
            this.name = name;
            this.resourceMaps.AddRange(resourceMaps);
        }
        #endregion
    }
}
