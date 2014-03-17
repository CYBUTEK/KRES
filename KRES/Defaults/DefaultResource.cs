using System;
using System.Collections.Generic;
using System.Linq;
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

        private string[] excludedBiomes = new string[] { };
        public string[] ExcludedBiomes
        {
            get { return this.excludedBiomes; }
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

        private int seed = 0;
        public int Seed
        {
            get { return this.seed; }
            set { this.seed = value; }
        }
        #endregion

        #region Initialisation
        public DefaultResource(ConfigNode configNode, Random random)
        {
            configNode.TryGetValue("name", ref this.name);
            configNode.TryGetValue("type", ref this.type);
            configNode.TryGetValue("density", ref this.density);
            configNode.TryGetValue("octaves", ref this.octaves);
            configNode.TryGetValue("persistence", ref this.persistence);
            configNode.TryGetValue("frequency", ref this.frequency);
            configNode.TryGetValue("minAltitude", ref this.minAltitude);
            configNode.TryGetValue("maxAltitude", ref this.maxAltitude);
            configNode.TryGetValue("biomes", ref this.biomes);
            configNode.TryGetValue("excludedBiomes", ref this.excludedBiomes);
            if (this.type == "ore") { this.seed = random.Next(999999999); }
        }
        #endregion

        #region Public Methods
        public ConfigNode CreateConfigNode()
        {
            ConfigNode configNode = new ConfigNode("KRES_DATA");
            configNode.AddValue("name", this.name);
            return configNode;
        }
        #endregion
    }
}
