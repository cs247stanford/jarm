using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace GestureRecognizer
{
    class SwipeToRightGesture : GestureBase
    {

        public SwipeToRightGesture() : base(GestureType.SwipeRight) { }
        private SkeletonPoint validatePosition;
        private SkeletonPoint startingPosition;
        private float shoulderDiff;
        private double SWIPE_BUFFER = 50; // the hand must move at least this many pixels to trigger action

        protected override bool ValidateGestureStartCondition(Skeleton skeleton)
        {

            var handRightPosition = skeleton.Joints[JointType.HandLeft].Position;
            var handLeftPosition = skeleton.Joints[JointType.HandRight].Position;
            var elbowRightPosition = skeleton.Joints[JointType.ElbowLeft].Position;
            var elbowLeftPosition = skeleton.Joints[JointType.ElbowRight].Position;
            var shoulderLeftPosition = skeleton.Joints[JointType.ShoulderRight].Position;
            var spinePosition = skeleton.Joints[JointType.Spine].Position;
            
            if ((handLeftPosition.Y < shoulderRightPosition.Y) && // rhand is above rshoulder
                //(handLeftPosition.Y < elbowRightPosition.Y) && // rhand is above relbow
                //handRightPosition.Y < spinePosition.Y)
                (handRightPosition.Y > spinePosition.Y))
            {
                shoulderDiff = GestureHelper.GetJointDistance(skeleton.Joints[JointType.HandLeft], skeleton.Joints[JointType.ShoulderRight]);
                
                validatePosition = skeleton.Joints[JointType.HandLeft].Position;
                startingPosition = skeleton.Joints[JointType.HandLeft].Position;
                
                System.Diagnostics.Debug.WriteLine(">>>SwipeToRight start condition valid...");
                System.Diagnostics.Debug.WriteLine("validatePosition: " + validatePosition);
                System.Diagnostics.Debug.WriteLine("handLeftPosition: " + handRightPosition);
                System.Diagnostics.Debug.WriteLine("shoulderLeftPosition: " + shoulderRightPosition);
                System.Diagnostics.Debug.WriteLine("difference: " + shoulderDiff);

                return true;
            }

            return false;

        }

        protected override bool IsGestureValid(Skeleton skeleton)
        {

            var currentHandLeftPosition = skeleton.Joints[JointType.HandRight].Position;
            
            if (validatePosition.X > currentHandLeftPosition.X &&
              (Math.Abs(validatePosition.Y - currentHandLeftPosition.Y) < VERTICAL_SWIPE_BUFFER))
            {
                System.Diagnostics.Debug.WriteLine("^^^SwipeToRight gesture is valid...");
                validatePosition = currentHandLeftPosition;
                return true;
            }

            return false;
        }

        protected override bool ValidateGestureEndCondition(Skeleton skeleton)
        {

            double distance = Math.Abs(startingPosition.X - validatePosition.X);
            float currentShoulderDiff = GestureHelper.GetJointDistance(skeleton.Joints[JointType.HandLeft], skeleton.Joints[JointType.ShoulderRight]);

            if (distance > SWIPE_BUFFER) // && currentShoulderDiff < shoulderDiff)
            {
                System.Diagnostics.Debug.WriteLine("<<<SwipeToRight gesture has ended...");
                return true;
            }

            return false;

        }

        protected override bool ValidateBaseCondition(Skeleton skeleton)
        {
            
            var handRightPosition = skeleton.Joints[JointType.HandLeft].Position;
            var handLeftPosition = skeleton.Joints[JointType.HandRight].Position;
            var elbowRightPosition = skeleton.Joints[JointType.ElbowLeft].Position;
            var elbowLeftPosition = skeleton.Joints[JointType.ElbowRight].Position;
            var shoulderRightPosition = skeleton.Joints[JointType.ShoulderLeft].Position;
            var shoulderLeftPosition = skeleton.Joints[JointType.ShoulderRight].Position;

            if ((handLeftPosition.Y > elbowRightPosition.Y) &&
                (handLeftPosition.Y > shoulderRightPosition.Y) && 
                (handRightPosition.Y > elbowLeftPosition.Y) &&
                (handRightPosition.Y > shoulderLeftPosition.Y))        
            {
                return true;
            }

            return false;

        }

    }
}
