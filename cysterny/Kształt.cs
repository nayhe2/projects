using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cysterny
{
    public abstract class Kształt
    {
        public static double nextX;

        public double x { get; private set; }

        public Kształt()
        {
            x = nextX;
        }

        public abstract double CalculateVolume();

        public abstract double CalculatePouredWaterVolume(double waterLevel);

        public abstract double GetHeight();
    }
}