using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPFitLib;

namespace PollMethodUsingThreads
{
    public class Entry
    {
        public int time { get; set; }
        public double value { get; set; }
        public bool isMaximum { get; set; }
        public Entry(int time, double value,bool isMaximum)
        {
            this.time = time;
            this.value = value;
            this.isMaximum = isMaximum;
        }
    }

    public class ExtremaFinder
    {
        public static List<Entry> getExtremes(int maxToler, double[] data)
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
            double extremumValue=data[0];
            int extremumIndex=0;
            for (int i = 0; i < data.Length-1; i++)
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

                        successiveCounter=0;
                    }
                    else
                    {
                        successiveCounter++;
                        if (successiveCounter == maxToler)
                        {
                            //extremes.Add(new Entry(i - successiveCounter  + 2, data[i - successiveCounter  + 2]));
                            extremes.Add(new Entry(extremumIndex, extremumValue,false));
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
                        extremes.Add(new Entry(extremumIndex,extremumValue,true));
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
        public static List<Entry> findGlobalExtrema(int insideRatio, double[] data, int maxToler)
        {
            return removeLocalExtrema(insideRatio, getExtremes(maxToler, data), data);
        }
        public static List<Entry> removeLocalExtrema(int insideRatio, List<Entry> allExtrema, double[] data)
        {
            int n=data.Length;
            double[] extCopy = new double[n];
            for (int i = 0; i < n; i++)
                extCopy[i] = data[i];
            Array.Sort(extCopy);
            double length = (1.0 - insideRatio / 100.0) / 2.0;
            int downIndex = (int)Math.Floor(n * length);
            int topIndex = (int)Math.Ceiling(n - n * length);
            return removeLocalExtrema2(extCopy[topIndex], extCopy[downIndex], allExtrema);
        }
        
        //new Algorithm 2
        public static List<Entry> removeLocalExtrema2(double minimalMaxValue, double minimalMinValue, List<Entry> allExtrema)
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
                } else
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
            for(int i=0;i<n;i++)
                if(!isRemoved[i]){
                    startIndex=i;
                    break;
                }
            if(startIndex==-1){
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
                } else
                    if (!e1.isMaximum && !e2.isMaximum)
                    {
                        //ima barem 1 ekstrem megju dvata
                        if (startIndex != currentIndex - 1)
                        {
                            double max = allExtrema.ElementAt(startIndex+1).value;
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
            for(int i=0;i<n;i++)
                if (!isRemoved[i])
                {
                    globalExtrema.Add(allExtrema.ElementAt(i));
                    count++;
                }
            return globalExtrema;
        }
        public static List<Entry> removeLocalExtrema(double minimalMaxValue, double minimalMinValue, List<Entry> allExtrema)
        {
            //Console.WriteLine(minimalMaxValue + " T" + minimalMinValue);
            int n = allExtrema.Count;
            bool[] isRemoved = new bool[n];
            int i = 0;
            foreach (Entry entry in allExtrema)
            {
                if (entry.value <= minimalMinValue)
                {

                } else
                    if (entry.value >= minimalMaxValue)
                    {

                    }
                    else
                    {
                        isRemoved[i] = true;
                    }
                /*if (entry.value <= minimalMinValue || entry.value >= minimalMaxValue)
                {
                    //Console.WriteLine(entry.time + " " + entry.value + " ");
                    isRemoved[i] = false;
                }
                else
                {
                    isRemoved[i] = true;
                    //Console.WriteLine("Isfrlena vrednost =>" + entry.time + " " + entry.value + " ");
                }*/
                i++;
            }
            List<Entry> globalExtrema=new   List<Entry>();
            for(i=0;i<n-1;i++)
                if (isRemoved[i] && isRemoved[i + 1])
                {
                    //Two extremas for discarding
                    //Last chance to stay: Measure and compare the difference between the extremes
                    /*
                    if (i > 0 && i + 2 < n)
                    { //one left and one right extrema
                        
                        int dif1 = allExtrema.ElementAt(i).time - allExtrema.ElementAt(i-1).time;
                        int dif2 = allExtrema.ElementAt(i+1).time - allExtrema.ElementAt(i).time;
                        Console.WriteLine(dif1 + " " + dif2);
                        if (dif1 <= dif2 && (dif1 / 1.0) / dif2 >= 0.8)
                        {
                            globalExtrema.Add(allExtrema.ElementAt(i));
                            globalExtrema.Add(allExtrema.ElementAt(i+1));
                        } else 
                        if (dif2 <= dif1 && (dif2 / 1.0) / dif1 >= 0.8)
                        {
                            globalExtrema.Add(allExtrema.ElementAt(i));
                            globalExtrema.Add(allExtrema.ElementAt(i+1));
                        }
                    } else
                        if (i + 3 < n)
                        {
                            //two right extrema
                            int dif1 = allExtrema.ElementAt(i+3).time - allExtrema.ElementAt(i +2).time;
                            int dif2 = allExtrema.ElementAt(i + 1).time - allExtrema.ElementAt(i).time;
                            if (dif1 <= dif2 && (dif1 / 1.0) / dif2 >= 0.8)
                            {
                                globalExtrema.Add(allExtrema.ElementAt(i));
                                globalExtrema.Add(allExtrema.ElementAt(i + 1));
                            }
                            else
                                if (dif2 <= dif1 && (dif2 / 1.0) / dif1 >= 0.8)
                                {
                                    globalExtrema.Add(allExtrema.ElementAt(i));
                                    globalExtrema.Add(allExtrema.ElementAt(i + 1));
                                }
                        }
                        if (i > 1)
                        {
                            //two left extrema
                            int dif1 = allExtrema.ElementAt(i - 1).time - allExtrema.ElementAt(i - 2).time;
                            int dif2 = allExtrema.ElementAt(i + 1).time - allExtrema.ElementAt(i).time;
                            if (dif1 <= dif2 && (dif1 / 1.0) / dif2 >= 0.8)
                            {
                                globalExtrema.Add(allExtrema.ElementAt(i));
                                globalExtrema.Add(allExtrema.ElementAt(i + 1));
                            }
                            else
                                if (dif2 <= dif1 && (dif2 / 1.0) / dif1 >= 0.8)
                                {
                                    globalExtrema.Add(allExtrema.ElementAt(i));
                                    globalExtrema.Add(allExtrema.ElementAt(i + 1));
                                }
                        }*/
                    i++;
                    if (i == n - 2)
                    {
                        isRemoved[n - 1] = false;
                    }
                }
                else
                {
                    globalExtrema.Add(allExtrema.ElementAt(i));
                }
            if(n>0&&!isRemoved[n-1])
                globalExtrema.Add(allExtrema.ElementAt(n - 1));
            if (n > 1 && isRemoved[n - 1] && !isRemoved[n - 2])
            {
                globalExtrema.Add(allExtrema.ElementAt(n - 1));
            }

            return globalExtrema;
        }

        //Write the extrema information for the given extremes and data in a file
        //FORMAT
        //FIRST ALL DATA CHARACTERISTIC 8
        //SECOND ALL EXTREMAS 
        //THIRD FILTERED EXTREMAS
        //FORTH THE SELECTED PERIOD FOR ANALYSIS
        public static void writeToFile(string path, double[] data, List<Entry> extremes,List<Entry> allExtremes, int coverageRatio, int maxTolerance)
        {
            System.IO.File.WriteAllText(path, "");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                file.WriteLine(data.Length);
                for(int i=0;i<data.Length;i++)
                    file.WriteLine(String.Format("{0:F10}", data[i]));
                file.WriteLine(allExtremes.Count);
                foreach (Entry entry in allExtremes)
                {
                    file.WriteLine((entry.time+1) + " " + String.Format("{0:F10}", entry.value));
                }
                file.WriteLine(extremes.Count);
                foreach (Entry entry in extremes)
                {
                    file.WriteLine((entry.time+1) + " " + String.Format("{0:F10}", entry.value));
                }
                List<Entry> tempoExtremes = ExtremaFinder.calculateTempo(data,coverageRatio,maxTolerance);
                if (tempoExtremes == null)
                {
                    List<Entry> tempoMiddleExtremes = ExtremaFinder.calculateTempoFromMiddle(extremes);
                    if (tempoMiddleExtremes == null)
                    {
                        file.WriteLine("0");
                    }
                    else
                    {
                        file.WriteLine((tempoMiddleExtremes.ElementAt(2).time - tempoMiddleExtremes.ElementAt(0).time));
                        for (int i = tempoMiddleExtremes.ElementAt(0).time; i <= tempoMiddleExtremes.ElementAt(2).time; i++)
                        {
                            file.WriteLine((i + 1) + " " + String.Format("{0:F10}", data[i]));
                        }

                    }
                }
                else
                {
                    file.WriteLine((tempoExtremes.ElementAt(2).time - tempoExtremes.ElementAt(0).time));
                    for (int i = tempoExtremes.ElementAt(0).time; i <= tempoExtremes.ElementAt(2).time; i++)
                    {
                        file.WriteLine((i + 1) + " " + String.Format("{0:F10}", data[i]));
                    }
                }
            } 
        }

        public static double[] fitLeastSquare(double[] data){
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

        public static double[] getCharacteristic2(string fileName)
        {
            string[] lines = System.IO.File.ReadAllLines(fileName);
            int n = lines.Length;
            double[] characteristic2 = new double[n];
            int i = 0;
            foreach (string line in lines)
            {
                string[] part = line.Split(' ');
                characteristic2[i++] = double.Parse(part[1]);
            }
            return characteristic2;
        }

        public static double[] getCharacteristicFVD(string fileName)
        {
            string[] lines = System.IO.File.ReadAllLines(fileName);
            int n = lines.Length;
            double[] characteristic2 = new double[n];
            int i = 0;
            foreach (string line in lines)
            {
                string[] part = line.Split(' ');
                characteristic2[i++] = double.Parse(part[2]);
            }
            return characteristic2;
        }

        public static double[] getCharacteristicFHFD(string fileName)
        {
            string[] lines = System.IO.File.ReadAllLines(fileName);
            int n = lines.Length;
            double[] characteristic2 = new double[n];
            int i = 0;
            foreach (string line in lines)
            {
                string[] part = line.Split(' ');
                characteristic2[i++] = double.Parse(part[6]);
            }
            return characteristic2;
        }
        public static double[] getCharacteristic7(string fileName)
        {
            string[] lines = System.IO.File.ReadAllLines(fileName);
            int n = lines.Length;
            double[] characteristic7 = new double[n];
            int i = 0;
            foreach (string line in lines)
            {
                string[] part = line.Split(' ');
                if (i == 0)
                    characteristic7[i] = double.Parse(part[5]);
                else
                    characteristic7[i] = characteristic7[i - 1] + double.Parse(part[5]);
                i++;
            }
            return characteristic7;
        }
        public static double[] getCharacteristic7(string fileName,int fromIndex,int length)
        {
            string[] lines = System.IO.File.ReadAllLines(fileName);
            int n = lines.Length;
            double[] characteristic7 = new double[length];
            if (fromIndex < 0)
                fromIndex = 0;
            string[] part = lines[fromIndex].Split(' ');
            characteristic7[0] = double.Parse(part[5]);
            for (int i = 1; i < length; i++)
            {
                part = lines[fromIndex+i].Split(' ');
                characteristic7[i] = characteristic7[i - 1] + double.Parse(part[5]);
            }
           
            return characteristic7;
        }
        public static void calculateExtremesAndWriteToFiles(string fileName, int coverageRatio, int maxTolerance)
        {
            double[] characteristic7 = getCharacteristic7(fileName);
            int n = characteristic7.Length;

            double[] characteristic8 = fitLeastSquare(characteristic7);
            List<Entry> extremi = ExtremaFinder.findGlobalExtrema(coverageRatio, characteristic8, maxTolerance);
            List<Entry> allExtrema = ExtremaFinder.getExtremes(maxTolerance, characteristic8);
            string writePath = fileName.Substring(0, fileName.Length - 8) + " ekstremi.txt";
            ExtremaFinder.writeToFile(writePath, characteristic8, extremi, allExtrema,coverageRatio,maxTolerance);
                
        }

        //returns a list of 3 extremes
        public static List<Entry> calculateTempo(double[] data, int coverageRatio, int maxTolerance)
        {
            List<Entry> extremes = ExtremaFinder.findGlobalExtrema(coverageRatio, data, maxTolerance);
            List<Entry> result = new List<Entry>();
            //special cases
            if (extremes.Count <= 1)
            {
                result.Add(new Entry(0,data[0],false));
                result.Add(new Entry(data.Length/2,data[data.Length/2],true));
                result.Add(new Entry(data.Length-1,data[data.Length-1],false));
                return result;
            } else if (extremes.Count==2) {
                //two global extremes
                Entry left=extremes.ElementAt(0);
                Entry right=extremes.ElementAt(1);
                if (!left.isMaximum && !right.isMaximum)
                {
                    //two minimums
                    result.Add(left);
                    result.Add(new Entry((left.time + right.time) / 2, data[(left.time + right.time) / 2], true));
                    result.Add(right);
                } else
                    if (!left.isMaximum && right.isMaximum)
                    {
                        //left minimum right maximum
                        int difLen = right.time - left.time;
                        double min = double.MaxValue;
                        int minIndex = data.Length-1;
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
                        double min=double.MaxValue;
                        int minIndex=0;
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
                        result.Add(new Entry(minIndex,data[minIndex],false));
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
                if (!extremes.ElementAt(i).isMaximum && extremes.ElementAt(i + 1).isMaximum && !extremes.ElementAt(i+2).isMaximum)
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
                int righLength = data.Length-extremes.ElementAt(firstIndex + 2).time;
                Entry leftExtreme;
                Entry middleExtreme;
                Entry rightExtreme;
                if (leftLength >= righLength)
                {
                    //more data on the left side
                    middleExtreme = extremes.ElementAt(firstIndex);
                    rightExtreme = extremes.ElementAt(firstIndex + 1);
                    int neededLength = extremes.ElementAt(firstIndex + 2).time - extremes.ElementAt(firstIndex + 1).time;
                    if (leftLength >= neededLength)
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
                    if (righLength >= neededLength)
                    {
                        //there is data
                        rightExtreme = new Entry(extremes.ElementAt(firstIndex + 2).time + neededLength, data[extremes.ElementAt(firstIndex + 2).time + neededLength], false);
                    }
                    else
                    {
                        //there isnt enought data 
                        rightExtreme = new Entry(data.Length-1, data[data.Length-1], false);
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

        //returns the 3 extremes in one period of the dance//
        //the extremes are calculated by closeness to the average between all neighbouring extremes
        public static List<Entry> calculateTempoFromStart(List<Entry> extremes) {
            double avg = 0.0;
            int n = extremes.Count;
            int count = 0;
            for (int i = 0; i < n - 2; i++)
            {
                if (!extremes.ElementAt(i).isMaximum)
                {
                    avg += extremes.ElementAt(i + 2).time - extremes.ElementAt(i).time;
                    count++;
                }
            }
            if(count==0)
                return null;
            avg /= count;
            double minDif = avg;
            int startIndex=-1;
            for (int i = 0; i < n - 2; i++)
            {
                if ((!extremes.ElementAt(i).isMaximum)&&(Math.Abs(extremes.ElementAt(i + 2).time - extremes.ElementAt(i).time)-avg)<=minDif)
                {
                    minDif = Math.Abs(extremes.ElementAt(i + 2).time - extremes.ElementAt(i).time-avg);
                    startIndex = i;
                }
            }
            List<Entry> period=new List<Entry>();
            period.Add(extremes.ElementAt(startIndex));
            period.Add(extremes.ElementAt(startIndex+1));
            period.Add(extremes.ElementAt(startIndex+2));
            return period;
        }

        public static List<Entry> calculateTempoFromMiddle(List<Entry> extremes)
        {
            double avg = 0.0;
            int n = extremes.Count;
            int count = 0;
            for (int i = 0; i < n - 2; i++)
            {
                if (extremes.ElementAt(i).isMaximum)
                {
                    avg += extremes.ElementAt(i + 2).time - extremes.ElementAt(i).time;
                    count++;
                }
            }
            if (count == 0)
                return null;
            avg /= count;
            double minDif = avg;
            int startIndex = -1;
            for (int i = 0; i < n - 2; i++)
            {
                if ((extremes.ElementAt(i).isMaximum) && (Math.Abs(extremes.ElementAt(i + 2).time - extremes.ElementAt(i).time) - avg) <= minDif)
                {
                    minDif = Math.Abs(extremes.ElementAt(i + 2).time - extremes.ElementAt(i).time - avg);
                    startIndex = i;
                }
            }
            List<Entry> period = new List<Entry>();
            period.Add(extremes.ElementAt(startIndex));
            period.Add(extremes.ElementAt(startIndex + 1));
            period.Add(extremes.ElementAt(startIndex + 2));
            return period;
        }
    }

    enum Ora { Ramnoto, Pajdusko, o6n3n};
}
