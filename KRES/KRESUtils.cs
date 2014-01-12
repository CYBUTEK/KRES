using System;
using UnityEngine;

namespace KRES
{
    public static class KRESUtils
    {
        public static bool TryParseCelestialBody(string name, out CelestialBody result)
        {
            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (body.bodyName == name)
                {
                    result = body;
                    return true;
                }
            }
            result = null;
            return false;
        }
    }
}
