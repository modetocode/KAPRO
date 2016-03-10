using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using MPFitLib;

namespace MoetoOro
{
    public class Entry
    {
        public int time { get; set; }
        public double value { get; set; }
        public bool isMaximum { get; set; }
        public Entry(int time, double value, bool isMaximum)
        {
            this.time = time;
            this.value = value;
            this.isMaximum = isMaximum;
        }
    }
    class RamnotoOneDancePeriodSelector : OneDancePeriodSelector
    {
        public RamnotoOneDancePeriodSelector(Skeleton[] skeleton): base(skeleton,4)
        {
        }

        /// <summary>
        /// Selects one period of the dance Ramnoto and returns an array of two elements: the fromIndex and toIndex of the selected period
        /// </summary>
        /// <returns></returns>
        public new int[] selectOnePeriod(){
            int[] dataIndex = new int[2];
            CharacteristicAttributesCalculator calculator=new CharacteristicAttributesCalculator(skeletonData);
            List<Entry> selectedPeriod=base.selectOnePeriod();
            double[] FHSD = calculator.calculateFeetHorizontalSideDistance();
            int fromIndex = selectedPeriod.ElementAt(0).time;
            int toIndex = selectedPeriod.ElementAt(2).time;
            for (int i = 0; i < 4; i++)
            {
                if (fromIndex == 0)
                    break;
                fromIndex--;
            }
            for (int i = 0; i < 4; i++)
            {
                if (toIndex == 0)
                    break;
                toIndex--;
            }
            dataIndex[0] = fromIndex;
            dataIndex[1] = toIndex;
            int consecInc = 0;
            //check to find 3 consequtive increases in the FSHD attribute
            //when you find then a new fromIndex has been found
            for (int i = fromIndex; i < fromIndex + 10; i++)
            {
                if(i+1>=FHSD.Length)
                    break;
                if (FHSD[i + 1] - FHSD[i] > 0)
                {
                    consecInc++;
                    if (consecInc == 3)
                    {
                        
                        dataIndex[0] = i - 2;
                        break;
                    }

                }
                else
                    consecInc = 0;
                
            }
            //check to find 3 consequtive increases in the FSHD attribute
            //when you find then a new toIndex has been found
            consecInc = 0;
            for (int i = toIndex; i < toIndex + 10; i++)
            {
                if (i+1 >= FHSD.Length)
                    break;
                if (FHSD[i + 1] - FHSD[i] > 0)
                {
                    consecInc++;
                    if (consecInc == 3)
                    {
                        dataIndex[1] = i - 2;
                        break;
                    }

                }
                else
                    consecInc = 0;
            }
            return dataIndex;
        }
    }

    class O6N3NOneDancePeriodSelector : OneDancePeriodSelector
    {
        public O6N3NOneDancePeriodSelector(Skeleton[] skeleton): base(skeleton, 4)
        {
        }

        /// <summary>
        /// Selects one period of the dance Ramnoto and returns an array of two elements: the fromIndex and toIndex of the selected period
        /// </summary>
        /// <returns></returns>
        public new int[] selectOnePeriod()
        {
            int[] dataIndex = new int[2];
            CharacteristicAttributesCalculator calculator = new CharacteristicAttributesCalculator(skeletonData);
            List<Entry> selectedPeriod = base.selectOnePeriod();
            double[] FHSD = calculator.calculateFeetHorizontalSideDistance();
            int fromIndex = selectedPeriod.ElementAt(0).time;
            int toIndex = selectedPeriod.ElementAt(2).time;
            for (int i = 0; i < 4; i++)
            {
                if (fromIndex == 0)
                    break;
                fromIndex--;
            }
            for (int i = 0; i < 4; i++)
            {
                if (toIndex == 0)
                    break;
                toIndex--;
            }
            dataIndex[0] = fromIndex;
            dataIndex[1] = toIndex;
            int consecInc = 0;
            //check to find 3 consequtive increases in the FSHD attribute
            //when you find then a new fromIndex has been found
            for (int i = fromIndex; i < fromIndex + 10; i++)
            {
                if (i + 1 >= FHSD.Length)
                    break;
                if (FHSD[i + 1] - FHSD[i] > 0)
                {
                    consecInc++;
                    if (consecInc == 3)
                    {
                        dataIndex[0] = i - 2;
                        break;
                    }

                }
                else
                    consecInc = 0;
            }
            //check to find 3 consequtive increases in the FSHD attribute
            //when you find then a new toIndex has been found
            consecInc = 0;
            for (int i = toIndex; i < toIndex + 10; i++)
            {
                if (i + 1 >= FHSD.Length)
                    break;
                if (FHSD[i + 1] - FHSD[i] > 0)
                {
                    consecInc++;
                    if (consecInc == 3)
                    {
                        dataIndex[1] = i - 2;
                        break;
                    }

                }
                else
                    consecInc = 0;
            }
            return dataIndex;
        }
    }

