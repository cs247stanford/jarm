using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace GestureRecognizer
{

    public class GestureRecognitionEngine
    {

        public int SkipFramesAfterGestureIsDetected = 0;
        public event EventHandler<GestureEventArgs> GestureRecognized;
        public GestureType GestureType { get; set; }
        public Skeleton Skeleton { get; set; }
        public bool IsGestureDetected { get; set; }

        private List<GestureBase> gestureCollection = null;

        public GestureRecognitionEngine()
        {
            this.InitializeGesture();
        }


        /// <summary>
        /// Initialize the gesture
        /// </summary>
        private void InitializeGesture()
        {

            this.gestureCollection = new List<GestureBase>();
           // this.gestureCollection.Add(new SwipeToLeftGesture());
            this.gestureCollection.Add(new PullDownGesture());

        }

        /// <summary>
        /// Starts the gesture recognition. 
        /// </summary>
        public void StartRecognize()
        {

            if (this.IsGestureDetected)
            {
                while (this.SkipFramesAfterGestureIsDetected <= 30)
                {
                    this.SkipFramesAfterGestureIsDetected++;
                }
                this.RestGesture();
                return;
            }

            foreach (var item in this.gestureCollection)
            {
                if (item.CheckForGesture(this.Skeleton))
                {
                    if (this.GestureRecognized != null)
                    {
                        this.GestureRecognized(this, new GestureEventArgs(RecognitionResult.Success, item.GestureType));
                        this.IsGestureDetected = true;
                    }
                }
            }

        }


        /// <summary>
        /// Rests the gesture.
        /// </summary>
        private void RestGesture()
        {

            this.gestureCollection = null;
            this.InitializeGesture();
            this.SkipFramesAfterGestureIsDetected = 0;
            this.IsGestureDetected = false;

        }



    }
}
