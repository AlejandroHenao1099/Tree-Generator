using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshGenerator
{
    public static class MeshCreator
    {
        public static Mesh Create(ShapeType shape, int resolution = 10, float radius = 1)
        {
            switch (shape)
            {
                case ShapeType.SphereUV:
                    return UVSphere.Create(resolution, resolution, radius);
                case ShapeType.Plane:
                    return UVSphere.Create(resolution, resolution, radius);
                default:
                    return null;
            }
        }
    }

    public enum ShapeType
    {
        SphereUV, Plane, CubeSphere, Cube, Polygon, Cylinder, Capsule, IcoSphere, RoundedCube, Cross, Spline,
        Octahedron,
        None
    }
}