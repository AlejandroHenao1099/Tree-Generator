using System.Collections.Generic;
using UnityEngine;
using TreeCreator;

public class MovementAlongBranch : MonoBehaviour
{
    public TrunkMono trunkMono;
    [Range(0f, 1f)]
    public float t;
    public int angle;
    private TrunkNode trunk;

    private void Start() {
        trunk = trunkMono.trunkNode;
    }

    private void Update()
    {
        transform.position = trunk.GetPositionOnSurface(t, (float)angle);
    }
}