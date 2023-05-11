using UnityEngine;

public interface IShape
{
    void Update();
    Vector3 GetPositionOnSurface(Vector2 coords);
}
