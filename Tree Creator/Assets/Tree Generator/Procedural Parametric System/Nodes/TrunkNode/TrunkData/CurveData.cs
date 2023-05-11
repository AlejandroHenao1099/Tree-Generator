using UnityEngine;

namespace TreeCreator
{
    [System.Serializable]
    public struct CurveData
    {
        [Header("Length Settings")]
        public float nScale;
        public float nScaleV;
        public float nLength;
        public float nLengthV;



        [Header("Curve Settings")]
        public int nCurveRes;
        public int nCurve;
        public int nCurveBack;
        public int nCurveV;
        public ShapeCurve shapeCurve;



        [Header("Spiral Settings")]
        public float nRadiusBase;
        public float nRadiusTop;
        public int nRotateSpiral;
        public int nRotateSpiralV;



        public bool EqualCurve(CurveData obj)
        {
            if (
                this.nLength == obj.nLength &&
                this.nLengthV == obj.nLengthV &&
                this.nScale == obj.nScale &&
                this.nScaleV == obj.nScaleV &&
                this.nCurveRes == obj.nCurveRes &&
                this.nCurve == obj.nCurve &&
                this.nCurveBack == obj.nCurveBack &&
                this.nCurveV == obj.nCurveV &&
                this.shapeCurve == obj.shapeCurve
            )
            {
                if (this.shapeCurve == ShapeCurve.Spiral)
                {
                    if (
                        this.nRadiusBase.Equals(obj.nRadiusBase) &&
                        this.nRadiusTop.Equals(obj.nRadiusTop) &&
                        this.nRotateSpiral.Equals(obj.nRotateSpiral) &&
                        this.nRotateSpiralV.Equals(obj.nRotateSpiralV)
                    )
                        return true;
                    else
                        return false;
                }
                return true;
            }
            return false;
        }
    }
}