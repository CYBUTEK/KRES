using UnityEngine;

namespace KRES.Defaults
{
    public class DefaultResource
    {
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

        private string biome = string.Empty;
        public string Biome
        {
            get { return this.biome; }
            set { this.biome = value; }
        }

        private Color colour = new Color(0, 0, 0, 0);
        public Color Colour
        {
            get { return this.colour; }
            set { this.colour = value; }
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
    }
}
