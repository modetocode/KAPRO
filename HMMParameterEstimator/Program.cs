using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Statistics.Models.Markov;

namespace HMMParameterEstimator
{
    public class HMMWriter
    {
        public static void writeHMMtoFile(string writeFilePath, HiddenMarkovModel HMM)
        {
            System.IO.File.WriteAllText(writeFilePath, "");
            using (System.IO.StreamWriter output = new System.IO.StreamWriter(writeFilePath, true))
            {
                double[,]A=HMM.Transitions;
                double[,]B=HMM.Emissions;
                double[] Pi = HMM.Probabilities;
                int statesNo = HMM.States;
                int symbolsNo = HMM.Symbols;
                output.WriteLine(statesNo + " " + symbolsNo);
                
                for (int i = 0; i < statesNo; i++)
                {
                    for (int j = 0; j < symbolsNo - 1; j++)
                    {
                        output.Write(String.Format("{0:F5}", B[i, j]) + " ");
                    }
                    output.Write(String.Format("{0:F5}", B[i, symbolsNo-1]));
                    output.WriteLine();
                }
                for (int i = 0; i < statesNo; i++)
                {
                    for (int j = 0; j < statesNo - 1; j++)
                    {
                        output.Write(String.Format("{0:F5}", A[i, j]) + " ");
                    }
                    output.Write(String.Format("{0:F5}", A[i, statesNo - 1]));
                    output.WriteLine();
                }
                for (int i = 0; i < statesNo-1; i++)
                {
                    output.Write(String.Format("{0:F3}", Pi[i])+" ");
                }
                output.Write(String.Format("{0:F3}", Pi[statesNo-1]));
            }
        }
    }
    public class RamnotoHMMGenerator{
        static int statesNo=64;
        static int symbolsNo=15;
        static double[,] B = new double[statesNo, 2 * symbolsNo]; // Emission probabilities
        static double[,] A = new double[statesNo, statesNo]; // Transition probabilities
        static double[] pi = new double[statesNo]; // Initial state probabilities

