using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPFitLib;
using PollMethodUsingThreads;
namespace FitTes
{
    class Program
    {
        static void Main(string[] args)
        {

            //writeExtremesToFiles();
            //testScaling();
            //writeLabelsToFiles();
            testLabelingForOneClass();
            //ExtremaFinder.calculateExtremesAndWriteToFiles("D:\\Podatoci za magisterska\\Darko\\testiranje\\6n3n 3 full.txt", 70, 4);

            Console.ReadKey();
        }
        public static void testLabelingForOneClass()
        {
            //Test labeling for one dance
            string file = @"D:\Podatoci za magisterska\Test\17 full.txt";
            double[] dataC7 = ExtremaFinder.getCharacteristic7(file);
            double[] dataC2 = ExtremaFinder.getCharacteristic2(file);
            double[] dataFVD = ExtremaFinder.getCharacteristicFVD(file);
            double[] dataFHFD = ExtremaFinder.getCharacteristicFHFD(file);
            double[] data = ExtremaFinder.fitLeastSquare(dataC7);
            List<Entry> extremes = ExtremaFinder.findGlobalExtrema(70, data, 4);
            //foreach (Entry entry in extremes)
            //    Console.WriteLine(entry.time + " " + entry.value + " " + entry.isMaximum);
            List<Entry> tempoExtremes = ExtremaFinder.calculateTempo(data, 70, 4);
            //Console.WriteLine(tempoExtremes.ElementAt(1).time - tempoExtremes.ElementAt(0).time);
            //Console.WriteLine(tempoExtremes.ElementAt(2).time - tempoExtremes.ElementAt(1).time);
            Console.WriteLine(tempoExtremes.ElementAt(0).time + " " + tempoExtremes.ElementAt(2).time);
            int t1 = tempoExtremes.ElementAt(0).time;
            int t2 = tempoExtremes.ElementAt(2).time - 1;
            double[] dataC7M = ExtremaFinder.getCharacteristic7(file, t1 -2, t2 - t1+1);
            //Console.WriteLine(t1 + " " + t2);
            double[] bodyMovementData = new double[t2 - t1 + 1];
            double[] horizontalDistanceData = new double[t2 - t1 + 1];
            double[] FHFD = new double[t2 - t1 + 1];
            double[] FVD = new double[t2 - t1 + 1];
            for (int i = t1; i <= t2; i++)
            {
                bodyMovementData[i - t1] = dataC7M[i - t1];
                if (t1 >= 2)
                {
                    horizontalDistanceData[i - t1] = dataC2[i - 2];
                    FHFD[i - t1] = dataFHFD[i - 2];
                    FVD[i - t1] = dataFVD[i - 2];
                }
                else
                {
                    horizontalDistanceData[i - t1] = dataC2[i];
                    FHFD[i - t1] = dataFHFD[i];
                    FVD[i - t1] = dataFVD[i];
                }
            }
            double[] bodyMovementDataScaled = Scalling.scale(bodyMovementData, 100);
            double[] horizontalDistanceDataScaled = Scalling.scale(horizontalDistanceData, 100);
            double[] FHFDScaled = Scalling.scale(FHFD, 100);
            double[] FVDScaled = Scalling.scale(FVD, 100);
            PajduskoLabeling pajduskoLabeling = new PajduskoLabeling(FVDScaled, FHFDScaled, bodyMovementDataScaled);
            int[] labels = pajduskoLabeling.calculateLabels(bodyMovementData.Length);
            for (int i = 0; i < labels.Length; i++)
                Console.Write(labels[i] + " ");
            /*
            RamnotoRitam7_8Labeling ramnotoLabeling = new RamnotoRitam7_8Labeling(horizontalDistanceDataScaled, bodyMovementDataScaled);

            int[] labels = ramnotoLabeling.calculateLabels(bodyMovementData.Length);
            Console.WriteLine(file);
            for (int i = 0; i < labels.Length; i++)
                Console.Write(labels[i] + " ");
            Console.WriteLine();
            Console.WriteLine("Label count by group");
            int[] labelCount = ramnotoLabeling.getLabelCountByGroup();

            for (int i = 0; i < labelCount.Length / RamnotoRitam7_8Labeling.getTotalTypeLabels(); i++)
            {
                for (int j = 0; j < RamnotoRitam7_8Labeling.getTotalTypeLabels(); j++)
                {
                    Console.Write(j + " ");
                }
                Console.WriteLine();
                for (int j = 0; j < RamnotoRitam7_8Labeling.getTotalTypeLabels(); j++)
                {
                    Console.Write(labelCount[i * RamnotoRitam7_8Labeling.getTotalTypeLabels() + j] + " ");
                }
                Console.WriteLine();
            }
             */
            Console.WriteLine();
            Console.WriteLine("Finished testing labeling");
        }
        public static void writeExtremesToFiles()
        {
            string[] files = System.IO.File.ReadAllLines(@"D:\Ekstremi\siteSnimkiPajdusko.txt");
            foreach (string file in files)
            {
                ExtremaFinder.calculateExtremesAndWriteToFiles(file, 70, 4);
            }
            Console.WriteLine("Finished writing Extremes to Files");
        }
        public static void writeLabelsToFiles()
        {
            string[] snimki = { @"D:\HMM\snimkiUcenjeRamnotoSiteIsfrleniNajlosite2.txt", @"D:\HMM\snimkiUcenje6Napred3NazadIsfreniNajlosite.txt", @"D:\HMM\snimkiUcenjePajdusko.txt" };
            //Labels writing to files
            LabelWritingFile.writeLabel6Napred3Nazad(snimki[2], @"D:\HMM\6Napred3Nazad labels.txt", 4, 70,100,true);
            LabelWritingFile.writeLabelRamnoto(snimki[0], @"D:\HMM\ramnotoSite labels.txt", 4, 70,100,true);
            LabelWritingFile.writeLabel6Napred3Nazad(snimki[2], @"D:\HMM\6Napred3Nazad labels no text.txt", 4, 70, 100,false);
            LabelWritingFile.writeLabelRamnoto(snimki[0], @"D:\HMM\ramnotoSite labels no text.txt", 4, 70, 100,false);
            Console.WriteLine("Finished Writing Labels to files");
        }
        public static void testScaling()
        {
            
            //Test scalling
            double[] data={1.23,1.756,0.2,-2.5};
            double[] scaled=Scalling.scale(data, 8);
            for (int i = 0; i < scaled.Length; i++)
            {
                Console.Write(scaled[i]+" ");
            }
            Console.WriteLine("Testing Scalling is finished");
        }
        

    }
}
