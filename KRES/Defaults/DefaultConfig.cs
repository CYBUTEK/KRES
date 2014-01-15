using System.Collections.Generic;
using System.Linq;

namespace KRES.Defaults
{
    public class DefaultConfig
    {
        #region Properties
        private string name = string.Empty;
        public string Name
        {
            get { return this.name; }
        }

        private string description = string.Empty;
        public string Description
        {
            get { return this.description; }
        }

        private List<DefaultBody> bodies = new List<DefaultBody>();
        public List<DefaultBody> Bodies
        {
            get { return this.bodies; }
        }
        #endregion

        #region Initialisation
        public DefaultConfig() { }

        public DefaultConfig(ConfigNode configNode)
        {
            this.name = configNode.GetValue("name");
            this.description = configNode.GetValue("description");

            foreach (ConfigNode bodyNode in configNode.GetNodes("KRES_BODY"))
            {
                if (FlightGlobals.Bodies.Any(b => b.name == bodyNode.GetValue("name")))
                {
                    this.bodies.Add(new DefaultBody(bodyNode));
                }
            }
        }
        #endregion

        #region Public Methods
        public DefaultBody GetBody(string name)
        {
            foreach (DefaultBody body in this.bodies)
            {
                if (body.Name == name)
                {
                    return body;
                }
            }
            return null;
        }

        public bool HasBody(string name)
        {
            foreach (DefaultBody body in this.bodies)
            {
                if (body.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
