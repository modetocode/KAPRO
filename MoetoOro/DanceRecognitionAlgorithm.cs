using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Collections;
using Accord.Statistics.Models.Markov;
using HMMParameterEstimator;
namespace MoetoOro
{
    /// <summary>
    /// Class for storing the result of the algorythm of recognition of dance
    /// </summary>
    public class AlgorythmResult
    {
        public int fromIndex { get; set; } //starting index from which the skeleton data is part of the selected dance period
        public int toIndex {get;set;} //ending index from which the skeleton data is part of the selected dance period
        public DanceType danceType { get; set; } //the type of the recognised dance
        public AlgorythmResult(int fromIndex, int toIndex, DanceType danceType)
        {
            this.fromIndex = fromIndex;
            this.toIndex = toIndex;
            this.danceType = danceType;
        }
    }

    /// <summary>
    /// Class for executing the dance recognition algorithm
    /// </summary>
    class DanceRecognitionAlgorithm
    {
        /// <summary>
        /// The gathered skeleton data 
        /// </summary>
        private Skeleton[] skeletonData;
        /// <summary>
        /// The length of the data when the data is scaled
        /// </summary>
        private const int scaledDataLength = 100;

        /// <summary>
        /// The time when a skeleton data is fetched is stored here (for every frame)
        /// </summary>
        private long[] fetchedTime;


        private Hashtable fromIndexHashTable;
        private Hashtable toIndexHashTable;
        public DanceRecognitionAlgorithm(Skeleton[] skeletonData,long[] fetchedTime)
        {
            //1 step: Collecting data
            this.skeletonData = skeletonData;
            this.fetchedTime = fetchedTime;
            fromIndexHashTable = new Hashtable();
            toIndexHashTable = new Hashtable();
        }

