
using System;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{


    public abstract class Container
    {
        public enum PrefabAction
        {
            Select,
            Delete,
            Reload,
            Edit
        }
        public GameObject prefab;

        public Texture2D preview;


        //Pivot del objeto. El objeto se va a colocar aqui. lo que tengo que saber es ver donde está el pivot y Render.Bound.Center.
        //En la funcion AutoPivotGUI se puede ver como se puede calcular el pivot del objeto. El pivot, se coloca no en el centro geometrico si no en en el plano XZ.
        public Vector3 pivot;
        public bool autoPivot;
        public bool autoScale = false;
        protected Vector3 realScale;
        public Vector3 scale = Vector3.one;

        public Vector2Int cellSize = Vector2Int.one;

        private bool autosize = true;
#if UNITY_EDITOR
        public abstract void ShowGUI(LevelEditor.EditorScripts.PrefabCollectionWindow window);
        public abstract void ShowGUIEdit(EditorWindow window);
        public virtual void Reload(EditorWindow window)
        {
            preview = AssetPreview.GetAssetPreview(prefab);
            while (AssetPreview.IsLoadingAssetPreview(prefab.GetInstanceID()))
            {
                preview = AssetPreview.GetAssetPreview(prefab);
            }
        }

        //Calcula el pivote de un objeto. Devuelve ese punto en coordenadas del objeto.No pone el pivot nuevo. Sirve para cuando se rota un objeto.
        public virtual Vector3 CalculatePivot()
        {
            return prefab.transform.CalculatePivot();
        }

        protected void AutoPivotGUI(Renderer render)
        {
            autoPivot = EditorGUILayout.Toggle(Style.LABLE_AUTOPIVOT, autoPivot);
            if (autoPivot)
            {

                if (render != null)
                {
                    Bounds b = render.bounds;
                    float y = render.bounds.center.y;
                    pivot = render.transform.InverseTransformPoint(b.center);
                    pivot.y -= b.extents.y;

                }
            }
            else
            {
                pivot = EditorGUILayout.Vector3Field("Pivot", pivot);
            }
        }

        protected void AutoScaleGUI(Renderer render, Vector2 DesirableSize)
        {
            bool change = EditorGUILayout.Toggle(Style.LABLE_AUTO_SCALE, autoScale);
            autoScale = change;
            if (autoScale)
            {
                if (render != null)
                {
                    Bounds b = render.bounds;
                    Vector3 ActualSize = b.size;
                    float xMult = DesirableSize.x / ActualSize.x;
                    float zMult = DesirableSize.y / ActualSize.z;
                    float finalSize = xMult >= zMult ? zMult : xMult;
                    scale = new Vector3(finalSize, finalSize, finalSize);
                }
            }
            else
            {
                scale = EditorGUILayout.Vector3Field("Scale Multiplier", scale);
            }
        }

        public void CalculateScale(Vector2 DesirableSize)
        {
            Renderer render = prefab.GetComponentInChildren<Renderer>();
            Bounds b = render.bounds;
            Vector3 ActualSize = b.size;
            float xMult = DesirableSize.x / ActualSize.x;
            float zMult = DesirableSize.y / ActualSize.z;
            float finalSize = xMult >= zMult ? zMult : xMult;
            scale = new Vector3(finalSize, finalSize, finalSize);
        }
#endif



    }
}
