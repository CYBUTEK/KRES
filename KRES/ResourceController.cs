using System;
using UnityEngine;

namespace KRES
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class ResourceController : MonoBehaviour
    {
        private void Start()
        {
            DebugWindow.Instance.SetTexture(FlightGlobals.Bodies.Find(b => b.name == "Kerbin").BiomeMap.Map, "Kerbin Biome Map");
        }
    }
}
