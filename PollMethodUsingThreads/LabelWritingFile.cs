using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PollMethodUsingThreads
{
    public class LabelWritingFile
    {
        public static void writeLabelRamnoto(string filesPath, string writeFilePath, int maxTollerance, int ratio, int scalingDataCount, bool toWriteFilePath)
        {
            string[] files = System.IO.File.ReadAllLines(filesPath);
            string path = writeFilePath;
            int[] labelCount=new int[RamnotoRitam7_8Labeling.getTotalTypeLabels()*RamnotoRitam7_8Labeling.getTotalBars()];
            for (int i = 0; i < labelCount.Length; i++)
                labelCount[i] = 0;
            System.IO.File.WriteAllText(path, "");

            foreach (string file in files)
            {
                double[] dataC7 = ExtremaFinder.getCharacteristic7(file);
                double[] dataC2 = ExtremaFinder.getCharacteristic2(file);
                double[] data = ExtremaFinder.fitLeastSquare(dataC7);
                List<Entry> extremes = ExtremaFinder.findGlobalExtrema(ratio, data, maxTollerance);
                List<Entry> tempoExtremes = ExtremaFinder.calculateTempo(data, ratio, maxTollerance);
                int t1 = tempoExtremes.ElementAt(0).time;
                int t2 = tempoExtremes.ElementAt(2).time-1;
                double[] bodyMovementData = new double[t2 - t1 + 1];
                double[] horizontalDistanceData = new double[t2 - t1 + 1];
                for (int i = t1; i <= t2; i++)
                {
                    if (t1 >= 2)
                    {
                        bodyMovementData[i - t1] = dataC7[i-2];
                        horizontalDistanceData[i - t1] = dataC2[i - 2];
                    }
                    else
                    {
                        bodyMovementData[i - t1] = dataC7[i];
                        horizontalDistanceData[i - t1] = dataC2[i];
                    }
                }
                double[] bodyMovementDataScaled = Scalling.scale(bodyMovementData, scalingDataCount);
                double[] horizontalDistanceDataScaled = Scalling.scale(horizontalDistanceData, scalingDataCount);
                RamnotoRitam7_8Labeling ramnotoLabeling = new RamnotoRitam7_8Labeling(horizontalDistanceDataScaled, bodyMovementDataScaled);
                using (System.IO.StreamWriter output = new System.IO.StreamWriter(path, true))
                {
                    int[] labels = ramnotoLabeling.calculateLabels(bodyMovementData.Length);
                    if(toWriteFilePath)
                        output.WriteLine(file);
                    for (int i = 0; i < labels.Length-1; i++)
                        output.Write(labels[i] + "-");
                    output.WriteLine(labels[labels.Length-1]);
                }
                int[] newLabelCount = ramnotoLabeling.getLabelCountByGroup();
                for (int i = 0; i < labelCount.Length; i++)
                    labelCount[i] += newLabelCount[i];
            }
            using (System.IO.StreamWriter output = new System.IO.StreamWriter(path, true))
            {
                output.WriteLine("label count by bars:");
                int k = 0;
                int differentLabelType=RamnotoRitam7_8Labeling.getTotalTypeLabels()/RamnotoRitam7_8Labeling.getGroupRatioLength();
                for (int i = 0; i < labelCount.Length; i+=differentLabelType)
                {
                    int sum = 0;
                    for (int j = 0; j < differentLabelType; j++)
                        sum += labelCount[i + j];
                    for (int j = i; j < i + differentLabelType; j++)
                        output.Write(((k*differentLabelType)+j%differentLabelType)+": "+labelCount[j] + " (" + String.Format("{0:0.00}",labelCount[j]/(double)sum*100.0)+" %) ");
                    output.WriteLine();
                    k = (k + 1) % RamnotoRitam7_8Labeling.getGroupRatioLength();
                }
            }
            Console.WriteLine("Finished Writing Labels for Ramnoto 7/8");
            string hmmFile = @"D:\HMM\HMMramnoto parameters.txt";
            System.IO.File.WriteAllText(hmmFile, "");
            using (System.IO.StreamWriter output = new System.IO.StreamWriter(hmmFile, true))
            {
                output.WriteLine((RamnotoRitam7_8Labeling.getTotalBars()*RamnotoRitam7_8Labeling.getGroupRatioLength())+" "+RamnotoRitam7_8Labeling.getTotalTypeLabels());
                int k = 0;
                int o = RamnotoRitam7_8Labeling.getTotalTypeLabels()/RamnotoRitam7_8Labeling.getGroupRatioLength();
                for (int i = 0; i < labelCount.Length; i += o)
                {
                    int sum=0;
                    for (int j = 0; j < k * o; j++)
                        output.Write(String.Format("{0:0.00000}", 0.000)+" ");
                    //output.WriteLine();
                    for (int j = 0; j < o; j++)
                        sum += labelCount[i+j];
                    for (int j = i; j < i + o; j++)
                        output.Write(String.Format("{0:0.00000}", labelCount[j] / (double)sum)+" ");
                    //output.WriteLine();
                    for (int j = 0; j < (RamnotoRitam7_8Labeling.getGroupRatioLength()-k-1) * o; j++)
                        output.Write(String.Format("{0:0.00000}", 0.000) + " ");
                    output.WriteLine();
                    k = (k + 1) % RamnotoRitam7_8Labeling.getGroupRatioLength();
                }
            }
            Console.WriteLine("Written the HMM parameters in D:\\HMM\\HMMramnoto parameters.txt");
        }

        public static void writeLabel6Napred3Nazad(string filesPath, string writeFilePath, int maxTollerance, int ratio, int scalingDataCount,bool toWriteFilePath)
        {
            string[] files = System.IO.File.ReadAllLines(filesPath);
            string path = writeFilePath;
            int[] labelCount = new int[O6N3NLabeling.getTotalTypeLabels() * O6N3NLabeling.getTotalBars()];
            for (int i = 0; i < labelCount.Length; i++)
                labelCount[i] = 0;
            System.IO.File.WriteAllText(path, "");

            foreach (string file in files)
            {
                double[] dataC7 = ExtremaFinder.getCharacteristic7(file);
                double[] dataC2 = ExtremaFinder.getCharacteristic2(file);
                double[] data = ExtremaFinder.fitLeastSquare(dataC7);
                List<Entry> extremes = ExtremaFinder.findGlobalExtrema(ratio, data, maxTollerance);
                List<Entry> tempoExtremes = ExtremaFinder.calculateTempo(data, ratio, maxTollerance);
                int t1 = tempoExtremes.ElementAt(0).time;
                int t2 = tempoExtremes.ElementAt(2).time-1;
                double[] bodyMovementData = new double[t2 - t1 + 1];
                double[] horizontalDistanceData = new double[t2 - t1 + 1];
                for (int i = t1; i <= t2; i++)
                {
                    if (t1 >= 2)
                    {
                        bodyMovementData[i - t1] = dataC7[i - 2];
                        horizontalDistanceData[i - t1] = dataC2[i - 2];
                    }
                    else
                    {
                        bodyMovementData[i - t1] = dataC7[i];
                        horizontalDistanceData[i - t1] = dataC2[i];
                    }
                }
                double[] bodyMovementDataScaled = Scalling.scale(bodyMovementData,scalingDataCount);
                double[] horizontalDistanceDataScaled = Scalling.scale(horizontalDistanceData, scalingDataCount);
                O6N3NLabeling o6n3nLabeling = new O6N3NLabeling(horizontalDistanceDataScaled, bodyMovementDataScaled);
                using (System.IO.StreamWriter output = new System.IO.StreamWriter(path, true))
                {
                    int[] labels = o6n3nLabeling.calculateLabels(bodyMovementData.Length);
                    if(toWriteFilePath)
                        output.WriteLine(file); 
                    for (int i = 0; i < labels.Length-1; i++)
                        output.Write(labels[i] + "-");
                    output.WriteLine(labels[labels.Length-1]);
                }
                int[] newLabelCount = o6n3nLabeling.getLabelCountByGroup();
                for (int i = 0; i < labelCount.Length; i++)
                    labelCount[i] += newLabelCount[i];
            }
            using (System.IO.StreamWriter output = new System.IO.StreamWriter(path, true))
            {
                output.WriteLine("label count by bars:");
                int k = 0;
                int differentLabelType = O6N3NLabeling.getTotalTypeLabels() / O6N3NLabeling.getGroupRatioLength();
                for (int i = 0; i < labelCount.Length; i += differentLabelType)
                {
                    int sum = 0;
                    for (int j = 0; j < differentLabelType; j++)
                        sum += labelCount[i + j];
                    for (int j = i; j < i + differentLabelType; j++)
                        output.Write(((k * differentLabelType) + j % differentLabelType) + ": " + labelCount[j] + " (" + String.Format("{0:0.00}", labelCount[j] / (double)sum * 100.0) + " %) ");
                    output.WriteLine();
                    k = (k + 1) % O6N3NLabeling.getGroupRatioLength();
                }
            }
            Console.WriteLine("Finished Writing Labels for 6 napred 3 nazad 9/8");
            string hmmFile = @"D:\HMM\HMM6n3n parameters.txt";
            System.IO.File.WriteAllText(hmmFile, "");
            using (System.IO.StreamWriter output = new System.IO.StreamWriter(hmmFile, true))
            {
                output.WriteLine((O6N3NLabeling.getTotalBars() * O6N3NLabeling.getGroupRatioLength()) + " " + O6N3NLabeling.getTotalTypeLabels());
                int k = 0;
                int o = O6N3NLabeling.getTotalTypeLabels() / O6N3NLabeling.getGroupRatioLength();
                for (int i = 0; i < labelCount.Length; i += o)
                {
                    int sum = 0;
                    for (int j = 0; j < k * o; j++)
                        output.Write(String.Format("{0:0.00000}", 0.000) + " ");
                    //output.WriteLine();
                    for (int j = 0; j < o; j++)
                        sum += labelCount[i + j];
                    for (int j = i; j < i + o; j++)
                        output.Write(String.Format("{0:0.00000}", labelCount[j] / (double)sum) + " ");
                    //output.WriteLine();
                    for (int j = 0; j < (O6N3NLabeling.getGroupRatioLength() - k - 1) * o; j++)
                        output.Write(String.Format("{0:0.00000}", 0.000) + " ");
                    output.WriteLine();
                    k = (k + 1) % O6N3NLabeling.getGroupRatioLength();
                }
            }
            Console.WriteLine("Written the HMM parameters in D:\\HMM\\HMM6n3n parameters.txt");
        }
        
    }
}
