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

        protected override bool ValidateGestureStartCondition(Skeleton skeleton)
        {

            var handLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            var handRightPosition = skeleton.Joints[JointType.HandRight].Position;
            var rightElbow = skeleton.Joints[JointType.ElbowRight].Position;
            var shoulderRightPosition = skeleton.Joints[JointType.ShoulderRight].Position;
            var spinePosition = skeleton.Joints[JointType.Spine].Position;
            
            if ((handRightPosition.Y < shoulderRightPosition.Y) &&
                (handRightPosition.Y > rightElbow.Y) &&
                handLeftPosition.Y < spinePosition.Y)
            {
                shoulderDiff = GestureHelper.GetJointDistance(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.ShoulderLeft]);
                validatePosition = skeleton.Joints[JointType.HandRight].Position;
                startingPosition = skeleton.Joints[JointType.HandRight].Position;
                return true;
            }

            return false;

        }

        protected override bool IsGestureValid(Skeleton skeleton)
        {

            var currentHandRightPosition = skeleton.Joints[JointType.HandRight].Position;
            
            if (validatePosition.X < currentHandRightPosition.X)
            {
                return false;
            }

            validatePosition = currentHandRightPosition;
            return true;

        }

        protected override bool ValidateGestureEndCondition(Skeleton skeleton)
        {

            double distance = Math.Abs(startingPosition.X - validatePosition.X);
            float currentShoulderDiff = GestureHelper.GetJointDistance(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.ShoulderLeft]);

            if (distance > 0.1 && currentShoulderDiff < shoulderDiff)
            {
                return true;
            }

            return false;

        }

        protected override bool ValidateBaseCondition(Skeleton skeleton)
        {
            
            var handRightPosition = skeleton.Joints[JointType.HandRight].Position;
            var handLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            var shoulderRightPosition = skeleton.Joints[JointType.ShoulderRight].Position;
            var spinePosition = skeleton.Joints[JointType.Spine].Position;

            if ((handRightPosition.Y < shoulderRightPosition.Y) &&
                (handRightPosition.Y > skeleton.Joints[JointType.ElbowRight].Position.Y) &&
                (handLeftPosition.Y < spinePosition.Y))
            {
                return true;
            }

            return false;

        }

    }
}
