using UnityEditor;
using UnityEngine;

public class SetLegacyAnimation : MonoBehaviour
{
    [MenuItem("Tools/Set Animation to Legacy")]
    static void SetAnimationToLegacy()
    {
        foreach (Object obj in Selection.objects)
        {
            if (obj is AnimationClip)
            {
                AnimationClip clip = (AnimationClip)obj;
                SerializedObject serializedClip = new SerializedObject(clip);
                serializedClip.FindProperty("m_Legacy").boolValue = true;
                serializedClip.ApplyModifiedProperties();
            }
        }
    }
}