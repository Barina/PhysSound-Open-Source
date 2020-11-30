using UnityEngine;
using System.Collections.Generic;

namespace PhysSound
{
    [AddComponentMenu("PhysSound/PhysSound Terrain")]
    public class PhysSoundTerrain : PhysSoundBase
    {
        public Terrain Terrain;
        public List<PhysSoundMaterial> SoundMaterials = new List<PhysSoundMaterial>();
        readonly Dictionary<int, PhysSoundComposition> compDic = new Dictionary<int, PhysSoundComposition>();

        TerrainData terrainData;
        Vector3 terrainPos;

        void Start()
        {
            terrainData = Terrain.terrainData;
            terrainPos = Terrain.transform.position;

            foreach (PhysSoundMaterial mat in SoundMaterials)
            {
                if (!compDic.ContainsKey(mat.MaterialTypeKey))
                    compDic.Add(mat.MaterialTypeKey, new PhysSoundComposition(mat.MaterialTypeKey));
            }
        }

        /// <summary>
        /// Gets the most prominent PhysSound Material at the given point on the terrain.
        /// </summary>
        public override PhysSoundMaterial GetPhysSoundMaterial(Vector3 contactPoint)
        {
            int dominantIndex = GetDominantTexture(contactPoint);

            if (dominantIndex < SoundMaterials.Count && SoundMaterials[dominantIndex] != null)
                return SoundMaterials[dominantIndex];

            return null;
        }

        /// <summary>
        /// Gets the composition of PhysSound Materials at the given point on the terrain.
        /// </summary>
        public Dictionary<int, PhysSoundComposition> GetComposition(Vector3 contactPoint)
        {
            foreach (PhysSoundComposition c in compDic.Values)
                c.Reset();

            float[] mix = GetTextureMix(contactPoint);

            for (int i = 0; i < mix.Length; i++)
            {
                if (i >= SoundMaterials.Count)
                    break;

                if (SoundMaterials[i] == null)
                    continue;

                if (compDic.TryGetValue(SoundMaterials[i].MaterialTypeKey, out PhysSoundComposition comp))
                    comp.Add(mix[i]);
            }

            return compDic;
        }

        private float[] GetTextureMix(Vector3 worldPos)
        {
            int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
            int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

            float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

            float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];

            for (int i = 0; i < cellMix.Length; i++)
                cellMix[i] = splatmapData[0, 0, i];

            return cellMix;
        }

        private int GetDominantTexture(Vector3 worldPos)
        {
            float[] mix = GetTextureMix(worldPos);

            float maxMix = 0;
            int maxMixIndex = 0;

            for (int j = 0; j < mix.Length; j++)
            {
                if (mix[j] > maxMix)
                {
                    maxMixIndex = j;
                    maxMix = mix[j];
                }
            }

            return maxMixIndex;
        }
    }

    public class PhysSoundComposition
    {
        public int KeyIndex;
        public float Value;
        public int Count;

        public PhysSoundComposition(int key)
        {
            KeyIndex = key;
        }

        public void Reset()
        {
            Value = 0;
            Count = 0;
        }

        public void Add(float val)
        {
            Value += val;
            Count++;
        }

        public float GetAverage() => Value / Count;
    }
}