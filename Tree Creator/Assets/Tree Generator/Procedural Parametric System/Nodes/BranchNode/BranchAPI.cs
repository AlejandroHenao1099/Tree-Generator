using System.Collections.Generic;
using UnityEngine;

namespace TreeCreator
{
    public partial class BranchNode
    {
        public Vector3[] GetPoints() => points.ToArray();
        public IBranchWrite[] GetChilds() => childs != null ? childs.ToArray() : null;
        public int GetLevelBranch() => levelBranch;

        public void UpdateCoreData(int levelBranch, IBranchRead parent)
        {
            this.levelBranch = levelBranch;
            this.parent = parent;
        }

        public void UpdateIndexData(int indexBranch, int localIndex)
        {
            this.indexBranch = indexBranch;
            this.localIndex = localIndex;
        }

        public float GetLenght()
        {
            float lenghtBranch = 0f;
            float lenght_child_max = branchData.NLength + branchData.NLengthV;
            float offset = parent.GetOffset(normalizedPosition);
            if (levelBranch == 1)
                lenghtBranch = parent.GetLenght() * lenght_child_max *
                    TreeUtils.ShapeRatio(treeData.Shape, normalizedPosition);
            else
                lenghtBranch = lenght_child_max * (parent.GetLenght() - 0.6f * offset);

            return Mathf.Max(0.01f, lenghtBranch);
        }

        public void UpdateUp(Vector3 up)
        {
            this.up = up;
        }

        public void UpdateNormalizePosition(float newPos)
        {
            newPos = Mathf.Clamp01(newPos);
            normalizedPosition = newPos;
        }

        public float GetOffset(float normalizePosition)
        {
            normalizePosition = Mathf.Clamp01(normalizePosition);
            return mainSpline.GetLenghtAt(normalizePosition);
        }

        public Mesh GetBranchMesh() => mesh;

        public float GetRadiusBase()
        {
            float radius = parent.GetRadiusBase() * Mathf.Pow((GetLenght() / parent.GetLenght()), treeData.RatioPower);
            return Mathf.Max(0.00001f, radius);
        }

        private float GetRadiusAt(float z)
        {
            z = Mathf.Clamp01(z);
            float unit_taper = GetUnitTaper();
            float taper = branchData.NTaper;
            float radiusStem = GetRadiusBase();
            float taperZ = radiusStem * (1f - unit_taper * z);

            float radiusZ = 0f;
            if (taper >= 0 && taper < 1)
                radiusZ = taperZ;
            else if (taper >= 1 && taper <= 3)
            {
                float z_2 = (1f - z) * GetLenght();
                float depth = 0;
                if (taper < 2 || z_2 < taperZ)
                    depth = 1;
                else
                    depth = taper - 2;

                float z_3 = 0f;
                if (taper < 2)
                    z_3 = z_2;
                else
                    z_3 = z_2 - 2f * taperZ * (int)(z_2 / (2 * taperZ) + 0.5f);

                if (taper < 2 && z_3 >= taperZ)
                    radiusZ = taperZ;
                else
                    radiusZ = (1f - depth) * taperZ + depth *
                        Mathf.Sqrt(Mathf.Abs((taperZ * taperZ) - (z_3 - taperZ) * (z_3 - taperZ)));
            }
            return Mathf.Max(0f, radiusZ);
        }

        private float GetUnitTaper()
        {
            float nTaper = branchData.NTaper;
            if (nTaper >= 0f && nTaper < 1f)
                return nTaper;
            if (nTaper >= 1f && nTaper < 2f)
                return 2f - nTaper;
            return 0f;
        }

        public void GetPNBOnSurface(float t, float angle,
            out Vector3 position, out Vector3 normal, out Vector3 bitangent)
        {
            GetNormalizedIndex(t, angle, out int indexRing, out int nextRing,
                out int indexAngle, out int nextAngle, out float lerpRing, out float lerpAngle);

            position = GetDataOnSurface(meshData.vertices,
                indexRing, nextRing, indexAngle, nextAngle, lerpRing, lerpAngle);

            Vector3 p00 = Vector3.zero;
            Vector3 p10 = Vector3.zero;
            Vector3 p01 = Vector3.zero;
            Vector3 p11 = Vector3.zero;
            Vector2 coords = Vector2.zero;

            coords = new Vector2(lerpAngle, lerpRing);

            p00 = meshData.vertices[indexRing * resolutionHorizontal + indexAngle];
            p10 = meshData.vertices[indexRing * resolutionHorizontal + nextAngle];
            p01 = meshData.vertices[nextRing * resolutionHorizontal + indexAngle];
            p11 = meshData.vertices[nextRing * resolutionHorizontal + nextAngle];
            position = BilinearInterpolation(coords, p00, p10, p01, p11);
            position = transformMesh.localToWorldMatrix.MultiplyPoint3x4(position);

            bitangent = (p01 - p00).normalized;
            bitangent = transformMesh.localToWorldMatrix.MultiplyVector(bitangent);

            p00 = meshData.normals[indexRing * resolutionHorizontal + indexAngle];
            p10 = meshData.normals[indexRing * resolutionHorizontal + nextAngle];
            p01 = meshData.normals[nextRing * resolutionHorizontal + indexAngle];
            p11 = meshData.normals[nextRing * resolutionHorizontal + nextAngle];
            normal = BilinearInterpolation(coords, p00, p10, p01, p11).normalized;
            normal = transformMesh.localToWorldMatrix.MultiplyVector(normal);
        }

