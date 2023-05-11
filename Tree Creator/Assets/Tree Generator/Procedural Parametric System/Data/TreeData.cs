using UnityEngine;

namespace TreeCreator
{
    [System.Serializable]
    public class TreeData
    {
        [Header("General Setting")]
        [SerializeField]
        private Shape shape = Shape.InverseConical;
        [SerializeField, Range(0f, 1f)]
        private float baseSize = 0.4f;
        [SerializeField]
        private float scale = 1;
        [SerializeField]
        private float scaleV = 0;
        [SerializeField]
        private float zScale = 0;
        [SerializeField]
        private float zScaleV = 0;
        [SerializeField,Range(0, 10), Min(0)]
        private int levels = 1;



        [Header("Radius Settings")]
        [SerializeField, Min(0.01f)]
        private float ratio = 0.015f;
        [SerializeField, Min(0.01f)]
        private float ratioPower = 1;
        [SerializeField, Min(0)]
        private int lobes = 5;
        [SerializeField, Min(0f)]
        private float lobeDepth = 0;
        [SerializeField, Min(0f)]
        private float flare = 0f;


        [Header("Leaves Settings")]
        [SerializeField, Min(0)]
        private int leaves;
        [SerializeField,Min(0.01f)]
        private float leafScale;
        [SerializeField, Min(0.01f)]
        private float leafScaleX;


        [SerializeField, Header("Bias Settings")]
        private float attractionUp;


        [Header("Pruning Settings")]
        [SerializeField]
        private float pruneRatio;
        [SerializeField]
        private float pruneWidth;
        [SerializeField]
        private float pruneWidthPeak;
        [SerializeField]
        private float prunePowerLow;
        [SerializeField]
        private float prunePowerHigh;



        [SerializeField]
        private TrunkData trunkData;

        [SerializeField]
        private BranchData level1;
        [SerializeField]
        private BranchData level2;
        [SerializeField]
        private BranchData level3;

        private float[] ERROR_VALUES = new float[5];

        public TrunkData GetTrunkData() => trunkData;

        public BranchData GetBranchData(int level)
        {
            switch (level)
            {
                case 1:
                    return level1;
                case 2:
                    return level2;
                case 3:
                    return level3;
                default:
                    return level3;
            }
        }

        public float GetError(int level)
        {
            level = Mathf.Clamp(level, 0, 4);
            return ERROR_VALUES[level];
        }

        public void ModifyError(int level, float value)
        {
            level = Mathf.Clamp(level, 0, 4);
            ERROR_VALUES[level] -= value;
        }

        public void RestartErrors()
        {
            for (int i = 0; i < ERROR_VALUES.Length; i++)
                ERROR_VALUES[i] = 0;
        }

        public float GetLenghtChildMax(int level)
        {
            var currentLevel = GetBranchData(level);
            return currentLevel.NLength + currentLevel.NLengthV;
        }

        public float GetScaleTree() => Mathf.Max(0.01f, scale + scaleV);

        public bool CanHaveChilds(int levelBranch)
        {
            return levelBranch + 1 <= levels;
        }

        private Shape prevShape;
        private float prevBaseSize;
        private float prevScale;
        private float prevScaleV;
        private float prevZScale;
        private float prevZScaleV;
        private int prevLevels;
        private float prevRatio;
        private float prevRatioPower;
        private int prevLobes;
        private float prevLobeDepth;
        private float prevFlare;
        private int prevLeaves;
        private float prevLeafScale;
        private float prevLeafScaleX;
        private float prevAttractionUp;
        private float prevPruneRatio;
        private float prevPruneWidth;
        private float prevPruneWidthPeak;
        private float prevPrunePowerLow;
        private float prevPrunePowerHigh;


        private TrunkData prevTrunkData;
        private BranchData prevLevel1;
        private BranchData prevLevel2;
        private BranchData prevLevel3;


        // 301 215 39 59 nequi

        public int UpdateAtLevel()
        {
            int indexToReturn = 0;
            if (level3.Equals(prevLevel3) == false)
                indexToReturn = 3;
            if (level2.Equals(prevLevel2) == false)
                indexToReturn = 2;
            if (level1.Equals(prevLevel1) == false)
                indexToReturn = 1;
            if (trunkData.Equals(prevTrunkData) == false)
                indexToReturn = 0;
            if (EqualsTreeScale() == false || EqualTrunkMeshStructure() == false)
                indexToReturn = 0;

            if (AttractionUp.Equals(prevAttractionUp) == false)
                indexToReturn = Mathf.Min(indexToReturn, 2);
            if (Levels != prevLevels)
                indexToReturn = Mathf.Min(indexToReturn, Levels);

            // prevLevel3 = level3;
            // prevLevel2 = level2;
            // prevLevel1 = level1;
            // prevTrunkData = trunkData;
            return indexToReturn;
        }

