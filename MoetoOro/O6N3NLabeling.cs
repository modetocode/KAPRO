using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoetoOro
{
    public class O6N3NLabeling
    {
        private static int barCount=3;
        private static double[] groupRatio = { 2.0 / 9.0, 2.0 / 9.0, 2.0/9.0, 3.0/9.0 };
        private double[] horizontalWidth {get;set;}
        private double[] bodyMovement {get;set;}
        public O6N3NLabeling(double[] horizontalWidth, double[] bodyMovement)
        {
            this.horizontalWidth = horizontalWidth;
            this.bodyMovement = bodyMovement;
        }
        private static int differentTypesCount = 4;
        /*Labels: 
        //1 part 2/9
        0: <-> width, -> body
        1: >-< width, -> body
        2: <-> width, <- body
        3: >-< width, <- body
        //2 part 2/9
        4: <-> width, -> body
        5: >-< width, -> body
        6: <-> width, <- body
        7: >-< width, <- body
        //3 part 2/9
        8: <-> width, -> body
        9: >-< width, -> body
        10: <-> width, <- body
        11: >-< width, <- body
        //4 part 3/9
        12: <-> width, -> body
        13: >-< width, -> body
        14: <-> width, <- body
        15: >-< width, <- body 
        //Pravilna sekvenca
         * 0 - 5 - 8 - (13, 12) - 0 - 5 - 8 - 13 - 2 - 7 - 10 - 15 
        */
        private int getLabel(double bodyMovement1, double bodyMovement2, double horizontalWidth1, double horizontalWidth2, int partNo){
            double difBody = bodyMovement2 - bodyMovement1;
            double difWidth = horizontalWidth2 - horizontalWidth1;
            int k = 4;
                if (difBody > 0.0)
                {
                    if (difWidth > 0.0)
                        return partNo * k;
                    else
                        return partNo * k + 1;
                }
                else
                {
                    if (difWidth > 0.0)
                        return partNo * k + 2;
                    else
                        return partNo * k + 3;
                }
        }
        
        public int[] calculateLabels(int originalDataSize)
        {
            int[] labels = new int[horizontalWidth.Length - 1];
            int[] barIndex = DataSplitter.splitDataIndexes(horizontalWidth.Length, barCount);
            for (int i = 0; i < barCount; i++)
            {
                //Console.WriteLine("Interval [ " + barIndex[i] + " , " + barIndex[i + 1]+" ]");
                int[] barPartIndex = DataBarSplitter.splitDataIndexes(barIndex[i+1]-barIndex[i], groupRatio);
                for (int k = 0; k < groupRatio.Length; k++)
                {
                    //Console.WriteLine("Subinterval [ " + (barIndex[i] + barPartIndex[k]) + " , " + (barIndex[i] + barPartIndex[k+1]) + " ]");
                    for (int j = barPartIndex[k]; j < barPartIndex[k + 1]; j++)
                    {
                        //Console.WriteLine((barIndex[i] + j) + " " + bodyMovement[barIndex[i] + j] + " " + horizontalWidth[barIndex[i] + j]);
                            
                        if ((barIndex[i] + j) < horizontalWidth.Length -1 )
                        {
                           labels[barIndex[i] + j] = getLabel(bodyMovement[barIndex[i] + j], bodyMovement[barIndex[i] + j + 1], horizontalWidth[j], horizontalWidth[j + 1], k);
                        }
                    }
                }
            }
            return labels;
            //return getLabelCorrections(labels, originalDataSize, bodyMovement.Length);
        }
       
        //dopolnitelno popravanje na labelite otkako ke bidat izgenerirani
        private int[] getLabelCorrections(int[] labels, int dataLength, int dataScaledLength)
        {
            int maxLabelCorrections = (int)Math.Round(1.5 * dataScaledLength / dataLength);
            int[,] rules = new int[,] { { 0, 0, 1 }, { 1, 1, 0 }, { 2, 0, 1 }, { 3, 1, 0 }, { 4, 0, 1 }, { 5, 1, 0 }, { 6, 0, 1 }, { 7, 1, 2 }, { 7, 0, 2 }, { 8, 2, 3 }, { 9, 3, 2 }, { 10, 2, 3 } };
            int currentAccentBeat = 0;
            int currentRuleIndex = 0;
            for (int i = 1; i < labels.Length; i++)
            {
                if (labels[i - 1] / differentTypesCount != labels[i] / differentTypesCount)
                {
                    if (currentRuleIndex >= (rules.Length / 3))
                        break;
                    //a new accent part has been found
                    while (rules[currentRuleIndex, 0] == currentAccentBeat)
                    {
                        //a rule has been satisfied
                        //now check if right side of the next period has the given label numbers
                        bool hasLabels = true;
                        for (int j = 0; j < maxLabelCorrections - 1; j++)
                            if (labels[i + j] % differentTypesCount != rules[currentRuleIndex, 2])
                            {
                                hasLabels = false;
                                break;
                            }
                        if (hasLabels)
                        {
                            int max = 1;
                            int consequitiveNumber = 1;
                            //now ckeck if left side of the previous period has the given label numbers
                            for (int j = i - 1; j > 0; j--)
                            {
                                if (labels[j - 1] / differentTypesCount != labels[j] / differentTypesCount && labels[j] % differentTypesCount != rules[currentRuleIndex, 1])
                                    break;
                                if (labels[j - 1] % differentTypesCount == labels[j] % differentTypesCount && labels[j] % differentTypesCount == rules[currentRuleIndex, 1])
                                {
                                    consequitiveNumber++;
                                }
                                else
                                {
                                    if (consequitiveNumber > max)
                                        max = consequitiveNumber;
                                    consequitiveNumber = 1;
                                }

                            }
                            if (consequitiveNumber > max)
                                max = consequitiveNumber;
                            if (max >= maxLabelCorrections - 1)
                            {
                                //starting from the left of the current period change labels if the label numbers are appropriate
                                //stop when a innapropriate label number is found
                                for (int j = 0; j < maxLabelCorrections; j++)
                                {
                                    if (i - j - 1 < 0)
                                        break;
                                    if (labels[i - j - 1] % differentTypesCount == rules[currentRuleIndex, 2])
                                        labels[i - j - 1] = labels[i];
                                    else
                                        break;
                                }
                            }

                        }

                        //now check if left side of the next period has the given label numbers
                        hasLabels = true;
                        for (int j = 0; j < maxLabelCorrections - 1; j++)
                        {
                            if (i - j - 1 < 0)
                                break;
                            if (labels[i - j - 1] % differentTypesCount != rules[currentRuleIndex, 1])
                            {
                                hasLabels = false;
                                break;
                            }
                        }
                        if (hasLabels)
                        {
                            int max = 1;
                            int consequitiveNumber = 1;
                            //now ckeck if right side of the previous period has the given label numbers
                            for (int j = i; j < labels.Length - 1; j++)
                            {
                                if (labels[j + 1] / differentTypesCount != labels[j] / differentTypesCount && labels[j] % differentTypesCount != rules[currentRuleIndex, 2])
                                    break;
                                if (labels[j] % differentTypesCount == labels[j + 1] % differentTypesCount && labels[j] % differentTypesCount == rules[currentRuleIndex, 2])
                                {
                                    consequitiveNumber++;
                                }
                                else
                                {
                                    if (consequitiveNumber > max)
                                        max = consequitiveNumber;
                                    consequitiveNumber = 1;
                                }

                            }
                            if (consequitiveNumber > max)
                                max = consequitiveNumber;
                            if (max >= maxLabelCorrections - 1)
                            {
                                //starting from the right of the next starting period change labels if the label numbers are appropriate
                                //stop when a innapropriate label number is found
                                int j;
                                for (j = 0; j < maxLabelCorrections; j++)
                                {
                                    if (labels[i + j] % differentTypesCount == rules[currentRuleIndex, 1])
                                        labels[i + j] = labels[i - 1];
                                    else
                                        break;
                                }
                                i += j;
                            }
                        }
                        currentRuleIndex++;
                        if (currentRuleIndex >= (rules.Length / 3))
                            break;
                    }
                    currentAccentBeat++;
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
