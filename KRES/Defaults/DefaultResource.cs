using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KRES.Extensions;

namespace KRES.Defaults
{
    public class DefaultResource
    {
        #region Properties
        private string name = string.Empty;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private string type = string.Empty;
        public string Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        private string[] biomes = new string[] { };
        public string[] Biomes
        {
            get { return this.biomes; }
            set { this.biomes = value; }
        }

        private Color colour = KRESUtils.BlankColour;
        public Color Colour
        {
            get { return this.colour; }
            set { this.colour = value; }
        }

        private double minAltitude = double.NaN;
        public double MinAltitude
        {
            get { return this.minAltitude; }
            set { this.minAltitude = value; }
        }

        private double maxAltitude = double.NaN;
        public double MaxAltitude
        {
            get { return this.maxAltitude; }
            set { this.maxAltitude = value; }
        }

        private double density = 0;
        public double Density
        {
            get { return this.density; }
            set { this.density = value; }
        }

        private double octaves = 0;
        public double Octaves
        {
            get { return this.octaves; }
            set { this.octaves = value; }
        }

        private double persistence = 0;
        public double Persistence
        {
            get { return this.persistence; }
            set { this.persistence = value; }
        }

        private double frequency = 0;
        public double Frequency
        {
            get { return this.frequency; }
            set { this.frequency = value; }
        }

        private double seed = Random.Range(0, int.MaxValue);
        public double Seed
        {
            get { return this.seed; }
            set { this.seed = value; }
        }
        #endregion

        #region Initialisation
        public DefaultResource(ConfigNode configNode)
        {
            configNode.TryGetValue("name", ref this.name);
            configNode.TryGetValue("type", ref this.type);
            configNode.TryGetValue("colour", ref this.colour);
            configNode.TryGetValue("density", ref this.density);
            configNode.TryGetValue("octaves", ref this.octaves);
            configNode.TryGetValue("persistence", ref this.persistence);
            configNode.TryGetValue("frequency", ref this.frequency);
            configNode.TryGetValue("minAltitude", ref this.minAltitude);
            configNode.TryGetValue("maxAltitude", ref this.maxAltitude);
            configNode.TryGetValues("biome", ref this.biomes);
            if (this.type == "ore") { this.seed = Random.Range(0, 999999999); }
        }
        #endregion

        #region Public Methods
        public ConfigNode CreateConfigNode()
        {
            ConfigNode configNode = new ConfigNode("KRES_RESOURCE");
            configNode.AddValue("name", this.name);
            configNode.AddValue("density", this.density);
            configNode.AddValue("type", this.type);
            if (this.type == "ore")
            {
                configNode.AddValue("seed", this.seed.ToString("000000000"));
                configNode.AddValue("colour", KRESUtils.ColorToString(this.colour));
                configNode.AddValue("octaves", this.octaves);
                configNode.AddValue("persistence", this.persistence);
                configNode.AddValue("frequency", this.frequency);
            }
            if (!double.IsNaN(this.minAltitude)) { configNode.AddValue("minAltitude", this.minAltitude); }
            if (!double.IsNaN(this.maxAltitude)) { configNode.AddValue("maxAltitude", this.maxAltitude); }
            if (this.biomes.Length > 0) { configNode.AddValues("biome", this.biomes); }
            return configNode;
        }
        #endregion
    }
}
