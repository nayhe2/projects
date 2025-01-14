using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace cysterny
{
    class CysternaTester
    {
        private string filePath = "../../test.txt";

        public event Action<List<Test>> TestCompleted;
        public List<Test> testList=new List<Test>();
        public void RunTests()
        {
            string[] lines = File.ReadAllLines(filePath);
            int tests = int.Parse(lines[0]);

            int lineNumber = 1;

            for (int t = 0; t < tests; t++)
            {
                Kształt.nextX = 220;
                Test test = ReadTest(lines, ref lineNumber);
                test.Execute();
                testList.Add(test);
                
            }
            TestCompleted?.Invoke(testList);
        }
        private Test ReadTest(string[] lines, ref int lineNumber)
        {
            int figuresCount = int.Parse(lines[lineNumber]);
            lineNumber++;

            Test test = new Test();

            for (int i = 0; i < figuresCount; i++)
            {
                string[] figureInfo = lines[lineNumber].Split(' ');
                ValidateParameters(figureInfo);

                Kształt figura = CreateShape(figureInfo);
                if (figura is Prostopadloscian prostopadloscian)
                    Kształt.nextX += prostopadloscian.w + 300;
                else if (figura is Stozek stozek)
                    Kształt.nextX += 2 * stozek.r + 300;
                else if (figura is Walec walec)
                    Kształt.nextX += 2 * walec.r + 300;
                else if (figura is Kula kula)
                    Kształt.nextX += 2 * kula.r + 300;
               
                test.AddShape(figura);
                lineNumber++;
            }

            test.expectedVolume = double.Parse(lines[lineNumber]);
            lineNumber++;

            return test;
        }

        private void ValidateParameters(string[] figureInfo)
        {
            switch (figureInfo[0][0])
            {
                case 'p':
                    if (figureInfo.Length != 5)
                        throw new ArgumentException("Nieodpowiednia ilość parametrów dla prostopadłościanu.");
                    break;
                case 'w':
                    if (figureInfo.Length != 4)
                        throw new ArgumentException("Nieodpowiednia ilość parametrów dla walca.");
                    break;
                case 's':
                    if (figureInfo.Length != 4)
                        throw new ArgumentException("Nieodpowiednia ilość parametrów dla stożka.");
                    break;
                case 'k':
                    if (figureInfo.Length != 3)
                        throw new ArgumentException("Nieodpowiednia ilość parametrów dla kuli.");
                    break;
                default:
                    throw new ArgumentException("Nieznany typ figury");
            }
        }

        private Kształt CreateShape(string[] figureInfo)
        {
            switch (figureInfo[0][0])
            {
                case 'p':
                    return new Prostopadloscian(double.Parse(figureInfo[1]), double.Parse(figureInfo[2]), double.Parse(figureInfo[3]), double.Parse(figureInfo[4]));
                case 'w':
                    return new Walec(double.Parse(figureInfo[1]), double.Parse(figureInfo[2]), double.Parse(figureInfo[3]));
                case 's':
                    return new Stozek(double.Parse(figureInfo[1]), double.Parse(figureInfo[2]), double.Parse(figureInfo[3]));
                case 'k':
                    return new Kula(double.Parse(figureInfo[1]), double.Parse(figureInfo[2]));
                default:
                    throw new ArgumentException("Nieznany typ figury");
            }
        }
    }
}