using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PollMethodUsingThreads
{
    public class Scalling
    {
        //scale and return the data with newDataCount length
        public static double[] scale(double[] data, int newDataCount)
        {
            int n = data.Length;
            double lengthInt = (n-1)/(double)(newDataCount-1);
            double[,] line=new double[n-1,2];
            for (int i = 0; i < n - 1; i++)
            {
                line[i, 0] = data[i + 1] - data[i];
                line[i, 1] = (-i) * (data[i+1] - data[i]) + data[i];
                //Console.WriteLine(line[i, 0] + " " + line[i, 1] + " " + (line[i, 0] * i + line[i, 1]) + " " + (line[i, 0] * (i + 1) + line[i, 1]));
            }
            double currentInt = 0.0;
            double[] newData = new double[newDataCount];
            int ind=0;
            while (currentInt < n)
            {
                if (currentInt >= (double)(n-1))
                    break;
                int currentLine = (int)Math.Floor(currentInt);
                newData[ind++]=line[currentLine, 0] * currentInt + line[currentLine, 1];
                currentInt += lengthInt;
            }
            return newData;
        }
    }
}
