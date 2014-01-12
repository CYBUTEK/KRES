using System;
using UnityEngine;

namespace KRES
{
    public class KRESSettings
    {
        #region ConfigCreators
        //Adds a KRES_RESOURCE node
        private static void CreateResource(ConfigNode cfg, string name, float density, string type)
        {
            cfg.AddNode("KRES_RESOURCE").AddValue("name", name);
            foreach (ConfigNode resource in cfg.GetNodes("KRES_RESOURCE"))
            {
                if (resource.HasValue("name") && resource.GetValue("name") == name && !resource.HasValue("type"))
                {
                    if (type == "ore") { resource.AddValue("seed", UnityEngine.Random.Range(0, 999999999).ToString("000000000")); }
                    resource.AddValue("density", density);
                    resource.AddValue("type", type);
                }
            }
        }

        private static void CreateResource(ConfigNode cfg, string name, float density, string type, string biome)
        {
            cfg.AddNode("KRES_RESOURCE").AddValue("name", name);
            foreach (ConfigNode resource in cfg.GetNodes("KRES_RESOURCE"))
            {
                if (resource.HasValue("name") && resource.GetValue("name") == name && !resource.HasValue("type"))
                {
                    if (type == "ore") { resource.AddValue("seed", UnityEngine.Random.Range(0, 999999999).ToString("000000000")); }
                    resource.AddValue("density", density);
                    resource.AddValue("type", type);
                    resource.AddValue("biome", biome);
                }
            }
        }

        #endregion

