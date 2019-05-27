
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using FullSerializer;
using static UnityEditor.EditorApplication;
using UnityEngine.SceneManagement;
using LevelEditor.EditorScripts;
using LevelEditor;

//Para facilitarme algunas funciones, como crear botones, o abrir popUps.
public static class GUIAuxiliar
{

    private static readonly fsSerializer _serializer = new fsSerializer();
    public const string PATH_LEVEL_EDITOR = "Assets/LevelEditor/";
    public const string PATH_LEVEL_EDITOR_ICON = PATH_LEVEL_EDITOR + "UI/Icons/";
    //Todas las funciones de boton que creo que voy a usar, con etiquetas,imagenes o GUIcontents
    #if UNITY_EDITOR
    #region Button
    public static T Button<T>(Delegate onClick, string label, params GUILayoutOption[] options)
    {
        if (GUILayout.Button(label, options))
        {
            return (T)onClick.DynamicInvoke();
        }
        return default(T);
    }

    public static T Button<T>(Delegate onClick, GUIContent content, params GUILayoutOption[] options)
    {
        if (GUILayout.Button(content, options))
        {
            return (T)onClick.DynamicInvoke();
        }
        return default(T);
    }


    public static T Button<T>(Delegate onClick, Texture content, params GUILayoutOption[] options)
    {
        if (GUILayout.Button(content, options))
        {
            return (T)onClick.DynamicInvoke();
        }
        return default(T);
    }

    public static T Button<T>(string label, Delegate onClick, params object[] options)
    {
        if (GUILayout.Button(label))
        {
            return (T)onClick.DynamicInvoke(options);
        }
        return default(T);
    }

    public static T Button<T>(GUIContent content, Delegate onClick, params object[] options)
    {
        if (GUILayout.Button(content))
        {
            return (T)onClick.DynamicInvoke(options);
        }
        return default(T);
    }


    public static T Button<T>(Texture content, Delegate onClick, params object[] options)
    {
        if (GUILayout.Button(content))
        {
            onClick.DynamicInvoke(options);
        }
        return default(T);
    }


    public static void Button(Delegate onClick, string label, params GUILayoutOption[] options)
    {
        if (GUILayout.Button(label, options))
        {
            onClick.DynamicInvoke();
        }

    }

    public static void Button(Delegate onClick, GUIContent content, params GUILayoutOption[] options)
    {
        if (GUILayout.Button(content, options))
        {
            onClick.DynamicInvoke();
        }

    }


    public static void Button(Delegate onClick, Texture content, params GUILayoutOption[] options)
    {
        if (GUILayout.Button(content, options))
        {
            onClick.DynamicInvoke();
        }

    }

    public static void Button(string label, Delegate onClick, params object[] options)
    {
        if (GUILayout.Button(label))
        {
            onClick.DynamicInvoke(options);
        }

    }

    public static void Button(GUIContent content, Delegate onClick, params object[] options)
    {
        if (GUILayout.Button(content))
        {
            onClick.DynamicInvoke(options);
        }

    }


    public static void Button(Texture content, Delegate onClick, params object[] options)
    {
        if (GUILayout.Button(content))
        {
            onClick.DynamicInvoke(options);
        }

    }

    #endregion
    #endif
    #region Objects
    //Cuando usas Destroy en el editor, necesitas usar Destroy Inmediate, pero si estas en play, necesitas usar Destroy
    public static void Destroy(UnityEngine.Object o)
    {
        if (Application.isPlaying)
        {
            GameObject.Destroy(o);
        }

        else
        {
            #if UNITY_EDITOR
            Undo.DestroyObjectImmediate(o);
            #endif
        }
    }

    //Auxiliar function to create a object.
    public static GameObject Instanciate(GameObject prefab, Transform transform, Vector3 position, Quaternion rotation, Vector3 scale, bool instancing = false)
    {
        #if !UNITY_EDITOR
        instancing = false;
        #endif

        GameObject realObject;
        if (instancing)
        {
            #if UNITY_EDITOR
            realObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            realObject.transform.parent = transform;
            realObject.transform.SetPositionAndRotation(position, rotation);
            #endif

        }
        else
        {
            realObject = GameObject.Instantiate(prefab, position, rotation, transform);

        }

        Vector3 realScale = Vector3.Scale(scale, new Vector3(1 / transform.lossyScale.x, 1 / transform.lossyScale.y, 1 / transform.lossyScale.z));
        realObject.transform.localScale = Vector3.Scale(realObject.transform.localScale, realScale);

        #if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(realObject, "CellCreated");
        #endif
        return realObject;
    }

    public static Vector3 CalculatePivot(this Transform transform)
    {
        Vector3 newPivot = Vector3.zero;
        Renderer render = transform.GetComponentInChildren<Renderer>();
        if (render != null)
        {
            Bounds b = render.bounds;
            float y = render.bounds.center.y;
            newPivot = transform.InverseTransformPoint(b.center);
            newPivot.y -= b.extents.y;
        }
        return newPivot;
    }
    #endregion
    
    
    #region Utility
    
    public static string Serialize<T>(T value)
    {
        fsData data;
        _serializer.TrySerialize(value, out data).AssertSuccessWithoutWarnings();

        // emit the data via JSON
        return fsJsonPrinter.CompressedJson(data);
    }
    public static T Deserialize<T>(string json)
    {
        fsData data = fsJsonParser.Parse(json);

        // step 2: deserialize the data
        T deserialized = default(T);
        _serializer.TryDeserialize(data, ref deserialized).AssertSuccessWithoutWarnings();

        return deserialized;
    }

    /// <summary>
    /// Creates a new Scene in the editor view that is empty.
    /// </summary>
    /// <param name="name">Scene name</param>
    /// <param name="newSceneReference">The new scene created</param>
    /// <returns>The path to the preview scene, so you can store that path and relead that scene from the disk</returns>
    /// 
    #if UNITY_EDITOR
    public static string OpenNewScene(string name, out Scene newSceneReference)
    {
        
        if (EditorSceneManager.GetActiveScene().name.Equals(name))
        {
            EditorUtility.DisplayDialog("Warning", "Alredy in this scene", "Continue");
            newSceneReference = new Scene();
            return "";
        }

        string previewScene = EditorSceneManager.GetActiveScene().path;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Scene actual = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        newSceneReference = actual;
        SceneView.lastActiveSceneView.Frame(new Bounds(Vector3.zero, Vector3.one));
        actual.name = name;
        return previewScene;
    }
    
    public static string CreateTemporalScene(string path, Scene scene ,string name){
       
        string pathToScene = path+name+".unity";
        EditorSceneManager.SaveScene(scene,pathToScene);
        return pathToScene;
        
        
    }

    #endif
    #endregion
    
  

}

