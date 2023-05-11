using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeCreator
{
    public struct Turtle
    {
        private Matrix4x4 orientation;

        public Turtle(int doodle)
        {
            orientation = new Matrix4x4();
            orientation.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);
        }

        public Turtle(Vector3 forward, Vector3 up)
        {
            orientation = new Matrix4x4();
            orientation.SetTRS(Vector3.zero, Quaternion.LookRotation(forward, up), Vector3.one);
        }

        public void Roll(float angle)
        {
            var currentPoint = GetPosition();
            var currentRotation = orientation.rotation;
            currentRotation *= Quaternion.Euler(0, 0, -angle);
            orientation.SetTRS(currentPoint, currentRotation, Vector3.one);
        }

        public void Pitch(float angle)
        {
            var currentPoint = GetPosition();
            var currentRotation = orientation.rotation;
            currentRotation *= Quaternion.Euler(-angle, 0, 0);
            orientation.SetTRS(currentPoint, currentRotation, Vector3.one);
        }

        public void Turn(float angle)
        {
            var currentPoint = GetPosition();
            var currentRotation = orientation.rotation;
            currentRotation *= Quaternion.Euler(0, angle, 0);
            orientation.SetTRS(currentPoint, currentRotation, Vector3.one);
        }

        public void Move(float distance)
        {
            var currentPoint = orientation.MultiplyPoint3x4(Vector3.zero);
            var forward = orientation.MultiplyVector(Vector3.forward);
            currentPoint += forward * distance;
            orientation.SetTRS(currentPoint, orientation.rotation, Vector3.one);
        }

        public void SetPosition(Vector3 newPos)
        {
            orientation.SetTRS(newPos, orientation.rotation, Vector3.one);
        }

        public Vector3 GetPosition()
        {
            return orientation.MultiplyPoint3x4(Vector3.zero);
        }

        public Vector3 GetForward()
        {
            return orientation.MultiplyVector(Vector3.forward);
        }

        public Vector3 GetRight()
        {
            return orientation.MultiplyVector(Vector3.right);
        }

        public Vector3 GetUp()
        {
            return orientation.MultiplyVector(Vector3.up);
        }

        public void Bias(Vector3 directionBias, float deltaRadians)
        {
            var currentPos = orientation.MultiplyPoint3x4(Vector3.zero);
            var newDir = Vector3.RotateTowards(GetForward(), directionBias, deltaRadians, 0f);

            Vector3 newUp = CalculateNewUp(newDir);
            Quaternion q = Quaternion.LookRotation(newDir, newUp);
            Quaternion newRotation = Quaternion.Slerp(orientation.rotation, q, 1f);
            orientation.SetTRS(currentPos, newRotation, Vector3.one);
        }

        private Vector3 CalculateNewUp(Vector3 currentDirection)
        {
            var newDir = currentDirection;
            var currentUp = orientation.MultiplyVector(Vector3.up);
            var normal = Vector3.Cross(newDir, currentUp).normalized;
            var q = Quaternion.LookRotation(newDir, normal);
            var newUp = (q * Quaternion.Euler(0, 90, 0)) * Vector3.forward;
            if (Vector3.Dot(currentUp, newUp) > Vector3.Dot(currentUp, -newUp))
                return newUp;
            else
                return -newUp;
        }

    }
}