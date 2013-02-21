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
        private SkeletonPoint startingRightPosition;
        private float validateShoulderDiff;
        private float startingShoulderDiff;
        private double HORIZONTAL_SWIPE_BUFFER = 0.3; // the hand must move at least this many pixels to trigger action
        private double VERTICAL_SWIPE_BUFFER = 0.3; // the hand must move at least this many pixels to trigger action


        protected override bool ValidateGestureStartCondition(Skeleton skeleton)
        {

            var handRightPosition = skeleton.Joints[JointType.HandRight].Position;
            var handLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            //var elbowRightPosition = skeleton.Joints[JointType.ElbowRight].Position;
            //var elbowLeftPosition = skeleton.Joints[JointType.ElbowLeft].Position;
            var shoulderLeftPosition = skeleton.Joints[JointType.ShoulderLeft].Position;
            var spinePosition = skeleton.Joints[JointType.Spine].Position;

            if (handLeftPosition.X > spinePosition.X &&
                handLeftPosition.Y < spinePosition.Y &&
                //(handLeftPosition.Y < shoulderLeftPosition.Y) && // rhand is above rshoulder
                //(handLeftPosition.Y < elbowLeftPosition.Y) && // rhand is above relbow
                //handRightPosition.Y < spinePosition.Y)
                (handRightPosition.Y > spinePosition.Y))
            {
                startingShoulderDiff = GestureHelper.GetJointDistance(skeleton.Joints[JointType.HandLeft], skeleton.Joints[JointType.ShoulderRight]);

                validatePosition = handLeftPosition;//skeleton.Joints[JointType.HandLeft].Position;
                startingPosition = handLeftPosition;//skeleton.Joints[JointType.HandLeft].Position;
                startingRightPosition = handRightPosition;//skeleton.Joints[JointType.HandLeft].Position;


                //System.Diagnostics.Debug.WriteLine(">>>SwipeToRight start condition valid...");
                //System.Diagnostics.Debug.WriteLine("validatePosition: " + validatePosition);
                //System.Diagnostics.Debug.WriteLine("handLeftPosition: " + handLeftPosition);
                //System.Diagnostics.Debug.WriteLine("shoulderLeftPosition: " + shoulderLeftPosition);
                //System.Diagnostics.Debug.WriteLine("difference: " + shoulderDiff);

                return true;
            }

            return false;

        }

        protected override bool IsGestureValid(Skeleton skeleton)
        {

            var currentHandLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            float currentShoulderDiff = GestureHelper.GetJointDistance(skeleton.Joints[JointType.HandLeft], skeleton.Joints[JointType.ShoulderRight]);

            if ((validatePosition.X > currentHandLeftPosition.X) &&
               (Math.Abs(startingPosition.Y - currentHandLeftPosition.Y) < VERTICAL_SWIPE_BUFFER)
                && currentShoulderDiff < validateShoulderDiff)
            {
                System.Diagnostics.Debug.WriteLine("^^^SwipeToRight gesture is valid...");
                validatePosition = currentHandLeftPosition;
                validateShoulderDiff = currentShoulderDiff;
                return true;
            }
            return false;
        }

        protected override bool ValidateGestureEndCondition(Skeleton skeleton)
        {

            double distance = Math.Abs(startingPosition.X - validatePosition.X);
            //float currentShoulderDiff = GestureHelper.GetJointDistance(skeleton.Joints[JointType.HandLeft], skeleton.Joints[JointType.ShoulderRight]);

            if ((distance > HORIZONTAL_SWIPE_BUFFER) && (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.Spine].Position.Y)) //The Right hand is still below the spine
            // && currentShoulderDiff < startingShoulderDiff)
            {
                System.Diagnostics.Debug.WriteLine("SWIPE Right DETECTED!!!!!!!");
                return true;
            }

            return false;

        }

        protected override bool ValidateBaseCondition(Skeleton skeleton)
        {
            /*
            var handRightPosition = skeleton.Joints[JointType.HandRight].Position;
            var handLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            var elbowRightPosition = skeleton.Joints[JointType.ElbowRight].Position;
            var elbowLeftPosition = skeleton.Joints[JointType.ElbowLeft].Position;
            var shoulderRightPosition = skeleton.Joints[JointType.ShoulderRight].Position;
            var shoulderLeftPosition = skeleton.Joints[JointType.ShoulderLeft].Position;

            if (//(handLeftPosition.Y > elbowLeftPosition.Y) &&
                (handLeftPosition.Y > shoulderLeftPosition.Y) &&
                //(handRightPosition.Y > elbowRightPosition.Y) &&
                (handRightPosition.Y > shoulderRightPosition.Y))
            {
                return true;
            }

            return false;*/
            return true;

        }

    }
}
