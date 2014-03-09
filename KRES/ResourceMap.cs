using System;
using UnityEngine;

namespace KRES
{
    public class ResourceMap
    {
        #region Fields
        private double[,] densityMap;
        private bool[,] uncoveredMap;
        private Texture2D hiddenTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private Color hiddenPixel = KRESUtils.BlankColour;
        #endregion

        #region Properties
        private PartResourceDefinition resource;
        /// <summary>
        /// Gets and sets the resource associated with this map.
        /// </summary>
        public PartResourceDefinition Resource
        {
            get { return this.resource; }
            set { this.resource = value; }
        }

        private Texture2D texture;
        /// <summary>
        /// Gets the planetary overlay texture.
        /// </summary>
        public Texture2D Texture
        {
            get { return this.texture; }
        }

        private Color colour;
        /// <summary>
        /// Gets and sets the colour used when generating the texture.
        /// </summary>
        public Color Colour
        {
            get { return this.colour; }
            set { this.colour = value; }
        }
        #endregion

        #region Initialisation
        /// <summary>
        /// Instantiate a blank resource map.
        /// </summary>
        public ResourceMap(string resourceName, Color colour)
        {
            this.hiddenTexture.SetPixel(1, 1, new Color(0, 0, 0, 0));
            this.resource = PartResourceLibrary.Instance.GetDefinition(resourceName);
            this.colour = colour;
        }

        /// <summary>
        /// Instantiate a generated resource map.
        /// </summary>
        public ResourceMap(string resourceName, Color colour, int bodyRadius, int pointDistance, int seed)
        {
            this.resource = PartResourceLibrary.Instance.GetDefinition(resourceName);
            this.colour = colour;
            GenerateDensityMap(bodyRadius, pointDistance, seed);
        }

        /// <summary>
        /// Instantiate a resource map with the given texture
        /// </summary>
        public ResourceMap(string resourceName, Texture2D map)
        {
            this.resource = PartResourceLibrary.Instance.GetDefinition(resourceName);
            this.texture = map; 
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Generates the resource map.
        /// </summary>
        public void GenerateDensityMap(int bodyRadius, int pointDistance, int seed)
        {
            // TODO: Generate resource map from point distances.

            int width = 360;
            int height = 360;

            this.densityMap = new double[width, height];
            this.uncoveredMap = new bool[width, height];
            float noiseScale = 5f;

            for (int y = 0; y < this.densityMap.GetLength(1); y++)
            {
                for (int x = 0; x < this.densityMap.GetLength(0); x++)
                {
                    float sample = Mathf.PerlinNoise((float)x / (float)width * noiseScale, (float)y / (float)height * noiseScale);
                    if (sample < 0.5f) sample = 0;

                    this.densityMap[x, y] = sample;
                    this.uncoveredMap[x, y] = true;
                }
            }

            GenerateTexture();
        }

        /// <summary>
        /// Generates a texture based on the current density map.
        /// </summary>
        public void GenerateTexture()
        {
            texture = new Texture2D(this.densityMap.GetLength(0), this.densityMap.GetLength(1), TextureFormat.ARGB32, true);
            Color[] colours = new Color[texture.width * texture.height];

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    if (this.uncoveredMap[x, y])
                    {
                        texture.SetPixel(x, y, new Color(this.colour.r, this.colour.g, this.colour.b, (float)this.densityMap[x, y]));
                    }
                    else
                    {
                        texture.SetPixel(x, y, this.hiddenPixel);
                    }
                }
            }
            texture.Apply();

            DebugWindow.Instance.SetTexture(this.texture);
        }

        /// <summary>
        /// Gets the density of resource at a specific longitude and latitude.
        /// </summary>
        public double DensityAtPosition(float longitude, float latitude)
        {
            int x = (int)((this.texture.width / 360f) * longitude);
            int y = (int)((this.texture.height / 180f) * (latitude + 90f));

            return this.densityMap[x, y];
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
                        if (material.name.Contains("ResourceMap-" + this.resource.name))
                        {
                            containsMaterial = true;
                            material.mainTexture = this.texture;
                            break;
                        }
                    }

                    if (!containsMaterial)
                    {
                        Material material = new Material(Shader.Find("Unlit/Transparent"));
                        material.name = "ResourceMap-" + this.resource.name;
                        material.mainTexture = this.texture;

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
                        if (material.name.Contains("ResourceMap-" + this.resource.name))
                        {
                            material.mainTexture = this.hiddenTexture;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
