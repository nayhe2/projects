using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cysterny
{
    public class Prostopadloscian : Kształt
    {
        public double b;
        public double h;
        public double w;
        public double d;

        public Prostopadloscian(double b, double h, double w, double d) : base()
        {
            this.b = b;
            this.h = h;
            this.w = w;
            this.d = d;
        }

        public override double CalculateVolume()
        {
            return this.w * this.d * this.h;
        }

        public override double CalculatePouredWaterVolume(double waterLevel)
        {
            double adjustedHeight = waterLevel - this.b;
            if (adjustedHeight > 0 && adjustedHeight < this.h)
            {
                return this.w * this.d * adjustedHeight;
            }
            else if (adjustedHeight >= this.h)
            {
                return this.CalculateVolume();
            }
            return 0;
        }

        public override double GetHeight()
        {
            return this.h + this.b;
        }
    }
}