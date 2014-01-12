using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KRES
{
    public class StorageModule : PartModule
    {
        [KSPField]
        public float maxStorageMass = 0;

        public float usedStorageMass = 0;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Max Storage Mass", guiFormat = "#,#.", guiUnits = "kg")]
        private float guiMaxStorageMass = 0;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Used Storage Mass", guiFormat = "#,#.", guiUnits = "kg")]
        private float guiUsedStorageMass = 0;

        private List<PartResource> hiddenResources = new List<PartResource>();

        private void Start()
        {
            // Set the max storage mass gui field and format to kg.
            this.guiMaxStorageMass = this.maxStorageMass * 1000f;
        }

        // Updates the maximum amount for every resource based on the maximum storage mass.
        private void LateUpdate()
        {
            if (HighLogic.LoadedSceneIsEditor || (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel == this.vessel))
            {
                this.usedStorageMass = this.part.GetResourceMass();
                double availableMass = this.maxStorageMass - this.usedStorageMass;

                // Show hidden resources if space is available.
                if (HighLogic.LoadedSceneIsEditor && availableMass > 0)
                {
                    if (this.hiddenResources.Count > 0)
                    {
                        this.part.Resources.list.AddRange(this.hiddenResources);
                        this.hiddenResources.Clear();
                    }
                }

                foreach (PartResource resource in this.part.Resources)
                {
                    if (resource.info.density > 0) // Resource has a density.
                    {
                        if (availableMass > 0) // Space is available
                        {
                            resource.maxAmount = resource.amount + (availableMass / resource.info.density);
                        }
                        else if (resource.maxAmount != resource.amount) // No space available and maxAmount is not amount.
                        {
                            if (double.IsNaN(resource.amount) && HighLogic.LoadedSceneIsEditor) // Amount is NaN and in editor hide resource.
                            {
                                resource.amount = 0;
                                resource.maxAmount = 0;
                                this.hiddenResources.Add(resource);
                            }
                            else // Set max amount to amount.
                            {
                                resource.maxAmount = resource.amount;
                            }
                        }
                    }
                }

                // Remove all resources from the part that are set as hidden.
                this.part.Resources.list.RemoveAll(r => this.hiddenResources.Contains(r));

                // Format the used storage mass gui field to kg.
                this.guiUsedStorageMass = this.usedStorageMass * 1000f;
            }
        }
    }
}
