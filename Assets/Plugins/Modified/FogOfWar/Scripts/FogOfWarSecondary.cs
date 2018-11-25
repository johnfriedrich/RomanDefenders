using UnityEngine;

namespace FoW
{
    [AddComponentMenu("FogOfWar/FogOfWarSecondary")]
    [RequireComponent(typeof(Camera))]
    public class FogOfWarSecondary : MonoBehaviour
    {
        public int team = 0;

        Transform _transform;
        Camera _camera;

        void Start()
        {
            _transform = transform;
            _camera = GetComponent<Camera>();
            _camera.depthTextureMode |= DepthTextureMode.Depth;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            FogOfWar.GetFogOfWarTeam(team).RenderFog(source, destination, _camera, _transform);
        }
    }
}