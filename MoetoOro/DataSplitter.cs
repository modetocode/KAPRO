using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoetoOro
{
    //splits the data into equal intervals (bars)
    public class DataSplitter
    {
        public static int[] splitDataIndexes(int dataCount,int barsCount)
        {
            int n = dataCount;
            int mod = n % barsCount;
            int[] startIndex = new int[barsCount + 1];
            startIndex[0] = 0;
            startIndex[barsCount] = n;
            for (int i = 1; i < barsCount; i++)
            {
                startIndex[i] = startIndex[i - 1] + n / barsCount;
                if (i <= mod)
                    startIndex[i]++;
            }
            return startIndex;
        }
    }
}
