using UnityEngine;

namespace LODHeightMaps
{
    public class LODSystem : MonoBehaviour
    {
        public int maxLOD = 2;
        public int maxResolution = 64;
        public int quantityPlanes = 3;
        public float maxSize = 100f;

        public float frequencyNoise;

        public Material materialLOD;
        public Transform player;


        private FastNoiseLite noise;

        private Transform[] gridContainers;
        private Vector3Int[] currentPositions;
        Vector3 center;
        Vector3Int newCenter;
        // private Vector3[] corners;

        private void Start()
        {
            // corners = new Vector3[8];
            noise = new FastNoiseLite();
            noise.SetFrequency(frequencyNoise);
            center = Vector3.zero;
            CreateContainers();
            UpdatePositions(center);
            // SetPositionGrids(center);
        }

        private void CreateContainers()
        {
            gridContainers = new Transform[quantityPlanes * quantityPlanes];
            currentPositions = new Vector3Int[quantityPlanes * quantityPlanes];
            for (int i = 0; i < quantityPlanes * quantityPlanes; i++)
            {
                var newContainer = new GameObject("Grid Container");
                newContainer.AddComponent<MeshFilter>().mesh =
                    MeshGenerator.Plane.Create(maxResolution, maxResolution, maxSize, maxSize);
                newContainer.AddComponent<MeshRenderer>().material = materialLOD;
                var current = newContainer.transform;
                current.SetParent(transform);
                gridContainers[i] = current;
                gridContainers[i].gameObject.SetActive(false);
            }
            // gridContainers = new Transform[4 + (8 * (maxLOD - 1))];
            // float currSize = maxSize;
            // int i = 0;
            // for (; i < 4;)
            // {
            //     var newContainer = new GameObject("Grid Container");
            //     newContainer.AddComponent<MeshFilter>().mesh =
            //         MeshGenerator.Plane.Create(maxResolution, maxResolution, currSize, currSize);
            //     newContainer.AddComponent<MeshRenderer>().material = materialLOD;
            //     var current = newContainer.transform;
            //     current.SetParent(transform);
            //     gridContainers[i++] = current;
            // }

            // currSize *= 2;
            // for (int k = 0; k < maxLOD - 1; k++)
            // {
            //     for (int n = 0; n < 8; n++)
            //     {
            //         var newContainer = new GameObject("Grid Container");
            //         newContainer.AddComponent<MeshFilter>().mesh =
            //             MeshGenerator.Plane.Create(maxResolution, maxResolution, currSize, currSize);
            //         newContainer.AddComponent<MeshRenderer>().material = materialLOD;
            //         var current = newContainer.transform;
            //         current.SetParent(transform);
            //         gridContainers[i++] = current;
            //     }
            //     currSize *= 3;
            // }
        }

        private void UpdatePositions(Vector3 center)
        {
            Vector3 midleSquare = new Vector3(maxSize / 2, 0, maxSize / 2);
            for (int y = 0, n = 0; y < quantityPlanes; y++)
            {
                for (int x = 0; x < quantityPlanes; x++)
                {
                    if (!gridContainers[n].gameObject.activeInHierarchy)
                    {
                        gridContainers[n].gameObject.SetActive(true);
                        Vector3Int newPos = new Vector3Int(x * (int)maxSize, 0, y * (int)maxSize);
                        currentPositions[n] = newPos;
                        gridContainers[n++].localPosition = newPos;
                    }
                }
            }
        }

        private void SetPositionGrids(Vector3 currentCenter)
        {
            float currSize = maxSize;
            Vector3 midleSquare = new Vector3(currSize * 0.5f, 0, currSize * 0.5f);
            // for (int i = 0; i < 4; i++)
            for (int z = 0, i = 0; z < 2; z++)
                for (int x = 0; x < 2; x++)
                    gridContainers[i++].localPosition = currentCenter + new Vector3((float)x * currSize, 0, (float)z * currSize) - midleSquare;

            currSize *= 2;
            for (int k = 0, i = 4; k < maxLOD - 1; k++)
            {
                midleSquare = new Vector3(currSize, 0, currSize);
                for (int z = 0; z < 3; z++)
                    for (int x = 0; x < 3; x++)
                    {
                        if (x == 1 && z == 1) continue;
                        gridContainers[i++].localPosition = currentCenter + new Vector3((float)x * currSize, 0, (float)z * currSize) - midleSquare;
                    }
                currSize *= 3;
            }

        }

        private void Update()
        {
            Vector3 playerPos = player.position;
            playerPos.y = 0;
            newCenter = new Vector3Int(Mathf.RoundToInt(playerPos.x), 0, Mathf.RoundToInt(playerPos.z));
            int res = newCenter.x % (int)maxSize;
            res = res > maxSize / 2f ? -((int)maxSize - res) : res;
            newCenter.x -= res;
            res = newCenter.z % (int)maxSize;
            res = res > maxSize / 2f ? -((int)maxSize - res) : res;
            newCenter.z -= res;

            for (int i = 0; i < gridContainers.Length; i++)
            {   
                float distance = (currentPositions[i] - playerPos).sqrMagnitude;
                if (distance > maxSize * maxSize)
                {
                    for (int y = 0; y < quantityPlanes; y++)
                    {
                        for (int x = 0; x < quantityPlanes; x++)
                        {
                            bool canUpdate = true;
                            Vector3Int currPos = new Vector3Int(x * (int)maxSize, 0, y * (int)maxSize) + newCenter;
                            for (int k = 0; k < currentPositions.Length; k++)
                            {
                                if (k == i)
                                    continue;
                                if (currentPositions[k] == currPos)
                                {
                                    canUpdate = false;
                                    break;
                                }
                            }
                            if (canUpdate)
                            {
                                gridContainers[i].localPosition = currPos;
                                currentPositions[i] = currPos;
                            }
                        }
                    }
                }
            }
        }

        private void RepositionGrids()
        {
            // Vector3 midleSquare = new Vector3(maxSize * 0.5f, 0, maxSize * 0.5f);
            // float midleSize = maxSize * 0.5f;
            // for (int i = 0; i < 4; i++)
            // {

            // }
        }

        private void SetCorners()
        {
            Vector3 midleSquare = new Vector3(maxSize * 0.5f, 0, maxSize * 0.5f);
            float midleSize = maxSize * 0.5f;
            // gridContainers[0] 
            for (int i = 0; i < 8; i++)
            {

            }
        }
    }
}