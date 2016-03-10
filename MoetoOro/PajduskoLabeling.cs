using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoetoOro
{
    public class PajduskoLabeling
    {
        private static int barCount = 5;
        private static double[] groupRatio = { 1.0 / 2.0, 1.0 / 2.0 };
        private double[] FVD { get; set; }
        private double[] FHFD { get; set; }
        private double[] CTSM { get; set; }
        public PajduskoLabeling(double[] FVD, double[] FHFD, double[] CTSM)
        {
            this.FVD = FVD;
            this.FHFD = FHFD;
            this.CTSM = CTSM;
        }

        private static int differentTypesCount = 4;
        /*Labels: 
        1 period FVD+CTSM
        2 period FVD+CTSM
        3 period FHFD+CTSM
        4 period FVD+FHFD
        5 period FVD+FHFD
        
        */
        private int getLabel(double CTSM1, double CTSM2, double FVD1, double FVD2, double FHFD1, double FHFD2, int barNo, int partNo)
        {
            int k = 4;
            if (barNo == 0 || barNo == 1)
            {
                double difCTSM = CTSM2 - CTSM1;
                double difFVD = FVD2 - FVD1;
                if (difCTSM > 0)
                {
                    if (difFVD > 0)
                        return partNo * k;
                    else
                        return partNo * k + 1;
                }
                else
                {
                    if (difFVD > 0)
                        return partNo * k + 2;
                    else
                        return partNo * k + 3;
                }

            }
            else if (barNo == 2)
            {
                double difCTSM = CTSM2 - CTSM1;
                double difFHFD = FHFD2 - FHFD1;
                if (difCTSM > 0)
                {
                    if (difFHFD > 0)
                        return partNo * k;
                    else
                        return partNo * k + 1;
                }
                else
                {
                    if (difFHFD > 0)
                        return partNo * k + 2;
                    else
                        return partNo * k + 3;
                }
            }
            else if (barNo == 3 || barNo == 4)
            {
                double difFVD = FVD2 - FVD1;
                double difFHFD = FHFD2 - FHFD1;
                if (difFVD > 0)
                {
                    if (difFHFD > 0)
                        return partNo * k;
                    else
                        return partNo * k + 1;
                }
                else
                {
                    if (difFHFD > 0)
                        return partNo * k + 2;
                    else
                        return partNo * k + 3;
                }
            }
            else
                return -1;
        }

        public int[] calculateLabels(int originalDataSize)
        {
            int[] labels = new int[FHFD.Length - 1];
            int[] barIndex = DataSplitter.splitDataIndexes(FHFD.Length, barCount);
            for (int i = 0; i < barCount; i++)
            {
                //Console.WriteLine("Interval [ " + barIndex[i] + " , " + barIndex[i + 1] + " ]");
                int[] barPartIndex = DataBarSplitter.splitDataIndexes(barIndex[i + 1] - barIndex[i], groupRatio);
                for (int k = 0; k < groupRatio.Length; k++)
                {
                    //Console.WriteLine("Subinterval [ " + (barIndex[i] + barPartIndex[k]) + " , " + (barIndex[i] + barPartIndex[k + 1]) + " ]");
                    for (int j = barPartIndex[k]; j < barPartIndex[k + 1]; j++)
                    {
                        //Console.WriteLine((barIndex[i] + j) + " " + FHFD[barIndex[i] + j] + " " + FVD[barIndex[i] + j] + " " + CTSM[barIndex[i] + j]);

                        if ((barIndex[i] + j) < FHFD.Length - 1)
                        {
                            if (i == 0 || i == 1)
                            {
                                labels[barIndex[i] + j] = getLabel(CTSM[barIndex[i] + j], CTSM[barIndex[i] + j + 1], FVD[barIndex[i] + j], FVD[barIndex[i] + j + 1], 0.0, 0.0, i, k);
                            }
                            else if (i == 2)
                            {
                                labels[barIndex[i] + j] = getLabel(CTSM[barIndex[i] + j], CTSM[barIndex[i] + j + 1], 0.0, 0.0, FHFD[barIndex[i] + j], FHFD[barIndex[i] + j + 1], i, k);
                            }
                            else
                            {
                                labels[barIndex[i] + j] = getLabel(0.0, 0.0, FVD[barIndex[i] + j], FVD[barIndex[i] + j + 1], FHFD[barIndex[i] + j], FHFD[barIndex[i] + j + 1], i, k);

                            }
                        }
                    }
                }
            }
            return labels;
        }

        public static int getTotalTypeLabels()
        {
            return differentTypesCount * groupRatio.Length;
        }

        public static int getTotalBars()
        {
            return barCount;
        }
        public static double[] getGroupRatio()
        {
            return groupRatio;
        }
        public static int getGroupRatioLength()
        {
            return groupRatio.Length;
        }
    }
}
