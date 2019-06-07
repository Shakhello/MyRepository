using ShapeArea.Interface;
using System;

namespace ShapeArea.Model
{
    /// <summary>
    /// Треугольник определяется по длине трех сторон
    /// </summary>
    public class Triangle : IShape
    {
        private const string lengthExMsg = "Длина должна быть больше 0";
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public Triangle(double a, double b, double c)
        {
            if (a <= 0)
            {
                throw new ArgumentOutOfRangeException("a", lengthExMsg);
            }
            else if (b <= 0)
            {
                throw new ArgumentOutOfRangeException("b", lengthExMsg);
            }
            else if (c <= 0)
            {
                throw new ArgumentOutOfRangeException("c", lengthExMsg);
            }
            A = a;
            B = b;
            C = c;
        }

        public double GetArea()
        {
            double p = (A + B + C) / 2;
            return Math.Pow(p * (p - A) * (p - B) * (p - C), 0.5);
        }

        public bool IsRight()
        {
            if (C >= A && C >= B)
            {
                return IsPifagor(A, B, C);
            }
            else if (B >= A && B >= C)
            {
                return IsPifagor(A, C, B);
            }
            else
            {
                return IsPifagor(B, C, A);
            }
        }

        private bool IsPifagor(double cathetus1, double cathetus2, double hypotenuse)
        {
            return Math.Pow(hypotenuse, 2) == Math.Pow(cathetus1, 2) + Math.Pow(cathetus2, 2);
        }
    }
}
