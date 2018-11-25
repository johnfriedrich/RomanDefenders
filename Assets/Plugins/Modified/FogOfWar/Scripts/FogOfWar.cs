using UnityEngine;
using System.Collections.Generic;

namespace FoW
{
    public enum FogOfWarPhysics
    {
        None,
        Physics3D
    }

    public enum FogOfWarPlane
    {
        YZ,
        XZ // 3D
    }

    class FoWIDs
    {
        public int mainTex;
        public int skyboxTex;
        public int fogColorTex;
        public int fogColorTexScale;
        public int cameraDir;
        public int cameraWS;
        public int frustumCornersWS;
        public int fogColor;
        public int mapOffset;
        public int mapSize;
        public int fogTextureSize;
        public int fogTex;
        public int outsideFogStrength;

        public void Initialise()
        {
            mainTex = Shader.PropertyToID("_MainTex");
            skyboxTex = Shader.PropertyToID("_SkyboxTex");
            fogColorTex = Shader.PropertyToID("_FogColorTex");
            fogColorTexScale = Shader.PropertyToID("_FogColorTexScale");
            cameraDir = Shader.PropertyToID("_CameraDir");
            cameraWS = Shader.PropertyToID("_CameraWS");
            frustumCornersWS = Shader.PropertyToID("_FrustumCornersWS");
            fogColor = Shader.PropertyToID("_FogColor");
            mapOffset = Shader.PropertyToID("_MapOffset");
            mapSize = Shader.PropertyToID("_MapSize");
            fogTextureSize = Shader.PropertyToID("_FogTextureSize");
            fogTex = Shader.PropertyToID("_FogTex");
            outsideFogStrength = Shader.PropertyToID("_OutsideFogStrength");
        }
    }

    [AddComponentMenu("FogOfWar/FogOfWar")]
    public class FogOfWar : MonoBehaviour
    {
        [Header("Map")]
        public Vector2i mapResolution = new Vector2i(128, 128);
        public float mapSize = 128;
        public Vector2 mapOffset = Vector2.zero;

        public FogOfWarPlane plane = FogOfWarPlane.XZ;
        public FogOfWarPhysics physics = FogOfWarPhysics.Physics3D;
        
        [Header("Visuals")]
        public FilterMode filterMode = FilterMode.Bilinear;
        public Color fogColor = Color.black;
        public Texture2D fogColorTexture = null;
        public float fogColorTextureScale = 1;
        public float fogColorTextureHeight = 0;
        public bool fogFarPlane = true;
        [Range(0.0f, 1.0f)]
        public float outsideFogStrength = 1;
        public bool clearFog = false;
        public LayerMask clearFogMask = -1;
        public int blurAmount = 0;
        public int blurIterations = 0;
        public FogOfWarBlurType blurType = FogOfWarBlurType.Gaussian3;

        [Header("Behaviour")]
        public int team = 0;
        public bool updateAutomatically = true;
        [Range(0.0f, 1.0f)]
        public float partialFogAmount = 0.5f;
        float _fadeAmount = 0;
        public float fadeSpeed = 10;
        
        [Header("Multithreading")]
        public bool multithreaded = false;
        [Range(1, 8)]
        public int threads = 2;
        public double maxMillisecondsPerFrame = 5;
        FogOfWarThreadPool _threadPool = null;
        int _currentUnitProcessing = 0;
        float _timeSinceLastUpdate = 0;
        System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
        bool _isFirstProcessingFrame = true;

        // core stuff
        Material _material;
        public Texture2D fogTexture { get; private set; }
        Texture _finalFogTexture = null;
        byte[] _fogValuesCopy = null;
        public byte[] fogValues { get { return _fogValuesCopy; } set { _drawer.SetValues(value); } }
        FogOfWarDrawerSoftware _drawer = null;
        FogOfWarBlur _blur = new FogOfWarBlur();

        Transform _transform;
        Camera _camera;

        static FoWIDs _ids = null;
        static Shader _fogOfWarShader = null;
        public static Shader fogOfWarShader { get { if (_fogOfWarShader == null) _fogOfWarShader = Resources.Load<Shader>("FogOfWarShader"); return _fogOfWarShader; } }
        static Shader _clearFogShader = null;
        public static Shader clearFogShader { get { if (_clearFogShader == null) _clearFogShader = Resources.Load<Shader>("ClearFogShader"); return _clearFogShader; } }

