using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace MoetoOro
{
    public static class SkeletonPositionCorrector
    {
        /// <summary>
        /// Calculate the value of F[value] where F is a line between two points: (time,X cord at time), (time2, X cord at time2)
        /// </summary>
        /// <param name="skeleton">The array of skeletons</param>
        /// <param name="index">The index for which value is between t[index] and t[index+1] < </param>
        /// <param name="jointType">The type of skeleton joint for which the F will be calculated</param>
        /// <param name="t">The array of fetched times</param>
        /// <param name="value">The value for F[value] to be calculated</param>
        /// <returns></returns>
        public static float getFx(Skeleton[] skeleton, int index,JointType jointType, int[] t, float value)
        {
            float vi1 = skeleton[index].Joints[jointType].Position.X;
            float vi2 = skeleton[index+1].Joints[jointType].Position.X;
            return getF(vi1, vi2, t[index], t[index + 1], value);
        }

        /// <summary>
        /// Calculate the value of F[value] where F is a line between two points: (time,Y cord at time), (time2, Y cord at time2)
        /// </summary>
        /// <param name="skeleton">The array of skeletons</param>
        /// <param name="index">The index for which value is between t[index] and t[index+1] < </param>
        /// <param name="jointType">The type of skeleton joint for which the F will be calculated</param>
        /// <param name="t">The array of fetched times</param>
        /// <param name="value">The value for F[value] to be calculated</param>
        /// <returns></returns>
        public static float getFy(Skeleton[] skeleton, int index, JointType jointType, int[] t, float value)
        {
            float vi1 = skeleton[index].Joints[jointType].Position.Y;
            float vi2 = skeleton[index + 1].Joints[jointType].Position.Y;
            return getF(vi1, vi2, t[index], t[index + 1], value);
        }


        /// <summary>
        /// Calculate the value of F[value] where F is a line between two points: (time,Z cord at time), (time2, Z cord at time2)
        /// </summary>
        /// <param name="skeleton">The array of skeletons</param>
        /// <param name="index">The index for which value is between t[index] and t[index+1] < </param>
        /// <param name="jointType">The type of skeleton joint for which the F will be calculated</param>
        /// <param name="t">The array of fetched times</param>
        /// <param name="value">The value for F[value] to be calculated</param>
        /// <returns></returns>
        public static float getFz(Skeleton[] skeleton, int index, JointType jointType, int[] t, float value)
        {
            float vi1 = skeleton[index].Joints[jointType].Position.Z;
            float vi2 = skeleton[index + 1].Joints[jointType].Position.Z;
            return getF(vi1, vi2, t[index], t[index + 1], value);
        }

        /// <summary>
        /// Equation of line from two points (ti1,vi1) and (ti2,vi2)
        /// </summary>
        /// <param name="vi1"></param>
        /// <param name="vi2"></param>
        /// <param name="ti1"></param>
        /// <param name="ti2"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float getF(float vi1, float vi2, float ti1, float ti2, float t)
        {
            return ((vi2 - vi1) / (ti2 - ti1)) * (t - ti1) + vi1;
        }

        /// <summary>
        /// Method for correcting the positions of the skeletons based on the fetched time
        /// </summary>
        /// <param name="skeleton"></param>
        /// <param name="fetchedTime"></param>
        /// <returns></returns>
        public static Skeleton[] correctSkeletonPosition(Skeleton[] skeleton, long[] fetchedTime)
        {
            int n = skeleton.Length;
            //the time difference between the first and the current fetched time t
            int[] t = new int[n];
            t[0] = 0;
            for (int i = 1; i < n; i++)
                t[i] = (int)(fetchedTime[i] - fetchedTime[0]);
            //one period (in ms) of the corrected positions
            float period = t[n-1] / (float)(n - 1);
            //the corrected Skeleton data
            Skeleton[] correctedSkeleton = new Skeleton[n];
            //copy the skeleton data
            //the first and the last skeletons will stay the same
            for (int i = 0; i < n; i++)
                correctedSkeleton[i] = SkeletonClone.Clone(skeleton[i]);
            //the currenttime for which the skeleton positions will be calculated
            float currentTime = 0;
            //the current skeleton index (we will work with the skeleton[currentIndex] and skeleton[currentIndex+1]
            int currentIndex = 0;
            //calculate the remaining skeletons
            for (int k = 1; k < n - 1; k++)
            {
                //calculate the next time point
                currentTime += period;
                //calculate the next skeleton index
                while (currentTime >= t[currentIndex + 1])
                    currentIndex++;
                //create all the values for the joints
                foreach (JointType type in Enum.GetValues(typeof(JointType)))
                {
                    Joint currentJoint = correctedSkeleton[k].Joints[type];
                    SkeletonPoint point = new SkeletonPoint();
                    point.X = getFx(skeleton, currentIndex, type, t, currentTime);
                    point.Y = getFy(skeleton, currentIndex, type, t, currentTime);
                    point.Z = getFz(skeleton, currentIndex, type, t, currentTime);
                    currentJoint.Position = point;
                    correctedSkeleton[k].Joints[type] = currentJoint;
                }
            }
            return correctedSkeleton;
        }
    }
}
