using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

namespace TreeCreator
{
    public partial class TrunkNode
    {
        public Vector3[] GetPoints() => points.ToArray();
        public IBranchWrite[] GetChilds() => childs.ToArray();

        public void GetAxis(out Vector3 forward, out Vector3 up, out Vector3 right)
        {
            var currOr = GetCurrentOrientation();
            forward = currOr.MultiplyVector(Vector3.forward);
            right = currOr.MultiplyVector(Vector3.right);
            up = currOr.MultiplyVector(Vector3.up);
        }

        public float GetLenght()
        {
            float scaleTree = treeData.GetScaleTree();
            float lenght = (trunkData.NLength + trunkData.NLengthV) * scaleTree;
            return Mathf.Max(0.01f, lenght);
        }

        public float GetOffset(float normalizePosition)
        {
            normalizePosition = Mathf.Clamp01(normalizePosition);
            return mainSpline.GetLenghtAt(normalizePosition);
        }

        public void GetSplineinUse(out DynamicSpline spline)
        {
            if (trunkData.ShapeCurve == ShapeCurve.Default)
            {
                spline = mainSpline;
                return;
            }
            spline = secondarySpline;
        }

        public Mesh GetTrunkMesh() => mesh;

        public float GetRadiusBase()
        {
            float radius = GetLenght() * treeData.Ratio * trunkData.NScale;
            return Mathf.Max(0.000001f, radius);
        }

        private float GetRadiusAt(float t)
        {
            float z = Mathf.Clamp01(t);
            float unit_taper = GetUnitTaper();
            float taper = GetTaper();
            float radiusStem = GetLenght() * treeData.Ratio * trunkData.NScale;
            float taperZ = radiusStem * (1f - unit_taper * z);

            float radiusZ = 0f;
            if (taper >= 0f && taper < 1f)
                radiusZ = taperZ;
            else if (taper >= 1f && taper <= 3f)
            {
                float z_2 = (1f - z) * GetLenght();
                float depth = 0f;
                if (taper < 2f || z_2 < taperZ)
                    depth = 1f;
                else
                    depth = taper - 2f;

                float z_3 = 0f;
                if (taper < 2f)
                    z_3 = z_2;
                else
                    z_3 = z_2 - 2f * taperZ * (int)(z_2 / (2f * taperZ) + 0.5f);

                if (taper < 2f && z_3 >= taperZ)
                    radiusZ = taperZ;
                else
                    radiusZ = (1f - depth) * taperZ + depth *
                        Mathf.Sqrt(Mathf.Abs((taperZ * taperZ) - (z_3 - taperZ) * (z_3 - taperZ)));
            }
            float y = 1f - 8f * z;
            y = Mathf.Max(0f, y);
            float flareZ = treeData.Flare * (Mathf.Pow(100f, y) - 1f) / 100f + 1f;
            float radius = radiusZ * flareZ;
            return Mathf.Max(0f, radius);
        }

        private float GetUnitTaper()
        {
            float taper = GetTaper();
            if (taper >= 0f && taper < 1f)
                return taper;
            if (taper >= 1f && taper < 2f)
                return 2f - taper;
            return 0f;
        }

        private float GetTaper()
        {
            return trunkData.NTaper;
        }

        public void GetPNBOnSurface(float normalizedPosition, float angle,
            out Vector3 position, out Vector3 normal, out Vector3 bitangent)
        {
            GetNormalizedIndex(normalizedPosition, angle, out int indexRing, out int nextRing,
                out int indexAngle, out int nextAngle, out float lerpRing, out float lerpAngle);

            int hor = GetResolutionHorizontal();
            Vector3 p00 = Vector3.zero;
            Vector3 p10 = Vector3.zero;
            Vector3 p01 = Vector3.zero;
            Vector3 p11 = Vector3.zero;
            Vector2 coords = Vector2.zero;

            coords = new Vector2(lerpAngle, lerpRing);

            p00 = meshData.vertices[indexRing * hor + indexAngle];
            p10 = meshData.vertices[indexRing * hor + nextAngle];
            p01 = meshData.vertices[nextRing * hor + indexAngle];
            p11 = meshData.vertices[nextRing * hor + nextAngle];
            position = BilinearInterpolation(coords, p00, p10, p01, p11);

            bitangent = (p01 - p00).normalized;

            p00 = meshData.normals[indexRing * hor + indexAngle];
            p10 = meshData.normals[indexRing * hor + nextAngle];
            p01 = meshData.normals[nextRing * hor + indexAngle];
            p11 = meshData.normals[nextRing * hor + nextAngle];
            normal = BilinearInterpolation(coords, p00, p10, p01, p11).normalized;
        }

        public void GetPNOnSurface(float t, float angle,
            out Vector3 position, out Vector3 normal)
        {
            GetNormalizedIndex(t, angle, out int indexRing, out int nextRing,
                out int indexAngle, out int nextAngle, out float lerpRing, out float lerpAngle);

            position = GetDataOnSurface(meshData.vertices,
                indexRing, nextRing, indexAngle, nextAngle, lerpRing, lerpAngle);


            normal = GetDataOnSurface(meshData.normals,
                indexRing, nextRing, indexAngle, nextAngle, lerpRing, lerpAngle).normalized;
        }

