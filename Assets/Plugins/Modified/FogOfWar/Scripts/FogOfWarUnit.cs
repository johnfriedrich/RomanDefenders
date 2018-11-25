using UnityEngine;
using System.Collections.Generic;

namespace FoW
{
    public enum FogOfWarShapeType
    {
        Circle,
        Box,
        Texture
    }

    [AddComponentMenu("FogOfWar/FogOfWarUnit")]
    public class FogOfWarUnit : MonoBehaviour
    {
        public int team = 0;

        [Header("Shape")]
        public FogOfWarShapeType shapeType = FogOfWarShapeType.Circle;
        public Vector2 offset = Vector2.zero;

        // circle
        public float radius = 5.0f;
        [Range(0.0f, 1.0f)]
        public float innerRadius = 1;
        [Range(0.0f, 180.0f)]
        public float angle = 180;

        // texture
        public Texture2D texture;
        public bool rotateToForward = false;

        [Header("Line of Sight")]
        public LayerMask lineOfSightMask = 0;
        public float lineOfSightPenetration = 0;
        public bool cellBased = false;
        public bool antiFlicker = false;

        float[] _distances = null;

        Transform _transform;

        static List<FogOfWarUnit> _registeredUnits = new List<FogOfWarUnit>();
        public static List<FogOfWarUnit> registeredUnits { get { return _registeredUnits; } }

        void Awake()
        {
            _transform = transform;
        }

        void OnEnable()
        {
            registeredUnits.Add(this);
        }

        void OnDisable()
        {
            registeredUnits.Remove(this);
        }

        static bool CalculateLineOfSight3D(Vector3 eye, float radius, float penetration, LayerMask layermask, float[] distances, Vector3 up, Vector3 forward)
        {
            bool hashit = false;
            float angle = 360.0f / distances.Length;
            RaycastHit hit;

            for (int i = 0; i < distances.Length; ++i)
            {
                Vector3 dir = Quaternion.AngleAxis(angle * i, up) * forward;
                if (Physics.Raycast(eye, dir, out hit, radius, layermask))
                {
                    distances[i] = (hit.distance + penetration) / radius;
                    if (distances[i] < 1)
                        hashit = true;
                    else
                        distances[i] = 1;
                }
                else
                    distances[i] = 1;
            }

            return hashit;
        }

        public float[] CalculateLineOfSight(FogOfWarPhysics physicsmode, Vector3 eyepos, FogOfWarPlane plane)
        {
            if (lineOfSightMask == 0)
                return null;

            if (_distances == null)
                _distances = new float[256];
                if (CalculateLineOfSight3D(eyepos, radius, lineOfSightPenetration, lineOfSightMask, _distances, Vector3.up, Vector3.forward))
                    return _distances;
            return null;
        }

        static float Sign(float v)
        {
            if (Mathf.Approximately(v, 0))
                return 0;
            return v > 0 ? 1 : -1;
        }

        void FillShape(FogOfWar fow, FogOfWarShape shape)
        {
            if (antiFlicker)
            {
                // snap to nearest fog pixel
                shape.eyePosition = FogOfWarConversion.SnapWorldPositionToNearestFogPixel(fow, FogOfWarConversion.WorldToFogPlane(_transform.position, fow.plane), fow.mapOffset, fow.mapResolution, fow.mapSize);
                shape.eyePosition = FogOfWarConversion.FogPlaneToWorld(shape.eyePosition.x, shape.eyePosition.y, _transform.position.y, fow.plane);
            }
            else
                shape.eyePosition = _transform.position;
            shape.foward = FogOfWarConversion.TransformFogPlaneForward(_transform, fow.plane);
            shape.offset = offset;
            shape.radius = radius;
        }

        FogOfWarShape CreateShape(FogOfWar fow)
        {
            if (shapeType == FogOfWarShapeType.Circle)
            {
                FogOfWarShapeCircle shape = new FogOfWarShapeCircle();
                FillShape(fow, shape);
                shape.innerRadius = innerRadius;
                shape.angle = angle;
                return shape;
            }
            else if (shapeType == FogOfWarShapeType.Box)
            {
                FogOfWarShapeBox shape = new FogOfWarShapeBox();
                FillShape(fow, shape);
                return shape;
            }
            else if (shapeType == FogOfWarShapeType.Texture)
            {
                if (texture == null)
                    return null;

                FogOfWarShapeTexture shape = new FogOfWarShapeTexture();
                FillShape(fow, shape);
                shape.texture = texture;
                shape.rotateToForward = rotateToForward;
                return shape;
            }
            return null;
        }

        public FogOfWarShape GetShape(FogOfWar fow, FogOfWarPhysics physics, FogOfWarPlane plane)
        {
            FogOfWarShape shape = CreateShape(fow);
            if (shape == null)
                return null;
            shape.lineOfSight = CalculateLineOfSight(physics, shape.eyePosition, plane);
            shape.visibleCells = null;
            return shape;
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            if (shapeType == FogOfWarShapeType.Circle)
                Gizmos.DrawWireSphere(transform.position, radius);
            else if (shapeType == FogOfWarShapeType.Box || shapeType == FogOfWarShapeType.Texture)
                Gizmos.DrawWireCube(transform.position, new Vector3(radius, radius, radius));
        }
    }
}