using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshGenerator
{
    //Triangular Face
    public struct Face
    {
        public int a;
        public int b;
        public int c;

        public int idFace;
        public Vector3 center;

        public Face(int a, int b, int c, int idFace, Vector3 center)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.idFace = idFace;
            this.center = center;
        }
    }
}

// Calle 31 # 71-71  Por la estacion Rosales metroplus