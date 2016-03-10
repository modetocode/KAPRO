using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace MoetoOro
{

    /// <summary>
    /// Class for calculating the characteristic attributes
    /// </summary>
    class CharacteristicAttributesCalculator
    {

        /// <summary>
        /// The skeleton data
        /// </summary>
        private Skeleton[] skeletonData;

        public CharacteristicAttributesCalculator(Skeleton[] skeletonData)
        {
            this.skeletonData = skeletonData;
        }


        /// <summary>
        /// TSM calculation
        /// </summary>
        /// <returns></returns>
        public double[] calculateTorsoSideMovement()
        {
            double[] TSM = new double[skeletonData.Length];
            TSM[0] = 0.0;
            for (int i = 1; i < skeletonData.Length; i++)
            {
                SkeletonPoint hipRightPoint = skeletonData[i-1].Joints[JointType.HipRight].Position;
                SkeletonPoint hipLeftPoint = skeletonData[i-1].Joints[JointType.HipLeft].Position;
                SkeletonPoint hipCenterPoint = skeletonData[i].Joints[JointType.HipCenter].Position;
                SkeletonPoint previousHipCenterPoint = skeletonData[i-1].Joints[JointType.HipCenter].Position;
                SkeletonPoint vector1 = VectorCalculations.createVector(previousHipCenterPoint, hipCenterPoint);
                SkeletonPoint vector2 = VectorCalculations.createVector(hipLeftPoint, hipRightPoint);
                if (VectorCalculations.calculateNorm(vector2) != 0)
                    TSM[i] = VectorCalculations.calculateScallarProduct(vector1, vector2) / VectorCalculations.calculateNorm(vector2);
                else
                    TSM[i] = 0.0;
            }
            return TSM;
        }

        /// <summary>
        /// calculates the FHSD attribute
        /// </summary>
        /// <returns></returns>
        public double[] calculateFeetHorizontalSideDistance()
        {
            double[] FHSD = new double[skeletonData.Length];
            for (int i = 0; i < skeletonData.Length; i++)
            {
                SkeletonPoint hipRightPoint = skeletonData[i].Joints[JointType.HipRight].Position;
                SkeletonPoint hipLeftPoint = skeletonData[i].Joints[JointType.HipLeft].Position;
                SkeletonPoint ankleRightPoint = skeletonData[i].Joints[JointType.AnkleRight].Position;
                SkeletonPoint ankleLeftPoint = skeletonData[i].Joints[JointType.AnkleLeft].Position;
                SkeletonPoint vector1 = VectorCalculations.createVector(ankleLeftPoint, ankleRightPoint);
                SkeletonPoint vector2 = VectorCalculations.createVector(hipLeftPoint, hipRightPoint);
                if (VectorCalculations.calculateNorm(vector2) != 0)
                    FHSD[i] = VectorCalculations.calculateScallarProduct(vector1, vector2) / VectorCalculations.calculateNorm(vector2);
                else
                    FHSD[i] = 0.0;
            }
            return FHSD;
        }


        /// <summary>
        /// calculates the FVD attribute
        /// </summary>
        /// <returns></returns>
        public double[] calculateFeetVerticalDistance()
        {
            double[] FVD = new double[skeletonData.Length];
            for (int i = 0; i < skeletonData.Length; i++)
            {
                SkeletonPoint ankleRightPoint = skeletonData[i].Joints[JointType.AnkleRight].Position;
                SkeletonPoint ankleLeftPoint = skeletonData[i].Joints[JointType.AnkleLeft].Position;
                SkeletonPoint shoulderCenterPoint = skeletonData[i].Joints[JointType.ShoulderCenter].Position;
                SkeletonPoint spinePoint = skeletonData[i].Joints[JointType.Spine].Position;
                SkeletonPoint vector1 = VectorCalculations.createVector(ankleLeftPoint, ankleRightPoint);
                SkeletonPoint vector2 = VectorCalculations.createVector(spinePoint, shoulderCenterPoint);
                if (VectorCalculations.calculateNorm(vector2) != 0.0)
                    FVD[i] = VectorCalculations.calculateScallarProduct(vector1, vector2) / VectorCalculations.calculateNorm(vector2);
                else
                    FVD[i] = 0.0;
            }
            return FVD;
        }


        /// <summary>
        /// Calculates the CTSM attribute
        /// </summary>
        /// <returns></returns>
        public double[] calculateCumulativeTorsoSideMovement()
        {
            double[] CTSM = new double[skeletonData.Length];
            double[] TSM = calculateTorsoSideMovement();
            CTSM[0] = TSM[0];
            for (int i = 1; i < skeletonData.Length; i++)
            {
                CTSM[i] = CTSM[i - 1] + TSM[i];
            }
            return CTSM;
        }


        /// <summary>
        /// calculates the FHFD attribute
        /// </summary>
        /// <returns></returns>
        public double[] calculateFeetHorizontalFrontDistance()
        {
            double[] FHFD = new double[skeletonData.Length];
            for (int i = 0; i < skeletonData.Length; i++)
            {
                SkeletonPoint hipRightPoint = skeletonData[i].Joints[JointType.HipRight].Position;
                SkeletonPoint hipLeftPoint = skeletonData[i].Joints[JointType.HipLeft].Position;
                SkeletonPoint ankleRightPoint = skeletonData[i].Joints[JointType.AnkleRight].Position;
                SkeletonPoint ankleLeftPoint = skeletonData[i].Joints[JointType.AnkleLeft].Position;
                SkeletonPoint hipCenterPoint = skeletonData[i].Joints[JointType.HipCenter].Position;
                SkeletonPoint spinePoint = skeletonData[i].Joints[JointType.Spine].Position;
                SkeletonPoint vector1 = VectorCalculations.createVector(ankleLeftPoint, ankleRightPoint);
                SkeletonPoint vector2 = VectorCalculations.calculateVectorProduct(VectorCalculations.createVector(hipLeftPoint, hipRightPoint), VectorCalculations.createVector(hipCenterPoint, spinePoint));
                if (VectorCalculations.calculateNorm(vector2) != 0)
                    FHFD[i] = VectorCalculations.calculateScallarProduct(vector1, vector2) / VectorCalculations.calculateNorm(vector2);
                else
                    FHFD[i] = 0.0;
            }
            return FHFD;
        }
    }
}
