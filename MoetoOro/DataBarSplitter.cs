using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoetoOro
{
    //split the data into one bar
    public class DataBarSplitter
    {
        public static int[] splitDataIndexes(int dataBarCount, double[] accentRatio)
        {
            int n = accentRatio.Length;
            int dataLeftCount = dataBarCount;
            int[] startIndex = new int[n + 1];
            startIndex[0] = 0;
            startIndex[n] = dataBarCount;
            for (int i = 1; i < n; i++)
            {
                startIndex[i] = startIndex[i-1]+(int)Math.Round(dataBarCount * accentRatio[i-1]);
                dataLeftCount = dataLeftCount - (int)Math.Round(dataBarCount * accentRatio[i - 1]);
            }
            return startIndex;
        }
    }
}
