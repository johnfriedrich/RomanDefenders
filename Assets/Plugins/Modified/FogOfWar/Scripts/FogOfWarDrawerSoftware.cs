using System;
using UnityEngine;

namespace FoW
{
    public class FogOfWarDrawerSoftware : FogOfWarDrawer
    {
        byte[] _values;
        FilterMode _filterMode;

        public override void GetValues(byte[] outvalues)
        {
            for (int i = 0; i < _values.Length; ++i)
                outvalues[i] = _values[i];
        }

        public override void SetValues(byte[] values)
        {
            for (int i = 0; i < _values.Length; ++i)
                _values[i] = values[i];
        }

        protected override void OnInitialise()
        {
            if (_values == null || _values.Length != _map.pixelCount)
                _values = new byte[_map.pixelCount];
            
            _filterMode = _map.filterMode;
        }

        public override void Clear(byte value)
        {
            for (int i = 0; i < _values.Length; ++i)
                _values[i] = value;
        }

        public override void Fade(int to, int amount)
        {
            for (int i = 0; i < _values.Length; ++i)
            {
                if (_values[i] < to)
                    _values[i] = (byte)Mathf.Min(_values[i] + amount, to);
            }
        }

        bool LineOfSightCanSee(FogOfWarShape shape, Vector2 offset, float fogradius)
        {
            if (shape.lineOfSight == null)
                return true;
            
            float idx = FogOfWarUtils.ClockwiseAngle(Vector2.up, offset) * shape.lineOfSight.Length / 360.0f;
            if (idx < 0)
                idx += shape.lineOfSight.Length;

            // sampling
            float value;
            if (_map.filterMode == FilterMode.Point)
                value = shape.lineOfSight[Mathf.RoundToInt(idx) % shape.lineOfSight.Length];
            else
            {
                int idxlow = Mathf.FloorToInt(idx);
                int idxhigh = (idxlow + 1) % shape.lineOfSight.Length;
                value = Mathf.LerpUnclamped(shape.lineOfSight[idxlow], shape.lineOfSight[idxhigh], idx % 1);
            }

            float dist = value * fogradius;
            return offset.sqrMagnitude < dist * dist;
        }

        bool LineOfSightCanSeeCell(FogOfWarShape shape, Vector2i offset)
        {
            if (shape.visibleCells == null)
                return true;

            int radius = Mathf.RoundToInt(shape.radius);
            int width = radius + radius + 1;

            offset.x += radius;
            if (offset.x < 0 || offset.x >= width)
                return true;

            offset.y += radius;
            if (offset.y < 0 || offset.y >= width)
                return true;

            return shape.visibleCells[offset.y * width + offset.x];
        }

        struct DrawInfo
        {
            public Vector2 fogCenterPos;
            public Vector2i fogEyePos;
            public Vector2 fogForward;
            public float forwardAngle;
            public int xMin;
            public int xMax;
            public int yMin;
            public int yMax;

