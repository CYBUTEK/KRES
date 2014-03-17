using System;
using System.Linq;
using System.IO;
using UnityEngine;
using KRES.Extensions;
using KRES.Defaults;

namespace KRES
{
    public class ResourceItem
    {
        public class ResourceMap
        {
            #region Propreties
            private string texturePath = string.Empty;
            /// <summary>
            /// Gets the path to the resource texture
            /// </summary>
            public string TexturePath
            {
                get { return this.texturePath; }
            }

            private string scannedTexturePath = string.Empty;
            /// <summary>
            /// Path to the Scanned texture of this instance
            /// </summary>
            public string ScannedTexturePath
            {
                get { return this.scannedTexturePath; }
            }

            private Color colour = KRESUtils.BlankColour;
            /// <summary>
            /// Gets and sets the colour used when generating the texture.
            /// </summary>
            public Color Colour
            {
                get { return this.colour; }
            }

            private double minAltitude = double.NegativeInfinity;
            /// <summary>
            /// Minimum altitude at which this resource can be found
            /// </summary>
            public double MinAltitude
            {
                get { return this.minAltitude; }
            }

            private double maxAltitude = double.PositiveInfinity;
            /// <summary>
            /// Maximum altitude at which this resource can be found
            /// </summary>
            public double MaxAltitude
            {
                get { return this.maxAltitude; }
            }

            private string[] biomes = new string[] { };
            /// <summary>
            /// Contains the only biomes where this resource can be found in
            /// </summary>
            public string[] Biomes
            {
                get { return this.biomes; }
            }

            private string[] excludedBiomes = new string[] { };
            /// <summary>
            /// Contains the only biomes this resource cannot be found in
            /// </summary>
            public string[] ExcludedBiomes
            {
                get { return this.excludedBiomes; }
            }
            #endregion

            #region Initialisation
            public ResourceMap(DefaultResource resource, string body)
            {
                string path = Path.Combine(KRESUtils.GetSavePath(), "KRESTextures/" + body + "/" + resource.Name);
                this.texturePath = path + ".png";
                this.scannedTexturePath = path + "_scanned.png";
                this.colour = ResourceInfoLibrary.Instance.GetResource(resource.Name).Colour;
                this.minAltitude = resource.MinAltitude;
                this.maxAltitude = resource.MaxAltitude;
                this.biomes = resource.Biomes;
                this.excludedBiomes = resource.ExcludedBiomes;
                if (!File.Exists(ScannedTexturePath))
                {
                    Texture2D texture = new Texture2D(1440, 720, TextureFormat.ARGB32, false);
                    File.WriteAllBytes(scannedTexturePath, texture.EncodeToPNG());
                    Texture2D.Destroy(texture);
                }
            }
            #endregion

            #region Public Methods
            public Texture2D GetTexture()
            {
                Texture2D texture = new Texture2D(1440, 720, TextureFormat.ARGB32, false);
                texture.LoadImage(File.ReadAllBytes(texturePath));
                return texture;
            }

