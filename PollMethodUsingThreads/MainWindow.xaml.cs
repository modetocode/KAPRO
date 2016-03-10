using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.ComponentModel;

namespace PollMethodUsingThreads
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Member Variables
        private KinectSensor _Kinect;
        private BackgroundWorker _Worker;
        private long totalTime;
        private long startTime;
        private long difference;
        #endregion Member Variables
        public MainWindow()
        {
            InitializeComponent();
            this.difference = 0;
            DiscoverKinectSensor();
            this.totalTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            this.startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            
            this._Worker = new BackgroundWorker();
            this._Worker.DoWork += Worker_DoWork;
            this._Worker.RunWorkerAsync();
            this.Unloaded += (s, e) => { this._Worker.CancelAsync(); };
            Console.WriteLine("START");

            //fajlovi
            System.IO.File.WriteAllText(@"D:\FINKI\IX Semestar\Kinect data\dat.txt", "");
            
            
        }
        private long initialTime;
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            initialTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            if (worker != null)
            {
                while (!worker.CancellationPending)
                {
                    //DiscoverKinectSensor();
                    //PollColorImageStream();
                    
                    PollSkeletonStream(100);
                    
                }
            }
        }

        private void DiscoverKinectSensor()
        {
            if (this._Kinect != null && this._Kinect.Status != KinectStatus.Connected)
            {
                this._Kinect = null;
            }
            if (this._Kinect == null)
            {
                this._Kinect = KinectSensor.KinectSensors
                .FirstOrDefault(x => x.Status == KinectStatus.Connected);
                if (this._Kinect != null)
                {
                    //this._Kinect.ColorStream.Enable();
                    this._Kinect.SkeletonStream.Enable();
                    this._Kinect.Start();
                    //ColorImageStream colorStream = this._Kinect.ColorStream;
                    /*
                    this.ColorImageElement.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this._ColorImageBitmap = new WriteableBitmap(colorStream.FrameWidth,
                        colorStream.FrameHeight,
                        96, 96, PixelFormats.Bgr32,
                        null);
                        this._ColorImageBitmapRect = new Int32Rect(0, 0, colorStream.FrameWidth,
                        colorStream.FrameHeight);
                        this._ColorImageStride = colorStream.FrameWidth *
                        colorStream.FrameBytesPerPixel;
                        this._ColorImagePixelData = new byte[colorStream.FramePixelDataLength];
                        this.ColorImageElement.Source = this._ColorImageBitmap;
                    }));*/
                }
            }
        }
        /*
       private void PollSkeletonStream(int frameRateMs)
       {

           if (this._Kinect == null)
           {
           }
           else
           {
               long startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
               SkeletonFrame frame = this._Kinect.SkeletonStream.OpenNextFrame(Convert.ToInt32(frameRateMs - difference));
               frame.FrameNumber
               Console.WriteLine(" Frame: " + (currentTime - totalTime) + " " + (currentTime - startTime) + " " + difference);
               difference=frameRateMs
           }
       }*/
        
        private void PollSkeletonStream(int frameRateMs)
       {
           if (this._Kinect == null)
           {
           }
           else
           {
               //Console.Write("Getting a new frame");
               long startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
               SkeletonFrame frame = this._Kinect.SkeletonStream.OpenNextFrame(Convert.ToInt32(frameRateMs));
               /*if (frame == null)
                   Console.WriteLine("Empty Frame");
               else
                   Console.Write("Frame nmbr: "+frame.FrameNumber+" "+frame.Timestamp+" ");
                */
               long fetchedTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
               if (frame == null)
               {
                   difference += fetchedTime - startTime;
               }
               else
               {
                   difference += fetchedTime - startTime;
                   Console.WriteLine(difference);
                   difference = frameRateMs - (fetchedTime - startTime);
                   if (difference < 0)
                       difference = 0;
                   System.Threading.Thread.Sleep(Convert.ToInt32(difference));
               }
               /*
 //              Console.Write("Fetched a new frame");
               //check how much time fetching the skeleton data was spent
               //Console.Write((fetchedTime - startTime));
               //if the time needed exceeds a little from the required FrameRate 
               if ((fetchedTime - startTime) > frameRateMs)
               {
                   //Console.WriteLine("FASTER!!! YOU ARE LATE "+(fetchedTime-startTime));
                   long differenceNow=fetchedTime-startTime;
                   //mark that there is a delay this that and the next fetching of data needs to be faster
                   difference = fetchedTime - startTime-frameRateMs;
                   if (difference >= frameRateMs)
                       difference = frameRateMs / 2;
                   //if an initialization of the sensor or another important event has happened then ignore it
               } else
               //if the fetching the skeleton data is less that required the thread has to wait
               {
                   System.Threading.Thread.Sleep(Convert.ToInt32(frameRateMs-(fetchedTime-startTime)));
                   difference = 0;
               }
               long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
               //print the information
               //Console.WriteLine(" Frame: " + (currentTime - totalTime) + " " + (currentTime - startTime) + " " + difference);
               */
               if (frame != null)
               {
                   prepareForDrawing(frame);
                   //Console.WriteLine
                   frame.Dispose();
               }
               //else
                 //  Console.WriteLine("Empty Frame");
           }

       }
        /*
        private void PollSkeletonStream()
        {
            if (this._Kinect == null)
            {
                //Notify that there are no available sensors.
            }
            else
            {
                SkeletonFrame frame;
                if(difference<100)
                    frame = this._Kinect.SkeletonStream.OpenNextFrame((100 - Convert.ToInt32(difference)) % 100);
                else
                    frame = this._Kinect.SkeletonStream.OpenNextFrame(100);
                long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                if (100 - currentTime + startTime - difference <= 0)
                {
                    Console.Write("Overflow");
                    difference = Math.Abs(100 - currentTime + startTime);
                }
                else
                {

                    System.Threading.Thread.Sleep(100 - Convert.ToInt32((currentTime - startTime) - difference));
                    difference = 0;
                }
                currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                Console.Write(" Frame: " + (currentTime - totalTime) + " " + (currentTime - startTime)+" "+difference);
                startTime = currentTime;
                if (frame != null)
                {
                    Console.WriteLine(" Not-null");
                    frame.Dispose();
                } else
                    Console.WriteLine(" Null");
            }
        }*/


        #region Drawing_Skeleton
        /// <summary>
        /// Niza od nizi na tocki na spojuvanje na skeletot na korisnikot
        /// </summary>
        private static readonly JointType[][] SkeletonSegmentRuns = new JointType[][]
        {
            new JointType[] 
            { 
                JointType.Head, JointType.ShoulderCenter, JointType.HipCenter 
            },
            new JointType[] 
            { 
                JointType.HandLeft, JointType.WristLeft, JointType.ElbowLeft, JointType.ShoulderLeft,
                JointType.ShoulderCenter,
                JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight
            },
            new JointType[]
            {
                JointType.FootLeft, JointType.AnkleLeft, JointType.KneeLeft, JointType.HipLeft,
                JointType.HipCenter,
                JointType.HipRight, JointType.KneeRight, JointType.AnkleRight, JointType.FootRight
            }
        };

        private SkeletonPoint vektorskiProizvod(SkeletonPoint vector1, SkeletonPoint vector2)
        {
            SkeletonPoint result = new SkeletonPoint();
            result.X = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
            result.Y = vector1.Z * vector2.X - vector1.X * vector2.Z;
            result.Z = vector1.X * vector2.Y - vector1.Y * vector2.X;
            return result;
        }
        private double skalarenProizvod(SkeletonPoint point1, SkeletonPoint point2)
        {
            return (point1.X * point2.X)  + (point1.Y * point2.Y) + (point1.Z * point2.Z) ;
        }

        private double efklidovoRastojanie(SkeletonPoint point1, SkeletonPoint point2)
        {
            return Math.Sqrt((point1.X - point2.X) * (point1.X - point2.X) + (point1.Y - point2.Y) * (point1.Y - point2.Y) + (point1.Z - point2.Z) * (point1.Z - point2.Z));
        }
        private double norma(SkeletonPoint vektor)
        {
            return Math.Sqrt(vektor.X * vektor.X + vektor.Y * vektor.Y + vektor.Z * vektor.Z);
        }
        private double angleBetweenPoints(SkeletonPoint p1, SkeletonPoint p2,SkeletonPoint p3)
        {
            SkeletonPoint v1 = napraviVektor(p2, p1);
            SkeletonPoint v2 = napraviVektor(p2, p3);
            return Math.Acos(skalarenProizvod(v1,v2)/(norma(v1)*norma(v2)))*180/Math.PI;
        }
        private SkeletonPoint napraviVektor(SkeletonPoint point1, SkeletonPoint point2)
        {
            SkeletonPoint vector = new SkeletonPoint();
            vector.X = point2.X - point1.X;
            vector.Y = point2.Y - point1.Y;
            vector.Z = point2.Z - point1.Z;
            return vector;
        }
        /// <summary>
        /// Niza so site prateni skeleti od senzorot
        /// </summary>
        private Skeleton[] skeletons = new Skeleton[0];

        /// <summary>
        /// Skelet na predavacot
        /// </summary>
        //private Skeleton nearestSkeleton;

        /// <summary>
        /// Vreme do koga skeletot ke bide oboen crveno
        /// </summary>
        private DateTime highlightTime = DateTime.MinValue;
        private SkeletonPoint previousHipCenterPoint;
        Boolean hasStartedGesture = false;
        int counter = 20;
        private void prepareForDrawing(SkeletonFrame frame)
        {
            // Alociraj memorija za nizata pri prvo povikuvanje
            if (this.skeletons.Length != frame.SkeletonArrayLength)
            {
                this.skeletons = new Skeleton[frame.SkeletonArrayLength];
            }
            // Zemi gi podatocite za skeletite na korisnicite
            frame.CopySkeletonDataTo(this.skeletons);

            // Pretpostavi deka nema najblizok skelet
            //var newNearestId = -1;
            //var newNearestIndex = -1;
            //var nearestDistance2 = double.MaxValue;
            var i = 0;
            // Pomini gi site skeleti

            foreach (var skeleton in this.skeletons)
            {
                // Proveruvaj samo skeleti koi se prateni od senzorot
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    hasStartedGesture = true;
                   /* SkeletonPoint handLeftPoint = skeleton.Joints[JointType.HandLeft].Position;
                    SkeletonPoint handRightPoint = skeleton.Joints[JointType.HandRight].Position;
                    double handDistance = efklidovoRastojanie(handLeftPoint, handRightPoint);
                    if (handDistance < 0.12 &&counter>20)
                        if (hasStartedGesture)
                        {
                            hasStartedGesture = false;
                            counter = 0;
                        }
                        else
                        {
                            hasStartedGesture = true;
                            counter = 0;
                        }*/
                    //else
                    //Console.WriteLine("{" + String.Format("{0:F10} {1:D}", handDistance,counter) + "}");
                    counter++;
                    if (!hasStartedGesture)
                        return;
                    //Ankle Positions
                    SkeletonPoint ankleRightPoint = skeleton.Joints[JointType.AnkleRight].Position;
                    SkeletonPoint ankleLeftPoint = skeleton.Joints[JointType.AnkleLeft].Position;
                    //Hip Positions
                    SkeletonPoint hipRightPoint = skeleton.Joints[JointType.HipRight].Position;
                    SkeletonPoint hipLeftPoint = skeleton.Joints[JointType.HipLeft].Position;
                    SkeletonPoint hipCenterPoint = skeleton.Joints[JointType.HipCenter].Position;
                    //Spine position
                    SkeletonPoint spinePoint = skeleton.Joints[JointType.Spine].Position;
                    //Knees position
                    SkeletonPoint kneeLeftPoint = skeleton.Joints[JointType.KneeLeft].Position;
                    SkeletonPoint kneeRightPoint = skeleton.Joints[JointType.KneeRight].Position;
                    SkeletonPoint shoulderCenterPoint = skeleton.Joints[JointType.ShoulderCenter].Position;
                    //Karakteristika 1
                    // Horizontalna rashirenost megju gluzdovite (zglobovite) na dvete noze
                    SkeletonPoint vektor1k1 = napraviVektor(ankleLeftPoint, ankleRightPoint);
                    SkeletonPoint vektor2k1 = napraviVektor(hipLeftPoint, hipRightPoint);
                    double karakteristika1 = skalarenProizvod(vektor1k1, vektor2k1) / norma(vektor2k1);

                    //Karakteristika 2
                    //Dvizenje na teloto
                    double karakteristika2 = 0.0;
                    if (previousHipCenterPoint != null)
                    {
                        SkeletonPoint vektor1k2 = napraviVektor(previousHipCenterPoint, hipCenterPoint);
                        SkeletonPoint vektor2k2 = napraviVektor(hipLeftPoint, hipRightPoint);
                        karakteristika2 = skalarenProizvod(vektor1k2, vektor2k2) / norma(vektor2k2);               
                    }
                    previousHipCenterPoint = hipCenterPoint;

                    //Karakteristika 3
                    //Vertikalno Rastojanie megju gluzdovite (zglobovite) na dvete noze
                    
                    SkeletonPoint vektor1k3 = napraviVektor(ankleLeftPoint,ankleRightPoint);
                    SkeletonPoint vektor2k3 = napraviVektor(spinePoint,shoulderCenterPoint);
                    double karakteristika3 = skalarenProizvod(vektor1k3, vektor2k3) / norma(vektor2k3);

                    //Karakteristika 4
                    //Vertikalno Rastojanie megju dvete kolena na nozete
                    SkeletonPoint vektor1k4 = napraviVektor(kneeLeftPoint, kneeRightPoint);
                    double karakteristika4 = skalarenProizvod(vektor1k4, vektor2k3) / norma(vektor2k3);
                    
                    //Karakteristika 5
                    //Horizontalno Rastojanie megju dvete kolena na nozete
                    double karakteristika5 = skalarenProizvod(vektor1k4, vektor2k1) / norma(vektor2k1);

                    //Karakteristika 6
                    //Efklidovo rastojanie megju dvata gluzda na nozete
                    double karakteristika6 = efklidovoRastojanie(ankleLeftPoint, ankleRightPoint);

                    //Karakteristika 7
                    //Horizontalna ispruzenost na nozete
                    SkeletonPoint vektor1k7 = vektorskiProizvod(napraviVektor(hipLeftPoint, hipRightPoint), napraviVektor(hipCenterPoint, spinePoint));
                    double karakteristika7 = skalarenProizvod(vektor1k1, vektor1k7) / norma(vektor1k7);

                     //Karakteristika 8
                    //Horizontalna ispruzenost na kolenata
                    SkeletonPoint vektor1k8 = vektorskiProizvod(napraviVektor(hipLeftPoint, hipRightPoint), napraviVektor(hipCenterPoint, spinePoint));
                    double karakteristika8 = skalarenProizvod(napraviVektor(kneeLeftPoint, kneeRightPoint), vektor1k8) / norma(vektor1k8);

                    //vektor1k3.Z = 0;
                    //vektor2k3.Z=0;
                    //double karakteristika4 = skalarenProizvod(vektor1k3, vektor2k3) / norma(vektor2k3);
                    //Console.WriteLine("Koleno:" + kneeLeftPoint.X + " " + kneeLeftPoint.Y + " " + kneeLeftPoint.Z + " " + kneeRightPoint.X + " " + kneeRightPoint.Y + " " + kneeRightPoint.Z + " ");
                    //double karakteristika3 = kneeRightPoint.Y - kneeLeftPoint.Y;
                   // double karakteristika3 = angleBetweenPoints(hipRightPoint, kneeRightPoint, ankleRightPoint);
                    //Pecatenje
                    /*
                    Console.WriteLine(String.Format("{0:F10} {1:F10} {2:F10} {3:F10} {4:F10} {5:F10} {6:F10}", karakteristika6, karakteristika1, karakteristika3, karakteristika5, karakteristika4, karakteristika2,karakteristika7));
                   // Console.WriteLine("{" + String.Format("{0:F10},{1:F10}", karakteristika3, karakteristika4) + "},");
                    //Console.WriteLine(String.Format("[ {0:F3} {1:F3} {2:F3} ] [{3:F3} {4:F3} {5:F3}]", hipLeftPoint.X,hipLeftPoint.Y,hipLeftPoint.Z, hipRightPoint.X,hipRightPoint.Y,hipRightPoint.Z));
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"D:\Podatoci za magisterska\podatoci.txt", true))
                    {
                        file.WriteLine(String.Format("{0:F10} {1:F10} {2:F10} {3:F10} {4:F10} {5:F10} {6:F10}", karakteristika6, karakteristika1, karakteristika3, karakteristika5, karakteristika4, karakteristika2,karakteristika7));
                    }
                    */
                    long nowTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    long dif = nowTime - initialTime;
                    //TSM FHSD FVD FHFD AHSD AVD AHFD
                    Console.WriteLine(String.Format("{0:F10} {1:F10} {2:F10} {3:F10} {4:F10} {5:F10} {6:F10} {7:F10}", karakteristika2, karakteristika1, karakteristika3, karakteristika7,karakteristika5,karakteristika4,karakteristika8,dif));
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"D:\Podatoci za magisterska\podatoci.txt", true))
                    {
                        file.WriteLine(String.Format("{0:F10} {1:F10} {2:F10} {3:F10} {4:F10} {5:F10} {6:F10} {7:F10}", karakteristika2, karakteristika1, karakteristika3, karakteristika7, karakteristika5, karakteristika4, karakteristika8,dif));
                    } 
                    break;
                }
                i++;
            }
        }
        
        #endregion

    }
}
