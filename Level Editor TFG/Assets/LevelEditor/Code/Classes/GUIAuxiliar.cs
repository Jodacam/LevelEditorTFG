#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;


//Para facilitarme algunas funciones, como crear botones, o abrir popUps.
public static class GUIAuxiliar
{
    //Todas las funciones de boton que creo que voy a usar, con etiquetas,imagenes o GUIcontents
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
            onClick.DynamicInvoke(options);
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


    public static void  Button(Delegate onClick, Texture content, params GUILayoutOption[] options)
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

    public static  void Button(GUIContent content, Delegate onClick, params object[] options)
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
    #region Objects
    //Cuando usas Destroy en el editor, necesitas usar Destroy Inmediate, pero si estas en play, necesitas usar Destroy
    public static void Destroy(UnityEngine.Object o)
    {
        if(Application.isPlaying)
        {
            GameObject.Destroy(o);
        }else
        {
           GameObject.DestroyImmediate(o);
        }
    }
    #endregion
}

#endif