        public void GetPNOnSurface(float t, float angle,
            out Vector3 position, out Vector3 normal)
        {
            GetNormalizedIndex(t, angle, out int indexRing, out int nextRing,
                out int indexAngle, out int nextAngle, out float lerpRing, out float lerpAngle);

            position = GetDataOnSurface(meshData.vertices,
                indexRing, nextRing, indexAngle, nextAngle, lerpRing, lerpAngle);

            position = transformMesh.localToWorldMatrix.MultiplyPoint3x4(position);


            normal = GetDataOnSurface(meshData.normals,
                indexRing, nextRing, indexAngle, nextAngle, lerpRing, lerpAngle).normalized;

            normal = transformMesh.localToWorldMatrix.MultiplyVector(normal);
        }

        public Vector3 GetPositionOnSurface(float t, float angle)
        {
            GetNormalizedIndex(t, angle, out int indexRing, out int nextRing,
                out int indexAngle, out int nextAngle, out float lerpRing, out float lerpAngle);

            var meshPosition = GetDataOnSurface(meshData.vertices,
                indexRing, nextRing, indexAngle, nextAngle, lerpRing, lerpAngle);

            return transformMesh.localToWorldMatrix.MultiplyPoint3x4(meshPosition);
        }

        public Vector3 GetNormalOnSurface(float t, float angle)
        {
            GetNormalizedIndex(t, angle, out int indexRing, out int nextRing,
                out int indexAngle, out int nextAngle, out float lerpRing, out float lerpAngle);

            var meshNormal = GetDataOnSurface(meshData.normals,
                indexRing, nextRing, indexAngle, nextAngle, lerpRing, lerpAngle).normalized;

            return transformMesh.localToWorldMatrix.MultiplyVector(meshNormal);
        }

        public Vector3 GetBitangentOnSurface(float t)
        {
            t = Mathf.Clamp01(t);

            if (t >= 1)
                return (meshData.vertices[(resolutionVertical - 1) * resolutionHorizontal] -
                        meshData.vertices[(resolutionVertical - 2) * resolutionHorizontal]).normalized;

            float lerpRing = Mathf.Lerp(0, resolutionVertical - 1, t);
            int indexRing = Mathf.FloorToInt(lerpRing);
            int nextRing = indexRing + 1 >= resolutionVertical ? indexRing : indexRing + 1;

            var meshBitangent = (meshData.vertices[nextRing * resolutionHorizontal] -
                        meshData.vertices[indexRing * resolutionHorizontal]).normalized;

            return transformMesh.localToWorldMatrix.MultiplyVector(meshBitangent);
        }

        private Vector3 GetDataOnSurface(List<Vector3> data,
            int indexRing, int nextRing, int indexAngle, int nextAngle, float lerpRing, float lerpAngle)
        {
            Vector3 p00 = Vector3.zero;
            Vector3 p10 = Vector3.zero;
            Vector3 p01 = Vector3.zero;
            Vector3 p11 = Vector3.zero;
            Vector2 coords = Vector2.zero;

            coords = new Vector2(lerpAngle, lerpRing);

            p00 = data[indexRing * resolutionHorizontal + indexAngle];
            p10 = data[indexRing * resolutionHorizontal + nextAngle];
            p01 = data[nextRing * resolutionHorizontal + indexAngle];
            p11 = data[nextRing * resolutionHorizontal + nextAngle];
            return BilinearInterpolation(coords, p00, p10, p01, p11);
        }

        private void GetNormalizedIndex(float t, float angle,
            out int indexRing, out int nextRing, out int indexAngle, out int nextAngle,
                out float lerpRing, out float lerpAngle)
        {
            t = Mathf.Clamp01(t);
            angle = angle >= 0 ? angle % 360 : -(Mathf.Abs(angle) % 360);
            angle = angle >= 0 ? angle : 360 + angle;

            lerpRing = Mathf.Lerp(0, resolutionVertical - 1, t);
            indexRing = Mathf.FloorToInt(lerpRing);
            lerpRing -= Mathf.Floor(lerpRing);

            lerpAngle = Mathf.InverseLerp(0, 360, angle);
            lerpAngle = Mathf.Lerp(0, resolutionHorizontal, lerpAngle);
            indexAngle = Mathf.FloorToInt(lerpAngle);
            lerpAngle -= Mathf.Floor(lerpAngle);

            nextRing = indexRing + 1 >= resolutionVertical ? indexRing : indexRing + 1;
            nextAngle = indexAngle + 1 >= resolutionHorizontal ? 0 : indexAngle + 1;

            if (t >= 1)
            {
                indexRing = resolutionVertical - 2;
                nextRing = resolutionVertical - 1;
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