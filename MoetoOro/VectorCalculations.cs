using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace MoetoOro
{
    /// <summary>
    /// Class containing all the Calculation methods for known geometric terms
    /// </summary>
    static class VectorCalculations
    {
        /// <summary>
        /// Method for vector product calculation
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static SkeletonPoint calculateVectorProduct(SkeletonPoint vector1, SkeletonPoint vector2)
        {
            SkeletonPoint result = new SkeletonPoint();
            result.X = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
            result.Y = vector1.Z * vector2.X - vector1.X * vector2.Z;
            result.Z = vector1.X * vector2.Y - vector1.Y * vector2.X;
            return result;
        }

        /// <summary>
        /// Method for scallar product calculation
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static double calculateScallarProduct(SkeletonPoint point1, SkeletonPoint point2)
        {
            return (point1.X * point2.X) + (point1.Y * point2.Y) + (point1.Z * point2.Z);
        }

        /// <summary>
        /// Method for calculating the euclediean distance between two points
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static double calculateEuclideanDistance(SkeletonPoint point1, SkeletonPoint point2)
        {
            return Math.Sqrt((point1.X - point2.X) * (point1.X - point2.X) + (point1.Y - point2.Y) * (point1.Y - point2.Y) + (point1.Z - point2.Z) * (point1.Z - point2.Z));
        }

        /// <summary>
        /// Method for vector norm calculation
        /// </summary>
        /// <param name="vektor"></param>
        /// <returns></returns>
        public static double calculateNorm(SkeletonPoint vector)
        {
            return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }

        /// <summary>
        /// Method for calculating the angle between two line segments for given three points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static double angleBetweenPoints(SkeletonPoint p1, SkeletonPoint p2, SkeletonPoint p3)
        {
            SkeletonPoint v1 = createVector(p2, p1);
            SkeletonPoint v2 = createVector(p2, p3);
            return Math.Acos(calculateScallarProduct(v1, v2) / (calculateNorm(v1) * calculateNorm(v2))) * 180 / Math.PI;
        }

        /// <summary>
        /// Method for vector creation for given two points. Creates a vector which starts from (0,0,0) position
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static SkeletonPoint createVector(SkeletonPoint point1, SkeletonPoint point2)
        {
            SkeletonPoint vector = new SkeletonPoint();
            vector.X = point2.X - point1.X;
            vector.Y = point2.Y - point1.Y;
            vector.Z = point2.Z - point1.Z;
            return vector;
        }
    }
}
