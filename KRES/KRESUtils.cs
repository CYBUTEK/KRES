using System;
using UnityEngine;

namespace KRES
{
    public static class KRESUtils
    {
        public static bool TryParseCelestialBody(string name, out CelestialBody result)
        {
            CelestialBody match = null;
            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (body.bodyName == name)
                {
                    match = body;
                    break;
                }
            }
            result = match;
            if (match == null) { return false; }
            else { return true; }
        }  
    }
}
