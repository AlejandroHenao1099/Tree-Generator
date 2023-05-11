using System.Collections.Generic;
using UnityEngine;

namespace ProceduralMeshGeneration
{
    public class MeshSelector : MonoBehaviour
    {
        public LayerMask layerVertex;
        public float maxDistance;
        public MonoMesh monoMesh;
        private List<Transform> vertexSelected;
        private Camera cam;

        private void Awake()
        {
            vertexSelected = new List<Transform>();
            cam = Camera.main;
        }

        private void Update()
        {
            SelectVertex();
        }

        private void SelectVertex()
        {
            var screenRay = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenRay, out RaycastHit hit, maxDistance, layerVertex))
            {
                if (vertexSelected.Contains(hit.transform) == false)
                {
                    vertexSelected.Add(hit.transform);
                    if (vertexSelected.Count >= 2)
                    {

                    }
                }
            }
        }


    }
}