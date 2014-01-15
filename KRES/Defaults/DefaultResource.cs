using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private float density = 0;
        public float Density
        {
            get { return this.density; }
            set { this.density = value; }
        }
    }
}