        static List<FogOfWar> _instances = new List<FogOfWar>();
        public static List<FogOfWar> instances { get { return _instances; } }

        public static FogOfWar GetFogOfWarTeam(int team)
        {
            return instances.Find(f => f.team == team);
        }

        void Awake()
        {
            if (_ids == null)
            {
                _ids = new FoWIDs();
                _ids.Initialise();
            }

            Reinitialize();
        }

        void OnEnable()
        {
            _instances.Add(this);
        }

        void OnDisable()
        {
            _instances.Remove(this);
        }

        // Call this whenever you change any of the size values of the map
        [ContextMenu("Reinitialize")]
        public void Reinitialize()
        {
            if (_fogValuesCopy == null || _fogValuesCopy.Length != mapResolution.x * mapResolution.y)
                _fogValuesCopy = new byte[mapResolution.x * mapResolution.y];

            if (_drawer == null)
                _drawer = new FogOfWarDrawerSoftware();
            _drawer.Initialise(new FogOfWarMap(this));
            _drawer.Clear(255);

            if (_material == null)
            {
                _material = new Material(fogOfWarShader);
                _material.name = "FogMaterial";
            }
        }

        // Increase skip to improve performance but sacrifice accuracy
        public float ExploredArea(int skip = 1)
        {
            skip = Mathf.Max(skip, 1);
            int total = 0;
            for (int i = 0; i < _fogValuesCopy.Length; i += skip)
                total += _fogValuesCopy[i];
            return (1.0f - total / (_fogValuesCopy.Length * 255.0f / skip)) * 2;
        }

        void Start()
        {
            _transform = transform;
            _camera = GetComponent<Camera>();
            _camera.depthTextureMode |= DepthTextureMode.Depth;
        }

        public Vector2i WorldPositionToFogPosition(Vector3 position)
        {
            Vector2 mappos = FogOfWarConversion.WorldToFogPlane(position, plane) - mapOffset;
            mappos.Scale(mapResolution.vector2 / mapSize);

            Vector2i mapposi = new Vector2i(mappos);
            mapposi += new Vector2i(mapResolution.x >> 1, mapResolution.y >> 1);
            return mapposi;
        }

        // Returns a value between 0 (not in fog) and 255 (fully fogged)
        public byte GetFogValue(Vector3 position)
        {
            Vector2i mappos = WorldPositionToFogPosition(position);
            mappos.x = Mathf.Clamp(mappos.x, 0, mapResolution.x - 1);
            mappos.y = Mathf.Clamp(mappos.y, 0, mapResolution.y - 1);
            return _fogValuesCopy[mappos.y * mapResolution.x + mappos.x];
        }

        public bool IsInFog(Vector3 position, byte minfog)
        {
            return GetFogValue(position) > minfog;
        }

        public bool IsInFog(Vector3 position, float minfog)
        {
            return IsInFog(position, (byte)(minfog * 255));
        }

        public bool IsInCompleteFog(Vector3 position)
        {
            return IsInFog(position, 240);
        }

        public bool IsInPartialFog(Vector3 position)
        {
            return IsInFog(position, 20);
        }

        public void Unfog(Rect rect)
        {
            _drawer.Unfog(rect);
        }

        public void Unfog(Bounds bounds)
        {
            Rect rect = new Rect();
            rect.min = FogOfWarConversion.WorldToFog(bounds.min, plane, mapOffset, mapResolution, mapSize);
            rect.max = FogOfWarConversion.WorldToFog(bounds.max, plane, mapOffset, mapResolution, mapSize);
            Unfog(rect);
        }

