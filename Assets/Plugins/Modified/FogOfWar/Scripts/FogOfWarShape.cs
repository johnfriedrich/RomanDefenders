using UnityEngine;

namespace FoW
{
    public abstract class FogOfWarShape
    {
        public Vector3 eyePosition;
        public Vector2 foward;
        public Vector2 offset;
        public float[] lineOfSight;
        public bool[] visibleCells;
        public float radius;
    }

    public class FogOfWarShapeCircle : FogOfWarShape
    {
        public float innerRadius;
        public float angle;

        public byte GetFalloff(float normdist)
        {
            if (normdist < innerRadius)
                return 0;
            return (byte)(Mathf.InverseLerp(innerRadius, 1, normdist) * 255);
        }
    }

    public class FogOfWarShapeBox : FogOfWarShape
    {
        //
    }

    public class FogOfWarShapeTexture : FogOfWarShape
    {
        public Texture2D texture;
        public bool rotateToForward = false;
    }
}