            public DrawInfo(FogOfWarMap map, FogOfWarShape shape, float xradius, float yradius)
            {
                // convert size to fog space
                fogForward = shape.foward;
                forwardAngle = FogOfWarUtils.ClockwiseAngle(Vector2.up, fogForward) * Mathf.Deg2Rad;
                float sin = Mathf.Sin(-forwardAngle);
                float cos = Mathf.Cos(-forwardAngle);
                Vector2 relativeoffset = new Vector2(shape.offset.x * cos - shape.offset.y * sin, shape.offset.x * sin + shape.offset.y * cos);

                fogCenterPos = FogOfWarConversion.WorldToFog(FogOfWarConversion.WorldToFogPlane(shape.eyePosition, map.plane) + relativeoffset, map.offset, map.resolution, map.size);
                fogEyePos = new Vector2i(FogOfWarConversion.WorldToFog(shape.eyePosition, map.plane, map.offset, map.resolution, map.size));

                // find ranges
                if (shape.visibleCells == null)
                {
                    xMin = Mathf.Max(0, Mathf.RoundToInt(fogCenterPos.x - xradius));
                    xMax = Mathf.Min(map.resolution.x - 1, Mathf.RoundToInt(fogCenterPos.x + xradius));
                    yMin = Mathf.Max(0, Mathf.RoundToInt(fogCenterPos.y - yradius));
                    yMax = Mathf.Min(map.resolution.y - 1, Mathf.RoundToInt(fogCenterPos.y + yradius));
                }
                else
                {
                    fogCenterPos = FogOfWarConversion.SnapToNearestFogPixel(fogCenterPos);
                    fogEyePos = new Vector2i(FogOfWarConversion.SnapToNearestFogPixel(FogOfWarConversion.WorldToFog(shape.eyePosition, map.offset, map.resolution, map.size)));

                    Vector2i pos = new Vector2i(Mathf.RoundToInt(fogCenterPos.x), Mathf.RoundToInt(fogCenterPos.y));
                    Vector2i rad = new Vector2i(Mathf.RoundToInt(xradius), Mathf.RoundToInt(yradius));
                    xMin = Mathf.Max(0, Mathf.RoundToInt(pos.x - rad.x));
                    xMax = Mathf.Min(map.resolution.x - 1, Mathf.RoundToInt(pos.x + rad.x));
                    yMin = Mathf.Max(0, Mathf.RoundToInt(pos.y - rad.y));
                    yMax = Mathf.Min(map.resolution.y - 1, Mathf.RoundToInt(pos.y + rad.y));
                }
            }
        }

        byte SampleTexture(Texture2D texture, float u, float v)
        {
            // GetPixel() and GetPixelBilinear() are not supported on other threads!
            if (_map.multithreaded)
                return 0;

            float value = 0;
            if (_filterMode == FilterMode.Point)
                value = 1 - texture.GetPixel(Mathf.FloorToInt(u * texture.width), Mathf.FloorToInt(v * texture.height)).r;
            else
                value = 1 - texture.GetPixelBilinear(u, v).r;
            return (byte)(value * 255);
        }

        void Unfog(int x, int y, byte v)
        {
            int index = y * _map.resolution.x + x;
            if (_values[index] > v)
                _values[index] = v;
        }

        protected override void DrawCircle(FogOfWarShapeCircle shape)
        {
            int fogradius = Mathf.RoundToInt(shape.radius * _map.pixelSize);
            int fogradiussqr = fogradius * fogradius;
            DrawInfo info = new DrawInfo(_map, shape, fogradius, fogradius);

            // view angle stuff
            float dotangle = 1 - shape.angle / 90;

            for (int y = info.yMin; y <= info.yMax; ++y)
            {
                for (int x = info.xMin; x <= info.xMax; ++x)
                {
                    // is pixel within circle radius
                    Vector2 centeroffset = new Vector2(x, y) - info.fogCenterPos;
                    if (shape.visibleCells == null && centeroffset.sqrMagnitude >= fogradiussqr)
                        continue;

                    // check if in view angle
                    if (dotangle > -0.99f && Vector2.Dot(centeroffset.normalized, info.fogForward) <= dotangle)
                        continue;

                    // can see pixel
                    Vector2i offset = new Vector2i(x, y) - info.fogEyePos;
                    if (!LineOfSightCanSee(shape, offset.vector2, fogradius))
                        continue;

                    if (!LineOfSightCanSeeCell(shape, offset))
                        continue;

                    Unfog(x, y, shape.GetFalloff(centeroffset.magnitude / (_map.pixelSize * shape.radius)));
                }
            }
        }

