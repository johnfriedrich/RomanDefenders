using UnityEngine;

public class TerrainReset : MonoBehaviour {

    [SerializeField]
    private Terrain terrain;
    private float[,] originalHeights;

    private void Awake() {
        originalHeights = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
        EventManager.Instance.OnGameResetPostEvent += ResetObject;
    }

    private void ResetObject() {
        terrain.terrainData.SetHeights(0, 0, originalHeights);
    }

}
