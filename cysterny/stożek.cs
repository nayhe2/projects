using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cysterny
{
    public class Stozek : Kształt
    {
        public double b;
        public double r;
        public double h;

        public Stozek(double b, double r, double h) : base()
        {
            this.b = b;
            this.r = r;
            this.h = h;
        }

        public override double CalculateVolume()
        {
            return (Math.PI * Math.Pow(r, 2) * h) / 3.0;
        }

        public override double CalculatePouredWaterVolume(double waterLevel)
        {
            double adjustedHeight = waterLevel - this.b;
            if (adjustedHeight <= 0)
            {
                return 0;
            }

            double h = Math.Min(this.h, adjustedHeight);
            double volume = (Math.PI * Math.Pow(this.r, 2) * h) / 3.0;
            return volume;
        }

        public override double GetHeight()
        {
            return this.h + this.b;
        }
    }
}