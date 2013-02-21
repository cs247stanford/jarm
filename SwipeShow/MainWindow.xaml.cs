//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.Slideshow
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Controls;
    using Microsoft.Kinect;
    using Microsoft.Samples.Kinect.SwipeGestureRecognizer;
    using GestureRecognizer;
    using KinectPresentor;
    //using System.Drawing;
    
    /*
     * public form1() { InitializeComponent();}
     * 
     * private void button(object sender, EventArgs e) {
     * Graphics g = this.CreateGraphics();
     * Pen black = new Pen(Color.Green, 10);
     * g.FillRectangle(green.Brush, new Rectangle(200, 200, 100, 300);
     * }
     */
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        /// <summary>
        /// The recognizer being used.
        /// </summary>
        //private readonly Recognizer activeRecognizer;
        GestureRecognitionEngine recognitionEngine;

        /// <summary>
        /// The paths of the picture files.
        /// </summary>
        private readonly string[] picturePaths = CreatePicturePaths();

        /// <summary>
        /// Array of arrays of contiguous line segements that represent a skeleton.
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

        /// <summary>
        /// The sensor we're currently tracking.
        /// </summary>
        private KinectSensor nui;

        /// <summary>
        /// Related things
        /// </summary>
        private bool relatedActivated = false;
        private bool relatedJustDeactivated = false;

        /// <summary>
        /// There is currently no connected sensor.
        /// </summary>
        private bool isDisconnectedField = true;

        /// <summary>
        /// Any message associated with a failure to connect.
        /// </summary>
        private string disconnectedReasonField;

        /// <summary>
        /// Array to receive skeletons from sensor, resize when needed.
        /// </summary>
        public Skeleton[] skeletons = new Skeleton[0];

        /// <summary>
        /// Time until skeleton ceases to be highlighted.
        /// </summary>
        private DateTime highlightTime = DateTime.MinValue;
        
        /// <summary>
        /// The ID of the skeleton to highlight.
        /// </summary>
        private int highlightId = -1;

        /// <summary>
        /// The ID if the skeleton to be tracked.
        /// </summary>
        private int nearestId = -1;

        /// <summary>
        /// The index of the current image.
        /// </summary>
        private int indexField = 1;

        /// <summary>
        /// The stopwatch that we use to detect click events.
        /// </summary>
        private Stopwatch stopwatch;

        /// <summary>
        /// Current x value of pointer
        /// </summary>
        private double currentX = 0.0;

        /// <summary>
        /// Current y value of pointer
        /// </summary>
        private double currentY = 0.0;

        /// <summary>
        /// Threshold distance at which user is determined to be pointing at a different point than before
        /// </summary>
        private double APPROX_VALUE = 50;

        /// <summary>
        /// True if the related items pane is currently being displayed
        /// </summary>
        private bool relatedItemsDown = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.PreviousPicture = this.LoadPicture(this.Index - 1);
            this.Picture = this.LoadPicture(this.Index);
            this.NextPicture = this.LoadPicture(this.Index + 1);
            this.ParentPicture = null;
            //this.RelatedPicture = this.LoadPicture(this.Index - 1);

            InitializeComponent();

            // Create the gesture recognizer.
            //this.activeRecognizer = this.CreateRecognizer();
            recognitionEngine = new GestureRecognitionEngine();
            recognitionEngine.GestureRecognized += new EventHandler<GestureEventArgs>(recognitionEngine_GestureRecognized);


            // Wire-up window loaded event.
            Loaded += this.OnMainWindowLoaded;
        }

        void recognitionEngine_GestureRecognized(object sender, GestureEventArgs e)
        {

            String recognizedGesture = e.GestureType.ToString();

            switch (recognizedGesture) {

                case "PullDown":

                    if (relatedActivated)
                        break;

                    if (relatedJustDeactivated)
                    {
                        relatedJustDeactivated = false;
                        break;
                    }

                    var pullDownStoryboard = Resources["TopPullDown"] as Storyboard;

                    if (pullDownStoryboard != null)
                    {
                        pullDownStoryboard.Begin();
                    }
                    
                    relatedItemsDown = true;

                    relatedActivated = true;

                    //HighlightSkeleton(e.Skeleton);
                    break;

                case "PushUp":

                    if (!relatedActivated)
                        break;
                    
                    var pushUpStoryboard = Resources["TopPushUp"] as Storyboard;

                    if (pushUpStoryboard != null)
                    {
                        pushUpStoryboard.Begin();
                    }
                    
                    relatedItemsDown = true;

                    relatedActivated = false;

                    relatedJustDeactivated = true;

                    break;
                
                default:
                    break;

            }

            if (recognizedGesture.Equals("PullDown"))
            {
                
            }

            //System.Diagnostics.Debug.WriteLine(recognizedGesture);

            


            //MessageBox.Show(e.GestureType.ToString());

        }

        /// <summary>
        /// Event implementing INotifyPropertyChanged interface.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a value indicating whether no Kinect is currently connected.
        /// </summary>
        public bool IsDisconnected
        {
            get
            {
                return this.isDisconnectedField;
            }

            private set
            {
                if (this.isDisconnectedField != value)
                {
                    this.isDisconnectedField = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("IsDisconnected"));
                    }
                }
            }
        }

        /// <summary>
        /// Gets any message associated with a failure to connect.
        /// </summary>
        public string DisconnectedReason
        {
            get
            {
                return this.disconnectedReasonField;
            }

            private set
            {
                if (this.disconnectedReasonField != value)
                {
                    this.disconnectedReasonField = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("DisconnectedReason"));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the index number of the image to be shown.
        /// </summary>
        public int Index
        {
            get
            {
                return this.indexField;
            }

            set
            {
                if (this.indexField != value)
                {
                    this.indexField = value;

                    // Notify world of change to Index and Picture.
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("Index"));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the previous image displayed.
        /// </summary>
        public BitmapImage PreviousPicture { get; private set; }

        /// <summary>
        /// Gets the current image to be displayed.
        /// </summary>
        public BitmapImage Picture { get; private set; }

        /// <summary>
        /// Gets teh next image displayed.
        /// </summary>
        public BitmapImage NextPicture { get; private set; }


        /// <summary>
        /// Gets the current image to be displayed.
        /// </summary>
        public BitmapImage ParentPicture { get; private set; }

       
        /// <summary>
        /// Gets the previous image displayed.
        /// </summary>
        public BitmapImage RelatedPicture { get; private set; }
        
        
        /// <summary>
        /// Get list of files to display as pictures.
        /// </summary>
        /// <returns>Paths to pictures.</returns>
        private static string[] CreatePicturePaths()
        {
            var list = new List<string>();

            var commonPicturesPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);
            list.AddRange(Directory.GetFiles(commonPicturesPath, "*.jpg", SearchOption.AllDirectories));
            if (list.Count == 0)
            {
                list.AddRange(Directory.GetFiles(commonPicturesPath, "*.png", SearchOption.AllDirectories));
            }

            if (list.Count == 0)
            {
                var myPicturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                list.AddRange(Directory.GetFiles(myPicturesPath, "*.jpg", SearchOption.AllDirectories));
                if (list.Count == 0)
                {
                    list.AddRange(Directory.GetFiles(commonPicturesPath, "*.png", SearchOption.AllDirectories));
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Load the picture with the given index.
        /// </summary>
        /// <param name="index">The index to use.</param>
        /// <returns>Corresponding image.</returns>
        private BitmapImage LoadPicture(int index)
        {
            BitmapImage value;

            if (this.picturePaths.Length != 0)
            {
                var actualIndex = index % this.picturePaths.Length;
                if (actualIndex < 0)
                {
                    actualIndex += this.picturePaths.Length;
                }

                Debug.Assert(0 <= actualIndex, "Index used will be non-negative");
                Debug.Assert(actualIndex < this.picturePaths.Length, "Index is within bounds of path array");

                try
                {
                    value = new BitmapImage(new Uri(this.picturePaths[actualIndex]));
                }
                catch (NotSupportedException)
                {
                    value = null;
                }
            }
            else
            {
                value = null;
            }

            return value;
        }

        /*
        /// <summary>
        /// Create a wired-up recognizer for running the slideshow.
        /// </summary>
        /// <returns>The wired-up recognizer.</returns>
        private Recognizer CreateRecognizer()
        {
            // Instantiate a recognizer.
            var recognizer = new Recognizer();

           // Wire-up swipe right to manually advance picture.
              recognizer.SwipeRightDetected += (s, e) =>
              {

                  System.Diagnostics.Debug.WriteLine("Right swipe detected");

                  if (e.Skeleton.TrackingId == nearestId)
                  {
                      Index++;

                      // Setup corresponding picture if pictures are available.
                      this.PreviousPicture = this.Picture;
                      this.Picture = this.NextPicture;
                      this.NextPicture = LoadPicture(Index + 1);

                      // Notify world of change to Index and Picture.
                      if (this.PropertyChanged != null)
                      {
                          this.PropertyChanged(this, new PropertyChangedEventArgs("PreviousPicture"));
                          this.PropertyChanged(this, new PropertyChangedEventArgs("Picture"));
                          this.PropertyChanged(this, new PropertyChangedEventArgs("NextPicture"));
                      }

                      var storyboard = Resources["LeftAnimate"] as Storyboard;
                      if (storyboard != null)
                      {
                          storyboard.Begin();
                      }

                      HighlightSkeleton(e.Skeleton);
                  }
              };

              // Wire-up swipe left to manually reverse picture.
              recognizer.SwipeLeftDetected += (s, e) =>
              {

                  System.Diagnostics.Debug.WriteLine("Left swipe detected");

                  if (e.Skeleton.TrackingId == nearestId)
                  {
                      Index--;

                      // Setup corresponding picture if pictures are available.
                      this.NextPicture = this.Picture;
                      this.Picture = this.PreviousPicture;
                      this.PreviousPicture = LoadPicture(Index - 1);

                      // Notify world of change to Index and Picture.
                      if (this.PropertyChanged != null)
                      {
                          this.PropertyChanged(this, new PropertyChangedEventArgs("PreviousPicture"));
                          this.PropertyChanged(this, new PropertyChangedEventArgs("Picture"));
                          this.PropertyChanged(this, new PropertyChangedEventArgs("NextPicture"));
                      }

                      var storyboard = Resources["RightAnimate"] as Storyboard;
                      if (storyboard != null)
                      {
                          storyboard.Begin();
                      }

                      HighlightSkeleton(e.Skeleton);
                  }
              };
              


            // Wire-up swipe up to go back to the parent slide (after discussing a related slide).
            //Will eventually be SwipeUpDetected
            recognizer.SwipeLeftDetected += (s, e) =>
            {

                System.Diagnostics.Debug.WriteLine("Left (up) swipe detected");

                if (e.Skeleton.TrackingId == nearestId)
                {
                    //Index++;
                    if (this.ParentPicture != null)
                    {
                        this.Picture = this.ParentPicture;
                        this.ParentPicture = null;
                        //this.NextPicture = LoadPicture(Index + 1);

                        // Notify world of change to Index and Picture.
                        if (this.PropertyChanged != null)
                        {
                            //this.PropertyChanged(this, new PropertyChangedEventArgs("ParentPicture"));
                            this.PropertyChanged(this, new PropertyChangedEventArgs("Picture"));
                        }

                        var storyboard = Resources["TopAnimate"] as Storyboard;
                        if (storyboard != null)
                        {
                            storyboard.Begin();
                        }

                        HighlightSkeleton(e.Skeleton);
                    }
                    else
                    {
                        Debug.WriteLine("Tried to swipe up without parent slide");
                    }
                }
                };


                //Will eventually be PullDownDetected
                recognizer.SwipeRightDetected += (s, e) =>
               {

                   System.Diagnostics.Debug.WriteLine("Right swipe detected");

                   if (e.Skeleton.TrackingId == nearestId)
                   {

                       // Setup corresponding picture if pictures are available.
                       // Will eventually load in the correct "related slides" image
                       this.PreviousPicture = this.Picture;
                       this.Picture = this.NextPicture;
                       this.NextPicture = LoadPicture(Index + 1);
                     
                       

                       // Notify world of change to RelatedPicture.
                       if (this.PropertyChanged != null)
                       {
                           this.PropertyChanged(this, new PropertyChangedEventArgs("RelatedItems"));
                       }

                       var storyboard = Resources["TopPullDown"] as Storyboard;
                       if (storyboard != null)
                       {
                           storyboard.Begin();
                       }
                       relatedItemsDown = true;
                       HighlightSkeleton(e.Skeleton);
                   }
               };

                  //Will eventually be PushUpDetected
                   recognizer.SwipeLeftDetected += (s, e) =>
                   {

                       System.Diagnostics.Debug.WriteLine("Right swipe detected");

                       if (e.Skeleton.TrackingId == nearestId)
                       {

                           // Setup corresponding picture if pictures are available.
                          // this.NextPicture = LoadPicture(Index + 1);

                           //Notify world of change to Index and Picture.
                           //if (this.PropertyChanged != null)
                           //{
                           //    this.PropertyChanged(this, new PropertyChangedEventArgs("RelatedItems"));
                           //}

                           var storyboard = Resources["TopPushUp"] as Storyboard;
                           if (storyboard != null)
                           {
                               storyboard.Begin();
                           }
                           relatedItemsDown = false;
                           HighlightSkeleton(e.Skeleton);
                       }
                   };

                return recognizer;
            }*/

        /// <summary>
        /// Handle insertion of Kinect sensor.
        /// </summary>
        private void InitializeNui()
        {
            this.UninitializeNui();

            var index = 0;
            while (this.nui == null && index < KinectSensor.KinectSensors.Count)
            {
                try
                {
                    this.nui = KinectSensor.KinectSensors[index];

                    this.nui.Start();

                    this.IsDisconnected = false;
                    this.DisconnectedReason = null;
                }
                catch (IOException ex)
                {
                    this.nui = null;

                    this.DisconnectedReason = ex.Message;
                }
                catch (InvalidOperationException ex)
                {
                    this.nui = null;

                    this.DisconnectedReason = ex.Message;
                }

                index++;
            }

            if (this.nui != null)
            {
                this.nui.SkeletonStream.Enable();

                this.nui.SkeletonFrameReady += this.OnSkeletonFrameReady;

                stopwatch = new Stopwatch();
                stopwatch.Start();
            }
        }

        /// <summary>
        /// Handle removal of Kinect sensor.
        /// </summary>
        private void UninitializeNui()
        {
            if (this.nui != null)
            {
                this.nui.SkeletonFrameReady -= this.OnSkeletonFrameReady;

                this.nui.Stop();

                this.nui = null;
            }

            this.IsDisconnected = true;
            this.DisconnectedReason = null;
        }

        /// <summary>
        /// Window loaded actions to initialize Kinect handling.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {

            String path;
            try
            {
                path = Directory.GetCurrentDirectory();
                Debug.WriteLine(path);
            }
            catch (Exception ef) { 
                Debug.WriteLine(ef);
            }

            // Start the Kinect system, this will cause StatusChanged events to be queued.
            this.InitializeNui();

            // Handle StatusChange events to pick the first sensor that connects.
            KinectSensor.KinectSensors.StatusChanged += (s, ee) =>
            {
                switch (ee.Status)
                {
                    case KinectStatus.Connected:
                        if (nui == null)
                        {
                            Debug.WriteLine("New Kinect connected");

                            InitializeNui();
                        }
                        else
                        {
                            Debug.WriteLine("Existing Kinect signalled connection");
                        }

                        break;
                    default:
                        if (ee.Sensor == nui)
                        {
                            Debug.WriteLine("Existing Kinect disconnected");

                            UninitializeNui();
                        }
                        else
                        {
                            Debug.WriteLine("Other Kinect event occurred");
                        }

                        break;
                }
            };
        }

        /// <summary>
        /// Handler for skeleton ready handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Get the frame.
            using (var frame = e.OpenSkeletonFrame())
            {
                // Ensure we have a frame.
                if (frame != null)
                {
                    // Resize the skeletons array if a new size (normally only on first call).
                    if (this.skeletons.Length != frame.SkeletonArrayLength)
                    {
                        this.skeletons = new Skeleton[frame.SkeletonArrayLength];
                    }

                    // Get the skeletons.
                    frame.CopySkeletonDataTo(this.skeletons);

                    //Skeleton firstSkeleton = (this.skeletons)[0];    


                    Skeleton firstSkeleton = null;
                    foreach (Skeleton trackedSkeleton in this.skeletons)
                    {
                        if (trackedSkeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            firstSkeleton = trackedSkeleton;
                            break;
                        } 
                    }

                    if (firstSkeleton == null)
                    {
                        firstSkeleton = this.skeletons[0];
                    }

                    /*Skeleton firstSkeleton = (from trackskeleton in this.skeletons
                                              where trackskeleton.TrackingState == SkeletonTrackingState.Tracked
                                              select trackskeleton).FirstOrDefault();*/

                    // Hand tracking code
                    if (firstSkeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                    {
                        this.MapJointsWithUIElement(firstSkeleton);
                    }

                    /*
                    // Assume no nearest skeleton and that the nearest skeleton is a long way away.
                    var newNearestId = -1;
                    var nearestDistance2 = double.MaxValue;

                    // Look through the skeletons.
                    foreach (var skeleton in this.skeletons)
                    {
                        // Only consider tracked skeletons.
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            // Find the distance squared.
                            var distance2 = (skeleton.Position.X * skeleton.Position.X) +
                                (skeleton.Position.Y * skeleton.Position.Y) +
                                (skeleton.Position.Z * skeleton.Position.Z);

                            // Is the new distance squared closer than the nearest so far?
                            if (distance2 < nearestDistance2)
                            {
                                // Use the new values.
                                newNearestId = skeleton.TrackingId;
                                nearestDistance2 = distance2;
                            }
                        }
                    }

                    if (this.nearestId != newNearestId)
                    {
                        this.nearestId = newNearestId;
                    }
                    */


                    // Pass skeletons to recognizer.
                    //this.activeRecognizer.Recognize(sender, frame, this.skeletons);
                    recognitionEngine.Skeleton = firstSkeleton;
                    recognitionEngine.StartRecognize();

                    this.DrawStickMen(this.skeletons);
                }
            }
        }


       


        
        /// <summary>
        /// Uses the current y value to get the associated related item. 
        /// Then, switches to related item.
        /// </summary>
        /// <param name="y"></param>
        private void SelectRelatedItem(double x, double y)
        {
                // assume that the related slides are in an array filling up a horizontal bar at the top of the screen
                double SLIDE_WIDTH = ((Grid)(this.Content)).ActualWidth / 5;
                //Debug.WriteLine("WINDOW: " + SLIDE_WIDTH);

                //List<Slide> relatedSlides = new List<Slide>;
                //double SLIDE_PADDING
                //int numRelatedSlides // this is mapped to a "parent" slide
                //double WINDOW_WIDTH


                // given an x and y, return the slide at that position
                // for simplicity, let's assume there is no padding between the slides         
                int selectedSlideIndex = (int)(x - 1) / (int)SLIDE_WIDTH;
                //Slide selectedSlide = relatedSlides[selectedSlideIndex];

                //int newIndex = IndexFromXValue(selectedSlideIndex);

                Debug.WriteLine("SELECT ITEM AT " + selectedSlideIndex);
                this.ParentPicture = this.Picture;
                this.Picture = LoadPicture(selectedSlideIndex);

                // Notify world of change to Index and Picture.
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("ParentPicture"));
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Picture"));
                }
        }

        /// <summary>
        /// Selects object at current x, y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SelectObject(double x, double y)
        {
          
      
            if (relatedItemsDown && y < RelatedItems.ActualHeight)
            {
                //SelectRelatedItem(x, y);
            }
            else
            {
                //any other objects that could be selected (media, etc).
            }
        }

        //Returns true if the newest point is within APPROX_VALUE distance of the current x, y
        private bool isApproxSamePoint(double x, double y)
        {
            return !(Math.Sqrt(Math.Pow(Math.Abs(currentX - x), 2) + Math.Pow(Math.Abs(currentY - y), 2))>APPROX_VALUE);
        }

        private void MapJointsWithUIElement(Skeleton skeleton)
        {
            Point rightPoint = this.ScalePosition(skeleton.Joints[JointType.HandRight].Position);
            Canvas.SetLeft(RightHandPointer, rightPoint.X);
            Canvas.SetTop(RightHandPointer, rightPoint.Y);
           
            /*if (!isApproxSamePoint(rightPoint.X, rightPoint.Y))
            {
                currentX = rightPoint.X;
                currentY = rightPoint.Y;
                stopwatch.Restart();
            }
            if (stopwatch.ElapsedMilliseconds >= 2000)
            {
                Debug.WriteLine("CLICK");
                SelectObject(rightPoint.X, rightPoint.Y);
                stopwatch.Restart();

            }*/

        }

        private Point ScalePosition(SkeletonPoint skeletonPoint)
        {

            DepthImagePoint depthPoint = this.nui.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X * 1.4 * ((Grid)(this.Content)).ActualWidth / 640, depthPoint.Y * 1.4 * ((Grid)(this.Content)).ActualHeight / 480);

        }


        /// <summary>
        /// Select a skeleton to be highlighted.
        /// </summary>
        /// <param name="skeleton">The skeleton</param>
        private void HighlightSkeleton(Skeleton skeleton)
        {
            // Set the highlight time to be a short time from now.
            this.highlightTime = DateTime.UtcNow + TimeSpan.FromSeconds(0.5);

            // Record the ID of the skeleton.
            this.highlightId = skeleton.TrackingId;
        }

        /// <summary>
        /// Draw stick men for all the tracked skeletons.
        /// </summary>
        /// <param name="skeletons">The skeletons to draw.</param>
        private void DrawStickMen(Skeleton[] skeletons)
        {
            // Remove any previous skeletons.
            StickMen.Children.Clear();

            foreach (var skeleton in skeletons)
            {
                // Only draw tracked skeletons.
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    // Draw a background for the next pass.
                    this.DrawStickMan(skeleton, Brushes.WhiteSmoke, 7);
                }
            }

            foreach (var skeleton in skeletons)
            {
                // Only draw tracked skeletons.
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    // Pick a brush, Red for a skeleton that has recently gestures, black for the nearest, gray otherwise.
                    var brush = DateTime.UtcNow < this.highlightTime && skeleton.TrackingId == this.highlightId ? Brushes.Red :
                        skeleton.TrackingId == this.nearestId ? Brushes.Black : Brushes.Gray;

                    // Draw the individual skeleton.
                    this.DrawStickMan(skeleton, brush, 3);
                }
            }
        }

        /// <summary>
        /// Draw an individual skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton to draw.</param>
        /// <param name="brush">The brush to use.</param>
        /// <param name="thickness">This thickness of the stroke.</param>
        private void DrawStickMan(Skeleton skeleton, Brush brush, int thickness)
        {
            Debug.Assert(skeleton.TrackingState == SkeletonTrackingState.Tracked, "The skeleton is being tracked.");

            foreach (var run in SkeletonSegmentRuns)
            {
                var next = this.GetJointPoint(skeleton, run[0]);
                for (var i = 1; i < run.Length; i++)
                {
                    var prev = next;
                    next = this.GetJointPoint(skeleton, run[i]);

                    var line = new Line
                    {
                        Stroke = brush,
                        StrokeThickness = thickness,
                        X1 = prev.X,
                        Y1 = prev.Y,
                        X2 = next.X,
                        Y2 = next.Y,
                        StrokeEndLineCap = PenLineCap.Round,
                        StrokeStartLineCap = PenLineCap.Round
                    };

                    StickMen.Children.Add(line);
                }
            }
        }

        /// <summary>
        /// Convert skeleton joint to a point on the StickMen canvas.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <param name="jointType">The joint to project.</param>
        /// <returns>The projected point.</returns>
        private Point GetJointPoint(Skeleton skeleton, JointType jointType)
        {
            var joint = skeleton.Joints[jointType];

            // Points are centered on the StickMen canvas and scaled according to its height allowing
            // approximately +/- 1.5m from center line.
            var point = new Point
            {
                X = (StickMen.Width / 2) + (StickMen.Height * joint.Position.X / 3),
                Y = (StickMen.Width / 2) - (StickMen.Height * joint.Position.Y / 3)
            };

            return point;
        }
    }
}
