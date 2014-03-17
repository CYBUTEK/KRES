using System.Collections.Generic;
using System.IO;
using UnityEngine;
using KRES.Extensions;

namespace KRES.Data
{
    public class DataBody
    {
        #region Propreties
        private string name = string.Empty;
        public string Name
        {
            get { return this.name; }
        }

        private string type = string.Empty;
        public string Type
        {
            get { return this.type; }
        }

        private double currentError = 1d;
        public double CurrentError
        {
            get { return this.currentError; }
            set { this.currentError = value; }
        }
        #endregion

        #region Constructor
        public DataBody(string body, string type)
        {
            this.name = body;
            this.type = type;
            currentError = 1d;
        }

        public DataBody(ConfigNode body, string type)
        {
            this.name = body.name;
            this.type = type;
            if (!body.TryGetValue("currentError", ref currentError))
            {
                currentError = 1d;
                body.AddValue("currentError", currentError.ToString("0.00000"));
            }
        }
        #endregion
    }
}