        // Checks the visibility of an area, where a value of 0 is fully unfogged and 1 if fully fogged
        public float VisibilityOfArea(Bounds worldbounds)
        {
            Vector2 min = FogOfWarConversion.WorldToFog(worldbounds.min, plane, mapOffset, mapResolution, mapSize);
            Vector2 max = FogOfWarConversion.WorldToFog(worldbounds.max, plane, mapOffset, mapResolution, mapSize);

            int xmin = Mathf.Clamp(Mathf.RoundToInt(min.x), 0, mapResolution.x);
            int xmax = Mathf.Clamp(Mathf.RoundToInt(max.x), 0, mapResolution.x);
            int ymin = Mathf.Clamp(Mathf.RoundToInt(min.y), 0, mapResolution.y);
            int ymax = Mathf.Clamp(Mathf.RoundToInt(max.y), 0, mapResolution.y);

            float total = 0;
            int count = 0;
            for (int y = ymin; y < ymax; ++y)
            {
                for (int x = xmin; x < xmax; ++x)
                {
                    ++count;
                    total += _fogValuesCopy[y * mapResolution.x + x] / 255.0f;
                }
            }

            return total / count;
        }

        public void SetAll(byte value = 255)
        {
            _drawer.Clear(value);
        }

        void ProcessUnits(bool checkstopwatch)
        {
            // remove any invalid units
            FogOfWarUnit.registeredUnits.RemoveAll(u => u == null);

            double millisecondfrequency = 1000.0 / System.Diagnostics.Stopwatch.Frequency;
            for (; _currentUnitProcessing < FogOfWarUnit.registeredUnits.Count; ++_currentUnitProcessing)
            {
                if (!FogOfWarUnit.registeredUnits[_currentUnitProcessing].isActiveAndEnabled || FogOfWarUnit.registeredUnits[_currentUnitProcessing].team != team)
                    continue;

                FogOfWarShape shape = FogOfWarUnit.registeredUnits[_currentUnitProcessing].GetShape(this, physics, plane);
                if (multithreaded && updateAutomatically)
                    _threadPool.Run(() => _drawer.Draw(shape));
                else
                    _drawer.Draw(shape);

                // do the timer check here so that at least one unit will be processed!
                if (checkstopwatch && _stopwatch.ElapsedTicks * millisecondfrequency >= maxMillisecondsPerFrame)
                {
                    ++_currentUnitProcessing;
                    break;
                }
            }
        }

        [ContextMenu("Manual Update")]
        public void ManualUpdate()
        {
            ManualUpdate(1);
        }

        public void ManualUpdate(float timesincelastupdate)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Cannot do Manual Update when not playing!", this);
                return;
            }

            if (updateAutomatically)
            {
                Debug.LogWarning("Cannot do Manual Update when updateAutomatically is true!", this);
                return;
            }