        public bool HasUpdated()
        {
            if (
            shape == prevShape &&
            baseSize.Equals(prevBaseSize) &&
            scale.Equals(prevScale) &&
            scaleV.Equals(prevScaleV) &&
            zScale.Equals(prevZScale) &&
            zScaleV.Equals(prevZScaleV) &&
            // levels == prevLevels &&
            ratio.Equals(prevRatio) &&
            ratioPower.Equals(prevRatioPower) &&
            lobes == prevLobes &&
            lobeDepth.Equals(prevLobeDepth) &&
            flare.Equals(prevFlare) &&
            leaves == prevLeaves &&
            leafScale.Equals(prevLeafScale) &&
            leafScaleX.Equals(prevLeafScaleX) &&
            // attractionUp.Equals(prevAttractionUp) &&
            pruneRatio.Equals(prevPruneRatio) &&
            pruneWidth.Equals(prevPruneWidth) &&
            pruneWidthPeak.Equals(prevPruneWidthPeak) &&
            prunePowerLow.Equals(prevPrunePowerLow) &&
            prunePowerHigh.Equals(prevPrunePowerHigh)
            )
                return false;

            // prevShape = shape;
            // prevBaseSize = baseSize;
            // prevScale = scale;
            // prevScaleV = scaleV;
            // prevZScale = zScale;
            // prevZScaleV = zScaleV;
            // // prevLevels = levels;
            // prevRatio = ratio;
            // prevRatioPower = ratioPower;
            // prevLobes = lobes;
            // prevLobeDepth = lobeDepth;
            // prevFlare = flare;
            // prevLeaves = leaves;
            // prevLeafScale = leafScale;
            // prevLeafScaleX = leafScaleX;
            // prevAttractionUp = attractionUp;
            // prevPruneRatio = pruneRatio;
            // prevPruneWidth = pruneWidth;
            // prevPruneWidthPeak = pruneWidthPeak;
            // prevPrunePowerLow = prunePowerLow;
            // prevPrunePowerHigh = prunePowerHigh;
            return true;
        }

        public void UpdateCaching()
        {
            prevShape = Shape;
            prevBaseSize = BaseSize;
            prevScale = Scale;
            prevScaleV = ScaleV;
            prevZScale = ZScale;
            prevZScaleV = ZScaleV;
            prevLevels = Levels;
            prevRatio = Ratio;
            prevRatioPower = RatioPower;
            prevLobes = Lobes;
            prevLobeDepth = LobeDepth;
            prevFlare = Flare;
            prevLeaves = Leaves;
            prevLeafScale = LeafScale;
            prevLeafScaleX = LeafScaleX;
            prevAttractionUp = AttractionUp;
            prevPruneRatio = PruneRatio;
            prevPruneWidth = PruneWidth;
            prevPruneWidthPeak = PruneWidthPeak;
            prevPrunePowerLow = PrunePowerLow;
            prevPrunePowerHigh = PrunePowerHigh;

            prevLevel3 = level3;
            prevLevel2 = level2;
            prevLevel1 = level1;
            prevTrunkData = trunkData;
        }

        public bool EqualsTreeScale()
        {
            if (
            Scale.Equals(prevScale) &&
            ScaleV.Equals(prevScaleV) &&
            ZScale.Equals(prevZScale) &&
            ZScaleV.Equals(prevZScaleV)
            )
                return true;
            return false;
        }

        public bool EqualChildDistribution()
        {
            if (
            // Shape == prevShape &&
            BaseSize.Equals(prevBaseSize)
            )
                return true;
            return false;
        }

        public bool EqualTrunkMeshStructure()
        {
            if (
                Ratio.Equals(prevRatio) &&
                Lobes == prevLobes &&
                LobeDepth.Equals(prevLobeDepth) &&
                Flare.Equals(prevFlare)
            )
                return true;
            return false;

        }

        public bool EqualBranchMeshStructure()
        {
            if (
                Shape == prevShape &&
                RatioPower.Equals(prevRatioPower) &&
                Lobes == prevLobes &&
                LobeDepth.Equals(prevLobeDepth) &&
                EqualChildDistribution() == true
            )
                return true;
            return false;
        }

        public bool EqualLevels()
        {
            return Levels == prevLevels;
        }


        public Shape Shape { get => shape; }
        public float BaseSize { get => Mathf.Clamp01(baseSize); }
        public float Scale { get => Mathf.Max(0.01f, scale); }
        public float ScaleV { get => scaleV; }
        public float ZScale { get => Mathf.Max(0.01f, zScale); }
        public float ZScaleV { get => zScaleV; }
        public int Levels { get => Mathf.Clamp(levels, 0, 10); }



        public float Ratio { get => Mathf.Max(0.01f, ratio); }
        public float RatioPower { get => Mathf.Max(0, ratioPower); }
        public int Lobes { get => Mathf.Max(0, lobes); }
        public float LobeDepth { get => Mathf.Max(0, lobeDepth); }
        public float Flare { get => Mathf.Max(0, flare); }


        public int Leaves { get => Mathf.Max(0, leaves); }
        public float LeafScale { get => Mathf.Max(0.01f, leafScale); }
        public float LeafScaleX { get => Mathf.Max(0.01f, leafScaleX); }


        public float AttractionUp { get => attractionUp; }


        public float PruneRatio { get => pruneRatio; }
        public float PruneWidth { get => pruneWidth; }
        public float PruneWidthPeak { get => pruneWidthPeak; }
        public float PrunePowerLow { get => prunePowerLow; }
        public float PrunePowerHigh { get => prunePowerHigh; }

    }
}