        /// <summary>
        /// Adds edge in the state graph
        /// </summary>
        /// <param name="fromStateIndex"></param>
        /// <param name="toStateIndex"></param>
        /// <param name="likelihood"></param>
        /// <param name="symbolIndex"></param>
        public static void addState(int fromStateIndex, int[] toStateIndex, double[] likelihood,int symbolIndex){
            
            for (int i = 0; i < toStateIndex.Length; i++)
            {
                A[fromStateIndex, toStateIndex[i]] = likelihood[i];
            }
            if (symbolIndex >= symbolsNo)
            {
                for (int i = symbolsNo; i < symbolsNo * 2; i++)
                    B[fromStateIndex, i] = 0.00001;
            }
            else
            {
                for (int i = 0; i < symbolsNo; i++)
                    B[fromStateIndex, i] = 0.00001;
            }
            B[fromStateIndex,symbolIndex]=1.0-0.00001*symbolsNo;
        }
        public static HiddenMarkovModel generateInitialHMM()
        {
            addState(0,new int[]{0,1,2,4},new double[]{0.1,0.35,0.2,0.35},11);
            addState(1, new int[] {1, 2, 4 }, new double[] {0.2, 0.4, 0.4 }, 0);
            addState(2,new int[]{2, 3,5,7},new double[]{0.4, 0.3,0.15,0.15},1);
            addState(3, new int[] { 3, 8 }, new double[] { 0.7, 0.3 }, 2);
            addState(4, new int[] { 4, 14 }, new double[] { 0.85, 0.15 }, 1);
            addState(5, new int[] { 5, 6 }, new double[] { 0.7, 0.3 }, 11);
            addState(6, new int[] { 6, 11 }, new double[] { 0.75, 0.25 }, 12);
            addState(7, new int[] { 7, 13 }, new double[] { 0.85, 0.15 }, 11);
            addState(8, new int[] { 8, 9 }, new double[] { 0.65, 0.35 }, 17);
            addState(9, new int[] { 9, 10, 15 }, new double[] { 0.65, 0.2, 0.15 }, 16);
            addState(10, new int[] { 10, 24 }, new double[] { 0.85, 0.15 }, 15);
            addState(11, new int[] { 11, 12 }, new double[] { 0.7, 0.3 }, 27);
            addState(12, new int[] { 12, 14 }, new double[] { 0.8, 0.2 }, 26);
            addState(13, new int[] { 13, 14 }, new double[] { 0.85, 0.15 }, 26);
            addState(14, new int[] { 14, 15 }, new double[] { 0.7, 0.3 }, 16);
            addState(15, new int[] { 15, 16, 18, 19, 21, 23 }, new double[] { 0.4, 0.05, 0.15, 0.2, 0.1, 0.1 }, 15);
            addState(16, new int[] { 16, 17 }, new double[] { 0.7, 0.3 }, 24);
            addState(17, new int[] { 17, 25 }, new double[] { 0.4, 0.6 }, 18);
            addState(18, new int[] { 18, 26 }, new double[] { 0.8, 0.2 }, 24);
            addState(19, new int[] { 19, 20 }, new double[] { 0.7, 0.3 }, 24);
            addState(20, new int[] { 20, 27 }, new double[] { 0.65, 0.35 }, 25);
            addState(21, new int[] { 21, 22 }, new double[] { 0.7, 0.3 }, 26);
            addState(22, new int[] { 22, 29 }, new double[] { 0.65, 0.35 }, 27);
            addState(23, new int[] { 23, 31 }, new double[] { 0.85, 0.15 }, 26);
            addState(24, new int[] { 24, 32, 34 }, new double[] { 0.2, 0.4, 0.4 }, 0);
            addState(25, new int[] { 25, 32, 34 }, new double[] { 0.2, 0.4, 0.4 }, 3);
            addState(26, new int[] { 26, 32, 34 }, new double[] { 0.7, 0.15, 0.15 }, 9);
            addState(27, new int[] { 27, 28 }, new double[] { 0.6, 0.4 }, 10);
            addState(28, new int[] { 28, 32, 34 }, new double[] { 0.5, 0.25, 0.25 }, 9);
            addState(29, new int[] { 29, 30 }, new double[] { 0.6, 0.4 }, 12);
            addState(30, new int[] { 30, 32, 34 }, new double[] { 0.5, 0.25, 0.25 }, 11);
            addState(31, new int[] { 31, 32, 34 }, new double[] { 0.7, 0.15, 0.15 }, 11);
            addState(32, new int[] { 32, 33 }, new double[] { 0.75, 0.25 }, 1);
            addState(33, new int[] { 33, 35 }, new double[] { 0.6, 0.4 },2);
            addState(34, new int[] { 34, 37 }, new double[] { 0.85, 0.15 }, 1);
            addState(35, new int[] { 35, 36 }, new double[] { 0.6, 0.4 }, 17);
            addState(36, new int[] { 36, 38, 39, 42, 44 }, new double[] { 0.5, 0.15, 0.15, 0.1, 0.1 }, 16);
            addState(37, new int[] { 37, 38, 39, 42, 44 }, new double[] { 0.8, 0.075, 0.075, 0.025, 0.025 }, 16);
            addState(38, new int[] { 38, 45 }, new double[] { 0.7, 0.3 }, 15);
            addState(39, new int[] { 39, 40, 41 }, new double[] { 0.6, 0.2, 0.2 }, 24);
            addState(40, new int[] { 40, 46 }, new double[] { 0.6, 0.4 }, 25);
            addState(41, new int[] { 41, 47 }, new double[] { 0.6, 0.4 }, 29);
            addState(42, new int[] { 42, 43 }, new double[] { 0.6, 0.4 }, 22);
            addState(43, new int[] { 43, 49 }, new double[] { 0.6, 0.4 }, 23);
            addState(44, new int[] { 44, 51 }, new double[] { 0.8, 0.2 }, 24);
            addState(45, new int[] { 45, 52, 54 }, new double[] { 0.4, 0.3, 0.3 }, 0);
            addState(46, new int[] { 46, 48 }, new double[] { 0.6, 0.4 }, 10);
            addState(47, new int[] { 47, 48 }, new double[] { 0.6, 0.4 }, 14);
            addState(48, new int[] { 48, 52, 54 }, new double[] { 0.6, 0.2, 0.2 }, 9);
            addState(49, new int[] { 49, 50 }, new double[] { 0.65, 0.35 }, 8);
            addState(50, new int[] { 50, 52, 54 }, new double[] { 0.6, 0.2, 0.2 }, 7);
            addState(51, new int[] { 51, 52, 54 }, new double[] { 0.7, 0.15, 0.15 }, 9);
            addState(52, new int[] { 52, 53 }, new double[] { 0.7, 0.3 }, 1);
            addState(53, new int[] { 53, 55 }, new double[] { 0.7, 0.3 }, 2);
            addState(54, new int[] { 54, 57 }, new double[] { 0.85, 0.15 }, 1);
            addState(55, new int[] { 55, 56 }, new double[] { 0.7, 0.3 }, 17);
            addState(56, new int[] { 56, 58, 59, 62 }, new double[] { 0.7, 0.1, 0.1, 0.1 }, 16);
            addState(57, new int[] { 57, 58, 59, 62 }, new double[] { 0.85, 0.05, 0.05, 0.05 }, 16);
            addState(58, new int[] { 58 }, new double[] { 1.0 }, 15);
            addState(59, new int[] { 59, 60, 61 }, new double[] { 0.8, 0.1, 0.1 }, 26);
            addState(60, new int[] { 60 }, new double[] { 1.0 }, 27);
            addState(61, new int[] { 61 }, new double[] { 1.0 }, 28);
            addState(62, new int[] { 62, 63 }, new double[] { 0.7, 0.3 }, 20);
            addState(63, new int[] { 63 }, new double[] { 1.0 }, 21);
            pi[0] = 0.5;
            pi[1] = 0.5;
            return new HiddenMarkovModel(A,B,pi);
        }
    }

