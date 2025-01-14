using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace cysterny
{
    public partial class Form1 : Form
    {
        public struct ShapeWithWaterLevel
        {
            public Kształt Shape { get; }
            public double WaterLevel { get; }

            public ShapeWithWaterLevel(Kształt shape, double waterLevel)
            {
                Shape = shape;
                WaterLevel = waterLevel;
            }
        }

        private CysternaTester tester = new CysternaTester();

        private List<ShapeWithWaterLevel> shapes = new List<ShapeWithWaterLevel>();
        private List<Test> testResults = new List<Test>();
       
        private int currentTestIndex;

        public Form1()
        {
            InitializeComponent();
            RunTestsAndVisualize();
        }

        private void RunTestsAndVisualize()
        {
            tester.TestCompleted += TestCompleted;
            tester.RunTests();
        }

        private void TestCompleted(List<Test> tests)
        {
            testResults = tests;
            currentTestIndex = 0;
            DisplayCurrentTest();
        }

        private void DisplayCurrentTest()
        {
            if (testResults.Count > 0 && currentTestIndex < testResults.Count)
            {
                shapes.Clear();

                var currentTest = testResults[currentTestIndex];
                double currentWaterLevel = currentTest.CalculatedWaterLevel;

                for (int i = 0; i < currentTest.shapes.Count; i++)
                {
                    shapes.Add(new ShapeWithWaterLevel(currentTest.shapes[i], currentWaterLevel));
                }

                Invalidate();
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (testResults.Count > 0)
            {
                currentTestIndex++;
                if (currentTestIndex >= testResults.Count)
                {
                    currentTestIndex = 0;
                }

                DisplayCurrentTest();
            }
        }

        private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawShapes(e.Graphics);
        }

        private void DrawShapes(Graphics g)
        {
            float scale = 20; // skala rysowania
            int scrollOffset = hScrollBar1.Value;
            foreach (var shapeWithWaterLevel in shapes)
            {
                DrawShape(g, shapeWithWaterLevel.Shape, shapeWithWaterLevel.WaterLevel, scale, scrollOffset);
            }
        }

        private void DrawShape(Graphics g, Kształt shape, double waterLevel, float scale, int scrollOffset)
        {
            int xOffset = (int)shape.x - scrollOffset; // przesunięcie figury na osi X, z uwzględnieniem przewijania strony
            int yOffset = this.ClientSize.Height - hScrollBar1.Height - 1; // start osi Y dla figur na dolnej krawędzi formularza zaraz nad scroll barem

            if (shape is Prostopadloscian p)
            {
                DrawCuboid(g, p, xOffset, yOffset, scale, waterLevel);
            }
            else if (shape is Walec w)
            {
                DrawCylinder(g, w, xOffset, yOffset, scale, waterLevel);
            }
            else if (shape is Stozek s)
            {
                DrawCone(g, s, xOffset, yOffset, scale, waterLevel);
            }
            else if (shape is Kula k)
            {
                DrawSphere(g, k, xOffset, yOffset, scale, waterLevel);
            }
        }

        private void DrawCuboid(Graphics g, Prostopadloscian p, int xOffset, int yOffset, float scale, double waterLevel)
        {
            var originalTransform = g.Transform;

            g.TranslateTransform(xOffset, yOffset);
            g.ScaleTransform(1, -1);

            float height = (float)p.h * scale;
            float width = (float)p.w * scale;
            float depth = (float)p.d * scale;
            float baseY = (float)p.b * scale;

            float realWaterHeight = (float)waterLevel - (float)p.b;
            if (waterLevel == -1) // OVERFLOW
            {
                realWaterHeight = height;
            }
            if (realWaterHeight > height)
            {
                realWaterHeight = height;
            }
            float scaledWaterHeight = realWaterHeight * scale;

            // rysowanie ścian prostopadłościanu
            PointF[] front = {
                new PointF(0, baseY),
                new PointF(width, baseY),
                new PointF(width, baseY + height),
                new PointF(0, baseY + height)
            };

            PointF[] top = {
                new PointF(0, baseY + height),
                new PointF(depth, baseY + height + depth),
                new PointF(width + depth, baseY + height + depth),
                new PointF(width, baseY + height)
            };

            PointF[] side = {
                new PointF(width, baseY),
                new PointF(width + depth, baseY + depth),
                new PointF(width + depth, baseY + height + depth),
                new PointF(width, baseY + height)
            };

            g.FillPolygon(Brushes.Gray, front);
            g.FillPolygon(Brushes.DarkGray, top);
            g.FillPolygon(Brushes.DimGray, side);

            // rysowanie poziomu wody
            if (scaledWaterHeight > 0.098)
            {
                PointF[] waterFront = {
                    new PointF(0, baseY),
                    new PointF(width, baseY),
                    new PointF(width, Math.Min(baseY + height, baseY + scaledWaterHeight)),
                    new PointF(0, Math.Min(baseY + height, baseY + scaledWaterHeight))
                };

                PointF[] waterTop = {
                    new PointF(0, Math.Min(baseY+height,baseY + scaledWaterHeight)),
                    new PointF(depth, Math.Min(baseY+height + depth,baseY + scaledWaterHeight + depth)),
                    new PointF(width + depth, Math.Min(baseY + height + depth,baseY + scaledWaterHeight + depth)),
                    new PointF(width, Math.Min(baseY+height,baseY + scaledWaterHeight))
                };

                PointF[] waterSide = {
                    new PointF(width, baseY),
                    new PointF(width + depth, baseY + depth),
                    new PointF(width + depth, Math.Min(baseY + height+depth, baseY + scaledWaterHeight + depth)),
                    new PointF(width, Math.Min(baseY + height, baseY + scaledWaterHeight))
                };

                // rysowanie wody
                g.FillPolygon(Brushes.Blue, waterFront);
                g.FillPolygon(Brushes.Blue, waterTop);
                g.FillPolygon(Brushes.Blue, waterSide);

                g.DrawPolygon(Pens.LightBlue, waterFront);
                g.DrawPolygon(Pens.LightBlue, waterTop);
                g.DrawPolygon(Pens.LightBlue, waterSide);
            }

            g.DrawPolygon(Pens.Black, front);
            g.DrawPolygon(Pens.Black, top);
            g.DrawPolygon(Pens.Black, side);

            g.Transform = originalTransform;
        }

        private void DrawCylinder(Graphics g, Walec w, int xOffset, int yOffset, float scale, double waterLevel)
        {
            var originalTransform = g.Transform;

            g.TranslateTransform(xOffset, yOffset);
            g.ScaleTransform(1, -1);

            float radius = (float)w.r * scale;
            float height = (float)w.h * scale;
            float baseY = (float)w.b * scale;

            float realWaterHeight = (float)waterLevel - (float)w.b;
            if (waterLevel == -1) // OVERFLOW
            {
                realWaterHeight = height;
            }
            if (realWaterHeight > height)
            {
                realWaterHeight = height;
            }
            float scaledWaterHeight = realWaterHeight * scale;

            // walec jako prostokąt z elipsami

            // dolna część walca
            g.DrawEllipse(Pens.Black, -radius, baseY, radius * 2, radius / 2);
            
            // boczna część walca
            g.DrawRectangle(Pens.Black, -radius, baseY + radius / 4, radius * 2, height);
            g.FillRectangle(Brushes.Gray, -radius, baseY + radius / 4, radius * 2, height);
            
            // dolna część walca
            g.FillEllipse(Brushes.Gray, -radius, baseY, radius * 2, radius / 2);
            
            // górna część walca
            g.FillEllipse(Brushes.DarkGray, -radius, baseY + height, radius * 2, radius / 2);
            g.DrawEllipse(Pens.Black, -radius, baseY + height, radius * 2, radius / 2);

            // rysowanie poziomu wody
            if (scaledWaterHeight > 0.098)
            {
               float waterY = baseY + Math.Min(scaledWaterHeight, height);

                // ustawienie wysokości prostokąta wody tak, aby kończył się na połowie elipsy
                float waterRectHeight = waterY + radius / 4 - (baseY + radius / 4);

                // rysowanie prostokąta wody
                g.FillRectangle(Brushes.Blue, -radius, baseY + radius / 4, radius * 2, waterRectHeight);

                // dolna część wody
                g.FillEllipse(Brushes.Blue, -radius, baseY, radius * 2, radius / 2);

                // rysowanie elipsy wody
                g.FillEllipse(Brushes.Blue, -radius, waterY, radius * 2, radius / 2);
                g.DrawEllipse(Pens.LightBlue, -radius, waterY, radius * 2, radius / 2);
                
            }

            // górna część walca
            g.DrawEllipse(Pens.Black, -radius, baseY + height, radius * 2, radius / 2);

            g.Transform = originalTransform;
        }

        private void DrawCone(Graphics g, Stozek s, int xOffset, int yOffset, float scale, double waterLevel)
        {
            var originalTransform = g.Transform;

            g.TranslateTransform(xOffset, yOffset);
            g.ScaleTransform(1, -1);

            float radius = (float)s.r * scale;
            float height = (float)s.h * scale;
            float baseY = (float)s.b * scale;

            float realWaterHeight = (float)waterLevel - (float)s.b;
            if (waterLevel == -1) // OVERFLOW
            {
                realWaterHeight = height;
            }
            if (realWaterHeight > height)
            {
                realWaterHeight = height;
            }
            float scaledWaterHeight = realWaterHeight * scale;

            // boczna część stożka
            PointF[] coneSide = {
                new PointF(0, baseY + height),
                new PointF(-radius, baseY + radius / 4),
                new PointF(radius, baseY + radius / 4)
            };
            g.FillPolygon(Brushes.Gray, coneSide);
            g.DrawPolygon(Pens.Black, coneSide);

            // dolna część stożka
            g.FillEllipse(Brushes.DarkGray, -radius, baseY, radius * 2, radius / 2);
            g.DrawEllipse(Pens.Black, -radius, baseY, radius * 2, radius / 2);

            // rysowanie poziomu wody
            if (scaledWaterHeight > 0.098)
            {
                // dolna część stożka
                g.FillEllipse(Brushes.Blue, -radius, baseY, radius * 2, radius / 2);
                g.DrawEllipse(Pens.Black, -radius, baseY, radius * 2, radius / 2);

                if (waterLevel == -1)
                {
                    g.FillPolygon(Brushes.Blue, coneSide);
                }
                else
                {
                    float waterY = baseY + Math.Min(scaledWaterHeight, height);

                    // dostosowanie promienia elipsy na poziomie wody
                    float waterRadius = radius * (1 - (scaledWaterHeight / height));

                    // zabezpieczenie przed zbyt dużym promieniem wody
                    if (waterRadius > radius)
                    {
                        waterRadius = radius;
                    }

                    float dropFactor = scaledWaterHeight / height; // współczynnik obniżenia zależny od wysokości wody
                    float waterEllipseHeight = radius / 2 * (1 - dropFactor); // zmiana wysokości tylnej części elipsy dla bardziej realnego efektu

                    // minimalna wysokość elipsy
                    if (waterEllipseHeight < 0.01f)
                    {
                        waterEllipseHeight = 0.01f;
                    }

                    // wypełnienie wodą
                    g.FillPolygon(Brushes.Blue, new PointF[] {
                    new PointF(-radius, baseY + radius / 4), // dolny lewy punkt
                    new PointF(radius, baseY + radius / 4), // dolny prawy punkt
                    new PointF(waterRadius, waterY + waterEllipseHeight /2), // górny prawy punkt
                    new PointF(-waterRadius, waterY + waterEllipseHeight /2) // górny lewy punkt
                    });
                 
                    // elipsa jako wyznacznik poziomu wody
                    g.FillEllipse(Brushes.Blue, -waterRadius, waterY, waterRadius * 2, waterEllipseHeight);
                    g.DrawEllipse(Pens.LightBlue, -waterRadius, waterY, waterRadius * 2, waterEllipseHeight);
                }
            }
                
            g.Transform = originalTransform;
        }

        private void DrawSphere(Graphics g, Kula k, int xOffset, int yOffset, float scale, double waterLevel)
        {
            var originalTransform = g.Transform;

            g.TranslateTransform(xOffset, yOffset);
            g.ScaleTransform(1, -1);

            float radius = (float)k.r * scale;
            float baseY = (float)k.b * scale;
            float height = (float)k.r * 2 * scale;

            float realWaterHeight = (float)waterLevel - (float)k.b;
            if (waterLevel == -1) // OVERFLOW
            {
                realWaterHeight = radius * 2;
            }
            if (realWaterHeight > radius * 2)
            {
                realWaterHeight = radius * 2;
            }
            float scaledWaterHeight = realWaterHeight * scale;

            // rysowanie kuli
            g.FillEllipse(Brushes.Gray, -radius, baseY, radius * 2, radius * 2);
            g.DrawEllipse(Pens.Black, -radius, baseY, radius * 2, radius * 2);

            if (scaledWaterHeight > 0.098)
            {
                if (waterLevel == -1)
                {
                    g.FillEllipse(Brushes.Blue, -radius, baseY, radius * 2, radius * 2);
                    g.DrawEllipse(Pens.Black, -radius, baseY, radius * 2, radius * 2);
                }
                else
                {
                    g.FillEllipse(Brushes.Blue, -radius, baseY, radius * 2, radius * 2);

                    float waterY = baseY + Math.Min(scaledWaterHeight, radius * 2);

                    // dostosowywanie promienia elipsy odpowiadającej za wskazanie poziomu wody
                    float yFromCenter = Math.Abs(scaledWaterHeight - radius); 
                    float waterRadius = (float)Math.Sqrt(radius * radius - yFromCenter * yFromCenter);

                    // liczenie zmiennej wysokości elipsy
                    float dropFactor = scaledWaterHeight / height;
                    float waterEllipseHeight;
                    if ( waterY >= height/2)
                        waterEllipseHeight = radius / 2 * (1 - dropFactor);
                    else
                        waterEllipseHeight = radius / 2 * (dropFactor);

                    if (waterEllipseHeight < 0.01f)
                    {
                        waterEllipseHeight = 0.01f;
                    }
                    // wypełnienie w górę
                    g.FillRectangle(SystemBrushes.Control, -radius, waterY + waterEllipseHeight / 2, radius * 2, this.ClientSize.Height);

                    g.FillEllipse(Brushes.Blue, -waterRadius, waterY, waterRadius * 2, waterEllipseHeight);
                    g.DrawEllipse(Pens.LightBlue, -waterRadius, waterY, waterRadius * 2, waterEllipseHeight);
                    g.DrawEllipse(Pens.Black, -radius, baseY, radius * 2, radius * 2);
                }
            }

            g.Transform = originalTransform;
        }
    }
}