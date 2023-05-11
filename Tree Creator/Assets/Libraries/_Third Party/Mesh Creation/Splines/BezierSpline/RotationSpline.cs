using UnityEngine;

namespace MeshGenerator
{
    public class RotationSpline
    {
        private Vector3 currentDirection, previousDirection;
        private Vector3 right, left, forward, up, down;

        private Matrix4x4 transform;

        public RotationSpline()
        {
            transform = new Matrix4x4();
            transform.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);

            currentDirection = Vector3.forward;
            previousDirection = currentDirection;
        }

        public Vector3 GetPointOnCircle(int angle, float radius)
        {
            float angleRad = (float)angle * Mathf.Deg2Rad;
            var newPoint = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0) * radius;
            newPoint = transform.MultiplyPoint3x4(newPoint);
            return newPoint;
        }

        public Vector3 GetPoint(Vector3 point)
        {
            return transform.MultiplyPoint3x4(point);
        }

        public void Update(Vector3 direction)
        {
            currentDirection = direction;

            Vector3 newUp = CalculateNewUp();
            Quaternion q = Quaternion.LookRotation(direction, newUp);

            transform.SetTRS(Vector3.zero, q, Vector3.one);
        }

        private Vector3 CalculateNewUp()
        {
            CalculateCurrentRotationAxis();
            SearchAxisNearToUp();
            previousDirection = currentDirection;
            return SearchAxisNearToUp();
        }


        private void CalculateCurrentRotationAxis()
        {
            forward = currentDirection;
            right = (currentDirection - previousDirection).normalized;
            up = Vector3.Cross(forward, right);
            down = -up; // negative z axis
            left = -right; // negative x axis
        }

        private Vector3 SearchAxisNearToUp()
        {
            float maxDot = Mathf.NegativeInfinity;
            Vector3 currentTransformUp = transform.MultiplyPoint3x4(Vector3.up);
            int currentIndex = -1;

            float dot = Vector3.Dot(currentTransformUp, up);
            if (dot > maxDot && dot > 0.75f)
            {
                currentIndex = 0;
                maxDot = dot;
            }
            dot = Vector3.Dot(currentTransformUp, down);
            if (dot > maxDot && dot > 0.75f)
            {
                currentIndex = 1;
                maxDot = dot;
            }
            dot = Vector3.Dot(currentTransformUp, right);
            if (dot > maxDot && dot > 0.75f)
            {
                currentIndex = 2;
                maxDot = dot;
            }
            dot = Vector3.Dot(currentTransformUp, left);
            if (dot > maxDot && dot > 0.75f)
            {
                currentIndex = 3;
                maxDot = dot;
            }
            if (currentIndex != -1)
                return (currentIndex == 0 ? up : currentIndex == 1 ? down : currentIndex == 2 ? right : left);
            else
                return transform.MultiplyPoint3x4(Vector3.up);
        }
    }
}