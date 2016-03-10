using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PollMethodUsingThreads
{
    //Output
    // 0: dvizenje desno i rashiruvanje noze
    // 1: dvizenje desno i zblizuvanje noze
    // 2: dvizenje levo i rashiruvanje noze
    // 3: dvizenje levo i zblizuvanje noze

    public class RamnotoHMMOutputGenerator
    {
        public static int[] generateHMMOutput(double[] horizontalDistanceData, double[] bodyMovementData)
        {
            int parts = 3;
            Console.WriteLine(bodyMovementData.Length);
            int[] ouput = new int[parts];
            int n = bodyMovementData.Length;
            int mod = n % parts;
            int[] startIndex = new int[parts+1];
            startIndex[0]=0;
            startIndex[parts] = n;
            for (int i = 1; i < parts; i++)
            {
                startIndex[i] = startIndex[i - 1] + n / parts;
                if (i <= mod)
                    startIndex[i]++;       
            }
            for (int i = 0; i < parts; i++)
            {
                Console.WriteLine("["+startIndex[i]+" "+(startIndex[i+1]-1)+"]");
                int start = startIndex[i];
                int end=startIndex[i+1]-1;
                for (int j = start; j < end; j++)
                {
                    Console.Write((j+1)+" "+horizontalDistanceData[j] + " " + bodyMovementData[j]+" ");
                    double difHorizontalDistance = horizontalDistanceData[j + 1] - horizontalDistanceData[j];
                    double difBodyMovement = bodyMovementData[j + 1] - bodyMovementData[j];
                    if (difHorizontalDistance >= 0 && difBodyMovement >= 0)
                    {
                        Console.WriteLine("0");
                    } else
                    if (difHorizontalDistance < 0 && difBodyMovement >=0 )
                    {
                         Console.WriteLine("1");
                    } else
                        if (difHorizontalDistance >= 0 && difBodyMovement < 0)
                        {
                            Console.WriteLine("2");
                        }
                        else
                        {
                            Console.WriteLine("3");
                        }

                }
                Console.WriteLine((end+1)+" "+horizontalDistanceData[end] + " " + bodyMovementData[end]);
            }
            return null;
        }
    }
}
