using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace GestureRecognizer
{
    public class PullDownGesture : GestureBase
    {

        public PullDownGesture() : base(GestureType.PullDown) { }
        private SkeletonPoint validateLeftPosition;
        private SkeletonPoint validateRightPosition;
        private SkeletonPoint startingLeftPosition;
        private SkeletonPoint startingRightPosition;
        private float shoulderDiff;

        protected override bool ValidateGestureStartCondition(Microsoft.Kinect.Skeleton skeleton)
        {

            var handRightPosition = skeleton.Joints[JointType.HandRight].Position;
            var handLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            var shoulderRightPosition = skeleton.Joints[JointType.ShoulderRight].Position;
            var shoulderLeftPosition = skeleton.Joints[JointType.ShoulderLeft].Position;
            //var spinePosition = skeleton.Joints[JointType.Spine].Position;

            //System.Diagnostics.Debug.WriteLine("hands: (" + handLeftPosition.Y + "," + handRightPosition.Y + ") && shoulders: (" +
            //    shoulderLeftPosition.Y + "," + shoulderRightPosition.Y + ")");

            if ((handRightPosition.Y > shoulderRightPosition.Y) &&
                (handLeftPosition.Y > shoulderLeftPosition.Y))
            {
                validateLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
                startingLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
                validateRightPosition = skeleton.Joints[JointType.HandRight].Position;
                startingRightPosition = skeleton.Joints[JointType.HandRight].Position;
                System.Diagnostics.Debug.WriteLine("PullDownGesture start condition validated");
                return true;
            }

            return false;

        }

        protected override bool ValidateGestureEndCondition(Microsoft.Kinect.Skeleton skeleton)
        {

            var handRightPosition = skeleton.Joints[JointType.HandRight].Position;
            var handLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            var shoulderRightPosition = skeleton.Joints[JointType.ShoulderRight].Position;
            var shoulderLeftPosition = skeleton.Joints[JointType.ShoulderLeft].Position;
            //var spinePosition = skeleton.Joints[JointType.Spine].Position;

            if ((handRightPosition.Y > shoulderRightPosition.Y) &&
                (handLeftPosition.Y > shoulderLeftPosition.Y))
            {
                System.Diagnostics.Debug.WriteLine("PullDownGesture end condition validated");
                return true;
            }

            return false;

            /*double distance = Math.Abs(startingPosition.X - validatePosition.X);
            float currentShoulderDiff = GestureHelper.GetJointDistance(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.ShoulderLeft]);

            if (distance > 0.1 && currentShoulderDiff < shoulderDiff)
            {
                return true;
            }

            return false;*/

        }

        protected override bool ValidateBaseCondition(Microsoft.Kinect.Skeleton skeleton)
        {

            var handRightPosition = skeleton.Joints[JointType.HandRight].Position;
            var handLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            var shoulderRightPosition = skeleton.Joints[JointType.ShoulderRight].Position;
            var spinePosition = skeleton.Joints[JointType.Spine].Position;

            if ((handRightPosition.Y < shoulderRightPosition.Y) &&
                (handRightPosition.Y > skeleton.Joints[JointType.ElbowRight].Position.Y) &&
                (handLeftPosition.Y < spinePosition.Y))
            {
                System.Diagnostics.Debug.WriteLine("PullDownGesture validated");
                return true;
            }

            return false;

        }

        protected override bool IsGestureValid(Microsoft.Kinect.Skeleton skeleton)
        {

            var currentHandRightPosition = skeleton.Joints[JointType.HandRight].Position;
            var currentHandLeftPosition = skeleton.Joints[JointType.HandLeft].Position;
            
            if ((validateRightPosition.Y < currentHandRightPosition.Y) ||
                (validateLeftPosition.Y < currentHandLeftPosition.Y))
            {
                return false;
            }

            validateRightPosition = currentHandRightPosition;
            validateLeftPosition = currentHandLeftPosition;
            //System.Diagnostics.Debug.WriteLine("PullDownGesture is valid");
            return true;

        }

    }
}
