using System.Collections.Generic;

namespace KRES.Data
{
    public class DataType
    {
        #region Propreties
        private ResourceType type = ResourceType.ORE;
        public ResourceType Type
        {
            get { return this.type; }
        }

        private List<DataBody> bodies = new List<DataBody>();
        public List<DataBody> Bodies
        {
            get { return this.bodies; }
        }
        #endregion

        #region Contructor
        public DataType(string type)
        {
            this.type = KRESUtils.GetResourceType(type);
            foreach (CelestialBody body in KRESUtils.GetRelevantBodies(type))
            {
                bodies.Add(new DataBody(body.bodyName, type));
            }
        }

        public DataType(ConfigNode type)
        {
            this.type = KRESUtils.GetResourceType(type.name);
            foreach (ConfigNode body in type.nodes)
            {
                bodies.Add(new DataBody(body, type.name));
            }
        }
        #endregion

        #region Methods
        public DataBody GetBody(string name)
        {
            return this.Bodies.Find(b => b.Name == name);
        }
        #endregion
    }
}