    class PajduskoOneDancePeriodSelector : OneDancePeriodSelector
    {
        public PajduskoOneDancePeriodSelector(Skeleton[] skeleton) : base(skeleton, 8)
        {
        }

        /// <summary>
        /// Selects one period of the dance Pajdusko and returns an array of two elements: the fromIndex and toIndex of the selected period
        /// </summary>
        /// <returns></returns>
        public new int[] selectOnePeriod()
        {
            int[] dataIndex = new int[2];
            CharacteristicAttributesCalculator calculator = new CharacteristicAttributesCalculator(skeletonData);
            List<Entry> selectedPeriod = base.selectOnePeriod();
            double[] FVD = calculator.calculateFeetVerticalDistance();
            int fromIndex = selectedPeriod.ElementAt(0).time;
            int toIndex = selectedPeriod.ElementAt(2).time;
            for (int i = 0; i < 4; i++)
            {
                if (fromIndex == 0)
                    break;
                fromIndex--;
            }
            for (int i = 0; i < 4; i++)
            {
                if (toIndex == 0)
                    break;
                toIndex--;
            }
            dataIndex[0] = fromIndex;
            dataIndex[1] = toIndex;
            int consecInc = 0;
            //check to find 3 consequtive increases in the FSHD attribute
            //when you find then a new fromIndex has been found
            for (int i = fromIndex; i < fromIndex + 10; i++)
            {
                if (i + 1 >= FVD.Length)
                    break;
                if (FVD[i+1]>0 && (FVD[i + 1] - FVD[i]) > 0)
                {
                    consecInc++;
                    if (consecInc == 3)
                    {
                        dataIndex[0] = i - 2;
                        break;
                    }

                }
                else
                    consecInc = 0;
            }
            //check to find 3 consequtive increases in the FSHD attribute
            //when you find then a new toIndex has been found
            consecInc = 0;
            for (int i = toIndex; i < toIndex + 10; i++)
            {
                if (i + 1 >= FVD.Length)
                    break;
                if (FVD[i + 1] > 0 && (FVD[i + 1] - FVD[i]) > 0)
                {
                    consecInc++;
                    if (consecInc == 3)
                    {
                        dataIndex[1] = i - 2;
                        break;
                    }

                }
                else
                    consecInc = 0;
            }
            return dataIndex;
        }
    }

    class OneDancePeriodSelector
    {
        /// <summary>
        /// The max tolerance of consequtive opposite-monotonic data 
        /// </summary>
        private int maxTolerance;
        /// <summary>
        /// The percentage of  the data where extremes will be classified as local ones
        /// </summary>
        private const int insideRatio = 70;

        /// <summary>
        /// The skeleton data
        /// </summary>
        protected Skeleton[] skeletonData;

        /// <summary>
        /// Calculator of the characteristic attributes
        /// </summary>
        private CharacteristicAttributesCalculator characteristicAttributesCalculator;

        public OneDancePeriodSelector(Skeleton[] skeletonData,int maxTolerance)
        {
            this.skeletonData = skeletonData;
            this.characteristicAttributesCalculator = new CharacteristicAttributesCalculator(this.skeletonData);
            this.maxTolerance = maxTolerance;
        }