        public static void GenerateSettings(ConfigNode cfg)
        {
            //Checks for all planet nodes
            if (!cfg.HasNode("Kerbin"))
            {
                //Create Kerbin node
                Debug.Log("[KRES]: Creating Kerbin node");
                cfg.AddNode("Kerbin");
                ConfigNode node = cfg.GetNode("Kerbin");
                CreateResource(node, "Water", 0.3f, "ore");
                CreateResource(node, "Water", 1, "liquid");
                CreateResource(node, "Oxygen", 0.21f, "gas");
                CreateResource(node, "Nitrogen", 0.7f, "gas");
                CreateResource(node, "Carbon", 0.07f, "gas");
                CreateResource(node, "Water", 0.02f, "gas");
            }

            if (!cfg.HasNode("Mun"))
            {
                //Create Mun node
                Debug.Log("[KRES]: Creating Mun node");
                cfg.AddNode("Mun");
                ConfigNode node = cfg.GetNode("Mun");
                CreateResource(node, "Iron", 0.5f, "ore");
                CreateResource(node, "Carbon", 0.4f, "ore");
                CreateResource(node, "Aluminum", 0.15f, "ore");
                CreateResource(node, "Chromium", 0.1f, "ore");
                CreateResource(node, "Titanium", 0.05f, "ore");
                CreateResource(node, "Rare earths", 0.01f, "ore");
                CreateResource(node, "Plutonium", 0.005f, "ore");
                CreateResource(node, "Uranium", 0.005f, "ore");
                CreateResource(node, "Hydrogen", 0.8f, "ore");
                CreateResource(node, "Nitrogen", 0.5f, "ore");
                CreateResource(node, "Oxygen", 0.1f, "ore");
                CreateResource(node, "Water", 0.6f, "ore", "Poles");
            }

            if (!cfg.HasNode("Minmus"))
            {
                //Create Minmus node
                Debug.Log("[KRES]: Creating Minmus node");
                cfg.AddNode("Minmus");
                ConfigNode node = cfg.GetNode("Minmus");
                CreateResource(node, "Carbon", 0.3f, "ore");
                CreateResource(node, "Iron", 0.2f, "ore");
                CreateResource(node, "Aluminum", 0.15f, "ore");
                CreateResource(node, "Chromium", 0.1f, "ore");
                CreateResource(node, "Titanium", 0.15f, "ore");
                CreateResource(node, "Rare earths", 0.04f, "ore");
                CreateResource(node, "Plutonium", 0.02f, "ore");
                CreateResource(node, "Uranium", 0.02f, "ore");
                CreateResource(node, "Oxygen", 0.6f, "ore");
                CreateResource(node, "Hydrogen", 0.5f, "ore");
                CreateResource(node, "Nitrogen", 0.3f, "ore");
            }

            if (!cfg.HasNode("Moho"))
            {
                //Create Moho node
                Debug.Log("[KRES]: Creating Moho node");
                cfg.AddNode("Moho");
                ConfigNode node = cfg.GetNode("Moho");
                CreateResource(node, "Carbon", 0.31f, "ore");
                CreateResource(node, "Iron", 0.28f, "ore");
                CreateResource(node, "Titanium", 0.16f, "ore");
                CreateResource(node, "Aluminum", 0.08f, "ore");
                CreateResource(node, "Chromium", 0.04f, "ore");
                CreateResource(node, "Uranium", 0.04f, "ore");
                CreateResource(node, "Plutonium", 0.03f, "ore");
                CreateResource(node, "Rare earths", 0.03f, "ore");
                CreateResource(node, "Xenon", 0.6f, "ore");
                CreateResource(node, "Oxygen", 0.01f, "ore");
                CreateResource(node, "Hydrogen", 0.05f, "ore");
            }

            if (!cfg.HasNode("Eve"))
            {
                //Create Eve node
                Debug.Log("[KRES]: Creating Eve node");
                cfg.AddNode("Eve");
                ConfigNode node = cfg.GetNode("Eve");
                CreateResource(node, "Carbon", 0.2f, "ore");
                CreateResource(node, "Iron", 0.18f, "ore");
                CreateResource(node, "Titanium", 0.17f, "ore");
                CreateResource(node, "Plutonium", 0.1f, "ore");
                CreateResource(node, "Uranium", 0.09f, "ore");
                CreateResource(node, "Rare earths", 0.05f, "ore");
                CreateResource(node, "Aluminum", 0.03f, "ore");
                CreateResource(node, "Chromium", 0.01f, "ore");
                CreateResource(node, "Xenon", 0.6f, "ore");
                CreateResource(node, "Oxygen", 0.6f, "ore");
                CreateResource(node, "Hydrogen", 0.6f, "ore");
                CreateResource(node, "Nitrogen", 0.6f, "ore");
                CreateResource(node, "Water", 0.5f, "liquid");
                CreateResource(node, "Uranium", 0.01f, "liquid");
                CreateResource(node, "Carbon", 0.8f, "gas");
                CreateResource(node, "Nitrogen", 0.1f, "gas");
                CreateResource(node, "Xenon", 0.05f, "gas");
                CreateResource(node, "Water", 0.05f, "gas");
            }

            if (!cfg.HasNode("Gilly"))
            {
                //Create Gilly node
                Debug.Log("[KRES]: Creating Gilly node");
                cfg.AddNode("Gilly");
                ConfigNode node = cfg.GetNode("Gilly");
                CreateResource(node, "Carbon", 0.29f, "ore");
                CreateResource(node, "Aluminum", 0.24f, "ore");
                CreateResource(node, "Titanium", 0.21f, "ore");
                CreateResource(node, "Rare earths", 0.08f, "ore");
                CreateResource(node, "Chromium", 0.03f, "ore");
                CreateResource(node, "Iron", 0.02f, "ore");
                CreateResource(node, "Hydrogen", 0.6f, "ore");
                CreateResource(node, "Xenon", 0.5f, "ore");
                CreateResource(node, "Oxygen", 0.25f, "ore");
            }

            if (!cfg.HasNode("Duna"))
            {
                //Create Duna node
                Debug.Log("[KRES]: Creating Duna node");
                cfg.AddNode("Duna");
                ConfigNode node = cfg.GetNode("Duna");
                CreateResource(node, "Iron", 0.35f, "ore");
                CreateResource(node, "Carbon", 0.24f, "ore");
                CreateResource(node, "Titanium", 0.13f, "ore");
                CreateResource(node, "Aluminum", 0.1f, "ore");
                CreateResource(node, "Chromium", 0.06f, "ore");
                CreateResource(node, "Uranium", 0.01f, "ore");
                CreateResource(node, "Plutonium", 0.005f, "ore");
                CreateResource(node, "Rare earths", 0.005f, "ore");
                CreateResource(node, "Oxygen", 0.6f, "ore");
                CreateResource(node, "Water", 0.25f, "ore");
                CreateResource(node, "Hydrogen", 0.15f, "ore");
                CreateResource(node, "Nitrogen", 0.1f, "ore");
                CreateResource(node, "Carbon", 0.85f, "gas");
                CreateResource(node, "Nitrogen", 01f, "gas");
                CreateResource(node, "Water", 0.05f, "gas");
            }

            if (!cfg.HasNode("Ike"))
            {
                //Create Ike node
                Debug.Log("[KRES]: Creating Ike node");
                cfg.AddNode("Ike");
                ConfigNode node = cfg.GetNode("Ike");
                CreateResource(node, "Iron", 0.41f, "ore");
                CreateResource(node, "Carbon", 0.21f, "ore");
                CreateResource(node, "Titanium", 0.1f, "ore");
                CreateResource(node, "Aluminum", 0.08f, "ore");
                CreateResource(node, "Chromium", 0.03f, "ore");
                CreateResource(node, "Rare earths", 0.02f, "ore");
                CreateResource(node, "Uranium", 0.01f, "ore");
                CreateResource(node, "Plutonium", 0.01f, "ore");
                CreateResource(node, "Xenon", 0.4f, "ore");
                CreateResource(node, "Water", 0.1f, "ore");
                CreateResource(node, "Oxygen", 0.05f, "ore");
                CreateResource(node, "Hydrogen", 0.01f, "ore");
            }

            if (!cfg.HasNode("Dres"))
            {
                //Create Dres node
                Debug.Log("[KRES]: Creating Dres node");
                cfg.AddNode("Dres");
                ConfigNode node = cfg.GetNode("Dres");
                CreateResource(node, "Iron", 0.56f, "ore");
                CreateResource(node, "Titanium", 0.18f, "ore");
                CreateResource(node, "Carbon", 0.12f, "ore");
                CreateResource(node, "Aluminum", 0.07f, "ore");
                CreateResource(node, "Chromium", 0.03f, "ore");
                CreateResource(node, "Rare earths", 0.02f, "ore");
                CreateResource(node, "Uranium", 0.01f, "ore");
                CreateResource(node, "Plutonium", 0.005f, "ore");
                CreateResource(node, "Xenon", 0.5f, "ore");
                CreateResource(node, "Water", 0.2f, "ore");
                CreateResource(node, "Oxygen", 0.1f, "ore");
                CreateResource(node, "Hydrogen", 0.03f, "ore");
            }

            if (!cfg.HasNode("Jool"))
            {
                //Create Jool node
                Debug.Log("[KRES]: Creating Jool node");
                cfg.AddNode("Jool");
                ConfigNode node = cfg.GetNode("Jool");
                CreateResource(node, "Hydrogen", 0.6f, "gas");
                CreateResource(node, "Water", 0.2f, "gas");
                CreateResource(node, "Nitrogen", 0.15f, "gas");
                CreateResource(node, "Oxygen", 0.05f, "gas");
            }

            if (!cfg.HasNode("Laythe"))
            {
                //Create Laythe node
                Debug.Log("[KRES]: Creating Laythe node");
                cfg.AddNode("Laythe");
                ConfigNode node = cfg.GetNode("Laythe");
                CreateResource(node, "Iron", 0.36f, "ore");
                CreateResource(node, "Carbon", 0.21f, "ore");
                CreateResource(node, "Titanium", 0.19f, "ore");
                CreateResource(node, "Aluminum", 0.12f, "ore");
                CreateResource(node, "Chromium", 0.02f, "ore");
                CreateResource(node, "Rare earths", 0.01f, "ore");
                CreateResource(node, "Uranium", 0.005f, "ore");
                CreateResource(node, "Plutonium", 0.005f, "ore");
                CreateResource(node, "Oxygen", 0.6f, "ore");
                CreateResource(node, "Hydrogen", 0.6f, "ore");
                CreateResource(node, "Nitrogen", 0.5f, "ore");
                CreateResource(node, "Xenon", 0.25f, "ore");
                CreateResource(node, "Water", 0.2f, "ore");
                CreateResource(node, "Nitrogen", 0.5f, "liquid");
                CreateResource(node, "Water", 0.1f, "liquid");
                CreateResource(node, "Nitrogen", 0.69f, "gas");
                CreateResource(node, "Xenon", 0.2f, "gas");
                CreateResource(node, "Oxygen", 0.1f, "gas");
                CreateResource(node, "Water", 0.01f, "gas");
            }

            if (!cfg.HasNode("Vall"))
            {
                //Create Vall node
                Debug.Log("[KRES]: Creating Vall node");
                cfg.AddNode("Vall");
                ConfigNode node = cfg.GetNode("Vall");
                CreateResource(node, "Titanium", 0.32f, "ore");
                CreateResource(node, "Carbon", 0.26f, "ore");
                CreateResource(node, "Iron", 0.21f, "ore");
                CreateResource(node, "Aluminum", 0.08f, "ore");
                CreateResource(node, "Plutonium", 0.04f, "ore");
                CreateResource(node, "Rare earths", 0.03f, "ore");
                CreateResource(node, "Uranium", 0.01f, "ore");
                CreateResource(node, "Oxygen", 0.6f, "ore");
                CreateResource(node, "Nitrogen", 0.6f, "ore");
                CreateResource(node, "Hydrogen", 0.25f, "ore");
                CreateResource(node, "Xenon", 0.12f, "ore");
            }

            if (!cfg.HasNode("Tylo"))
            {
                //Create Tylo node
                Debug.Log("[KRES]: Creating Tylo node");
                cfg.AddNode("Tylo");
                ConfigNode node = cfg.GetNode("Tylo");
                CreateResource(node, "Iron", 0.51f, "ore");
                CreateResource(node, "Titanium", 0.12f, "ore");
                CreateResource(node, "Carbon", 0.07f, "ore");
                CreateResource(node, "Uranium", 0.04f, "ore");
                CreateResource(node, "Plutonium", 0.03f, "ore");
                CreateResource(node, "Rare earths", 0.01f, "ore");
                CreateResource(node, "Xenon", 0.6f, "ore");
                CreateResource(node, "Oxygen", 0.25f, "ore");
                CreateResource(node, "Hydrogen", 0.25f, "ore");
                CreateResource(node, "Nitrogen", 0.12f, "ore");
            }

            if (!cfg.HasNode("Bop"))
            {
                //Create Bop node
                Debug.Log("[KRES]: Creating Bop node");
                cfg.AddNode("Bop");
                ConfigNode node = cfg.GetNode("Bop");
                CreateResource(node, "Carbon", 0.21f, "ore");
                CreateResource(node, "Iron", 0.19f, "ore");
                CreateResource(node, "Titanium", 0.18f, "ore");
                CreateResource(node, "Aluminum", 0.14f, "ore");
                CreateResource(node, "Chromium", 0.1f, "ore");
                CreateResource(node, "Rare earths", 0.08f, "ore");
                CreateResource(node, "Uranium", 0.05f, "ore");
                CreateResource(node, "Plutonium", 0.02f, "ore");
                CreateResource(node, "Hydrogen", 0.5f, "ore");
                CreateResource(node, "Oxygen", 0.33f, "ore");
                CreateResource(node, "Xenon", 0.25f, "ore");
            }

            if (!cfg.HasNode("Pol"))
            {
                //Create Pol node
                Debug.Log("[KRES]: Creating Pol node");
                cfg.AddNode("Pol");
                ConfigNode node = cfg.GetNode("Pol");
                CreateResource(node, "Carbon", 0.24f, "ore");
                CreateResource(node, "Aluminum", 0.21f, "ore");
                CreateResource(node, "Titanium", 0.16f, "ore");
                CreateResource(node, "Rare earths", 0.1f, "ore");
                CreateResource(node, "Chromium", 0.08f, "ore");
                CreateResource(node, "Uranium", 0.04f, "ore");
                CreateResource(node, "Plutonium", 0.02f, "ore");
                CreateResource(node, "Xenon", 0.6f, "ore");
                CreateResource(node, "Oxygen", 0.5f, "ore");
                CreateResource(node, "Hydrogen", 0.33f, "ore");
                CreateResource(node, "Nitrogen", 0.12f, "ore");
            }

            if (!cfg.HasNode("Eeloo"))
            {
                //Create Eeloo node
                Debug.Log("[KRES]: Creating Eeloo node");
                cfg.AddNode("Eeloo");
                ConfigNode node = cfg.GetNode("Eeloo");
                CreateResource(node, "Carbon", 0.37f, "ore");
                CreateResource(node, "Iron", 0.31f, "ore");
                CreateResource(node, "Titanium", 0.12f, "ore");
                CreateResource(node, "Aluminum", 0.07f, "ore");
                CreateResource(node, "Rare earths", 0.4f, "ore");
                CreateResource(node, "Chromium", 0.01f, "ore");
                CreateResource(node, "Uranium", 0.01f, "ore");
                CreateResource(node, "Plutonium", 0.005f, "ore");
                CreateResource(node, "Oxygen", 0.33f, "ore");
                CreateResource(node, "Water", 0.2f, "ore");
                CreateResource(node, "Hydrogen", 0.12f, "ore");
                CreateResource(node, "Nitrogen", 0.05f, "ore");
            }
        }
    }
}
