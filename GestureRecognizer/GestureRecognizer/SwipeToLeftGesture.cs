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
        private float shoulderDiff;
        private double SWIPE_BUFFER = 50; // the hand must move at least this many pixels to trigger action

        protected override bool ValidateGestureStartCondition(Skeleton skeleton)
        {

            var handLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            var handRightPosition = skeleton.Joints[JointType.HandRight].Position;
            var elbowLeftPosition = skeleton.Joints[JointType.ElbowLeft].Position;
            var elbowRightPosition = skeleton.Joints[JointType.ElbowRight].Position;
            var shoulderRightPosition = skeleton.Joints[JointType.ShoulderRight].Position;
            var spinePosition = skeleton.Joints[JointType.Spine].Position;
            
            if ((handRightPosition.Y < shoulderRightPosition.Y) && // rhand is above rshoulder
                //(handRightPosition.Y < elbowRightPosition.Y) && // rhand is above relbow
                //handLeftPosition.Y < spinePosition.Y)
                (handLeftPosition.Y > spinePosition.Y))
            {
                shoulderDiff = GestureHelper.GetJointDistance(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.ShoulderLeft]);
                
                validatePosition = skeleton.Joints[JointType.HandRight].Position;
                startingPosition = skeleton.Joints[JointType.HandRight].Position;
                
                System.Diagnostics.Debug.WriteLine(">>>SwipeToLeft start condition valid...");
                System.Diagnostics.Debug.WriteLine("validatePosition: " + validatePosition);
                System.Diagnostics.Debug.WriteLine("handRightPosition: " + handRightPosition);
                System.Diagnostics.Debug.WriteLine("shoulderRightPosition: " + shoulderRightPosition);
                System.Diagnostics.Debug.WriteLine("difference: " + shoulderDiff);

                return true;
            }

            return false;

        }

        protected override bool IsGestureValid(Skeleton skeleton)
        {

            var currentHandRightPosition = skeleton.Joints[JointType.HandRight].Position;
            
            if (validatePosition.X > currentHandRightPosition.X && 
              (Math.Abs(validatePosition.Y - currentHandRightPosition.Y) < VERTICAL_SWIPE_BUFFER))
            {
                System.Diagnostics.Debug.WriteLine("^^^SwipeToLeft gesture is valid...");
                validatePosition = currentHandRightPosition;
                return true;
            }

            return false;
        }

        protected override bool ValidateGestureEndCondition(Skeleton skeleton)
        {

            double distance = Math.Abs(startingPosition.X - validatePosition.X);
            float currentShoulderDiff = GestureHelper.GetJointDistance(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.ShoulderLeft]);

            if (distance > SWIPE_BUFFER) // && currentShoulderDiff < shoulderDiff)
            {
                System.Diagnostics.Debug.WriteLine("<<<SwipeToLeft gesture has ended...");
                return true;
            }

            return false;

        }

        protected override bool ValidateBaseCondition(Skeleton skeleton)
        {
            
            var handLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            var handRightPosition = skeleton.Joints[JointType.HandRight].Position;
            var elbowLeftPosition = skeleton.Joints[JointType.ElbowLeft].Position;
            var elbowRightPosition = skeleton.Joints[JointType.ElbowRight].Position;
            var shoulderLeftPosition = skeleton.Joints[JointType.ShoulderLeft].Position;
            var shoulderRightPosition = skeleton.Joints[JointType.ShoulderRight].Position;

            if ((handRightPosition.Y > elbowRightPosition.Y) &&
                (handRightPosition.Y > shoulderRightPosition.Y) && 
                (handLeftPosition.Y > elbowLeftPosition.Y) &&
                (handLeftPosition.Y > shoulderLeftPosition.Y))        
            {
                return true;
            }

            return false;

        }

    }
}