        /// <summary>
        /// Write the calculated attributes for a recorded dance. Creates two files: files for calculated atrrbutes for the whole recording and for the selected period
        /// </summary>
        /// <param name="oldSkeletonData"></param>
        public void writeAttributesToFile(int fromIndex,int toIndex)
        {
            CharacteristicAttributesCalculator characteristicAttributesCalculator = new CharacteristicAttributesCalculator(skeletonData);
            double[] TSM = characteristicAttributesCalculator.calculateTorsoSideMovement();
            double[] FHSD = characteristicAttributesCalculator.calculateFeetHorizontalSideDistance();
            double[] FVD = characteristicAttributesCalculator.calculateFeetVerticalDistance();
            double[] CTSM = characteristicAttributesCalculator.calculateCumulativeTorsoSideMovement();
            double[] FHFD = characteristicAttributesCalculator.calculateFeetHorizontalFrontDistance();

            Skeleton[] data = new Skeleton[toIndex - fromIndex + 1];
            for (int i = fromIndex; i <= toIndex; i++)
                data[i - fromIndex] = skeletonData[i];
            CharacteristicAttributesCalculator characteristicAttributesCalculator2 = new CharacteristicAttributesCalculator(data);
            double[] TSM2 = characteristicAttributesCalculator2.calculateTorsoSideMovement();
            double[] FHSD2 = characteristicAttributesCalculator2.calculateFeetHorizontalSideDistance();
            double[] FVD2 = characteristicAttributesCalculator2.calculateFeetVerticalDistance();
            double[] CTSM2 = characteristicAttributesCalculator2.calculateCumulativeTorsoSideMovement();
            double[] FHFD2 = characteristicAttributesCalculator2.calculateFeetHorizontalFrontDistance();

            //write to files
            String allAttributesPath = @"D:\calculatedAttributes\attributesAll.txt";
            String selectedPeriodFilePath = @"D:\calculatedAttributes\attributesSelectedPeriod.txt";
            System.IO.File.WriteAllText(allAttributesPath, TSM.Length+"\n");
            System.IO.File.WriteAllText(selectedPeriodFilePath, TSM2.Length+"\n");
            for(int i=0;i<TSM.Length;i++)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(allAttributesPath, true))
                {
                    file.WriteLine(String.Format("{0:F10} {1:F10} {2:F10} {3:F10} {4:F10}", TSM[i], FHSD[i], FVD[i], FHFD[i], CTSM[i]));
                }
            }
            for (int i = 0; i < TSM2.Length; i++)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(selectedPeriodFilePath, true))
                {
                    file.WriteLine(String.Format("{0:F10} {1:F10} {2:F10} {3:F10} {4:F10}", TSM2[i], FHSD2[i], FVD2[i], FHFD2[i], CTSM2[i]));
                }
            }
        }

        /// <summary>
        /// Execute the algorythm for the Ramnoto Dance
        /// Returns the log10 Likelihood of the calculated label sequence
        /// </summary>
        /// <returns></returns>
        private double executeRamnotoAlgorithm()
        {
            RamnotoOneDancePeriodSelector ramnotoOneDancePeriodSelector = new RamnotoOneDancePeriodSelector(skeletonData);
            int[] onePeriodIndexes = ramnotoOneDancePeriodSelector.selectOnePeriod();
            int fromIndex = onePeriodIndexes[0];
            int toIndex = onePeriodIndexes[1];
            Skeleton[] onePeriodSkeletonData = new Skeleton[toIndex - fromIndex + 1];
            for (int i = fromIndex; i <= toIndex; i++)
            {
                onePeriodSkeletonData[i - fromIndex] = skeletonData[i];
            }
            CharacteristicAttributesCalculator characteristicAttributesCalculator = new CharacteristicAttributesCalculator(onePeriodSkeletonData);
            double[] TSM = characteristicAttributesCalculator.calculateTorsoSideMovement();
            double[] FHSD = characteristicAttributesCalculator.calculateFeetHorizontalSideDistance();
            double[] FVD = characteristicAttributesCalculator.calculateFeetVerticalDistance();
            double[] CTSM = characteristicAttributesCalculator.calculateCumulativeTorsoSideMovement();
            double[] FHFD = characteristicAttributesCalculator.calculateFeetHorizontalFrontDistance();
            //Scalling
            double[] scalledCTSM = Scalling.scale(CTSM, scaledDataLength);
            double[] scalledFHSD = Scalling.scale(FHSD, scaledDataLength);
            double[] scalledTSM = Scalling.scale(TSM, scaledDataLength);
            double[] scalledFVD = Scalling.scale(FVD, scaledDataLength);
            double[] scalledFHFD = Scalling.scale(FHFD, scaledDataLength);
            /* Old method
            //Labeling
            RamnotoRitam7_8Labeling ramnotoLabeling = new RamnotoRitam7_8Labeling(scalledFHSD, scalledCTSM);
            int[] labelsRamnoto = ramnotoLabeling.calculateLabels(FHSD.Length);
            //Classification
            HMMGenerator HMMramnoto = new HMMGenerator("parametersHMMramnoto.txt");
            double likelihoodRamnoto = HMMramnoto.evaluateSequence(labelsRamnoto);
            double log10RamnotoLikelihood = Math.Log10(likelihoodRamnoto);
            */
            //Labeling
            int[] unsyncedlabelsRamnoto = CalculateLabels.calculateLabels(scalledFHSD,scalledFVD,scalledFHFD,DanceType.Ramnoto);
            int[] labelsRamnoto = LabelsRhythmSplitter.getSyncedLabels(unsyncedlabelsRamnoto, DanceType.Ramnoto);            
            //HMM clasification
            HiddenMarkovModel HMMramnoto = RamnotoHMMGenerator.generateInitialHMM();
            double likelihoodRamnoto = HMMramnoto.Evaluate(labelsRamnoto);
            double log10RamnotoLikelihood = Math.Log10(likelihoodRamnoto);

            Console.WriteLine("Ramnoto From: " + fromIndex + " To: " + toIndex);
            Console.WriteLine("Ramnoto labels: ");
            for (int i = 0; i < labelsRamnoto.Length-1; i++)
            {
                Console.Write(labelsRamnoto[i] + "-");
            }
            Console.WriteLine(labelsRamnoto[labelsRamnoto.Length-1]);

            fromIndexHashTable.Add(DanceType.Ramnoto, fromIndex);
            toIndexHashTable.Add(DanceType.Ramnoto, toIndex);
            fromIndexHashTable.Add(DanceType.RamnotoLooking, fromIndex);
            toIndexHashTable.Add(DanceType.RamnotoLooking, toIndex);
            return log10RamnotoLikelihood;

        }


        /// <summary>
        /// Execute the algorythm for the O6N3N Dance
        /// Returns the log10 Likelihood of the calculated label sequence
        /// </summary>
        /// <returns></returns>
        private double executeO6N3NAlgorithm()
        {
            O6N3NOneDancePeriodSelector o6N3NOneDancePeriodSelector = new O6N3NOneDancePeriodSelector(skeletonData);
            int[] onePeriodIndexes = o6N3NOneDancePeriodSelector.selectOnePeriod();
            int fromIndex = onePeriodIndexes[0];
            int toIndex = onePeriodIndexes[1];
            Skeleton[] onePeriodSkeletonData = new Skeleton[toIndex - fromIndex + 1];
            for (int i = fromIndex; i <= toIndex; i++)
            {
                onePeriodSkeletonData[i - fromIndex] = skeletonData[i];
            }
            CharacteristicAttributesCalculator characteristicAttributesCalculator = new CharacteristicAttributesCalculator(onePeriodSkeletonData);
            double[] TSM = characteristicAttributesCalculator.calculateTorsoSideMovement();
            double[] FHSD = characteristicAttributesCalculator.calculateFeetHorizontalSideDistance();
            double[] FVD = characteristicAttributesCalculator.calculateFeetVerticalDistance();
            double[] CTSM = characteristicAttributesCalculator.calculateCumulativeTorsoSideMovement();
            double[] FHFD = characteristicAttributesCalculator.calculateFeetHorizontalFrontDistance();
            //Scalling
            double[] scalledCTSM = Scalling.scale(CTSM, scaledDataLength);
            double[] scalledFHSD = Scalling.scale(FHSD, scaledDataLength);
            double[] scalledTSM = Scalling.scale(TSM, scaledDataLength);
            double[] scalledFVD = Scalling.scale(FVD, scaledDataLength);
            double[] scalledFHFD = Scalling.scale(FHFD, scaledDataLength);
            /*
            //Labeling
            O6N3NLabeling o6n3nLabeling = new O6N3NLabeling(scalledFHSD, scalledCTSM);
            int[] labels6n3n = o6n3nLabeling.calculateLabels(FHSD.Length);
            //Classification
            HMMGenerator HMM6n3n = new HMMGenerator("parametersHMM6n3n.txt");
            double likelihood6n3n = HMM6n3n.evaluateSequence(labels6n3n);
            double log106n3nLikelihood = Math.Log10(likelihood6n3n);*/

            //Labeling
            int[] unsyncedlabels6n3n = CalculateLabels.calculateLabels(scalledFHSD, scalledFVD, scalledFHFD, DanceType.O6n3n);
            int[] labels6n3n = LabelsRhythmSplitter.getSyncedLabels(unsyncedlabels6n3n, DanceType.O6n3n);
                
            //HMM clasification
            HiddenMarkovModel HMM6n3n = O6N3NHMMGenerator.generateInitialHMM();
            double likelihoodRamnoto = HMM6n3n.Evaluate(labels6n3n);
            double log106n3nLikelihood = Math.Log10(likelihoodRamnoto);

            Console.WriteLine("6n3n From: " + fromIndex + " To: " + toIndex);
            Console.WriteLine("6n3n labels: ");
            for (int i = 0; i < labels6n3n.Length - 1; i++)
            {
                Console.Write(labels6n3n[i] + "-");
            }
            Console.WriteLine(labels6n3n[labels6n3n.Length - 1]);

            fromIndexHashTable.Add(DanceType.O6n3n, fromIndex);
            toIndexHashTable.Add(DanceType.O6n3n, toIndex);
            fromIndexHashTable.Add(DanceType.O6n3nLooking, fromIndex);
            toIndexHashTable.Add(DanceType.O6n3nLooking, toIndex);
            return log106n3nLikelihood;
        }

         /// <summary>
        /// Execute the algorythm for the Pajdusko Dance
        /// Returns the log10 Likelihood of the calculated label sequence
        /// </summary>
        /// <returns></returns>
        private double executePajduskoAlgorithm()
        {
            PajduskoOneDancePeriodSelector pajduskoOneDancePeriodSelector = new PajduskoOneDancePeriodSelector(skeletonData);
            int[] onePeriodIndexes = pajduskoOneDancePeriodSelector.selectOnePeriod();
            int fromIndex = onePeriodIndexes[0];
            int toIndex = onePeriodIndexes[1];
            Skeleton[] onePeriodSkeletonData = new Skeleton[toIndex - fromIndex + 1];
            for (int i = fromIndex; i <= toIndex; i++)
            {
                onePeriodSkeletonData[i - fromIndex] = skeletonData[i];
            }
            CharacteristicAttributesCalculator characteristicAttributesCalculator = new CharacteristicAttributesCalculator(onePeriodSkeletonData);
            double[] TSM = characteristicAttributesCalculator.calculateTorsoSideMovement();
            double[] FHSD = characteristicAttributesCalculator.calculateFeetHorizontalSideDistance();
            double[] FVD = characteristicAttributesCalculator.calculateFeetVerticalDistance();
            double[] CTSM = characteristicAttributesCalculator.calculateCumulativeTorsoSideMovement();
            double[] FHFD = characteristicAttributesCalculator.calculateFeetHorizontalFrontDistance();
            //Scalling
            double[] scalledCTSM = Scalling.scale(CTSM, scaledDataLength);
            double[] scalledFHSD = Scalling.scale(FHSD, scaledDataLength);
            double[] scalledTSM = Scalling.scale(TSM, scaledDataLength);
            double[] scalledFVD = Scalling.scale(FVD, scaledDataLength);
            double[] scalledFHFD = Scalling.scale(FHFD, scaledDataLength);
            /*
            //Labeling
            PajduskoLabeling pajduskoLabeling = new PajduskoLabeling(scalledFVD, scalledFHFD, scalledCTSM);
            int[] labelsPajdusko = pajduskoLabeling.calculateLabels(FHSD.Length);
            //Classification
            HMMGenerator HMMpajdusko = new HMMGenerator("parametersHMMpajdusko.txt");
            double likelihoodPajdusko = HMMpajdusko.evaluateSequence(labelsPajdusko);
            double log10PajduskoLikelihood = Math.Log10(likelihoodPajdusko);*/
            
            //Labeling
            int[] unsyncedPajduskoLabels = CalculateLabels.calculateLabels(scalledFHSD, scalledFVD, scalledFHFD, DanceType.Pajdusko);
            int[] labelsPajdusko = LabelsRhythmSplitter.getSyncedLabels(unsyncedPajduskoLabels, DanceType.Pajdusko);
            //HMM clasification
            HiddenMarkovModel HMMpajdusko = PajduskoHMMGenerator.generateInitialHMM();
            double likelihoodRamnoto = HMMpajdusko.Evaluate(labelsPajdusko);
            double log10PajduskoLikelihood = Math.Log10(likelihoodRamnoto);

            Console.WriteLine("Pajdusko From: " + fromIndex + " To: " + toIndex);
            Console.WriteLine("Pajdusko labels:");
            for (int i = 0; i < labelsPajdusko.Length - 1; i++)
            {
                Console.Write(labelsPajdusko[i] + "-");
            }
            Console.WriteLine(labelsPajdusko[labelsPajdusko.Length - 1]);

            fromIndexHashTable.Add(DanceType.Pajdusko, fromIndex);
            toIndexHashTable.Add(DanceType.Pajdusko, toIndex);
            fromIndexHashTable.Add(DanceType.PajduskoLooking, fromIndex);
            toIndexHashTable.Add(DanceType.PajduskoLooking, toIndex);
            return log10PajduskoLikelihood;
        }

        private bool isUserOnlyStanding()
        {
            RamnotoOneDancePeriodSelector ramnotoOneDancePeriodSelector = new RamnotoOneDancePeriodSelector(skeletonData);
            int[] onePeriodIndexes = ramnotoOneDancePeriodSelector.selectOnePeriod();
            int fromIndex = onePeriodIndexes[0];
            int toIndex = onePeriodIndexes[1];
            Skeleton[] onePeriodSkeletonData = new Skeleton[toIndex - fromIndex + 1];
            for (int i = fromIndex; i <= toIndex; i++)
            {
                onePeriodSkeletonData[i - fromIndex] = skeletonData[i];
            }
            CharacteristicAttributesCalculator characteristicAttributesCalculator = new CharacteristicAttributesCalculator(onePeriodSkeletonData);
            double[] FHSD = characteristicAttributesCalculator.calculateFeetHorizontalSideDistance();
            double[] FVD = characteristicAttributesCalculator.calculateFeetVerticalDistance();
            double[] FHFD = characteristicAttributesCalculator.calculateFeetHorizontalFrontDistance();
            //Scalling
            double[] scalledFHSD = Scalling.scale(FHSD, scaledDataLength);
            double[] scalledFVD = Scalling.scale(FVD, scaledDataLength);
            double[] scalledFHFD = Scalling.scale(FHFD, scaledDataLength);
            //Labeling
            int[] labels = CalculateLabels.calculateLabels(scalledFHSD, scalledFVD, scalledFHFD, DanceType.Ramnoto);
            int currentCount = 1;
            for (int i = 0; i < labels.Length-1; i++)
            {
                if (labels[i] == labels[i + 1])
                {
                    currentCount++;
                    if (currentCount == scaledDataLength / 3)
                    {
                        return true;
                    }
                }
                else
                {
                    currentCount = 1;
                }
            }
            return false;
            
        }
        ///
        /// <summary>
        /// Executes the algorythm
        /// </summary>
        /// <returns></returns>
        public AlgorythmResult execute()
        {
            Console.WriteLine("Execution of the algorithm");
            /*
            //Select one period of the dance
            //Just for testing fixed dance
            RamnotoOneDancePeriodSelector ramnotoOneDancePeriodSelector = new RamnotoOneDancePeriodSelector(skeletonData);
            int[] onePeriodIndexes = ramnotoOneDancePeriodSelector.selectOnePeriod();
            int fromIndex = onePeriodIndexes[0]; 
            int toIndex = onePeriodIndexes[1] ;
            */

            //Selection (making a decision)
            DanceType danceType;
            if (isUserOnlyStanding())
            {
                double log10RamnotoLikelihood = executeRamnotoAlgorithm();
                danceType = DanceType.NoDance;
            }
            else
            {
                double log10RamnotoLikelihood = executeRamnotoAlgorithm();
                double log106n3nLikelihood = executeO6N3NAlgorithm();
                double log10PajduskoLikelihood = executePajduskoAlgorithm();
                Console.WriteLine("Ramnoto likelihood: " + log10RamnotoLikelihood);
                Console.WriteLine("6n3n likelihood: " + log106n3nLikelihood);
                Console.WriteLine("Pajdusko likelihood: " + log10PajduskoLikelihood);
                danceType = DanceSelector.select(log10RamnotoLikelihood, log106n3nLikelihood, log10PajduskoLikelihood);
            }

            
            int fromIndex;
            if(fromIndexHashTable[danceType]!=null)
                fromIndex=(int)fromIndexHashTable[danceType];
            else
                fromIndex=(int)fromIndexHashTable[DanceType.Ramnoto];
            int toIndex;
            if(toIndexHashTable[danceType]!=null)
                toIndex=(int)toIndexHashTable[danceType];
            else
                toIndex = (int)toIndexHashTable[DanceType.Ramnoto];
            
            return new AlgorythmResult(fromIndex,toIndex,danceType);
        }
    }
}
