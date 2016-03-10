using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MoetoOro
{
 /// <summary>
 /// Class for cloning one skeleton
 /// </summary>
    static class SkeletonClone
    {
        /// <summary>
        /// Cloning the skeleton data
        /// </summary>
        /// <param name="skOrigin"></param>
        /// <returns></returns>
        public static Skeleton Clone(this Skeleton skOrigin)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(ms, skOrigin);

            ms.Position = 0;
            object obj = bf.Deserialize(ms);
            ms.Close();

            return obj as Skeleton;
        }
    }


    /// <summary>
    /// Class for storing the gathered skeleton data of the tracked user
    /// </summary>
    class KinectMovementDataGatherer
    {
        /// <summary>
        /// The skeleton data is stored in this variable
        /// </summary>
        private Skeleton[] skeletonData { get; set; }

        /// <summary>
        /// The time when a skeleton data is fetched is stored here (for every frame)
        /// </summary>
        private long[] fetchedTime { get; set; }

        /// <summary>
        /// The maximal number of data when the gathering automatically stops
        /// </summary>
        private const int maxDataCount = 300;

        /// <summary>
        /// The current number of stored data
        /// </summary>
        private int currentDataCount;

        /// <summary>
        /// The id of the tracked skeleton (tracked user) 
        /// </summary>
        private int trackedSkeletonId { get; set; }


        public KinectMovementDataGatherer()
        {
            skeletonData = new Skeleton[maxDataCount];
            fetchedTime = new long[maxDataCount];
            currentDataCount=0;
        }

        /// <summary>
        /// Function that checks if the maximal data count has been reached
        /// </summary>
        /// <returns></returns>
        public bool isDataGatheringConditionTrue(Skeleton skeleton)
        {
            if (currentDataCount >= maxDataCount)
                return false;
            return true;
            
        }


        /// <summary>
        /// Function that checks if the dancer has made the correct arms pose to make sure when to stop collecting Skeleton data
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        public bool isEndConditionTrue(Skeleton skeleton)
        {
            /*
            //check the pose of the dancer's arms
            SkeletonPoint elbowRightPoint = skeleton.Joints[JointType.ElbowRight].Position;
            SkeletonPoint elbowLeftPoint = skeleton.Joints[JointType.ElbowLeft].Position;
            SkeletonPoint handRightPoint = skeleton.Joints[JointType.HandRight].Position;
            SkeletonPoint handLeftPoint = skeleton.Joints[JointType.HandLeft].Position;
            SkeletonPoint shoulderRightPoint = skeleton.Joints[JointType.ShoulderRight].Position;
            SkeletonPoint shoulderLeftPoint = skeleton.Joints[JointType.ShoulderLeft].Position;
            //first the hand must be upper than the hand elbow
            if (handRightPoint.Y < elbowRightPoint.Y)
                return false;
            if (handLeftPoint.Y < elbowLeftPoint.Y)
                return false;
            //second the arms should be aside from the shoulders
            if (handRightPoint.X < shoulderRightPoint.X)
                return false;
            if (handLeftPoint.X > shoulderLeftPoint.X)
                return false;
            //the user has made the gesture
             
            return true;*/
            return false;
        }
        /// <summary>
        /// Adds the skeleton data in the array of skeleton data. 
        /// </summary>
        /// <param name="skeleton"></param>
        public void addSkeletonData(Skeleton skeleton,long fetchedTime){
            skeletonData[currentDataCount] = SkeletonClone.Clone(skeleton);
            this.fetchedTime[currentDataCount] = fetchedTime;
            currentDataCount++;
            if (currentDataCount == 1)
            {
                trackedSkeletonId = skeletonData[0].TrackingId;
            }
        }

       
        /// <summary>
        /// Removes the stored skeleton data so far
        /// </summary>
        public void clearSkeletonData()
        {
            currentDataCount = 0;
        }

        /// <summary>
        /// Return the skeleton data count
        /// </summary>
        /// <returns></returns>
        public int getSkeletonDataCount()
        {
            return currentDataCount;
        }

        /// <summary>
        /// Returns the current tracked skeleton
        /// </summary>
        /// <returns></returns>
        public int getTrackedSkeletonId()
        {
            return trackedSkeletonId;
        }


        /// <summary>
        /// Return an array containing the skeleton data
        /// </summary>
        /// <returns></returns>
        public Skeleton[] getSkeletonData()
        {
            Skeleton[] returnedSkeletonData = new Skeleton[currentDataCount];
            for (int i = 0; i < currentDataCount; i++)
                returnedSkeletonData[i] = skeletonData[i];
            return returnedSkeletonData;
        }

        /// <summary>
        /// Returns an array containing the skeleton data but corrected according to the fetched time
        /// </summary>
        /// <returns></returns>
        public Skeleton[] getCorrectedSkeletonData()
        {
            return SkeletonPositionCorrector.correctSkeletonPosition(getSkeletonData(), fetchedTime);
        }


        /// <summary>
        /// Returns an array containing all the fetched times of the gathering of skeleton data
        /// </summary>
        /// <returns></returns>
        public long[] getFetchedTimeArray()
        {
            long[] returnFetchedTime = new long[currentDataCount];
            for (int i = 0; i < currentDataCount; i++)
                returnFetchedTime[i] = fetchedTime[i];
            return returnFetchedTime;
        }


        /// <summary>
        /// Returns a skeleton for a given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Skeleton getSkeleton(int index)
        {
            return skeletonData[index];
        }


    }
}