    public class O6N3NHMMGenerator
    {
        static int statesNo = 66;
        static int symbolsNo = 15;
        static double[,] B = new double[statesNo, 2 * symbolsNo]; // Emission probabilities
        static double[,] A = new double[statesNo, statesNo]; // Transition probabilities
        static double[] pi = new double[statesNo]; // Initial state probabilities

        /// <summary>
        /// Adds edge in the state graph
        /// </summary>
        /// <param name="fromStateIndex"></param>
        /// <param name="toStateIndex"></param>
        /// <param name="likelihood"></param>
        /// <param name="symbolIndex"></param>
        public static void addState(int fromStateIndex, int[] toStateIndex, double[] likelihood, int symbolIndex)
        {

            for (int i = 0; i < toStateIndex.Length; i++)
            {
                A[fromStateIndex, toStateIndex[i]] = likelihood[i];
            }
            if (symbolIndex >= symbolsNo)
            {
                for (int i = symbolsNo; i < symbolsNo * 2; i++)
                    B[fromStateIndex, i] = 0.00001;
            }
            else
            {
                for (int i = 0; i < symbolsNo; i++)
                    B[fromStateIndex, i] = 0.00001;
            }
            B[fromStateIndex, symbolIndex] = 1.0 - 0.00001 * symbolsNo;
        }
        public static HiddenMarkovModel generateInitialHMM()
        {
            addState(0, new int[] { 0, 2}, new double[] { 0.6, 0.4 }, 11);
            addState(1, new int[] { 1, 2 }, new double[] { 0.8, 0.2 }, 0);
            addState(2, new int[] { 2, 3, 4 }, new double[] { 0.85, 0.1, 0.05 }, 1);
            addState(3, new int[] { 3, 5 }, new double[] { 0.8, 0.2 }, 2);
            addState(4, new int[] { 4, 6 }, new double[] { 0.8, 0.2 }, 11);
            addState(5, new int[] { 5, 7 }, new double[] { 0.8, 0.2 }, 17);
            addState(6, new int[] { 6, 7 }, new double[] { 0.7, 0.3 }, 26);
            addState(7, new int[] { 7, 8, 9 }, new double[] { 0.8, 0.15, 0.05 }, 16);
            addState(8, new int[] { 8, 9, 10 }, new double[] { 0.8, 0.1, 0.1 }, 15);
            addState(9, new int[] { 9, 11 }, new double[] { 0.85, 0.15 }, 24);
            addState(10, new int[] { 10, 12 }, new double[] { 0.85, 0.15 }, 26);
            addState(11, new int[] { 11, 13, 14 }, new double[] { 0.8, 0.1, 0.1 }, 9);
            addState(12, new int[] { 12, 13, 14 }, new double[] { 0.8, 0.1, 0.1 }, 11);
            addState(13, new int[] { 13, 14 }, new double[] { 0.7, 0.3 }, 0);
            addState(14, new int[] { 14, 15, 16, 19 }, new double[] { 0.8, 0.075, 0.025, 0.1 }, 1);
            addState(15, new int[] { 15, 17 }, new double[] { 0.8, 0.2 }, 2);
            addState(16, new int[] { 16, 18 }, new double[] { 0.8, 0.2 }, 11);
            addState(17, new int[] { 17, 19 }, new double[] { 0.75, 0.25 }, 17);
            addState(18, new int[] { 18, 19 }, new double[] { 0.75, 0.25 }, 26);
            addState(19, new int[] { 19, 20, 21 }, new double[] { 0.85, 0.075, 0.075 }, 16);
            addState(20, new int[] { 20, 21, 23 }, new double[] { 0.8, 0.1, 0.1 }, 15);
            addState(21, new int[] { 21, 22 }, new double[] { 0.8, 0.2 }, 24);
            addState(22, new int[] { 22, 24 }, new double[] { 0.8, 0.2 }, 9);
            addState(23, new int[] { 23, 24 }, new double[] { 0.8, 0.2 }, 0);
            addState(24, new int[] { 24, 25, 26, 29 }, new double[] { 0.85, 0.05, 0.05, 0.05 }, 1);
            addState(25, new int[] { 25, 27 }, new double[] { 0.8, 0.2 }, 2);
            addState(26, new int[] { 26, 28 }, new double[] { 0.8, 0.2 }, 11);
            addState(27, new int[] { 27, 29 }, new double[] { 0.8, 0.2 }, 17);
            addState(28, new int[] { 28, 29 }, new double[] { 0.8, 0.2 }, 26);
            addState(29, new int[] { 29, 30, 31 }, new double[] { 0.8, 0.15, 0.05 }, 16);
            addState(30, new int[] { 30, 31, 32 }, new double[] { 0.8, 0.1, 0.1 }, 15);
            addState(31, new int[] { 31, 33 }, new double[] { 0.85, 0.15 }, 24);
            addState(32, new int[] { 32, 34 }, new double[] { 0.85, 0.15 }, 26);
            addState(33, new int[] { 33, 35, 36 }, new double[] { 0.8, 0.1, 0.1 }, 9);
            addState(34, new int[] { 34, 35, 36 }, new double[] { 0.8, 0.1, 0.1 }, 11);
            addState(35, new int[] { 35, 36 }, new double[] { 0.7, 0.3 }, 0);
            addState(36, new int[] { 36, 37, 38, 41 }, new double[] { 0.8, 0.075, 0.025, 0.1 }, 1);
            addState(37, new int[] { 37, 39 }, new double[] { 0.8, 0.2 }, 2);
            addState(38, new int[] { 38, 40 }, new double[] { 0.8, 0.2 }, 11);
            addState(39, new int[] { 39, 41 }, new double[] { 0.75, 0.25 }, 17);
            addState(40, new int[] { 40, 41 }, new double[] { 0.75, 0.25 }, 26);
            addState(41, new int[] { 41, 42, 43 }, new double[] { 0.85, 0.075, 0.075 }, 16);
            addState(42, new int[] { 42, 43, 45 }, new double[] { 0.8, 0.1, 0.1 }, 15);
            addState(43, new int[] { 43, 44 }, new double[] { 0.8, 0.2 }, 24);
            addState(44, new int[] { 44, 46 }, new double[] { 0.8, 0.2 }, 9);
            addState(45, new int[] { 45, 46 }, new double[] { 0.8, 0.2 }, 0);
            addState(46, new int[] { 46, 47, 48, 51 }, new double[] { 0.85, 0.05, 0.05, 0.05 }, 1);
            addState(47, new int[] { 47, 49 }, new double[] { 0.8, 0.2 }, 2);
            addState(48, new int[] { 48, 50 }, new double[] { 0.8, 0.2 }, 11);
            addState(49, new int[] { 49, 51 }, new double[] { 0.8, 0.2 }, 17);
            addState(50, new int[] { 50, 51 }, new double[] { 0.8, 0.2 }, 26);
            addState(51, new int[] { 51, 52, 53 }, new double[] { 0.8, 0.15, 0.05 }, 16);
            addState(52, new int[] { 52, 53, 54 }, new double[] { 0.8, 0.1, 0.1 }, 15);
            addState(53, new int[] { 53, 55 }, new double[] { 0.85, 0.15 }, 24);
            addState(54, new int[] { 54, 56 }, new double[] { 0.85, 0.15 }, 26);
            addState(55, new int[] { 55, 57, 58 }, new double[] { 0.8, 0.1, 0.1 }, 9);
            addState(56, new int[] { 56, 57, 58 }, new double[] { 0.8, 0.1, 0.1 }, 11);
            addState(57, new int[] { 57, 58 }, new double[] { 0.7, 0.3 }, 0);
            addState(58, new int[] { 58, 59, 60, 63 }, new double[] { 0.8, 0.075, 0.025, 0.1 }, 1);
            addState(59, new int[] { 59, 61 }, new double[] { 0.8, 0.2 }, 2);
            addState(60, new int[] { 60, 62 }, new double[] { 0.8, 0.2 }, 11);
            addState(61, new int[] { 61, 63 }, new double[] { 0.75, 0.25 }, 17);
            addState(62, new int[] { 62, 63 }, new double[] { 0.75, 0.25 }, 26);
            addState(63, new int[] { 63, 64, 65 }, new double[] { 0.85, 0.075, 0.075 }, 16);
            addState(64, new int[] { 64 }, new double[] { 1.0 }, 15);
            addState(65, new int[] { 65 }, new double[] { 1.0 }, 26);
            
            pi[0] = 0.2;
            pi[1] = 0.4;
            pi[2] = 0.4;
            return new HiddenMarkovModel(A, B, pi);
        }
    }

