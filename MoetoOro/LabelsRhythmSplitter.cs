using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoetoOro
{
    public interface RhythmChoreography
    {
        /// <summary>
        /// How many Bars consists one choreography of the dance
        /// </summary>
        /// <returns></returns>
        int getBarCount();

        /// <summary>
        /// The group beat ratio of the beats in one bar
        /// </summary>
        /// <returns></returns>
        double[] getGroupBeatRatio();

        
    }

    /// <summary>
    /// Ramnoto
    /// </summary>
    public class RamnotoRhythmChoreography : RhythmChoreography
    {
        int RhythmChoreography.getBarCount()
        {
            return 3;
        }

        double[] RhythmChoreography.getGroupBeatRatio()
        {
            double[] rhythm={ 3.0 / 7.0, 4.0 / 7.0 };
            return rhythm;
        }
    }

    /// <summary>
    /// 6N3N
    /// </summary>
    public class O6N3NRhythmChoreography : RhythmChoreography
    {
        int RhythmChoreography.getBarCount()
        {
            return 3;
        }

        double[] RhythmChoreography.getGroupBeatRatio()
        {
            double[] rhythm = { 2.0 / 9.0, 2.0 / 9.0, 2.0 / 9.0, 3.0 / 9.0 };
            return rhythm;
        }
    }

    /// <summary>
    /// Pajdusko
    /// </summary>
    public class PajduskoRhythmChoreography : RhythmChoreography
    {
        int RhythmChoreography.getBarCount()
        {
            return 5;
        }

        double[] RhythmChoreography.getGroupBeatRatio()
        {
            double[] rhythm = { 1.0 / 2.0, 1.0 / 2.0 };
            return rhythm;
        }
    }
    public static class LabelsRhythmSplitter
    {

        public static int[] getSyncedLabels(int[] labels, DanceType danceType){
            int[] syncedLabels = new int[labels.Length];
            for (int i = 0; i < labels.Length; i++)
                syncedLabels[i] = labels[i];
            RhythmChoreography rhythmChoreography;
            if (danceType.Equals(DanceType.Ramnoto))
            {
                rhythmChoreography = new RamnotoRhythmChoreography();
            }
            else if (danceType.Equals(DanceType.O6n3n))
            {
                rhythmChoreography = new O6N3NRhythmChoreography();
            }
            else if (danceType.Equals(DanceType.Pajdusko))
            {
                rhythmChoreography = new PajduskoRhythmChoreography();
            }
            else
                return labels;
            int barCount = rhythmChoreography.getBarCount();
            double[] groupRatio = rhythmChoreography.getGroupBeatRatio();

            int[] barIndex = DataSplitter.splitDataIndexes(labels.Length, barCount);
            int beatGroupNumber = 0;
            int centroidNumber;
            if(danceType.Equals(DanceType.Pajdusko))
                centroidNumber=CentroidSelector.centroidsPajdusko.Length;
             else
                centroidNumber= CentroidSelector.centroids1.Length;
            for (int i = 0; i < barCount; i++)
            {
                int[] barPartIndex = DataBarSplitter.splitDataIndexes(barIndex[i + 1] - barIndex[i], groupRatio);
                for (int k = 0; k < groupRatio.Length; k++)
                {
                    for (int j = barPartIndex[k]; j < barPartIndex[k + 1]; j++)
                    {
                        syncedLabels[barIndex[i] + j] = (beatGroupNumber%2) * centroidNumber + labels[barIndex[i] + j];
                    }
                    beatGroupNumber++;
                }
            }
            return syncedLabels;
        }
    }
}
