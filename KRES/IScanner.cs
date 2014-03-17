using System;
using System.Collections.Generic;
using System.Linq;

namespace KRES
{
    public interface IScanner
    {
        double Scan();
        double ScanAmount();
    }

    public class OrbitalScanner : IScanner
    {
        #region Fields
        private ModuleKresScanner scanner = null;
        #endregion

        #region Constructor
        public OrbitalScanner(ModuleKresScanner scanner)
        {
            this.scanner = scanner;
            this.scanner.presence = " (surface%):";
            this.scanner.location = "Extractable";
        }
        #endregion

        #region Methods
        public double Scan()
        {
            if (!this.scanner.scannedFlag)
            {
                double current = ScanAmount();
                if (current > 0) { this.scanner.status = "Scanning..."; }
                else { this.scanner.status = "Not enough " + this.scanner.resource; }
                List<ModuleKresScanner> scanners = new List<ModuleKresScanner>(this.scanner.vessel.FindPartModulesImplementing<ModuleKresScanner>().Where(s => s != this.scanner && s.scannerType == this.scanner.scannerType));
                double others = 0d;
                if (scanners.Count > 0)
                {
                    others = scanners.Sum(m => m.scanner.ScanAmount());
                    scanners.ForEach(m => m.currentError -= current + others);
                }
                return current + others;
            }
            return 0d;
        }

        public double ScanAmount()
        {
            if (!this.scanner.scannedFlag)
            {
                if (!this.scanner.ResourceValid || CheatOptions.InfiniteFuel || this.scanner.part.RequestResource(this.scanner.resource, this.scanner.rate * TimeWarp.fixedDeltaTime) > 0d)
                {
                    this.scanner.scannedFlag = true;
                    double delta = Math.Abs(this.scanner.ASL - this.scanner.optimalAltitude);
                    double altitudeFactor = Math.Pow(2d, -delta / (this.scanner.scaleFactor * this.scanner.optimalAltitude));
                    double timeFactor = (1d / this.scanner.scanningSpeed) * TimeWarp.fixedDeltaTime;
                    return timeFactor * altitudeFactor;
                }
            }
            return 0d;
        }
        #endregion
    }

    public class AtmosphericScanner : IScanner
    {
        #region Fields
        private ModuleKresScanner scanner = null;
        #endregion

        #region Constructor
        public AtmosphericScanner(ModuleKresScanner scanner)
        {
            this.scanner = scanner;
            this.scanner.presence = " (vol/vol):";
            this.scanner.location = "Atmospheric";
        }
        #endregion

        #region Methods
        public double Scan()
        {
            if (!this.scanner.scannedFlag)
            {
                double current = ScanAmount();
                if (current > 0) { this.scanner.status = "Scanning..."; }
                else { this.scanner.status = "Not enough " + this.scanner.resource; }
                List<ModuleKresScanner> scanners = new List<ModuleKresScanner>(this.scanner.vessel.FindPartModulesImplementing<ModuleKresScanner>().Where(s => s != this.scanner && s.scannerType == this.scanner.scannerType));
                double others = 0d;
                if (scanners.Count > 0)
                {
                    others = scanners.Sum(m => m.scanner.ScanAmount());
                    scanners.ForEach(m => m.currentError -= current + others);
                }
                return current + others;
            }
            return 0d;
        }

        public double ScanAmount()
        {
            if (this.scanner.AtmosphericPressure <= 0d)
            {
                this.scanner.status = "No air";
                return 0d;
            }
            if (!this.scanner.scannedFlag)
            {
                if (!this.scanner.ResourceValid || CheatOptions.InfiniteFuel || this.scanner.part.RequestResource(this.scanner.resource, this.scanner.rate * TimeWarp.fixedDeltaTime) > 0d)
                {
                    this.scanner.scannedFlag = true;
                    double delta = -Math.Abs(this.scanner.optimalPressure - this.scanner.AtmosphericPressure);
                    double pressureFactor = Math.Pow(2d, delta / (this.scanner.scaleFactor * this.scanner.optimalPressure));
                    double timeFactor = ((1d - this.scanner.maxPrecision) / this.scanner.scanningSpeed) * TimeWarp.fixedDeltaTime;
                    return timeFactor * pressureFactor;
                }
            }
            return 0d;
        }
        #endregion
    }

    public class OceanicScanner : IScanner
    {
        #region Fields
        private ModuleKresScanner scanner = null;
        #endregion

        #region Constructor
        public OceanicScanner(ModuleKresScanner scanner)
        {
            this.scanner = scanner;
            this.scanner.presence = " (vol/vol):";
            this.scanner.location = "Oceanic";
        }
        #endregion

        #region Methods
        public double Scan()
        {
            if (!this.scanner.scannedFlag)
            {
                double current = ScanAmount();
                if (current > 0) { this.scanner.status = "Scanning..."; }
                else { this.scanner.status = "Not enough " + this.scanner.resource; }
                List<ModuleKresScanner> scanners = new List<ModuleKresScanner>(this.scanner.vessel.FindPartModulesImplementing<ModuleKresScanner>().Where(s => s != this.scanner && s.scannerType == this.scanner.scannerType));
                double others = 0d;
                if (scanners.Count > 0)
                {
                    others = scanners.Sum(m => m.scanner.ScanAmount());
                    scanners.ForEach(m => m.currentError -= current + others);
                }
                return current + others;
            }
            if (!this.scanner.vessel.Splashed) { this.scanner.status = "Not enough " + this.scanner.resource; }
            return 0d;
        }

        public double ScanAmount()
        {
            if (!this.scanner.vessel.Splashed)
            {
                this.scanner.status = "Not in water";
                return 0d;
            }
            if (!this.scanner.scannedFlag)
            {
                if (!this.scanner.ResourceValid || CheatOptions.InfiniteFuel || this.scanner.part.RequestResource(this.scanner.resource, this.scanner.rate * TimeWarp.fixedDeltaTime) > 0d)
                {
                    this.scanner.scannedFlag = true;
                    return (1d / this.scanner.scanningSpeed) * TimeWarp.fixedDeltaTime;
                }
            }
            return 0d;
        }
        #endregion
    }
}
