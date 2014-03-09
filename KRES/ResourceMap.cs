using System;
using UnityEngine;
using KRES.Extensions;

namespace KRES
{
    public class ResourceMap
    {
        #region Properties
        private PartResourceDefinition resource;
        /// <summary>
        /// Gets and sets the resource associated with this map.
        /// </summary>
        public PartResourceDefinition Resource
        {
            get { return this.resource; }
        }

        /// <summary>
        /// String name of the resource
        /// </summary>
        public string Name
        {
            get { return Resource.name; }
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
        /// <summary>
        /// Instantiate a resource map with the given texture
        /// </summary>
        public ResourceMap(string resourceName, Texture2D map, ConfigNode resource)
        {
            this.resource = PartResourceLibrary.Instance.GetDefinition(resourceName);
            this.texture = map;
            resource.TryGetValue("colour", ref colour);
            resource.TryGetValue("minAltitude", ref minAltitude);
            resource.TryGetValue("maxAltitude", ref maxAltitude);
            resource.TryGetValue("biomes", ref biomes);
            resource.TryGetValue("excludedBiomes", ref excludedBiomes);
        }
        #endregion

        #region Public Methods
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
                            material.mainTexture = this.texture;
                            break;
                        }
                    }

                    if (!containsMaterial)
                    {
                        Material material = new Material(Shader.Find("Unlit/Transparent"));
                        material.name = "KRESResourceMap";
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
        #endregion
    }
}
