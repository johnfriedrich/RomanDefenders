using UnityEngine;
using System.Collections.Generic;

namespace FoW
{
    class FogOfWarChunk
    {
        public Vector3i coordinate;
        public byte[] fogData;
    }

    [AddComponentMenu("FogOfWar/FogOfWarChunkManager")]
    [RequireComponent(typeof(FogOfWar))]
    public class FogOfWarChunkManager : MonoBehaviour
    {
        public Transform followTransform;
        public bool rememberFog = true;
        public float verticalChunkSize = 10;
        public float verticalChunkOffset = 0;

        List<FogOfWarChunk> _chunks = new List<FogOfWarChunk>();
        public int loadedChunkCount { get { return _chunks.Count; } }
        Vector3i _loadedChunk;

        FogOfWar _fogOfWar;
        int _mapResolution;
        int _valuesPerChunk { get { return (_mapResolution * _mapResolution) / 4; } }
        Vector3 _followPosition { get { return FogOfWarConversion.WorldToFogPlane3(followTransform.position, _fogOfWar.plane); } }

        void Awake()
        {
            _fogOfWar = GetComponent<FogOfWar>();

            if (_fogOfWar.mapResolution.x != _fogOfWar.mapResolution.y)
            {
                Debug.LogError("FogOfWarChunkManager requires FogOfWar Map Resolution to be square and a power of 2!");
                enabled = false;
            }

            _mapResolution = _fogOfWar.mapResolution.x;
        }

        void Start()
        {
            ForceLoad();
        }

        Vector3i CalculateBestChunk(Vector3 pos)
        {
            Vector3i chunk;

            chunk.x = Mathf.RoundToInt(pos.x / (_fogOfWar.mapSize / 2)) - 1;
            chunk.y = Mathf.RoundToInt(pos.y / (_fogOfWar.mapSize / 2)) - 1;
            chunk.z = Mathf.FloorToInt((pos.z - verticalChunkOffset) / verticalChunkSize);
            if (pos.z - verticalChunkOffset < 0)
                --chunk.z;

            return chunk;
        }

        void SaveChunk(byte[] data, int xc, int yc)
        {
            // reuse chunk if it already exists
            Vector3i coordinate = _loadedChunk + new Vector3i(xc, yc, 0);
            FogOfWarChunk chunk = _chunks.Find(c => c.coordinate == coordinate);
            if (chunk == null)
            {
                chunk = new FogOfWarChunk();
                chunk.coordinate = coordinate;
                chunk.fogData = new byte[_valuesPerChunk];
                _chunks.Add(chunk);
            }
            else if (chunk.fogData == null || chunk.fogData.Length != _valuesPerChunk)
                chunk.fogData = new byte[_valuesPerChunk];

            int halfmapsize = _mapResolution / 2;
            int xstart = halfmapsize * xc;
            int ystart = halfmapsize * yc;

            // copy values
            for (int y = 0; y < halfmapsize; ++y)
                System.Array.Copy(data, (ystart + y) * _mapResolution + xstart, chunk.fogData, y * halfmapsize, halfmapsize);
        }

        void SaveChunks()
        {
            // save all visible chunks
            byte[] data = _fogOfWar.fogValues;
            for (int y = 0; y < 2; ++y)
            {
                for (int x = 0; x < 2; ++x)
                    SaveChunk(data, x, y);
            }
        }

        void LoadChunk(byte[] data, int xc, int yc)
        {
            // only load if the chunk exists
            Vector3i coordinate = _loadedChunk + new Vector3i(xc, yc, 0);
            FogOfWarChunk chunk = _chunks.Find(c => c.coordinate == coordinate);
            if (chunk == null || chunk.fogData == null || chunk.fogData.Length != _valuesPerChunk)
                return;

            int halfmapsize = _mapResolution / 2;
            int xstart = halfmapsize * xc;
            int ystart = halfmapsize * yc;

            // copy values
            for (int y = 0; y < halfmapsize; ++y)
                System.Array.Copy(chunk.fogData, y * halfmapsize, data, (ystart + y) * _mapResolution + xstart, halfmapsize);
        }

        void LoadChunks()
        {
            byte[] data = new byte[_mapResolution * _mapResolution];

            // set fog full by default
            for (int i = 0; i < data.Length; ++i)
                data[i] = 255;

            // load each visible chunk
            for (int y = 0; y < 2; ++y)
            {
                for (int x = 0; x < 2; ++x)
                    LoadChunk(data, x, y);
            }

            // put the new map into fow
            _fogOfWar.fogValues = data;
        }

        void ForceLoad()
        {
            if (followTransform == null)
                return;

            Vector3i desiredchunk = CalculateBestChunk(_followPosition);

            // move fow
            float chunksize = _fogOfWar.mapSize * 0.5f;
            _fogOfWar.mapOffset = desiredchunk.vector2 * chunksize + Vector2.one * (chunksize);
            _loadedChunk = desiredchunk;
            _fogOfWar.Reinitialize();

            LoadChunks();
        }

        void Update()
        {
            if (followTransform == null)
                return;

            // is fow in the best position?
            if (CalculateBestChunk(_followPosition) != _loadedChunk)
            {
                SaveChunks();
                ForceLoad();

                // clear memory 
                if (!rememberFog)
                    _chunks.Clear();
            }
        }

        public void Clear()
        {
            _chunks.Clear();
        }
    }
}