    public class PajduskoHMMGenerator
    {
        static int statesNo = 63;
        static int symbolsNo = 15;
        static double[,] B = new double[statesNo, 2 * symbolsNo]; // Emission probabilities
        static double[,] A = new double[statesNo, statesNo]; // Transition probabilities
        static double[] pi = new double[statesNo]; // Initial state probabilities

        /// <summary>
        /// Adds edge in the state graph
        /// </summary>
        /// <param name="fromStateIndex"></param>
        /// <param name="toStateIndex"></param>
        /// <param name="likelihood"></param>
        /// <param name="symbolIndex"></param>
        public static void addState(int fromStateIndex, int[] toStateIndex, double[] likelihood, int symbolIndex)
        {

            for (int i = 0; i < toStateIndex.Length; i++)
            {
                A[fromStateIndex, toStateIndex[i]] = likelihood[i];
            }
            if (symbolIndex >= symbolsNo)
            {
                for (int i = symbolsNo; i < symbolsNo * 2; i++)
                    B[fromStateIndex, i] = 0.00001;
            }
            else
            {
                for (int i = 0; i < symbolsNo; i++)
                    B[fromStateIndex, i] = 0.00001;
            }
            B[fromStateIndex, symbolIndex] = 1.0 - 0.00001 * symbolsNo;
        }
        public static HiddenMarkovModel generateInitialHMM()
        {
            addState(0, new int[] { 0, 2 }, new double[] { 0.6, 0.4 }, 0);
            addState(1, new int[] { 1, 4 }, new double[] { 0.8, 0.2 }, 11);
            addState(2, new int[] { 2, 3, 4 }, new double[] { 0.6, 0.1, 0.3 }, 5);
            addState(3, new int[] { 3, 4 }, new double[] { 0.6, 0.4 }, 6);
            addState(4, new int[] { 4, 5, 6, 7 }, new double[] { 0.8, 0.066, 0.066, 0.066 }, 13);
            addState(5, new int[] { 5, 8 }, new double[] { 0.7, 0.3 }, 1);
            addState(6, new int[] { 6, 9 }, new double[] { 0.8, 0.2 }, 11);
            addState(7, new int[] { 7, 10 }, new double[] { 0.8, 0.2 }, 0);
            addState(8, new int[] { 8, 11 }, new double[] { 0.6, 0.4 }, 16);
            addState(9, new int[] { 9, 11 }, new double[] { 0.6, 0.4 }, 26);
            addState(10, new int[] { 10, 11 }, new double[] { 0.6, 0.4 }, 15);
            addState(11, new int[] { 11, 12, 13 }, new double[] { 0.85, 0.075, 0.075 }, 22);
            addState(12, new int[] { 12, 13, 14 }, new double[] { 0.8, 0.1, 0.1 }, 23);
            addState(13, new int[] { 13, 14 }, new double[] { 0.8, 0.2 }, 29);
            addState(14, new int[] { 14, 15, 16 }, new double[] { 0.8, 0.1, 0.1 }, 24);
            addState(15, new int[] { 15, 17 }, new double[] { 0.6, 0.4 }, 9);
            addState(16, new int[] { 16, 17 }, new double[] { 0.6, 0.4 }, 3);
            addState(17, new int[] { 17, 18, 19, 20, 22 }, new double[] { 0.85, 0.0375, 0.0375, 0.0375, 0.0375 }, 5);
            addState(18, new int[] { 18, 19, 20, 22 }, new double[] { 0.8, 0.066, 0.066, 0.066 }, 6);
            addState(19, new int[] { 19, 20, 22 }, new double[] { 0.85, 0.075, 0.075 }, 13);
            addState(20, new int[] { 20, 21, 25 }, new double[] { 0.75, 0.0125, 0.0125 }, 1);
            addState(21, new int[] { 21, 24 }, new double[] { 0.6, 0.4 }, 2);
            addState(22, new int[] { 22, 23 }, new double[] { 0.75, 0.25 }, 0);
            addState(23, new int[] { 23, 26 }, new double[] { 0.6, 0.4 }, 15);
            addState(24, new int[] { 24, 25 }, new double[] { 0.5, 0.5 }, 17);
            addState(25, new int[] { 25, 26 }, new double[] { 0.65, 0.35 }, 16);
            addState(26, new int[] { 26, 27, 29 }, new double[] { 0.85, 0.075, 0.075 }, 22);
            addState(27, new int[] { 27, 28 }, new double[] { 0.85, 0.15 }, 29);
            addState(28, new int[] { 28, 29 }, new double[] { 0.8, 0.2 }, 22);
            addState(29, new int[] { 29, 30 }, new double[] { 0.75, 0.25 }, 15);
            addState(30, new int[] { 30, 31 }, new double[] { 0.6, 0.4 }, 0);
            addState(31, new int[] { 31, 32, 33, 35 }, new double[] { 0.85, 0.05, 0.05, 0.05 }, 11);
            addState(32, new int[] { 32, 34 }, new double[] { 0.65, 0.35 }, 3);
            addState(33, new int[] { 33, 35 }, new double[] { 0.65, 0.35 }, 7);
            addState(34, new int[] { 34, 35 }, new double[] { 0.5, 0.5 }, 18);
            addState(35, new int[] { 35, 36 }, new double[] { 0.6, 0.4 }, 26);
            addState(36, new int[] { 36, 37, 38, 39 }, new double[] { 0.85, 0.05, 0.05, 0.05 }, 15);
            addState(37, new int[] { 37, 40 }, new double[] { 0.8, 0.2 }, 24);
            addState(38, new int[] { 38, 41 }, new double[] { 0.8, 0.2 }, 16);
            addState(39, new int[] { 39, 42 }, new double[] { 0.8, 0.2 }, 20);
            addState(40, new int[] { 40, 43 }, new double[] { 0.6, 0.4 }, 9);
            addState(41, new int[] { 41, 43 }, new double[] { 0.6, 0.4 }, 1);
            addState(42, new int[] { 42, 43 }, new double[] { 0.6, 0.4 }, 5);
            addState(43, new int[] { 43, 44 }, new double[] { 0.8, 0.2 }, 0);
            addState(44, new int[] { 44, 45, 46 }, new double[] { 0.85, 0.075, 0.075 }, 11);
            addState(45, new int[] { 45, 47 }, new double[] { 0.8, 0.2 }, 12);
            addState(46, new int[] { 46, 48 }, new double[] { 0.8, 0.2 }, 7);
            addState(47, new int[] { 47, 49 }, new double[] { 0.6, 0.4 }, 27);
            addState(48, new int[] { 48, 49 }, new double[] { 0.6, 0.4 }, 26);
            addState(49, new int[] { 49, 50 }, new double[] { 0.9, 0.1 }, 28);
            addState(50, new int[] { 50, 51 }, new double[] { 0.9, 0.1 }, 15);
            addState(51, new int[] { 51, 52, 53 }, new double[] { 0.7, 0.15, 0.15 }, 0);
            addState(52, new int[] { 52, 53, 54, 57 }, new double[] { 0.8, 0.066, 0.066, 0.066 }, 7);
            addState(53, new int[] { 53, 56, 57 }, new double[] { 0.8, 0.1, 0.1 }, 14);
            addState(54, new int[] { 54, 55 }, new double[] { 0.75, 0.25 }, 8);
            addState(55, new int[] { 55, 57 }, new double[] { 0.75, 0.25 }, 7);
            addState(56, new int[] { 56, 57 }, new double[] { 0.75, 0.25 }, 9);
            addState(57, new int[] { 57, 58 }, new double[] { 0.85, 0.15 }, 0);
            addState(58, new int[] { 58, 59 }, new double[] { 0.7, 0.3 }, 15);
            addState(59, new int[] { 59, 60, 61, 62 }, new double[] { 0.85, 0.05, 0.05, 0.05 }, 26);
            addState(60, new int[] { 60 }, new double[] { 1.0 }, 27);
            addState(61, new int[] { 61 }, new double[] { 1.0 }, 15);
            addState(62, new int[] { 62 }, new double[] { 1.0 }, 22);
            pi[0] = 0.5;
            pi[1] = 0.5;
            return new HiddenMarkovModel(A, B, pi);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            HiddenMarkovModel hmm=RamnotoHMMGenerator.generateInitialHMM();
            HMMWriter.writeHMMtoFile(@"C:\Users\DarkoLaptop2\Dropbox\Magisterska\calculations\hmm centroids\ramnotoInitialHMM.txt", hmm);
            hmm = O6N3NHMMGenerator.generateInitialHMM();
            HMMWriter.writeHMMtoFile(@"C:\Users\DarkoLaptop2\Dropbox\Magisterska\calculations\hmm centroids\o6n3nInitialHMM.txt", hmm);
            hmm = PajduskoHMMGenerator.generateInitialHMM();
            HMMWriter.writeHMMtoFile(@"C:\Users\DarkoLaptop2\Dropbox\Magisterska\calculations\hmm centroids\pajduskoInitialHMM.txt", hmm);
            
            Console.WriteLine("Finished");
            Console.ReadKey();
        }
    }
}
