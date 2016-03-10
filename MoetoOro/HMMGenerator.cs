using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Statistics.Models.Markov;

namespace MoetoOro
{
    class HMMGenerator
    {
        string parametersFilePath;
        HiddenMarkovModel HMM;

        public HMMGenerator(string parametersFilePath)
        {
            this.parametersFilePath = parametersFilePath;
            string[] lines = System.IO.File.ReadAllLines(parametersFilePath);
            string[] linePart = lines[0].Split(' ');
            int states = int.Parse(linePart[0]);
            int symbols = int.Parse(linePart[1]);
            double[,] B = new double[states, symbols]; // Emission probabilities
            double[,] A = new double[states, states]; // Transition probabilities
            double[] pi = new double[states]; // Initial state probabilities
            for (int j = 0; j < states; j++)
            {
                linePart = lines[j + 1].Split(' ');
                for (int k = 0; k < symbols; k++)
                {
                    B[j, k] = double.Parse(linePart[k]);
                }
            }
            for (int j = 0; j < states; j++)
            {
                linePart = lines[j + states + 1].Split(' ');
                for (int k = 0; k < states; k++)
                {
                    A[j, k] = double.Parse(linePart[k]);
                }
            }
            linePart = lines[2 * states + 1].Split(' ');
            for (int j = 0; j < states; j++)
            {
                pi[j] = double.Parse(linePart[j]);
            }
            HMM = new HiddenMarkovModel(A, B, pi);
        }

        public double evaluateSequence(int[] labels)
        {
            return HMM.Evaluate(labels);
        }
    }
}
