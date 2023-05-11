using UnityEngine;
namespace TreeCreator
{
    [System.Serializable]
    public struct BranchData
    {
        [Header("Rotation Settings")]
        [SerializeField, Range(0, 180)]
        private int nDownAngle;
        [SerializeField]
        private int nDownAngleV;
        [SerializeField]
        private int nRotate;
        [SerializeField]
        private int nRotateV;



        [Header("Branch Child Settings")]
        [SerializeField, Min(0)]
        private int nBranches;
        [SerializeField, Range(1, 10), Min(1)]
        private int nBranchesPerSection;



        [Header("Length Settings")]
        [SerializeField, Min(0.01f)]
        private float nLength;
        [SerializeField]
        private float nLengthV;



        [Header("Radius Settings")]
        [SerializeField, Range(0f, 3f)]
        private float nTaper;
        [SerializeField]
        private float offsetLobes;
        [SerializeField, Min(0.01f)]
        private float scalerLobeDepth;



        [Header("Split Settings")]
        [SerializeField]
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



        [Header("Mesh Settings")]
        [SerializeField, Range(10, 200), Min(10)]
        private int resolutionVertical;
        [SerializeField, Range(3, 30), Min(3)]
        private int resolutionHorizontal;

        public bool EqualChilds(BranchData obj)
        {
            if (obj.NBranches == this.NBranches &&
                obj.NBranchesPerSection == this.NBranchesPerSection)
                return true;
            return false;
        }

        public bool EqualOrientation(BranchData obj)
        {
            if (
                obj.NDownAngle == this.NDownAngle &&
                obj.NDownAngleV == this.NDownAngleV &&
                obj.NRotate == this.NRotate &&
                obj.NRotateV == this.NRotateV)
                return true;
            return false;
        }

        public bool EqualScale(BranchData obj)
        {
            if (
                    obj.NLength == this.NLength &&
                    obj.NLengthV == this.NLengthV &&
                    obj.NCurveRes == this.NCurveRes)
                return true;
            return false;
        }

        public bool EqualBranchScale(BranchData obj)
        {
            if (
                this.NLength == obj.NLength &&
                this.NLengthV == obj.NLengthV
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

        public bool EqualCurveStructure(BranchData obj)
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

        public bool EqualMeshStructure(BranchData obj)
        {
            if (
            this.NTaper.Equals(obj.NTaper) &&
            this.OffsetLobes.Equals(obj.OffsetLobes) &&
            this.ScalerLobeDepth.Equals(obj.ScalerLobeDepth) &&
            this.ResolutionHorizontal == obj.ResolutionHorizontal &&
            this.ResolutionVertical == obj.ResolutionVertical
            )
                return true;
            return false;
        }



        public int NDownAngle { get => Mathf.Clamp(nDownAngle, 0, 180); }
        public int NDownAngleV { get => nDownAngleV; }
        public int NRotate { get => nRotate; }
        public int NRotateV { get => nRotateV; }


        public int NBranches { get => Mathf.Max(0, nBranches); }
        public int NBranchesPerSection { get => Mathf.Clamp(nBranchesPerSection, 1, 10); }


        public float NLength { get => Mathf.Max(0.01f, nLength); }
        public float NLengthV { get => nLengthV; }


        public float NTaper { get => Mathf.Clamp(nTaper, 0f, 3f); }
        public float OffsetLobes { get => offsetLobes; }
        public float ScalerLobeDepth { get => Mathf.Max(0.01f, scalerLobeDepth); }


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


        public int ResolutionVertical { get => Mathf.Max(10, resolutionVertical); }
        public int ResolutionHorizontal { get => Mathf.Max(3, resolutionHorizontal); }

    }
}