using UnityEngine;

namespace TreeCreator
{
    [System.Serializable]
    public struct TrunkData
    {
        [Header("Length Settings")]
        [SerializeField, Min(0.01f)]
        private float nScale;
        [SerializeField, Min(0f)]
        private float nScaleV;
        [SerializeField, Min(0.01f)]
        private float nLength;
        [SerializeField]
        private float nLengthV;



        [Header("Radius Settings")]
        [SerializeField, Range(0f, 3f), Min(0f)]
        private float nTaper;
        [SerializeField]
        private float offsetLobes;



        [Header("Split Settings")]
        [SerializeField, Min(0)]
        private int nBaseSplits;
        [SerializeField, Min(0f)]
        private float nSegSplits;
        [SerializeField]
        private int nSplitAngle;
        [SerializeField]
        private int nSplitAngleV;



        [Header("Curve Settings")]
        [SerializeField, Min(2)]
        private int nCurveRes;
        [SerializeField]
        private int nCurve;
        [SerializeField]
        private int nCurveBack;
        [SerializeField]
        private int nCurveV;
        [SerializeField]
        private ShapeCurve shapeCurve;


        [Header("Spiral Settings")]
        [SerializeField, Min(0f)]
        private float nRadiusBase;
        [SerializeField, Min(0f)]
        private float nRadiusTop;
        [SerializeField]
        private int nRotateSpiral;
        [SerializeField]
        private int nRotateSpiralV;


        [Header("Branch Child Settings")]
        [SerializeField, Min(0)]
        private int nBranches;
        [SerializeField, Range(1, 10), Min(1)]
        private int nBranchesPerSection;


        [Header("Mesh Settings")]
        [SerializeField, Range(10, 200), Min(10)]
        private int resolutionVertical;
        [SerializeField, Range(3, 30), Min(3)]
        private int resolutionHorizontal;


        public bool EqualsScale(TrunkData obj)
        {
            if (obj.NLength == this.NLength &&
                            obj.NLengthV == this.NLengthV &&
                            obj.NScale == this.NScale &&
                            obj.NScaleV == this.NScaleV &&
                            obj.NCurveRes == this.NCurveRes)
                return true;
            return false;
        }

        public bool EqualTrunkScale(TrunkData obj)
        {
            if (
                this.NLength == obj.NLength &&
                this.NLengthV == obj.NLengthV &&
                this.NScale == obj.NScale &&
                this.NScaleV == obj.NScaleV
                // this.NCurveRes == obj.NCurveRes &&
                // this.NCurve == obj.NCurve &&
                // this.NCurveBack == obj.NCurveBack &&
                // this.NCurveV == obj.NCurveV &&
                // this.ShapeCurve == obj.ShapeCurve
            )
            {
                // if (this.ShapeCurve == ShapeCurve.Spiral)
                // {
                //     if (
                //         this.NRadiusBase.Equals(obj.NRadiusBase) &&
                //         this.NRadiusTop.Equals(obj.NRadiusTop) &&
                //         this.NRotateSpiral.Equals(obj.NRotateSpiral) &&
                //         this.NRotateSpiralV.Equals(obj.NRotateSpiralV)
                //     )
                //         return true;
                //     else
                //         return false;
                // }
                return true;
            }
            return false;
        }

        public bool EqualCurveStructure(TrunkData obj)
        {
            if (
                this.NCurveRes == obj.NCurveRes &&
                this.NCurve == obj.NCurve &&
                this.NCurveBack == obj.NCurveBack &&
                this.NCurveV == obj.NCurveV &&
                this.ShapeCurve == obj.ShapeCurve
            )
            {
                if (this.ShapeCurve == ShapeCurve.Spiral)
                {
                    if (
                        this.NRadiusBase.Equals(obj.NRadiusBase) &&
                        this.NRadiusTop.Equals(obj.NRadiusTop) &&
                        this.NRotateSpiral.Equals(obj.NRotateSpiral) &&
                        this.NRotateSpiralV.Equals(obj.NRotateSpiralV)
                    )
                        return true;
                    else
                        return false;
                }
                return true;
            }
            return false;
        }

        public bool EqualMeshStructure(TrunkData obj)
        {
            if (
            this.NTaper.Equals(obj.NTaper) &&
            this.OffsetLobes.Equals(obj.OffsetLobes)
            )
                return true;
            return false;
        }

        public bool EqualChilds(TrunkData obj)
        {
            if (this.NBaseSplits == obj.NBaseSplits &&
                // this.NBaseSplits <= 1 &&
                this.NBranches == obj.NBranches &&
                this.NBranchesPerSection == obj.NBranchesPerSection
            )
                return true;
            return false;
        }



        public float NScale { get => Mathf.Max(0.01f, nScale); }
        public float NScaleV { get => nScaleV; }
        public float NLength { get => Mathf.Max(0.01f, nLength); }
        public float NLengthV { get => nLengthV; }



        public float NTaper { get => Mathf.Clamp(nTaper, 0f, 3f); }
        public float OffsetLobes { get => offsetLobes; }



        public int NBaseSplits { get => Mathf.Max(0, nBaseSplits); }
        public float NSegSplits { get => Mathf.Max(0f, nSegSplits); }
        public int NSplitAngle { get => Mathf.Max(0, nSplitAngle); }
        public int NSplitAngleV { get => nSplitAngleV; }



        public int NCurveRes { get => Mathf.Max(2, nCurveRes); }
        public int NCurve { get => nCurve; }
        public int NCurveBack { get => nCurveBack; }
        public int NCurveV { get => nCurveV; }
        public ShapeCurve ShapeCurve { get => shapeCurve; }


        public float NRadiusBase { get => Mathf.Max(0, nRadiusBase); }
        public float NRadiusTop { get => Mathf.Max(0, nRadiusTop); }
        public int NRotateSpiral { get => nRotateSpiral; }
        public int NRotateSpiralV { get => nRotateSpiralV; }


        public int NBranches { get => Mathf.Max(0, nBranches); }
        public int NBranchesPerSection { get => Mathf.Clamp(nBranchesPerSection, 1, 10); }


        public int ResolutionVertical { get => Mathf.Max(10, resolutionVertical); }
        public int ResolutionHorizontal { get => Mathf.Max(3, resolutionHorizontal); }

    }
}