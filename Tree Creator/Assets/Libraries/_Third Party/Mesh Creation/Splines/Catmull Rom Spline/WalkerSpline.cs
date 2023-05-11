using UnityEngine;

namespace MeshGenerator
{
    public struct WalkerSpline
    {
        private Vector3 currentDirection, previousDirection;
        private Vector3 right, left, forward, up, down, minusRight, minusLeft;

        private Matrix4x4 transform;

        public WalkerSpline(Vector3 forward, Vector3 up)
        {
            transform = new Matrix4x4();
            var q = Quaternion.LookRotation(forward, up);
            transform.SetTRS(Vector3.zero, q, Vector3.one);

            currentDirection = forward;
            previousDirection = forward;
            right = left = this.forward = this.up = down = minusLeft = minusRight = Vector3.zero;
        }

        public Vector3 GetCylindricalPoint(float angle, float radius)
        {
            float angleRad = angle * Mathf.Deg2Rad;
            var newPoint = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0) * radius;
            newPoint = transform.MultiplyPoint3x4(newPoint);
            return newPoint;
        }

        public Vector3 GetPoint(Vector3 point)
        {
            return transform.MultiplyPoint3x4(point);
        }

        public void UpdateDirection(Vector3 newDirection)
        {
            currentDirection = newDirection;
            var currentPos = transform.MultiplyPoint3x4(Vector3.zero);

            Vector3 newUp = CalculateNewUp();
            Quaternion q = Quaternion.LookRotation(newDirection, newUp);

            transform.SetTRS(currentPos, q, Vector3.one);
        }

        public void UpdatePosition(Vector3 newPosition)
        {
            Quaternion q = transform.rotation;
            transform.SetTRS(newPosition, q, Vector3.one);
        }

        private Vector3 CalculateNewUp()
        {
            // CalculateCurrentRotationAxis();
            // SearchAxisNearToUp();
            // previousDirection = currentDirection;
            // return SearchAxisNearToUp();
            var newDir = currentDirection;
            var currentUp = transform.MultiplyVector(Vector3.up);
            var normal = Vector3.Cross(newDir, currentUp).normalized;
            var q = Quaternion.LookRotation(newDir, normal);
            var newUp = (q * Quaternion.Euler(0, 90, 0)) * Vector3.forward;
            if (Vector3.Dot(currentUp, newUp) > Vector3.Dot(currentUp, -newUp))
                return newUp;
            else
                return -newUp;
        }

        private void CalculateCurrentRotationAxis()
        {
            forward = currentDirection;
            right = (currentDirection - previousDirection).normalized;
            up = Vector3.Cross(forward, right);
            right = Vector3.Cross(forward, up);
            down = -up; // negative z axis
            left = -right; // negative x axis
        }

        private Vector3 SearchAxisNearToUp()
        {
            var prevUp = transform.MultiplyVector(Vector3.up);
            forward = currentDirection;
            right = (previousDirection - forward).normalized;

            up = Vector3.Cross(forward, right).normalized;
            right = Vector3.Cross(forward, up);

            down = -up;
            left = -right;
            var newUp = Vector3.zero;

            var nearUp = Vector3.Dot(prevUp, up) > Vector3.Dot(prevUp, down) ? up : down;
            float heightProject = Vector3.Dot(nearUp, prevUp);

            var cosUp = Vector3.Dot(prevUp, nearUp);
            var sinUp = Mathf.Sqrt(1f - (cosUp * cosUp));
            var minusUpBase = nearUp * heightProject;
            minusRight = (minusUpBase + (right * sinUp)).normalized;
            minusLeft = (minusUpBase + (left * sinUp)).normalized;

            var minusLateral = Vector3.Dot(prevUp, minusRight) > Vector3.Dot(prevUp, minusLeft) ? minusRight : minusLeft;

            var dotTransUp = Vector3.Dot(up, prevUp);

            if (dotTransUp >= 0.8f || dotTransUp <= -0.8f)
            {
                if (Vector3.Dot(prevUp, up) > Vector3.Dot(prevUp, down))
                    nearUp = up;
                else
                    newUp = down;
            }
            else if (dotTransUp <= 0.2f && dotTransUp >= -0.2f)
            {
                if (Vector3.Dot(prevUp, right) > Vector3.Dot(prevUp, left))
                    newUp = right;
                else
                    newUp = left;
            }
            else
            {
                newUp = minusLateral;
            }
            previousDirection = currentDirection;
            return newUp;



            // float maxDot = Mathf.NegativeInfinity;
            // Vector3 currentTransformUp = transform.MultiplyVector(Vector3.up);
            // int currentIndex = -1;

            // float dot = Vector3.Dot(currentTransformUp, up);
            // if (dot > maxDot && dot > 0.75f)
            // {
            //     Debug.Log(0);
            //     currentIndex = 0;
            //     maxDot = dot;
            // }
            // dot = Vector3.Dot(currentTransformUp, down);
            // if (dot > maxDot && dot > 0.75f)
            // {
            //     Debug.Log(1);
            //     currentIndex = 1;
            //     maxDot = dot;
            // }
            // dot = Vector3.Dot(currentTransformUp, right);
            // if (dot > maxDot && dot > 0.75f)
            // {
            //     Debug.Log(2);
            //     currentIndex = 2;
            //     maxDot = dot;
            // }
            // dot = Vector3.Dot(currentTransformUp, left);
            // if (dot > maxDot && dot > 0.75f)
            // {
            //     Debug.Log(3);
            //     currentIndex = 3;
            //     maxDot = dot;
            // }
            // if (currentIndex != -1)
            // {
            //     Debug.Log(-1);
            //     return (currentIndex == 0 ? up : currentIndex == 1 ? down : currentIndex == 2 ? right : left);
            // }
            // else
            // {
            //     Debug.Log(-2);
            //     return transform.MultiplyVector(Vector3.up);
            // }
        }

    }
}