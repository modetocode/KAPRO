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
using Microsoft.Samples.Kinect.WpfViewers;
using Microsoft.Kinect;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace MoetoOro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //inicialization of components of the form
            InitializeComponent();
            //when the main window is loaded try to connect with a kinect sensor
            Loaded += this.OnMainWindowLoaded;
        }

        #region Kinect Sensor Initialization
        /// <summary>
        /// When the main window is loaded try to connect with a kinect sensor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            //attach a handeler when a sensor is changed
            kinectSensorChooser.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser_KinectSensorChanged);
            //attach a handler when the visibility of the notification form is changed
            kinectSensorChooser.IsVisibleChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser_IsVisibleChanged);
        }

        /// <summary>
        /// Form actions when a change in the notification form controlling the sensor has occured
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void kinectSensorChooser_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //check the visibility of the kinect notification form
            if (((KinectSensorChooser)sender).Visibility == System.Windows.Visibility.Hidden)
            {
                //the notification form is now hidden 
                //the sensor is now working and collecting data
            }
            else
            {
                //the notification form has shown
                //a problem occured with the sensor
            }
        }


        private KinectSensor newSensor;
        /// <summary>
        /// This method is executed when a new kinect sensor has been attached 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void kinectSensorChooser_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //check first if the old sensor is stopped and released 
            KinectSensor oldSensor = (KinectSensor)e.OldValue;
            StopKinect(oldSensor);

            //remember the new sensor
            newSensor = (KinectSensor)e.NewValue;

            if (newSensor != null)
            {
                //now enabled the streams that this application will use
                //enable skeleton stream
                newSensor.SkeletonStream.Enable();
                try
                {
                    //try to start using the sensor
                    newSensor.Start();
                    this._Worker = new BackgroundWorker();
                    this._Worker.DoWork += Worker_DoWork;
                    this._Worker.WorkerSupportsCancellation=true;
                    this._Worker.RunWorkerAsync();
                }

                catch (System.IO.IOException)
                {
                    //the sensor is used in another application
                    //show notification
                    kinectSensorChooser.AppConflictOccurred();
                }
            }
        }

        /// <summary>
        /// Release the kinect sensor so other applications can use it
        /// </summary>
        private void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                this._Worker.CancelAsync();
                sensor.Stop();
                
            }
        }

        #endregion

        #region Working with Threads
        /// <summary>
        /// Worker working in background for skeleton polling
        /// </summary>
        private BackgroundWorker _Worker;
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (worker != null)
            {
                while (!worker.CancellationPending)
                {
                    PollSkeletonStream(100);
                }
            }
        }

        /// <summary>
        /// Thread for drawing the skeleton of the last period
        /// </summary>
        private BackgroundWorker onePeriodDrawingWorker;

        /// <summary>
        /// Object for storing the skeleton data of the last period of the dance
        /// </summary>
        private KinectMovementDataGatherer lastPeriodGatherer;
        /// <summary>
        /// Indexes indicating the start and the end of the last period of the dance
        /// </summary>
        private int lastPeriodFrom, lastPeriodTo;

        /// <summary>
        /// The work procedure that the thread for the last period will execute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnePeriodWorker_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (worker != null)
            {
                while(!worker.CancellationPending)
                {
                    System.Windows.Threading.Dispatcher canvasDispatcher2 = OnePeriodCanvas.Dispatcher;
                    Skeleton[] skeleton=lastPeriodGatherer.getCorrectedSkeletonData();
                    for (int i = lastPeriodFrom; i < lastPeriodTo; i++)
                    {
                        if (!worker.CancellationPending)
                        {
                            canvasDispatcher2.Invoke(new Action(() => drawSkeletonOnCanvas(skeleton[i], OnePeriodCanvas, Brushes.Black)));
                            System.Threading.Thread.Sleep(100);
                            canvasDispatcher2.Invoke(new Action(() => clearCanvas(OnePeriodCanvas)));
                            
                        }
                    }
                }
            }
        }

        #endregion

        #region Skeleton Stream Manipulation

        /// <summary>
        /// Array for storing the tracked skeletons 
        /// </summary>
        private Skeleton[] skeletons;

        /// <summary>
        /// Variable for indication if a skeleton is tracked
        /// </summary>
        private bool isSkeletonTracked = false;

        /// <summary>
        /// The skeleton data will of the tracked skeleton will be stored here
        /// </summary>
        private KinectMovementDataGatherer kinectMovementDataGatherer;
        
        /// <summary>
        /// When the process of recognition takes place pause the process of data gathering 
        /// </summary>
        private bool isDataProcessed = false;

        /// <summary>
        /// Minimal number of data required for starting of the algorythm
        /// </summary>
        private const int minimalDataRequired = 20;

        /// <summary>
        /// Counter of consequitive null frames when a skeleton is tracked
        /// </summary>
        private int nullFrameCounter = 0;
        /// <summary>
        /// The maximal number of null frames before a user's tracking is dropped
        /// </summary>
        private static int maxNullFrames = 5;

        /// <summary>
        /// The current number of consequtive frames that a tracked user has made the arm gesture
        /// </summary>
        private int armPoseCounter = 0;

        //The minimal number of consequtive frames before a user using the arm gesture will stop the data collection
        private static int minArmCounter = 10;

        /// <summary>
        /// Polling of skeleton stream
        /// </summary>
        /// <param name="frameRateMs"></param>
        private void PollSkeletonStream(int frameRateMs)
        {
            //remember the time before before fetching a frame
            long startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            SkeletonFrame frame = this.newSensor.SkeletonStream.OpenNextFrame(Convert.ToInt32(frameRateMs));
            //now remember the time after the frame is fetched
            long fetchedTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            //pause the thread from getting the next data 
            if ((fetchedTime - startTime) < frameRateMs)
            {
                System.Threading.Thread.Sleep(Convert.ToInt32(frameRateMs - (fetchedTime - startTime)));
                    
            }
            
            //check if a frame has been found
            if (frame != null)
            {
                //reset the counter of consequtive null frames
                nullFrameCounter = 0;

                //check if the recognition algorithm is not currently in progress
                if (!isDataProcessed)
                {
                    if (this.skeletons == null || frame.SkeletonArrayLength != this.skeletons.Length)
                        skeletons = new Skeleton[frame.SkeletonArrayLength];
                    //copy the skeleton data locally
                    frame.CopySkeletonDataTo(skeletons);

                    //check if a skeleton is already tracked
                    if (isSkeletonTracked)
                    {
                        //check if there is new data for the already tracked skeleton
                        bool isThere = false;
                        Skeleton trackedSkeleton = null;
                        foreach (Skeleton skeleton in skeletons)
                        {
                            if (skeleton.TrackingState == SkeletonTrackingState.Tracked && skeleton.TrackingId.Equals(kinectMovementDataGatherer.getTrackedSkeletonId()))
                            {
                                isThere = true;
                                trackedSkeleton = skeleton;
                                break;
                            }
                        }
                        //check if a tracked skeleton with the same tracking id has been found
                        if (isThere)
                        {
                            //the skeleton has been found 

                            //check if the user has made the arm gesture for stopping the gathering of data
                            if (kinectMovementDataGatherer.isEndConditionTrue(trackedSkeleton))
                            {
                                armPoseCounter++;
                                if (armPoseCounter == minArmCounter)
                                {
                                    //the minimal number of frames has been reached
                                    //stop collecting data
                                    isSkeletonTracked = false;
                                    //Now envoke the process of recognition
                                    //pause the process of data gathering
                                    isDataProcessed = true;
                                    if (kinectMovementDataGatherer.getSkeletonDataCount() > 10)
                                        executeDanceRecognitionAlgorythm();
                                    isDataProcessed = false;
                                    armPoseCounter = 0;
                                }
                            }
                            else
                            {
                                //the user hasn't made the arm gesture
                                armPoseCounter = 0;
                            }

                            //check if the condition for data gathering is satisfied
                            if (kinectMovementDataGatherer.isDataGatheringConditionTrue(trackedSkeleton))
                            {
                                //add the skeleton data
                                kinectMovementDataGatherer.addSkeletonData(trackedSkeleton, fetchedTime);
                            }
                            else
                            {
                                //The user has danced for too much time
                                isSkeletonTracked = false;
                                //Now envoke the process of recognition
                                //pause the process of data gathering
                                isDataProcessed = true;
                                //Console.WriteLine("The user hasn't made the arm gesture or danced too much time");
                                //execute the algorythm for recognition of dance
                                if (kinectMovementDataGatherer.getSkeletonDataCount() > 10)
                                    executeDanceRecognitionAlgorythm();
                                isDataProcessed = false;
                            }

                        }
                        else
                        {
                            //The tracked skeleton isn't there 
                            //Probably the user has left the view of the sensor
                            isSkeletonTracked = false;
                            //Now envoke the process of recognition
                            //pause the process of data gathering
                            isDataProcessed = true;
                            Console.WriteLine("The user has left the view of the sensor");
                            if (kinectMovementDataGatherer.getSkeletonDataCount() > 10)
                                executeDanceRecognitionAlgorythm();
                            isDataProcessed = false;
                        }
                    }
                    else
                    {
                        //there isn't a tracked skeleton
                        //try to find one and track it

                        //reset the counter of consequtive arm gesture pose for stopping the process of data gathering
                        armPoseCounter = 0;

                        kinectMovementDataGatherer = new KinectMovementDataGatherer();
                        //suppose there isn't any
                        bool isThere = false;
                        Skeleton trackedSkeleton = null;
                        foreach (Skeleton skeleton in skeletons)
                        {

                            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                            {
                                if (kinectMovementDataGatherer.isDataGatheringConditionTrue(skeleton))
                                {
                                    //found the first tracked skeleton
                                    trackedSkeleton = skeleton;
                                    isThere = true;
                                    break;
                                }
                            }
                        }
                        //check if a skeleton has been found
                        if (isThere)
                        {
                            //start tracking this skeleton
                            Console.WriteLine("Started tracking a skeleton");
                            isSkeletonTracked = true;
                            kinectMovementDataGatherer.addSkeletonData(trackedSkeleton, fetchedTime);
                        }
                        else
                        {
                            //there isn't a user in front of the sensor that is tracked
                        }


                    }

                    System.Windows.Threading.Dispatcher canvasDispatcher = SkeletonCanvas.Dispatcher;
                    canvasDispatcher.Invoke(new Action(() => clearCanvas(SkeletonCanvas)));
                    bool isThereSkeleton = false;
                    foreach (Skeleton skeleton in skeletons)
                    {
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            isThereSkeleton = true;
                            //draw the skeleton on the canvas
                            //paint the tracked skelton with another color
                            if (kinectMovementDataGatherer.getTrackedSkeletonId().Equals(skeleton.TrackingId))
                            {
                                canvasDispatcher.Invoke(new Action(() => drawSkeletonOnCanvas(skeleton, SkeletonCanvas, Brushes.Maroon)));
                            }
                            else
                            {
                                canvasDispatcher.Invoke(new Action(() => drawSkeletonOnCanvas(skeleton, SkeletonCanvas, Brushes.Black)));
                            }


                        }
                    }
                    if (!isThereSkeleton)
                    {
                        System.Windows.Threading.Dispatcher imageDispatcher = NoDancerPicture.Dispatcher;
                        imageDispatcher.Invoke(new Action(() => changeVisibilityNoDancerPicture(true)));
                    }
                    else
                    {
                        System.Windows.Threading.Dispatcher imageDispatcher = NoDancerPicture.Dispatcher;
                        imageDispatcher.Invoke(new Action(() => changeVisibilityNoDancerPicture(false)));
                    }
                }
                frame.Dispose();

            }
            else
            {
                //null frame
                //check if a user has already been tracked
                if (isSkeletonTracked)
                {
                    nullFrameCounter++;
                    if (nullFrameCounter > maxNullFrames)
                    {
                        Console.WriteLine("Too many NULL frames");
                        //there are too many null frames
                        //drop the collected data for recognition
                        isSkeletonTracked = false;
                        isDataProcessed = true;
                        if (kinectMovementDataGatherer.getSkeletonDataCount() > 10)
                            executeDanceRecognitionAlgorythm();
                        isDataProcessed = false;
                        nullFrameCounter = 0;
                    }
                }

            }
        }


        /// <summary>
        /// Executes the recognition algorythm, draws a figure repeating the selected period of the dance and shows a message of recognised dance
        /// </summary>
        public void executeDanceRecognitionAlgorythm()
        {
            DanceRecognitionAlgorithm algorithm = new DanceRecognitionAlgorithm(kinectMovementDataGatherer.getCorrectedSkeletonData(),kinectMovementDataGatherer.getFetchedTimeArray());
            AlgorythmResult result = algorithm.execute();
            System.Windows.Threading.Dispatcher infoDispatcher = InformationLabel.Dispatcher;
            infoDispatcher.Invoke(new Action(() => displayTypeOfDanceMessage(result.danceType)));
            if (onePeriodDrawingWorker != null)
                onePeriodDrawingWorker.CancelAsync();
            lastPeriodFrom = result.fromIndex;
            lastPeriodTo = result.toIndex;
            algorithm.writeAttributesToFile(result.fromIndex,result.toIndex);
            lastPeriodGatherer = kinectMovementDataGatherer;
            this.onePeriodDrawingWorker = new BackgroundWorker();
            this.onePeriodDrawingWorker.DoWork += OnePeriodWorker_Work;
            this.onePeriodDrawingWorker.WorkerSupportsCancellation = true;
            this.onePeriodDrawingWorker.RunWorkerAsync();
        }

        #endregion

        #region Canvas for Drawing Skeletons

        /// <summary>
        /// Method for conversion from Skeleton Coordinate System to the Canvas Coordinate System
        /// </summary>
        /// <param name="joint">The skeleton joint coordinates</param>
        /// <returns></returns>
        private Point getCanvasCoordinates(SkeletonPoint joint,Canvas canvas)
        {
            Point point = new Point();
            point.X = ((joint.X + 2.2) / 4.4) * canvas.Width;
            point.Y = (1 - (joint.Y + 1.6) / 3.2) * canvas.Height;
            return point;
        }

        /// <summary>
        /// Clears the drawing canvas
        /// </summary>
        public void clearCanvas(Canvas canvas)
        {
            canvas.Children.Clear();
        }

        /// <summary>
        /// Draw a skeleton on the canvas using the color defined in brush. 
        /// </summary>
        /// <param name="skeleton">The position of skeleton joints of the user</param>
        public  void drawSkeletonOnCanvas(Skeleton skeleton, Canvas canvas, Brush brush)
        {
            Brush leftLegBrush;
            Brush rightLegBrush;
            if (brush.Equals(Brushes.Black))
            {
                leftLegBrush = new SolidColorBrush(Color.FromArgb(255, 130, 130, 130));
                rightLegBrush = brush;
            }
            else if (brush.Equals(Brushes.Maroon))
            {
                leftLegBrush = new SolidColorBrush(Color.FromArgb(255, 180, 0, 0));
                rightLegBrush = new SolidColorBrush(Color.FromArgb(255, 128, 0, 0));
            }
            else
            {
                leftLegBrush = brush;
                rightLegBrush = brush;
            }
            SkeletonPoint headPoint = skeleton.Joints[JointType.Head].Position;
            SkeletonPoint hipCenterPoint = skeleton.Joints[JointType.HipCenter].Position;
            SkeletonPoint kneeLeftPoint = skeleton.Joints[JointType.KneeLeft].Position;
            SkeletonPoint kneeRightPoint = skeleton.Joints[JointType.KneeRight].Position;
            SkeletonPoint ankleRightPoint = skeleton.Joints[JointType.AnkleRight].Position;
            SkeletonPoint ankleLeftPoint = skeleton.Joints[JointType.AnkleLeft].Position;
            SkeletonPoint shoulderCenterPoint = skeleton.Joints[JointType.ShoulderCenter].Position;
            SkeletonPoint elbowRightPoint = skeleton.Joints[JointType.ElbowRight].Position;
            SkeletonPoint elbowLeftPoint = skeleton.Joints[JointType.ElbowLeft].Position;
            SkeletonPoint handRightPoint = skeleton.Joints[JointType.HandRight].Position;
            SkeletonPoint handLeftPoint = skeleton.Joints[JointType.HandLeft].Position;
            SkeletonPoint footRightPoint = skeleton.Joints[JointType.FootRight].Position;
            SkeletonPoint footLeftPoint = skeleton.Joints[JointType.FootLeft].Position;
            canvas.Children.Add(createLine(shoulderCenterPoint, hipCenterPoint, canvas,brush));
            canvas.Children.Add(createLine(hipCenterPoint, kneeLeftPoint, canvas, leftLegBrush));
            canvas.Children.Add(createLine(hipCenterPoint, kneeRightPoint, canvas, brush));
            canvas.Children.Add(createLine(kneeRightPoint, ankleRightPoint, canvas, rightLegBrush));
            canvas.Children.Add(createLine(kneeLeftPoint, ankleLeftPoint, canvas, leftLegBrush));
            canvas.Children.Add(createLine(ankleLeftPoint, footLeftPoint, canvas, leftLegBrush));
            canvas.Children.Add(createLine(ankleRightPoint, footRightPoint, canvas, rightLegBrush));
            canvas.Children.Add(createLine(shoulderCenterPoint, elbowRightPoint, canvas, brush));
            canvas.Children.Add(createLine(shoulderCenterPoint, elbowLeftPoint, canvas, brush));
            canvas.Children.Add(createLine(elbowRightPoint, handRightPoint, canvas, brush));
            canvas.Children.Add(createLine(elbowLeftPoint, handLeftPoint, canvas, brush));
            ArcSegment arc = new ArcSegment();
            Point headCanvasPoint = getCanvasCoordinates(headPoint, canvas);
            Point shoulderCenterCanvasPoint = getCanvasCoordinates(shoulderCenterPoint, canvas);
            Ellipse elipse = new Ellipse();
            double radius = canvas.Width/canvas.Height*10.0;
            elipse.Margin = new Thickness(shoulderCenterCanvasPoint.X-radius/2,shoulderCenterCanvasPoint.Y-radius,0,0);
            elipse.Width = radius;
            elipse.Height = radius;
            elipse.Fill = brush;
            canvas.Children.Add(elipse);
        }

        /// <summary>
        /// Creates a line from two skeleton joints  
        /// </summary>
        /// <param name="p1"> First Skeleton  joint</param>
        /// <param name="p2"> Second Skeleton joint</param>
        /// <returns></returns>
        private Line createLine(SkeletonPoint p1, SkeletonPoint p2, Canvas canvas, Brush brush)
        {
            Line line = new Line();
            line.StrokeThickness = 5;
            line.StrokeEndLineCap = PenLineCap.Round;
            line.StrokeStartLineCap = PenLineCap.Round;
            line.Stroke = brush;
            Point canvasPoint = getCanvasCoordinates(p1,canvas);
            Point canvasPoint2 = getCanvasCoordinates(p2,canvas);
            line.X1 = canvasPoint.X;
            line.Y1 = canvasPoint.Y;
            line.X2 = canvasPoint2.X;
            line.Y2 = canvasPoint2.Y;
            return line;
        }
        #endregion

        #region Screen Drawing functions

        private void displayTypeOfDanceMessage(DanceType danceType)
        {
            if (danceType.Equals(DanceType.Ramnoto))
            {
                InformationLabel.FontSize = 20;
                InformationLabel.Content = "Рамното оро"; 
            }
            else if (danceType.Equals(DanceType.O6n3n))
            {
                InformationLabel.FontSize = 20;
                InformationLabel.Content = "6 напред 3 назад";
            }
            else if (danceType.Equals(DanceType.Pajdusko))
            {
                InformationLabel.FontSize = 20;
                InformationLabel.Content = "Пајдушко оро";
            }
            else if (danceType.Equals(DanceType.NoDance))
            {
                InformationLabel.FontSize = 20;
                InformationLabel.Content = "Непознато";
            }
            else if (danceType.Equals(DanceType.RamnotoLooking))
            {
                InformationLabel.FontSize = 15;
                InformationLabel.Content = "Наликува на рамното оро";
            }
            else if (danceType.Equals(DanceType.O6n3nLooking))
            {
                InformationLabel.FontSize = 12;
                InformationLabel.Content = "Наликува на 6 напред 3 назад";
            }
            else if (danceType.Equals(DanceType.PajduskoLooking))
            {
                InformationLabel.FontSize = 15;
                InformationLabel.Content = "Наликува на пајдушко оро";
            }
            var storyboard = Resources["InfoMessageAnimate"] as Storyboard;
            if (storyboard != null)
                storyboard.Begin();
        }

        private void changeVisibilityNoDancerPicture(bool isVisible){
            if(isVisible)
                NoDancerPicture.Visibility=Visibility.Visible;
            else
                NoDancerPicture.Visibility = Visibility.Hidden;

        }
        #endregion
    }
}
