using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoetoOro;

namespace RecordingsAnalyser
{
    class Program
    {
        static void Main(string[] args)
        {
            //calculateLabels();
            calculateAllLabelsNewMethod();
            Console.WriteLine("Finished.");
            Console.ReadKey();
        }

        static void calculateAllLabelsNewMethod()
        {
            calculateLabelsNewMethod(@"D:\calculatedAttributes\ramnoto\snimkiUcenje.txt", @"D:\calculatedAttributes\ramnoto\labeliUcenje");
            calculateLabelsNewMethod(@"D:\calculatedAttributes\ramnoto\snimkiTestiranje.txt", @"D:\calculatedAttributes\ramnoto\labeliTestiranje");
            calculateLabelsNewMethod(@"D:\calculatedAttributes\6n3n\snimkiUcenje.txt", @"D:\calculatedAttributes\6n3n\labeliUcenje");
            calculateLabelsNewMethod(@"D:\calculatedAttributes\6n3n\snimkiTestiranje.txt", @"D:\calculatedAttributes\6n3n\labeliTestiranje");
            calculateLabelsNewMethod(@"D:\calculatedAttributes\pajdusko\snimkiUcenje.txt", @"D:\calculatedAttributes\pajdusko\labeliUcenje");
            calculateLabelsNewMethod(@"D:\calculatedAttributes\pajdusko\snimkiTestiranje.txt", @"D:\calculatedAttributes\pajdusko\labeliTestiranje");
        }
        static void calculateLabelsNewMethod(String recordingLocationsFilePath, String writeFilePath)
        {
            string unsyncedWriteFilePath = writeFilePath + "NewUnsynched.txt";
            string ramnotoWriteFilePath = writeFilePath + "NewRamnoto.txt";
            string pajduskoWriteFilePath = writeFilePath + "NewPajdusko.txt";
            string o6n3nWriteFilePath = writeFilePath + "New6N3N.txt";
            System.IO.File.WriteAllText(unsyncedWriteFilePath, "");
            System.IO.File.WriteAllText(ramnotoWriteFilePath, "");
            System.IO.File.WriteAllText(pajduskoWriteFilePath, "");
            System.IO.File.WriteAllText(o6n3nWriteFilePath, "");

            string[] files = System.IO.File.ReadAllLines(recordingLocationsFilePath + "");
            foreach (string file in files)
            {
                string[] lines = System.IO.File.ReadAllLines(file);
                int n = Int32.Parse(lines[0]);
                double[] TSM = new double[n];
                double[] FHSD = new double[n];
                double[] FVD = new double[n];
                double[] FHFD = new double[n];
                double[] CTSM = new double[n];
                for (int i = 0; i < n; i++)
                {
                    string[] part = lines[i + 1].Split(' ');
                    TSM[i] = Double.Parse(part[0]);
                    FHSD[i] = Double.Parse(part[1]);
                    FVD[i] = Double.Parse(part[2]);
                    FHFD[i] = Double.Parse(part[3]);
                    CTSM[i] = Double.Parse(part[4]);
                }
                int scaledDataLength = 100;
                double[] scalledCTSM = Scalling.scale(CTSM, scaledDataLength);
                double[] scalledFHSD = Scalling.scale(FHSD, scaledDataLength);
                double[] scalledTSM = Scalling.scale(TSM, scaledDataLength);
                double[] scalledFVD = Scalling.scale(FVD, scaledDataLength);
                double[] scalledFHFD = Scalling.scale(FHFD, scaledDataLength);
                int[] labels = CalculateLabels.calculateLabels(scalledFHSD, scalledFVD, scalledFHFD,DanceType.Ramnoto);
                using (System.IO.StreamWriter output = new System.IO.StreamWriter(unsyncedWriteFilePath, true))
                {
                    for (int i = 0; i < labels.Length - 2; i++)
                        output.Write(labels[i] + "-");
                    output.WriteLine(labels[labels.Length - 2]);
                }
                int[] syncedRamnotoLabels = LabelsRhythmSplitter.getSyncedLabels(labels, DanceType.Ramnoto);
                using (System.IO.StreamWriter output = new System.IO.StreamWriter(ramnotoWriteFilePath, true))
                {
                    for (int i = 0; i < syncedRamnotoLabels.Length - 2; i++)
                        output.Write(syncedRamnotoLabels[i] + "-");
                    output.WriteLine(syncedRamnotoLabels[syncedRamnotoLabels.Length - 2]);
                }
                int[] syncedO6n3nLabels = LabelsRhythmSplitter.getSyncedLabels(labels, DanceType.O6n3n);
                using (System.IO.StreamWriter output = new System.IO.StreamWriter(o6n3nWriteFilePath, true))
                {
                    for (int i = 0; i < syncedO6n3nLabels.Length - 2; i++)
                        output.Write(syncedO6n3nLabels[i] + "-");
                    output.WriteLine(syncedO6n3nLabels[syncedO6n3nLabels.Length - 2]);
                }
                labels = CalculateLabels.calculateLabels(scalledFHSD, scalledFVD, scalledFHFD, DanceType.Pajdusko);
                int[] syncedPajduskoLabels = LabelsRhythmSplitter.getSyncedLabels(labels, DanceType.Pajdusko);
                using (System.IO.StreamWriter output = new System.IO.StreamWriter(pajduskoWriteFilePath, true))
                {
                    for (int i = 0; i < syncedPajduskoLabels.Length - 2; i++)
                        output.Write(syncedPajduskoLabels[i] + "-");
                    output.WriteLine(syncedPajduskoLabels[syncedPajduskoLabels.Length - 2]);
                }
            }
        }
        /// <summary>
        /// calculate labels for the dances (old method)
        /// </summary>
        static void calculateLabels()
        {
            calculateLabelsForRecordings(@"D:\calculatedAttributes\ramnoto\snimkiUcenje.txt", @"D:\calculatedAttributes\ramnoto\labeliUcenje");
            calculateLabelsForRecordings(@"D:\calculatedAttributes\ramnoto\snimkiTestiranje.txt", @"D:\calculatedAttributes\ramnoto\labeliTestiranje");
            calculateLabelsForRecordings(@"D:\calculatedAttributes\6n3n\snimkiUcenje.txt", @"D:\calculatedAttributes\6n3n\labeliUcenje");
            calculateLabelsForRecordings(@"D:\calculatedAttributes\6n3n\snimkiTestiranje.txt", @"D:\calculatedAttributes\6n3n\labeliTestiranje");
            calculateLabelsForRecordings(@"D:\calculatedAttributes\pajdusko\snimkiUcenje.txt", @"D:\calculatedAttributes\pajdusko\labeliUcenje");
            calculateLabelsForRecordings(@"D:\calculatedAttributes\pajdusko\snimkiTestiranje.txt", @"D:\calculatedAttributes\pajdusko\labeliTestiranje");
        }
        static void calculateLabelsForRecordings(String recordingLocationsFilePath, String writeFilePath)
        {
            string[] files = System.IO.File.ReadAllLines(recordingLocationsFilePath);
            System.IO.File.WriteAllText(writeFilePath + "Ramnoto.txt", "");
            System.IO.File.WriteAllText(writeFilePath + "6N3N.txt", "");
            System.IO.File.WriteAllText(writeFilePath + "Pajdusko.txt", "");
            foreach (string file in files)
            {
                string[] lines = System.IO.File.ReadAllLines(file);
                int n = Int32.Parse(lines[0]);
                double[] TSM = new double[n];
                double[] FHSD = new double[n];
                double[] FVD = new double[n];
                double[] FHFD = new double[n];
                double[] CTSM = new double[n];
                for (int i = 0; i < n; i++)
                {
                    string[] part = lines[i + 1].Split(' ');
                    TSM[i] = Double.Parse(part[0]);
                    FHSD[i] = Double.Parse(part[1]);
                    FVD[i] = Double.Parse(part[2]);
                    FHFD[i] = Double.Parse(part[3]);
                    CTSM[i] = Double.Parse(part[4]);
                }
                int scaledDataLength = 100;
                double[] scalledCTSM = Scalling.scale(CTSM, scaledDataLength);
                double[] scalledFHSD = Scalling.scale(FHSD, scaledDataLength);
                double[] scalledTSM = Scalling.scale(TSM, scaledDataLength);
                double[] scalledFVD = Scalling.scale(FVD, scaledDataLength);
                double[] scalledFHFD = Scalling.scale(FHFD, scaledDataLength);
                O6N3NLabeling o6n3nLabeling = new O6N3NLabeling(scalledFHSD, scalledCTSM);
                RamnotoRitam7_8Labeling ramnotoLabeling = new RamnotoRitam7_8Labeling(scalledFHSD, scalledCTSM);
                PajduskoLabeling pajduskoLabeling = new PajduskoLabeling(scalledFVD, scalledFHFD, scalledCTSM);
            
                using (System.IO.StreamWriter output = new System.IO.StreamWriter(writeFilePath + "Ramnoto.txt", true))
                {
                    output.WriteLine(formLabelSequence(ramnotoLabeling.calculateLabels(0)));
                }
                using (System.IO.StreamWriter output = new System.IO.StreamWriter(writeFilePath + "6N3N.txt", true))
                {
                    output.WriteLine(formLabelSequence(o6n3nLabeling.calculateLabels(0)));
                }
                using (System.IO.StreamWriter output = new System.IO.StreamWriter(writeFilePath + "Pajdusko.txt", true))
                {
                    output.WriteLine(formLabelSequence(pajduskoLabeling.calculateLabels(0)));
                }
            }
            
        }

        static string formLabelSequence(int[] labels)
        {
            string res = "";
            for (int i = 0; i < labels.Length - 1; i++)
                res = res + labels[i] + "-";
            res += labels[labels.Length - 1];
            return res;
        }
    }
}
