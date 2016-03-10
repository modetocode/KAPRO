using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoetoOro
{
    public class Centroid
    {
        /// <summary>
        /// the x coordinate of the centroid (FHSD value)
        /// </summary>
        public double x { get; set; } 
        /// <summary>
        /// the y coordinate of the centroid (FVD value)
        /// </summary>
        public double y { get; set; }
        /// <summary>
        /// the z coordinate of the centroid (FHFD value)
        /// </summary>
        public double z { get; set; }
        /// <summary>
        /// the index of the centroid
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// the description (what the centroid means)
        /// </summary>
        public string description { get; set; }

        public Centroid(double x, double y, double z, int index, string description)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.index = index;
            this.description = description;
        }
    }
    public static class CentroidSelector
    {
        //centroids for Ramnoto and 6n3n
        public static Centroid[] centroids1 = { new Centroid(0.05   ,   0  ,   0   ,   0   ,  "the dancer is standing in place"), 
                                        new Centroid(0.25   ,   0   ,  0   ,   1   ,  "the dancer is spreding his legs a little"),    
                                        new Centroid(0.35   ,   0   ,  0   ,   2  ,  "the dancer is spreding his legs"),    
                                        new Centroid(-0.25  ,   0   ,  0   ,   3   ,  "the dancer is crossing his legs a little"), 
                                        new Centroid(-0.35  ,   0   ,  0   ,   4   ,  "the dancer is crossing his legs"),    
                                        new Centroid(0.05   , 0.15  ,  0   ,   5   ,  "the dancer is raising his right leg a little"), 
                                        new Centroid(0.05   , 0.20  ,  0   ,   6   ,  "the dancer is raising his right leg"),    
                                        new Centroid(0.05   , -0.15 ,  0   ,   7   ,  "the dancer is raising his left leg a little"), 
                                        new Centroid(0.05   , -0.20 ,  0   ,   8   ,  "the dancer is raising his left leg"),  
                                        new Centroid(0.05   ,   0   , 0.2  ,   9   ,  "the dancer is putting his left leg forward a little (or right leg backward)"), 
                                        new Centroid(0.05   ,   0   , 0.3  ,  10   ,  "the dancer is putting his left leg forward (or right leg backward)"),  
                                        new Centroid(0.05   ,   0   , -0.2 ,  11   ,  "the dancer is putting his right leg forward a little (or left leg backward)"), 
                                        new Centroid(0.05   ,   0   , -0.3 ,  12   ,  "the dancer is putting his right leg forward (or left leg backward)"),  
                                        new Centroid(0.05    , 0.25  ,-0.25 ,  13   ,  "the dancer is raising his right leg forward while moving"), 
                                        new Centroid(0.05    , -0.25 , 0.25 ,  14   ,  "the dancer is raising his left leg forward while moving"),  
                                      };
        //centroids for Pajdusko dance
        public static Centroid[] centroidsPajdusko = { new Centroid(0.1   ,   0  ,   0   ,   0   ,  "the dancer is standing in place"), 
                                        new Centroid(0.35   ,   0   ,  0   ,   1   ,  "the dancer is spreding his legs a little"),    
                                        new Centroid(0.45   ,   0   ,  0   ,   2  ,  "the dancer is spreding his legs"),    
                                        new Centroid(-0.35  ,   0   ,  0   ,   3   ,  "the dancer is crossing his legs a little"), 
                                        new Centroid(-0.45  ,   0   ,  0   ,   4   ,  "the dancer is crossing his legs"),    
                                        new Centroid(0.10   , 0.10  ,  0   ,   5   ,  "the dancer is raising his right leg a little"), 
                                        new Centroid(0.10   , 0.20  ,  0   ,   6   ,  "the dancer is raising his right leg"),    
                                        new Centroid(0.10   , -0.10 ,  0   ,   7   ,  "the dancer is raising his left leg a little"), 
                                        new Centroid(0.10   , -0.20 ,  0   ,   8   ,  "the dancer is raising his left leg"),  
                                        new Centroid(0.05   ,   0   , 0.3  ,   9   ,  "the dancer is putting his left leg forward a little (or right leg backward)"), 
                                        new Centroid(0.05   ,   0   , 0.4  ,  10   ,  "the dancer is putting his left leg forward (or right leg backward)"),  
                                        new Centroid(0.05   ,   0   , -0.3 ,  11   ,  "the dancer is putting his right leg forward a little (or left leg backward)"), 
                                        new Centroid(0.05   ,   0   , -0.4 ,  12   ,  "the dancer is putting his right leg forward (or left leg backward)"),  
                                        new Centroid(0.2    ,  0.2  , -0.25 ,  13   ,  "the dancer is raising his right leg forward while moving"), 
                                        new Centroid(0.2    ,  -0.2 , 0.25 ,  14   ,  "the dancer is raising his left leg forward while moving"),  
                                      };
        public static Centroid getClosestCentroid(double FHSD,double FVD, double FHFD, DanceType danceType)
        {
            if (!(danceType.Equals(DanceType.O6n3n)||danceType.Equals(DanceType.Pajdusko)||danceType.Equals(DanceType.Ramnoto)))
                return null;
            Centroid pointCentroid=new Centroid(FHSD,FVD,FHFD,-1,"");
            Centroid[] centroids;
            if (danceType.Equals(DanceType.O6n3n) || danceType.Equals(DanceType.Ramnoto))
            {
                centroids = centroids1;
            }
            else
                centroids = centroidsPajdusko;
            Centroid closestCentroid= centroids[0];
            double closestDistance= calculateCentroidsDistance(pointCentroid,centroids[0]);
            for (int i = 1; i < centroids.Length; i++)
            {
                if(calculateCentroidsDistance(pointCentroid,centroids[i])<closestDistance){
                    closestDistance = calculateCentroidsDistance(pointCentroid, centroids[i]);
                    closestCentroid = centroids[i];
                }
            }
            return closestCentroid;
        }

        static double calculateCentroidsDistance(Centroid centroid1, Centroid centroid2)
        {
            return Math.Sqrt((centroid2.x - centroid1.x) * (centroid2.x - centroid1.x) + (centroid2.y - centroid1.y) * (centroid2.y - centroid1.y) + (centroid2.z - centroid1.z) * (centroid2.z - centroid1.z));
        
        }
    }

    public static class CalculateLabels
    {
        public static int[] calculateLabels(double[] FHSD, double[] FVD, double[] FHFD, DanceType danceType)
        {
            int[] labels = new int[FHSD.Length];
            for (int i = 0; i < FHSD.Length; i++)
                labels[i] = CentroidSelector.getClosestCentroid(FHSD[i], FVD[i], FHFD[i], danceType).index;
            return labels;
        }
    }

}
