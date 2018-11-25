using UnityEngine;

public class TerrainLeveler : MonoBehaviour {

    public int SmoothArea;

    private Terrain myTerrain;
    private int xResolution;
    private int zResolution;

    private void OnEnable () {
        myTerrain = Terrain.activeTerrain;
        xResolution = myTerrain.terrainData.heightmapWidth;
        zResolution = myTerrain.terrainData.heightmapHeight;
        LevelTerrain();
    }
	
    private void LevelTerrain() {
        RaiselowerTerrainArea(transform.position, 12, 12, SmoothArea, myTerrain.SampleHeight(transform.position)/myTerrain.terrainData.size.y);
        enabled = false;
    }

    private void RaiselowerTerrainArea(Vector3 point, int lenx, int lenz, int smooth, float incdec) {
        int areax;
        int areaz;
        smooth += 1;
        float smoothing;
        int terX = (int)((point.x / myTerrain.terrainData.size.x) * xResolution);
        int terZ = (int)((point.z / myTerrain.terrainData.size.z) * zResolution);
        lenx += smooth;
        lenz += smooth;
        terX -= (lenx / 2);
        terZ -= (lenz / 2);
        if (terX < 0)
            terX = 0;
        if (terX > xResolution)
            terX = xResolution;
        if (terZ < 0)
            terZ = 0;
        if (terZ > zResolution)
            terZ = zResolution;
        float[,] heights = myTerrain.terrainData.GetHeights(terX, terZ, lenx, lenz);
        float y = incdec;
        for (smoothing = 1; smoothing < smooth + 1; smoothing++) {
            for (areax = (int)(smoothing / 2); areax < lenx - (smoothing / 2); areax++) {
                for (areaz = (int)(smoothing / 2); areaz < lenz - (smoothing / 2); areaz++) {
                    if ((areax > -1) && (areaz > -1) && (areax < xResolution) && (areaz < zResolution)) {
                        heights[areax, areaz] = y;
                    }
                }
            }
        }
        myTerrain.terrainData.SetHeights(terX, terZ, heights);
    }

}