            ProcessUnits(false);
            CompileFinalTexture(ref timesincelastupdate, false);
        }

        void Update()
        {
            if (!updateAutomatically)
                return;

            // prepare threads
            if (multithreaded)
            {
                if (_threadPool == null)
                    _threadPool = new FogOfWarThreadPool();
                threads = Mathf.Clamp(threads, 2, 8);
                _threadPool.maxThreads = threads;
                _threadPool.Clean();
            }
            else if (_threadPool != null)
            {
                _threadPool.StopAllThreads();
                _threadPool = null;
            }

            _stopwatch.Reset();
            _stopwatch.Start();

            // draw shapes
            ProcessUnits(true);

            // compile final texture
            _timeSinceLastUpdate += Time.deltaTime;
            CompileFinalTexture(ref _timeSinceLastUpdate, true);

            _stopwatch.Stop();
        }

        void CompileFinalTexture(ref float timesincelastupdate, bool checkstopwatch)
        {
            if (_currentUnitProcessing >= FogOfWarUnit.registeredUnits.Count && (!checkstopwatch || !multithreaded || _threadPool.hasAllFinished))
            {
                _drawer.GetValues(_fogValuesCopy);
                _currentUnitProcessing = 0;

                // prepare texture
                if (fogTexture == null)
                {
                    fogTexture = new Texture2D(mapResolution.x, mapResolution.y, TextureFormat.Alpha8, false);
                    fogTexture.wrapMode = TextureWrapMode.Clamp;
                    fogTexture.filterMode = filterMode;
                }
                else if (fogTexture.width != mapResolution.x || fogTexture.height != mapResolution.y)
                    fogTexture.Resize(mapResolution.x, mapResolution.y, TextureFormat.Alpha8, false);
                else
                    fogTexture.filterMode = filterMode;
                fogTexture.LoadRawTextureData(_fogValuesCopy);
                fogTexture.Apply();

                // apply blur
                _finalFogTexture = _blur.Apply(fogTexture, mapResolution, blurAmount, blurIterations, blurType);

                // fade in fog
                _fadeAmount += fadeSpeed * timesincelastupdate;
                byte fadebytes = (byte)(_fadeAmount * 255);
                if (fadebytes > 0)
                {
                    _drawer.Fade((byte)(partialFogAmount * 255), fadebytes);
                    _fadeAmount -= fadebytes / 255.0f;
                }

                timesincelastupdate = 0;

                if (!_isFirstProcessingFrame)
                    ProcessUnits(checkstopwatch);
                _isFirstProcessingFrame = true;
            }
            else
                _isFirstProcessingFrame = false;
        }

        // Returns the corner points relative to the camera's position (not rotation)
        static Matrix4x4 CalculateCameraFrustumCorners(Camera cam, Transform camtransform)
        {
            // Most of this was copied from the GlobalFog image effect standard asset due to the weird way to reconstruct the world position
            Matrix4x4 frustumCorners = Matrix4x4.identity;
            float camAspect = cam.aspect;
            float camNear = cam.nearClipPlane;
            float camFar = cam.farClipPlane;

            if (cam.orthographic)
            {
                float orthoSize = cam.orthographicSize;

                Vector3 far = camtransform.forward * camFar;
                Vector3 rightOffset = camtransform.right * (orthoSize * camAspect);
                Vector3 topOffset = camtransform.up * orthoSize;

                frustumCorners.SetRow(0, far + topOffset - rightOffset);
                frustumCorners.SetRow(1, far + topOffset + rightOffset);
                frustumCorners.SetRow(2, far - topOffset + rightOffset);
                frustumCorners.SetRow(3, far - topOffset - rightOffset);
            }
            else // perspective
            {
                float fovWHalf = cam.fieldOfView * 0.5f;
                float fovWHalfTan = Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

                Vector3 toRight = camtransform.right * (camNear * fovWHalfTan * camAspect);
                Vector3 toTop = camtransform.up * (camNear * fovWHalfTan);

                Vector3 topLeft = (camtransform.forward * camNear - toRight + toTop);
                float camScale = topLeft.magnitude * camFar / camNear;

                topLeft.Normalize();
                topLeft *= camScale;

                Vector3 topRight = (camtransform.forward * camNear + toRight + toTop);
                topRight.Normalize();
                topRight *= camScale;

                Vector3 bottomRight = (camtransform.forward * camNear + toRight - toTop);
                bottomRight.Normalize();
                bottomRight *= camScale;

                Vector3 bottomLeft = (camtransform.forward * camNear - toRight - toTop);
                bottomLeft.Normalize();
                bottomLeft *= camScale;

                frustumCorners.SetRow(0, topLeft);
                frustumCorners.SetRow(1, topRight);
                frustumCorners.SetRow(2, bottomRight);
                frustumCorners.SetRow(3, bottomLeft);
            }
            return frustumCorners;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            RenderFog(source, destination, _camera, _transform);
        }

        public void RenderFog(RenderTexture source, RenderTexture destination, Camera cam, Transform camtransform)
        {
            if (clearFog)
            {
                RenderTexture temprendertex = new RenderTexture(source.width, source.height, source.depth);
                RenderFogFull(source, temprendertex, cam, camtransform);
                RenderClearFog(temprendertex, destination);
                Destroy(temprendertex);
            }
            else
                RenderFogFull(source, destination, cam, camtransform);
        }

        void RenderFogFull(RenderTexture source, RenderTexture destination, Camera cam, Transform camtransform)
        {
            _material.SetTexture(_ids.fogTex, _finalFogTexture);
            _material.SetVector(_ids.fogTextureSize, mapResolution.vector2);
            _material.SetFloat(_ids.mapSize, mapSize);
            _material.SetVector(_ids.mapOffset, mapOffset);
            _material.SetColor(_ids.fogColor, fogColor);
            _material.SetMatrix(_ids.frustumCornersWS, CalculateCameraFrustumCorners(cam, camtransform));
            _material.SetVector(_ids.cameraWS, camtransform.position);
            _material.SetFloat(_ids.outsideFogStrength, outsideFogStrength);

            Vector4 camdir = camtransform.forward;
            camdir.w = cam.nearClipPlane;
            _material.SetVector(_ids.cameraDir, camdir);

            // orthographic is treated very differently in the shader, so we have to make sure it executes the right code
            _material.SetKeywordEnabled("CAMERA_PERSPECTIVE", !cam.orthographic);
            _material.SetKeywordEnabled("CAMERA_ORTHOGRAPHIC", cam.orthographic);

            // which plane will the fog be rendered to?
            _material.SetKeywordEnabled("PLANE_YZ", plane == FogOfWarPlane.YZ);
            _material.SetKeywordEnabled("PLANE_XZ", plane == FogOfWarPlane.XZ);

            _material.SetKeywordEnabled("TEXTUREFOG", fogColorTexture != null);
            if (fogColorTexture != null)
            {
                _material.SetTexture(_ids.fogColorTex, fogColorTexture);
                _material.SetVector(_ids.fogColorTexScale, new Vector2(fogColorTextureScale, fogColorTextureHeight));
            }

            _material.SetKeywordEnabled("FOGFARPLANE", fogFarPlane);
            _material.SetKeywordEnabled("CLEARFOG", clearFog);

            CustomGraphicsBlit(source, destination, _material);
        }

        void RenderClearFog(RenderTexture source, RenderTexture destination)
        {
            // create skybox camera
            Camera skyboxcamera = new GameObject("TempSkyboxFogCamera").AddComponent<Camera>();
            skyboxcamera.transform.parent = transform;
            skyboxcamera.transform.position = transform.position;
            skyboxcamera.transform.rotation = transform.rotation;
            skyboxcamera.fieldOfView = _camera.fieldOfView;
            skyboxcamera.clearFlags = CameraClearFlags.Skybox;
            skyboxcamera.targetTexture = new RenderTexture(source.width, source.height, source.depth);
            skyboxcamera.cullingMask = clearFogMask;
            skyboxcamera.orthographic = _camera.orthographic;
            skyboxcamera.orthographicSize = _camera.orthographicSize;
            skyboxcamera.rect = _camera.rect;

            // render skyboxcamera to texture
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = skyboxcamera.targetTexture;
            skyboxcamera.Render();
            Texture2D skyboximage = new Texture2D(skyboxcamera.targetTexture.width, skyboxcamera.targetTexture.height);
            skyboximage.ReadPixels(new Rect(0, 0, skyboxcamera.targetTexture.width, skyboxcamera.targetTexture.height), 0, 0);
            skyboximage.Apply();
            RenderTexture.active = currentRT;

            // overlay renders on eachother
            RenderTexture.active = destination;
            Material clearfogmat = new Material(clearFogShader);
            clearfogmat.SetTexture(_ids.skyboxTex, skyboximage);
            CustomGraphicsBlit(source, destination, clearfogmat);

            // ensure temp objects are destroyed
            Destroy(skyboxcamera.targetTexture);
            Destroy(skyboxcamera.gameObject);
            Destroy(clearfogmat);
            Destroy(skyboximage);
        }

        static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material material)
        {
            RenderTexture.active = dest;
            material.SetTexture(_ids.mainTex, source);
            material.SetPass(0);

            GL.PushMatrix();
            GL.LoadOrtho();

            GL.Begin(GL.QUADS);

            GL.MultiTexCoord2(0, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 3.0f); // BL

            GL.MultiTexCoord2(0, 1.0f, 0.0f);
            GL.Vertex3(1.0f, 0.0f, 2.0f); // BR

            GL.MultiTexCoord2(0, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f); // TR

            GL.MultiTexCoord2(0, 0.0f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 0.0f); // TL

            GL.End();
            GL.PopMatrix();
        }

        //void OnDrawGizmosSelected()
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 offset = FogOfWarConversion.FogPlaneToWorld(mapOffset.x, mapOffset.y, 0, plane);
            Vector3 size = FogOfWarConversion.FogPlaneToWorld(mapSize, mapSize, 0, plane);
            Gizmos.DrawWireCube(offset, size);

            Gizmos.color = new Color(1, 0, 0, 0.2f);
            Gizmos.DrawCube(offset, size);
        }
    }
}