        public List<Entry> selectOnePeriod()
        {
            double[] CTSM = characteristicAttributesCalculator.calculateCumulativeTorsoSideMovement();
            double[] fittedCTSM = fitLeastSquare(CTSM);
            return selectPeriod(fittedCTSM, insideRatio, maxTolerance);
        }


        private double[] fitLeastSquare(double[] data)
        {
            int n = data.Length;
            double[] x = new double[n];
            double[] y = new double[n];
            double[] ey = new double[n];
            for (int j = 0; j < n; j++)
            {
                x[j] = j;
                y[j] = data[j];
                ey[j] = 0.001;
            }
            CustomUserVariable v = new CustomUserVariable();
            v.X = x;
            v.Y = y;
            v.Ey = ey;

            double[] p = { 0.0, 0.0 };
            int status;

            mp_result result = new mp_result(2);
            status = MPFit.Solve(ForwardModels.LinFunc, n, 2, p, null, null, v, ref result);
            p[1] *= -1.0;

            double[] resultData = new double[n];
            for (int j = 0; j < n; j++)
            {
                resultData[j] = data[j] - (p[0] + p[1] * j);
            }
            return resultData;
        }
        /// <summary>
        /// Method for calculation of global extrema (removed local extrema) for Fitted CTSM data
        /// </summary>
        /// <param name="insideRatio"></param>
        /// <param name="data"></param>
        /// <param name="maxToler"></param>
        /// <returns></returns>
        private List<Entry> findGlobalExtrema(int insideRatio, double[] data, int maxToler)
        {
            return removeLocalExtrema(insideRatio, getExtremes(maxToler, data), data);
        }

        /// <summary>
        /// 
        /// Method for preparation for removal of local extrema from all of the extremes (calculation of important parameters)
        /// </summary>
        /// <param name="insideRatio"></param>
        /// <param name="allExtrema"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<Entry> removeLocalExtrema(int insideRatio, List<Entry> allExtrema, double[] data)
        {
            int n = data.Length;
            double[] extCopy = new double[n];
            for (int i = 0; i < n; i++)
                extCopy[i] = data[i];
            Array.Sort(extCopy);
            double length = (1.0 - insideRatio / 100.0) / 2.0;
            int downIndex = (int)Math.Floor(n * length);
            int topIndex = (int)Math.Ceiling(n - n * length);
            return removeLocalExtrema2(extCopy[topIndex], extCopy[downIndex], allExtrema);
        }


