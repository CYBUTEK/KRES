using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KRES
{
    public class StorageModule : PartModule
    {
        [KSPField]
        public float maxStorageMass = 0;

        // Updates the maximum amount for every resource based on the maximum storage mass.
        private void LateUpdate()
        {
            double availableMass = this.maxStorageMass - this.part.GetResourceMass();

            foreach (PartResource resource in this.part.Resources)
            {
                if (resource.info.density > 0)
                {
                    if (availableMass > 0)
                    {
                        resource.maxAmount = resource.amount + (availableMass / resource.info.density);
                    }
                    else if (resource.maxAmount != resource.amount)
                    {
                        if (double.IsNaN(resource.amount))
                        {
                            resource.amount = 0;
                        }
                        resource.maxAmount = resource.amount;
                    }
                }
            }
        }
    }
}