        protected override void DrawBox(FogOfWarShapeBox shape)
        {
            // convert size to fog space
            float fogradius = shape.radius * _map.pixelSize;
            DrawInfo info = new DrawInfo(_map, shape, shape.radius * _map.pixelSize, shape.radius * _map.pixelSize);

            for (int y = info.yMin; y <= info.yMax; ++y)
            {
                for (int x = info.xMin; x <= info.xMax; ++x)
                {
                    // can see pixel
                    Vector2i offset = new Vector2i(x, y) - info.fogEyePos;
                    if (!LineOfSightCanSee(shape, offset.vector2, fogradius))
                        continue;

                    if (!LineOfSightCanSeeCell(shape, offset))
                        continue;

                    _values[y * _map.resolution.x + x] = 0;
                }
            }
        }

        protected override void DrawTexture(FogOfWarShapeTexture shape)
        {
            if (shape.texture == null)
                return;

            if (shape.rotateToForward)
            {
                DrawRotatedTexture(shape);
                return;
            }

            // convert size to fog space
            float fogradius = shape.radius * _map.pixelSize;
            DrawInfo info = new DrawInfo(_map, shape, shape.radius * _map.pixelSize, shape.radius * _map.pixelSize);

            for (int y = info.yMin; y <= info.yMax; ++y)
            {
                for (int x = info.xMin; x <= info.xMax; ++x)
                {
                    // can see pixel
                    Vector2i offset = new Vector2i(x, y) - info.fogEyePos;
                    if (!LineOfSightCanSee(shape, offset.vector2, fogradius))
                        continue;

                    if (!LineOfSightCanSeeCell(shape, offset))
                        continue;

                    // read texture
                    float u = Mathf.InverseLerp(info.xMin, info.xMax, x);
                    float v = Mathf.InverseLerp(info.yMin, info.yMax, y);
                    Unfog(x, y, SampleTexture(shape.texture, u, v));
                }
            }
        }

        void DrawRotatedTexture(FogOfWarShapeTexture shape)
        {
            if (shape.texture == null)
                return;

            // convert size to fog space
            float size = new Vector2(shape.radius, shape.radius).magnitude * _map.pixelSize;
            Vector2 sizemul = new Vector2(size / (shape.radius * _map.pixelSize), size / (shape.radius * _map.pixelSize));
            float fogradius = size;
            DrawInfo info = new DrawInfo(_map, shape, size, size);

            // rotation stuff
            float sin = Mathf.Sin(info.forwardAngle);
            float cos = Mathf.Cos(info.forwardAngle);

            for (int y = info.yMin; y < info.yMax; ++y)
            {
                for (int x = info.xMin; x < info.xMax; ++x)
                {
                    // get rotated uvs
                    float u = Mathf.InverseLerp(info.xMin, info.xMax, x) - 0.5f;
                    float v = Mathf.InverseLerp(info.yMin, info.yMax, y) - 0.5f;

                    float uu = (u * cos - v * sin) * sizemul.x + 0.5f;
                    float vv = (v * cos + u * sin) * sizemul.y + 0.5f;
                    if (uu < 0 || uu >= 1 || vv < 0 || vv >= 1)
                        continue;

                    // can see pixel
                    Vector2i offset = new Vector2i(x, y) - info.fogEyePos;
                    if (!LineOfSightCanSee(shape, offset.vector2, fogradius))
                        continue;

                    if (!LineOfSightCanSeeCell(shape, offset))
                        continue;

                    // read texture
                    Unfog(x, y, SampleTexture(shape.texture, uu, vv));
                }
            }
        }

        public override void Unfog(Rect rect)
        {
            rect.xMin = Mathf.Max(rect.xMin, 0);
            rect.xMax = Mathf.Min(rect.xMax, _map.resolution.x);
            rect.yMin = Mathf.Max(rect.yMin, 0);
            rect.yMax = Mathf.Min(rect.yMax, _map.resolution.y);

            for (int y = (int)rect.yMin; y < (int)rect.yMax; ++y)
            {
                for (int x = (int)rect.xMin; x < (int)rect.xMax; ++x)
                    _values[y * _map.resolution.x + x] = 0;
            }
        }
    }
}