        /// <summary>
        /// Method for removal of local extrema for given data and calculated parameters
        /// </summary>
        /// <param name="minimalMaxValue"></param>
        /// <param name="minimalMinValue"></param>
        /// <param name="allExtrema"></param>
        /// <returns></returns>
        private static List<Entry> removeLocalExtrema2(double minimalMaxValue, double minimalMinValue, List<Entry> allExtrema)
        {
            int n = allExtrema.Count;
            bool[] isRemoved = new bool[n];
            for (int i = 0; i < n; i++)
            {
                isRemoved[i] = true;
            }
            for (int i = 0; i < n; i++)
            {
                Entry currentExtrema = allExtrema.ElementAt(i);
                if (currentExtrema.value >= minimalMaxValue)
                {
                    double max = currentExtrema.value;
                    int maxIndex = i;
                    for (int j = i; j < n; j++)
                    {
                        if (allExtrema.ElementAt(j).value < minimalMaxValue)
                            break;
                        else
                        {
                            if (allExtrema.ElementAt(j).value > max)
                            {
                                max = allExtrema.ElementAt(j).value;
                                maxIndex = j;
                            }
                        }
                    }
                    isRemoved[maxIndex] = false;
                }
                else
                    if (currentExtrema.value <= minimalMinValue)
                    {
                        double min = currentExtrema.value;
                        int minIndex = i;
                        for (int j = i; j < n; j++)
                        {
                            if (allExtrema.ElementAt(j).value > minimalMinValue)
                                break;
                            else
                            {
                                if (allExtrema.ElementAt(j).value > min)
                                {
                                    min = allExtrema.ElementAt(j).value;
                                    minIndex = j;
                                }
                            }
                        }
                        isRemoved[minIndex] = false;
                    }
            }
            int startIndex = -1;
            for (int i = 0; i < n; i++)
                if (!isRemoved[i])
                {
                    startIndex = i;
                    break;
                }
            if (startIndex == -1)
            {
                //nema nitu eden preostanat ekstrem
                return new List<Entry>();
            }
            if (startIndex > 0 && allExtrema.ElementAt(startIndex).isMaximum)
            {
                //ima barem eden ekstrem pred nego oznacen za otfrlanje
                double min = allExtrema.ElementAt(0).value;
                int minIndex = 0;
                for (int j = 1; j < startIndex; j++)
                    if (allExtrema.ElementAt(j).value < min)
                    {
                        min = allExtrema.ElementAt(j).value;
                        minIndex = j;
                    }
                isRemoved[minIndex] = false;
            }
            while (true)
            {
                int currentIndex = startIndex + 1;
                while (currentIndex < n && isRemoved[currentIndex])
                {
                    currentIndex++;
                }
                if (currentIndex >= n)
                    break;
                //dva ekstrema koi ne se za otfrlanje se najdeni
                //proveri go tipot na ekstremite
                Entry e1 = allExtrema.ElementAt(startIndex);
                Entry e2 = allExtrema.ElementAt(currentIndex);
                if (e1.isMaximum && e2.isMaximum)
                {
                    //ima barem 1 ekstrem megju dvata
                    if (startIndex != (currentIndex - 1))
                    {
                        double min = allExtrema.ElementAt(startIndex + 1).value;
                        int minIndex = startIndex + 1;
                        for (int i = startIndex + 2; i < currentIndex; i++)
                        {
                            if (allExtrema.ElementAt(i).value < min)
                            {
                                min = allExtrema.ElementAt(i).value;
                                minIndex = i;
                            }
                        }
                        isRemoved[minIndex] = false;
                    }
                }
                else
                    if (!e1.isMaximum && !e2.isMaximum)
                    {
                        //ima barem 1 ekstrem megju dvata
                        if (startIndex != currentIndex - 1)
                        {
                            double max = allExtrema.ElementAt(startIndex + 1).value;
                            int maxIndex = startIndex + 1;
                            for (int i = startIndex + 2; i < currentIndex - 1; i++)
                            {
                                if (allExtrema.ElementAt(i).value < max)
                                {
                                    max = allExtrema.ElementAt(i).value;
                                    maxIndex = i;
                                }
                            }
                            isRemoved[maxIndex] = false;
                        }
                    }
                    else
                    {
                        //megu dva razlicni tipa na ekstremi
                        //momentalno ne cepkaj so vakvi ekstremi
                    }
                startIndex = currentIndex;
            }
            int count = 0;
            List<Entry> globalExtrema = new List<Entry>();
            for (int i = 0; i < n; i++)
                if (!isRemoved[i])
                {
                    globalExtrema.Add(allExtrema.ElementAt(i));
                    count++;
                }
            return globalExtrema;
        }