        public Vector3 GetPositionOnSurface(float normalizePosition, float angle)
        {
            return GetDataOnSurface(normalizePosition, angle, meshData.vertices);
        }

        public Vector3 GetNormalOnSurface(float normalizePosition, float angle)
        {
            return GetDataOnSurface(normalizePosition, angle, meshData.normals).normalized;
        }

        public Vector3 GetBitangentOnSurface(float normalizePosition)
        {
            normalizePosition = Mathf.Clamp01(normalizePosition);
            int hor = GetResolutionHorizontal();
            int ver = GetResolutionVertical();

            if (normalizePosition >= 1)
                return (meshData.vertices[(ver - 1) * hor] -
                        meshData.vertices[(ver - 2) * hor]).normalized;

            float lerpRing = Mathf.Lerp(0, ver - 1, normalizePosition);
            int indexRing = Mathf.FloorToInt(lerpRing);
            int nextRing = indexRing + 1 >= ver ? indexRing : indexRing + 1;

            return (meshData.vertices[nextRing * hor] -
                        meshData.vertices[indexRing * hor]).normalized;
        }

        private Vector3 GetDataOnSurface(float normalizedPosition, float angle, List<Vector3> data)
        {
            int hor = GetResolutionHorizontal();
            int ver = GetResolutionVertical();

            GetNormalizedIndex(normalizedPosition, angle, out int indexRing, out int nextRing,
                out int indexAngle, out int nextAngle, out float lerpRing, out float lerpAngle);


            Vector3 p00 = Vector3.zero;
            Vector3 p10 = Vector3.zero;
            Vector3 p01 = Vector3.zero;
            Vector3 p11 = Vector3.zero;
            Vector2 coords = Vector2.zero;

            if (normalizedPosition >= 1)
            {
                p00 = data[(ver - 2) * hor + indexAngle];
                p10 = data[(ver - 2) * hor + nextAngle];
                p01 = data[(ver - 1) * hor + indexAngle];
                p11 = data[(ver - 1) * hor + nextAngle];
                coords = new Vector2(lerpAngle, 1f);
            }
            else if (normalizedPosition <= 0)
            {
                p00 = data[0 + indexAngle];
                p10 = data[0 + nextAngle];
                p01 = data[1 * hor + indexAngle];
                p11 = data[1 * hor + nextAngle];
                coords = new Vector2(lerpAngle, 0f);
            }
            else
            {
                p00 = data[indexRing * hor + indexAngle];
                p10 = data[indexRing * hor + nextAngle];
                p01 = data[nextRing * hor + indexAngle];
                p11 = data[nextRing * hor + nextAngle];
                coords = new Vector2(lerpAngle, lerpRing);
            }

            return BilinearInterpolation(coords, p00, p10, p01, p11);
        }

        private Vector3 GetDataOnSurface(List<Vector3> data,
            int indexRing, int nextRing, int indexAngle, int nextAngle, float lerpRing, float lerpAngle)
        {
            Vector3 p00 = Vector3.zero;
            Vector3 p10 = Vector3.zero;
            Vector3 p01 = Vector3.zero;
            Vector3 p11 = Vector3.zero;
            Vector2 coords = Vector2.zero;
            int hor = GetResolutionHorizontal();

            coords = new Vector2(lerpAngle, lerpRing);

            p00 = data[indexRing * hor + indexAngle];
            p10 = data[indexRing * hor + nextAngle];
            p01 = data[nextRing * hor + indexAngle];
            p11 = data[nextRing * hor + nextAngle];
            return BilinearInterpolation(coords, p00, p10, p01, p11);
        }

        private void GetNormalizedIndex(float normalizePosition, float angle,
            out int indexRing, out int nextRing, out int indexAngle, out int nextAngle,
                out float lerpRing, out float lerpAngle)
        {
            int ver = GetResolutionVertical();
            int hor = GetResolutionHorizontal();
            normalizePosition = Mathf.Clamp01(normalizePosition);
            angle = angle >= 0 ? angle % 360 : -(Mathf.Abs(angle) % 360);
            angle = angle >= 0 ? angle : 360 + angle;

            lerpRing = Mathf.Lerp(0, ver - 1, normalizePosition);
            indexRing = Mathf.FloorToInt(lerpRing);
            lerpRing -= Mathf.Floor(lerpRing);

            lerpAngle = Mathf.InverseLerp(0, 360, angle);
            lerpAngle = Mathf.Lerp(0, hor, lerpAngle);
            indexAngle = Mathf.FloorToInt(lerpAngle);
            lerpAngle -= Mathf.Floor(lerpAngle);

            nextRing = indexRing + 1 >= ver ? indexRing : indexRing + 1;
            nextAngle = indexAngle + 1 >= hor ? 0 : indexAngle + 1;

            if (normalizePosition >= 1)
            {
                indexRing = ver - 2;
                nextRing = ver - 1;
            }
        }

        private Vector3 BilinearInterpolation(Vector2 t, Vector3 p00, Vector3 p10, Vector3 p01, Vector3 p11)
        {
            float x = Mathf.Clamp01(t.x);
            float y = Mathf.Clamp01(t.y);
            Vector3 c0 = Vector3.Lerp(p00, p10, x);
            Vector3 c1 = Vector3.Lerp(p01, p11, x);
            return Vector3.Lerp(c0, c1, y);
        }
    }
}