using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// A ScriptableObject that contains an AudioClip and descriptors specifying how the clip should be played.
/// </summary>
[CreateAssetMenu(fileName = "New Sound File", menuName = "Sound File")]
public class ScriptableSoundFile : ScriptableObject
{
    [SerializeField]
    private AudioClip clip;

    [SerializeField]
    private float startTime;

    [SerializeField]
    private float endTime;

    [SerializeField]
    private float volume = 1;

    [SerializeField]
    private bool isLooping;

    public AudioClip Clip => clip;
    public float StartTime => startTime;
    public float EndTime => endTime;
    public float Volume => volume;
    public bool IsLooping => isLooping;

    [SerializeField]
    private ScriptableSoundFile nextSoundFile;

    public ScriptableSoundFile NextSoundFile => nextSoundFile;
}

#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableSoundFile))]
public class ScriptableSoundFileEditor : Editor
{
    private SerializedProperty clip;
    private SerializedProperty startTime;
    private SerializedProperty endTime;
    private SerializedProperty volume;
    private SerializedProperty isLooping;

    private void OnEnable()
    {
        clip = serializedObject.FindProperty("clip");
        startTime = serializedObject.FindProperty("startTime");
        endTime = serializedObject.FindProperty("endTime");
        volume = serializedObject.FindProperty("volume");
        isLooping = serializedObject.FindProperty("isLooping");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(clip);
        EditorGUILayout.PropertyField(startTime);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(endTime);
        if (GUILayout.Button("Set to Clip End"))
        {
            endTime.floatValue = ((AudioClip)clip.objectReferenceValue).length;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(volume);
        EditorGUILayout.PropertyField(isLooping);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("nextSoundFile"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