        /// <summary>
        /// Method for calculation of all extremes for a Fitted CTSM attribute
        /// </summary>
        /// <param name="maxToler"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<Entry> getExtremes(int maxToler, double[] data)
        {
            List<Entry> extremes = new List<Entry>();
            int increasingRate;
            int cInc = 0;
            int cDec = 0;
            for (int i = 1; i < maxToler; i++)
            {
                if (data[i] - data[i - 1] >= 0)
                    cInc++;
                else
                    cDec++;
            }
            if (cInc >= cDec)
                increasingRate = 1;
            else
                increasingRate = -1;
            int successiveCounter = 1;
            double extremumValue = data[0];
            int extremumIndex = 0;
            for (int i = 0; i < data.Length - 1; i++)
            {
                int currentRate;
                if (data[i + 1] - data[i] >= 0)
                    currentRate = 1;
                else
                    currentRate = -1;
                if (currentRate == 1)
                {
                    if (increasingRate == 1)
                    {
                        if (data[i + 1] > extremumValue)
                        {
                            extremumValue = data[i + 1];
                            extremumIndex = i + 1;
                        }

                        successiveCounter = 0;
                    }
                    else
                    {
                        successiveCounter++;
                        if (successiveCounter == maxToler)
                        {
                            //extremes.Add(new Entry(i - successiveCounter  + 2, data[i - successiveCounter  + 2]));
                            extremes.Add(new Entry(extremumIndex, extremumValue, false));
                            increasingRate = 1;
                            successiveCounter = 1;
                            i -= maxToler;
                        }

                    }
                }
                else if (increasingRate == -1)
                {
                    if (data[i + 1] < extremumValue)
                    {
                        extremumValue = data[i + 1];
                        extremumIndex = i + 1;
                    }
                    successiveCounter = 0;
                }
                else
                {
                    successiveCounter++;
                    if (successiveCounter == maxToler)
                    {
                        //extremes.Add(new Entry(i - successiveCounter + 2, data[i - successiveCounter + 2]));
                        extremes.Add(new Entry(extremumIndex, extremumValue, true));
                        increasingRate = -1;
                        successiveCounter = 1;
                        i -= maxToler;
                    }
                }
            }
            /*Console.WriteLine(successiveCounter);
            if (successiveCounter >= maxToler)
            {
                extremes.Add(new Entry(data.Length-1-successiveCounter+2,data[data.Length-1-successiveCounter+2]));
            }*/
            return extremes;
        }

