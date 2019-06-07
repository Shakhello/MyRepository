using ShapeArea.Interface;
using System;

namespace ShapeArea.Model
{
    /// <summary>
    /// Круг определяется его радиусом
    /// </summary>
    public class Circle : IShape
    {
        public double Radius { get; set; }
        public Circle(double radius)
        {
            if(radius <= 0)
            {
                throw new ArgumentOutOfRangeException("radius", "Радиус должен быть больше 0");
            }
            Radius = radius;
        }

        public double GetArea()
        {
            return Math.PI * Math.Pow(Radius, 2);
        }
    }
}
