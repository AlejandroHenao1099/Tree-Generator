using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProceduralMeshGeneration
{
    [CustomEditor(typeof(DynamicSpline))]
    public class EditorSpline : Editor
    {
        int selectedPoint = -1;
        DynamicSpline spline;

        private void OnSceneGUI()
        {
            spline = target as DynamicSpline;
            float stepCurve = 1f / (spline.resolutionCurve - 1);
            for (int i = 0; i < spline.resolutionCurve - 1; i++)
            {
                var startPos = spline.GetPoint(i * stepCurve);
                var endPos = spline.GetPoint((i + 1) * stepCurve);
                Handles.DrawLine(startPos, endPos, spline.thicknessLine);
            }
            for (int i = 0; i < spline.CountPoints; i++)
            {
                var pos = spline.GetArrayPoint(i);
                var size = HandleUtility.GetHandleSize(pos) * spline.sizePoint;
                if (Handles.Button(pos, Quaternion.identity, size, size * 1.3f, Handles.DotHandleCap))
                    selectedPoint = i;
                if (selectedPoint == i)
                {
                    var newPos = Handles.PositionHandle(pos, Quaternion.identity);
                    spline.SetArrayPoint(i, newPos);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            spline = target as DynamicSpline;
            spline.sizePoint = GUILayout.HorizontalSlider(spline.sizePoint, 0.1f, 10f);
            spline.resolutionCurve = EditorGUILayout.IntField(0);
            if (GUILayout.Button("Add Point"))
                spline.AddCurvePoint();
        }
    }
}