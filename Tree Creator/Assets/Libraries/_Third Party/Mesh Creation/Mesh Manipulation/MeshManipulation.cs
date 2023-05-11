using System.Collections.Generic;
using UnityEngine;

namespace MeshGenerator
{
    public static class MeshManipulation
    {
        private static Vector3[] vertices;
        private static Vector3[] normals;
        public static void AddNoise2Mesh(Mesh mesh, FastNoiseLite noise,
             bool domainWarm, float x = 0, float y = 0, float z = 0, float amplitude = 1)
        {
            if (vertices == null)
            {
                vertices = mesh.vertices;
                normals = mesh.normals;
            }
            var newVerts = new Vector3[vertices.Length];
            var len = vertices.Length;
            for (int i = 0; i < len; i++)
            {
                var currVer = vertices[i] + new Vector3(x, y, z);
                if (domainWarm)
                    noise.DomainWarp(ref currVer.x, ref currVer.y, ref currVer.z);

                var valueNoise = noise.GetNoise(currVer.x, currVer.y, currVer.z);
                valueNoise = (valueNoise * 0.5f) + 0.5f;
                newVerts[i] = vertices[i] + (normals[i] * valueNoise * amplitude);
            }
            mesh.vertices = newVerts;
            // mesh.RecalculateNormals();
        }
    }
}