        //returns a list of 3 extremes
        private List<Entry> selectPeriod(double[] data, int coverageRatio, int maxTolerance)
        {
            List<Entry> extremes = findGlobalExtrema(coverageRatio, data, maxTolerance);
            List<Entry> result = new List<Entry>();
            //special cases
            if (extremes.Count <= 1)
            {
                result.Add(new Entry(0, data[0], false));
                result.Add(new Entry(data.Length / 2, data[data.Length / 2], true));
                result.Add(new Entry(data.Length - 1, data[data.Length - 1], false));
                return result;
            }
            else if (extremes.Count == 2)
            {
                //two global extremes
                Entry left = extremes.ElementAt(0);
                Entry right = extremes.ElementAt(1);
                if (!left.isMaximum && !right.isMaximum)
                {
                    //two minimums
                    result.Add(left);
                    result.Add(new Entry((left.time + right.time) / 2, data[(left.time + right.time) / 2], true));
                    result.Add(right);
                }
                else
                    if (!left.isMaximum && right.isMaximum)
                    {
                        //left minimum right maximum
                        int difLen = right.time - left.time;
                        double min = double.MaxValue;
                        int minIndex = data.Length - 1;
                        for (int i = right.time + difLen - 5; i <= right.time + difLen + 5; i++)
                        {
                            if (i < data.Length)
                            {
                                if (data[i] < min)
                                {
                                    min = data[i];
                                    minIndex = i;
                                }
                            }
                        }
                        result.Add(left);
                        result.Add(right);
                        result.Add(new Entry(minIndex, data[minIndex], false));
                    }
                    else if (left.isMaximum && !right.isMaximum)
                    {
                        //left maximum right minimum
                        int difLen = right.time - left.time;
                        double min = double.MaxValue;
                        int minIndex = 0;
                        for (int i = left.time - difLen - 5; i <= left.time - difLen + 5; i++)
                        {
                            if (i >= 0)
                            {
                                if (data[i] < min)
                                {
                                    min = data[i];
                                    minIndex = i;
                                }
                            }
                        }
                        result.Add(new Entry(minIndex, data[minIndex], false));
                        result.Add(left);
                        result.Add(right);


                    }
                    else
                    {
                        //two maximums
                        result.Add(new Entry(0, data[0], false));
                        result.Add(new Entry(data.Length / 2, data[data.Length / 2], true));
                        result.Add(new Entry(data.Length - 1, data[data.Length - 1], false));
                    }
                return result;
            }
            double avg = 0.0;
            int n = extremes.Count;
            int count = 0;
            for (int i = 0; i < n - 2; i++)
            {
                if (!extremes.ElementAt(i).isMaximum && extremes.ElementAt(i + 1).isMaximum && !extremes.ElementAt(i + 2).isMaximum)
                {
                    avg += extremes.ElementAt(i + 2).time - extremes.ElementAt(i).time;
                    count++;
                }
            }
            if (count == 0)
            {
                //no extremas of type min max min
                int firstIndex = -1;
                //count if there is max min max
                for (int i = 0; i < n - 2; i++)
                {
                    if (extremes.ElementAt(i).isMaximum && !extremes.ElementAt(i + 1).isMaximum && extremes.ElementAt(i + 2).isMaximum)
                    {
                        firstIndex = i;
                        break;
                    }
                }
                if (firstIndex == -1)
                {
                    //return the full interval
                    result.Add(new Entry(0, data[0], false));
                    result.Add(new Entry(data.Length / 2, data[data.Length / 2], true));
                    result.Add(new Entry(data.Length - 1, data[data.Length - 1], false));
                    return result;
                }
                int leftLength = extremes.ElementAt(firstIndex).time;
                int righLength = data.Length - extremes.ElementAt(firstIndex + 2).time;
                Entry leftExtreme;
                Entry middleExtreme;
                Entry rightExtreme;
                if (leftLength >= righLength)
                {
                    //more data on the left side
                    middleExtreme = extremes.ElementAt(firstIndex);
                    rightExtreme = extremes.ElementAt(firstIndex + 1);
                    int neededLength = extremes.ElementAt(firstIndex + 2).time - extremes.ElementAt(firstIndex + 1).time;
                    if (leftLength > neededLength)
                    {
                        //there is data
                        leftExtreme = new Entry(extremes.ElementAt(firstIndex).time - neededLength, data[extremes.ElementAt(firstIndex).time - neededLength], false);
                    }
                    else
                    {
                        //there isnt enought data 
                        leftExtreme = new Entry(0, data[0], false);
                    }
                }
                else
                {
                    //more data on the right side
                    leftExtreme = extremes.ElementAt(firstIndex + 1);
                    middleExtreme = extremes.ElementAt(firstIndex + 2);
                    int neededLength = extremes.ElementAt(firstIndex + 1).time - extremes.ElementAt(firstIndex).time;
                    if (righLength > neededLength)
                    {
                        //there is data
                        rightExtreme = new Entry(extremes.ElementAt(firstIndex + 2).time + neededLength, data[extremes.ElementAt(firstIndex + 2).time + neededLength], false);
                    }
                    else
                    {
                        //there isnt enought data 
                        rightExtreme = new Entry(data.Length - 1, data[data.Length - 1], false);
                    }
                }

                result.Add(leftExtreme);
                result.Add(middleExtreme);
                result.Add(rightExtreme);
                return result;
            }
            avg /= count;
            double minDif = avg;
            int startIndex = -1;
            for (int i = 0; i < n - 2; i++)
            {
                if ((!extremes.ElementAt(i).isMaximum && extremes.ElementAt(i + 1).isMaximum && !extremes.ElementAt(i + 2).isMaximum)
                 && (Math.Abs(extremes.ElementAt(i + 2).time - extremes.ElementAt(i).time) - avg) <= minDif)
                {
                    minDif = Math.Abs(extremes.ElementAt(i + 2).time - extremes.ElementAt(i).time - avg);
                    startIndex = i;
                }
            }
            result.Add(extremes.ElementAt(startIndex));
            result.Add(extremes.ElementAt(startIndex + 1));
            result.Add(extremes.ElementAt(startIndex + 2));
            return result;
        }
    }
}
