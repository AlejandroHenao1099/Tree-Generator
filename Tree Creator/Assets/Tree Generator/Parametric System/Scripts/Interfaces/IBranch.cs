using UnityEngine;

namespace TreeCreator
{
    public interface IBranch
    {
        public float GetLenght();
        public float GetRadiusBase();
        public float GetOffset(float normalizePosition);
        public Vector3 GetPositionOnSurface(float t, float angle);
        public Vector3 GetNormalOnSurface(float t, float angle);
        public Vector3 GetBitangentOnSurface(float t);
        public void GetPNBOnSurface(float t, float angle, 
            out Vector3 position, out Vector3 normal, out Vector3 bitangent);
        public void GetPNOnSurface(float t, float angle, 
            out Vector3 position, out Vector3 normal);
    }
}