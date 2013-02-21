using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace GestureRecognizer
{
    class SwipeToLeftGesture : GestureBase
    {

        public SwipeToLeftGesture() : base(GestureType.SwipeLeft) { }
        private SkeletonPoint validatePosition;
        private SkeletonPoint startingPosition;
        private SkeletonPoint startingLeftPosition;
        private float validateShoulderDiff;
        private float startingShoulderDiff;
        private double HORIZONTAL_SWIPE_BUFFER = 0.3; // the hand must move at least this many pixels to trigger action
        private double VERTICAL_SWIPE_BUFFER = 0.3; // the hand must move at least this many pixels to trigger action


        protected override bool ValidateGestureStartCondition(Skeleton skeleton)
        {

            var handLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            var handRightPosition = skeleton.Joints[JointType.HandRight].Position;
            //var elbowLeftPosition = skeleton.Joints[JointType.ElbowLeft].Position;
            //var elbowRightPosition = skeleton.Joints[JointType.ElbowRight].Position;
            var shoulderRightPosition = skeleton.Joints[JointType.ShoulderRight].Position;
            var spinePosition = skeleton.Joints[JointType.Spine].Position;

            if (handRightPosition.X > spinePosition.X &&
                handRightPosition.Y < spinePosition.Y &&
                //(handRightPosition.Y < shoulderRightPosition.Y) && // rhand is above rshoulder
                //(handRightPosition.Y < elbowRightPosition.Y) && // rhand is above relbow
                //handLeftPosition.Y < spinePosition.Y)
                (handLeftPosition.Y > spinePosition.Y))
            {
                startingShoulderDiff = GestureHelper.GetJointDistance(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.ShoulderLeft]);

                validatePosition = handRightPosition;//skeleton.Joints[JointType.HandRight].Position;
                startingPosition = handRightPosition;//skeleton.Joints[JointType.HandRight].Position;
                startingLeftPosition = handLeftPosition;//skeleton.Joints[JointType.HandRight].Position;


                //System.Diagnostics.Debug.WriteLine(">>>SwipeToLeft start condition valid...");
                //System.Diagnostics.Debug.WriteLine("validatePosition: " + validatePosition);
                //System.Diagnostics.Debug.WriteLine("handRightPosition: " + handRightPosition);
                //System.Diagnostics.Debug.WriteLine("shoulderRightPosition: " + shoulderRightPosition);
                //System.Diagnostics.Debug.WriteLine("difference: " + shoulderDiff);

                return true;
            }

            return false;

        }

        protected override bool IsGestureValid(Skeleton skeleton)
        {

            var currentHandRightPosition = skeleton.Joints[JointType.HandRight].Position;
            float currentShoulderDiff = GestureHelper.GetJointDistance(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.ShoulderLeft]);

            if ((validatePosition.X > currentHandRightPosition.X) &&
               (Math.Abs(startingPosition.Y - currentHandRightPosition.Y) < VERTICAL_SWIPE_BUFFER)
                && currentShoulderDiff < validateShoulderDiff)
            {
                System.Diagnostics.Debug.WriteLine("^^^SwipeToLeft gesture is valid...");
                validatePosition = currentHandRightPosition;
                validateShoulderDiff = currentShoulderDiff;
                return true;
            }
            return false;
        }

        protected override bool ValidateGestureEndCondition(Skeleton skeleton)
        {

            double distance = Math.Abs(startingPosition.X - validatePosition.X);
            //float currentShoulderDiff = GestureHelper.GetJointDistance(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.ShoulderLeft]);

            if ((distance > HORIZONTAL_SWIPE_BUFFER) && (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.Spine].Position.Y)) //The left hand is still below the spine
            // && currentShoulderDiff < startingShoulderDiff)
            {
                System.Diagnostics.Debug.WriteLine("SWIPE LEFT DETECTED!!!!!!!");
                return true;
            }

            return false;

        }

        protected override bool ValidateBaseCondition(Skeleton skeleton)
        {
            /*
            var handLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            var handRightPosition = skeleton.Joints[JointType.HandRight].Position;
            var elbowLeftPosition = skeleton.Joints[JointType.ElbowLeft].Position;
            var elbowRightPosition = skeleton.Joints[JointType.ElbowRight].Position;
            var shoulderLeftPosition = skeleton.Joints[JointType.ShoulderLeft].Position;
            var shoulderRightPosition = skeleton.Joints[JointType.ShoulderRight].Position;

            if (//(handRightPosition.Y > elbowRightPosition.Y) &&
                (handRightPosition.Y > shoulderRightPosition.Y) &&
                //(handLeftPosition.Y > elbowLeftPosition.Y) &&
                (handLeftPosition.Y > shoulderLeftPosition.Y))
            {
                return true;
            }

            return false;*/
            return true;

        }

    }
}
