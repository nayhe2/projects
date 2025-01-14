using System;
using System.Collections.Generic;
namespace cysterny
{
    class Test
    {
        public double expectedVolume;
        public List<Kształt> shapes = new List<Kształt>();
        public double CalculatedWaterLevel;

        public void AddShape(Kształt shape)
        {
            shapes.Add(shape);
        }

        public void Execute()
        {
            CalculatedWaterLevel = FindWaterLevel(GetMaxHeight(), expectedVolume);
            if (CalculatedWaterLevel == -1)
            {
                Console.WriteLine("OVERFLOW");
            }
            else
            {
                Console.WriteLine(CalculatedWaterLevel.ToString("F2"));
            }
        }

        private double CalculateTotalCapacity()
        {
            double totalCapacity = 0;
            foreach (var shape in shapes)
            {
                totalCapacity += shape.CalculateVolume();
            }
            return totalCapacity;
        }

        private double GetMaxHeight()
        {
            double maxHeight = 0;
            foreach (var shape in shapes)
            {
                maxHeight = Math.Max(maxHeight, shape.GetHeight());
            }
            return maxHeight;
        }

        private double FindWaterLevel(double maxHeight, double targetVolume)
        {
            double min = 0.0;
            double max = maxHeight;

            if (CalculateTotalCapacity() < targetVolume)
            {
                return -1; // OVERFLOW
            }

            while (max - min > 0.01)
            {
                double mid = (min + max) / 2;
                double volume = CalculateTotalVolume(mid);

                if (volume >= targetVolume)
                {
                    max = mid;
                }
                else
                {
                    min = mid;
                }
            }

            return (min + max) / 2;
        }

        private double CalculateTotalVolume(double waterLevel)
        {
            double totalVolume = 0;
            foreach (var shape in shapes)
            {
                totalVolume += shape.CalculatePouredWaterVolume(waterLevel);
            }
            return totalVolume;
        }
    }
}