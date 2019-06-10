using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeArea.Model;
using System;

namespace UnitTestShapeStrategy
{
    [TestClass]
    public class ShapeTest
    {
        [TestMethod]
        public void CircleAreaTest()
        {
            var circle = new Circle(3);
            Assert.AreEqual(Math.PI * 3 * 3, circle.GetArea());
        }
        [TestMethod]
        public void CircleInvalidParamsTest()
        {
            var isEx = false;
            try
            {
                var circle = new Circle(-3);
            }
            catch (ArgumentOutOfRangeException)
            {
                isEx = true;
            }
            Assert.IsTrue(isEx);
        }
        [TestMethod]
        public void TriangleAreaTest()
        {
            var triangle = new Triangle(3, 4, 5);
            Assert.AreEqual(6, triangle.GetArea());
        }
        [TestMethod]
        public void TriangleRightTest()
        {
            var triangle = new Triangle(3, 4, 5);
            Assert.IsTrue(triangle.IsRight());
        }
        [TestMethod]
        public void TriangleInvalidParamsTest()
        {
            var isEx = false;
            try
            {
                var circle = new Triangle(-3, 4, 5);
            }
            catch (ArgumentOutOfRangeException)
            {
                isEx = true;
            }
            Assert.IsTrue(isEx);
        }
    }
}