            /// <summary>
            /// Shows the texture in scaled space.
            /// </summary>
            public bool ShowTexture(string bodyName)
            {
                foreach (Transform transform in ScaledSpace.Instance.scaledSpaceTransforms)
                {
                    if (transform.name == bodyName)
                    {
                        bool containsMaterial = false;

                        foreach (Material material in transform.renderer.materials)
                        {
                            if (material.name.Contains("KRESResourceMap"))
                            {
                                containsMaterial = true;
                                material.mainTexture = GetTexture();
                                break;
                            }
                        }

                        if (!containsMaterial)
                        {
                            Material material = new Material(Shader.Find("Unlit/Transparent"));
                            material.name = "KRESResourceMap";
                            material.mainTexture = GetTexture();

                            Material[] materials = transform.renderer.materials;
                            Array.Resize<Material>(ref materials, materials.Length + 1);
                            materials[materials.Length - 1] = material;
                            transform.renderer.materials = materials;
                        }
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Hide the texture in scaled space.
            /// </summary>
            public bool HideTexture(string bodyName)
            {
                foreach (Transform transform in ScaledSpace.Instance.scaledSpaceTransforms)
                {
                    if (transform.name == bodyName)
                    {
                        foreach (Material material in transform.renderer.materials)
                        {
                            if (material.name.Contains("KRESResourceMap"))
                            {
                                material.mainTexture = KRESUtils.BlankTexture;
                                return true;
                            }
                        }
                    }
                }
                return false;
            }

            public void SaveScannedTexture(Texture2D texture)
            {
                if (!string.IsNullOrEmpty(ScannedTexturePath))
                {
                    File.WriteAllBytes(ScannedTexturePath, texture.EncodeToPNG());
                    Texture2D.Destroy(texture);
                }
            }

            public Texture2D GetScannedTexture()
            {
                Texture2D texture = new Texture2D(1440, 720, TextureFormat.ARGB32, false);
                texture.LoadImage(File.ReadAllBytes(ScannedTexturePath));
                return texture;
            }
            #endregion
        }

        #region Constants
        private const double mapResolution = 1036800d;
        #endregion

        #region Propreties
        private ResourceType type = ResourceType.LIQUID;
        /// <summary>
        /// The type of resource this is
        /// </summary>
        public ResourceType Type
        {
            get { return this.type; }
        }

        private PartResourceDefinition resource = new PartResourceDefinition();
        /// <summary>
        /// Resource associated with this item
        /// </summary>
        public PartResourceDefinition Resource
        {
            get { return this.resource; }
        }

        /// <summary>
        /// Name of the resource
        /// </summary>
        public string Name
        {
            get { return this.Resource.name; }
        }

        private double actualError = 0d;
        /// <summary>
        /// How far away from the real value will this appear
        /// </summary>
        public double ActualError
        {
            get { return this.actualError; }
        }

        private double actualDensity = 0d;
        /// <summary>
        /// Actual density of this resource on the planet
        /// </summary>
        public double ActualDensity
        {
            get { return this.actualDensity; }
        }

        private ResourceMap map = null;
        /// <summary>
        /// ResourceMap associated with this ResourceItem
        /// </summary>
        public ResourceMap Map
        {
            get { return this.map; }
        }

        /// <summary>
        /// Whether this ResourceItem has a ResourceMap or not
        /// </summary>
        public bool HasMap
        {
            get { return this.map != null; }
        }
        #endregion

        #region Initialisation
        public ResourceItem(ConfigNode data, string resource, string body, string type, System.Random random)
        {
            DefaultResource defaultResource = DefaultLibrary.GetDefault(MapGenerator.DefaultName).GetBody(body).GetResourceOfType(resource, type);
            this.resource = PartResourceLibrary.Instance.GetDefinition(data.GetValue("name"));
            this.type = KRESUtils.GetResourceType(type);
            this.map = null;
            if (!data.TryGetValue("actualDensity", ref actualDensity))
            {
                actualDensity = KRESUtils.Clamp01(defaultResource.Density * (0.97d + (random.NextDouble() * 0.06d)));
            }

            if (!data.TryGetValue("actualError", ref actualError))
            {
                actualError = (random.NextDouble() * 2d) - 1d;
                data.AddValue("actualError", actualError);
            }
        }

        public ResourceItem(ConfigNode data, string resource, string body, System.Random random)
        {
            DefaultResource defaultResource = DefaultLibrary.GetDefault(MapGenerator.DefaultName).GetBody(body).GetResourceOfType(resource, "ore");
            this.resource = PartResourceLibrary.Instance.GetDefinition(resource);
            this.type = ResourceType.ORE;
            double density = defaultResource.Density;
            this.map = new ResourceMap(defaultResource, body);
            if (!data.TryGetValue("actualDensity", ref actualDensity))
            {
                Texture2D texture = Map.GetTexture();
                actualDensity = texture.GetPixels().Count(p => p.a > 0) / mapResolution;
                Texture2D.Destroy(texture);
                data.AddValue("actualDensity", actualDensity);
            }

            if (!data.TryGetValue("actualError", ref actualError))
            {
                actualError = (random.NextDouble() * 2d) - 1d;
                data.AddValue("actualError", actualError);
            }
        }
        #endregion
    }
}
