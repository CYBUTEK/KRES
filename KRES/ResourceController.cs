using System.Collections.Generic;
using UnityEngine;

namespace KRES
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class ResourceController : MonoBehaviour
    {
        #region Instance
        public static ResourceController Instance { get; private set; }
        #endregion

        #region Properties
        private List<ResourceBody> resourceBodies = new List<ResourceBody>();
        /// <summary>
        /// Gets and sets the global list of resource bodies.
        /// </summary>
        public List<ResourceBody> ResourceBodies
        {
            get { return this.resourceBodies; }
            set { this.resourceBodies = value; }
        }
        #endregion

        #region Initialisation
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Shows all resource map textures for the specified resource type.
        /// </summary>
        public void ShowResource(string resourceName)
        {
            if (ResourceLoader.Loaded)
            {
                DebugWindow.Instance.Print("Showing: " + resourceName);
                foreach (ResourceBody body in this.resourceBodies)
                {
                    foreach (ResourceMap map in body.ResourceMaps)
                    {
                        map.ShowTexture(body.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Hides all resource map textures for the specified resource type.
        /// </summary>
        public void HideResource(string resourceName)
        {
            if (ResourceLoader.Loaded)
            {
                DebugWindow.Instance.Print("Hiding: " + resourceName);
                foreach (ResourceBody body in this.resourceBodies)
                {
                    foreach (ResourceMap map in body.ResourceMaps)
                    {
                        map.HideTexture(body.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Hides all resource map textures.
        /// </summary>
        public void HideAllResources()
        {
            if (ResourceLoader.Loaded)
            {
                DebugWindow.Instance.Print("Hiding: All Resources");
                foreach (ResourceBody body in this.resourceBodies)
                {
                    foreach (ResourceMap map in body.ResourceMaps)
                    {
                        map.HideTexture(body.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Unloads all the resource maps.
        /// </summary>
        public void UnloadResources()
        {
            if (ResourceLoader.Loaded)
            {
                foreach (Transform transform in ScaledSpace.Instance.scaledSpaceTransforms)
                {
                    List<Material> materials = new List<Material>(transform.renderer.materials);
                    materials.RemoveAll(m => m.name.Contains("ResourceMap-"));
                    transform.renderer.materials = materials.ToArray();
                }
            }

            this.resourceBodies.Clear();
        }
        #endregion
    }
}
