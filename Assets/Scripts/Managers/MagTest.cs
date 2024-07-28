using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GameManager))]
public class MagTest : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();  //BeginHorizontal() 이후 부터는 GUI 들이 가로로 생성됩니다.
        GUILayout.FlexibleSpace(); // 고정된 여백을 넣습니다. ( 버튼이 가운데 오기 위함)
                                   //버튼을 만듭니다 . GUILayout.Button("버튼이름" , 가로크기, 세로크기)

        if (GUILayout.Button("자석 생성", GUILayout.Width(120), GUILayout.Height(30)))
        {
            GameManager.instance.IM.MagStart();
        }
        GUILayout.FlexibleSpace();  // 고정된 여백을 넣습니다.
        EditorGUILayout.EndHorizontal();  // 가로 생성 끝

    }
}

