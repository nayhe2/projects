using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cysterny
{
    public class Kula : Kształt
    {
        public double b;
        public double r;

        public Kula(double b, double r) : base()
        {
            this.b = b;
            this.r = r;
        }

        public override double CalculateVolume()
        {
            return (4.0 / 3.0) * Math.PI * Math.Pow(this.r, 3);
        }

        public override double CalculatePouredWaterVolume(double waterLevel)
        {
            double adjustedHeight = waterLevel - this.b;

            if (adjustedHeight >= 2 * r)
            {
                return CalculateVolume();
            }
            else if (adjustedHeight <= 0)
            {
                return 0;
            }

            double h = adjustedHeight;
            double volume = Math.PI * Math.Pow(h, 2) * (3 * r - h) / 3.0;
            return volume;
        }

        public override double GetHeight()
        {
            return 2 * r + this.b;
        }
    }
}