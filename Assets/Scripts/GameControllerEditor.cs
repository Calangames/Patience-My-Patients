#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor
{
    SerializedProperty teamCharacters, puzzles, handLeverUp, handLeverDown, floorButtonUp, floorButtonDown;

    static bool[] show = new bool[0];
    static bool showSprites = false;

    void OnEnable()
    {
        teamCharacters = serializedObject.FindProperty("teamCharacters");
        handLeverUp = serializedObject.FindProperty("handLeverUp");
        handLeverDown = serializedObject.FindProperty("handLeverDown");
        floorButtonUp = serializedObject.FindProperty("floorButtonUp");
        floorButtonDown = serializedObject.FindProperty("floorButtonDown");
        puzzles = serializedObject.FindProperty("puzzles");
        if (show.Length == 0)
        {
            show = new bool[puzzles.arraySize];
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(teamCharacters, true);
        showSprites = EditorGUILayout.Foldout(showSprites, "Levers Sprites");
        if (showSprites)
        {
            EditorGUILayout.PropertyField(handLeverUp);
            EditorGUILayout.PropertyField(handLeverDown);
            EditorGUILayout.PropertyField(floorButtonUp);
            EditorGUILayout.PropertyField(floorButtonDown);
        }
        SerializedProperty lever;
        SerializedProperty obstacles;
        for (int pIndex = 0; pIndex < puzzles.arraySize; pIndex++)
        {
            lever = puzzles.GetArrayElementAtIndex(pIndex).FindPropertyRelative("lever");
            obstacles = puzzles.GetArrayElementAtIndex(pIndex).FindPropertyRelative("obstacles");

            using (var rect = new EditorGUILayout.VerticalScope())
            {
                show[pIndex] = EditorGUILayout.Foldout(show[pIndex], "Puzzle " + (pIndex + 1));

                if (show[pIndex])
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        bool haveEmpty = HaveEmpty(lever, obstacles);
                        Color color = haveEmpty ? Color.red : Color.green;
                        EditorGUI.DrawRect(rect.rect, color);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PropertyField(lever, GUIContent.none);
                            obstacles.arraySize = obstacles.arraySize == 0 ? obstacles.arraySize + 1 : obstacles.arraySize;
                            for (int oIndex = 0; oIndex < obstacles.arraySize; oIndex++)
                            {
                                EditorGUILayout.PropertyField(obstacles.GetArrayElementAtIndex(oIndex), GUIContent.none);
                            }
                        }
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new EditorGUI.DisabledScope(obstacles.arraySize <= 1))
                            {
                                if (GUILayout.Button(new GUIContent("Remove obstacle"), GUILayout.Height(36f)))
                                {
                                    obstacles.arraySize--;
                                }
                            }
                            if (GUILayout.Button(new GUIContent("Add obstacle"), GUILayout.Height(36f)))
                            {
                                obstacles.arraySize++;
                                obstacles.GetArrayElementAtIndex(obstacles.arraySize - 1).objectReferenceValue = null;
                            }
                        }
                    }
                }
            }
            
        }
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUI.DisabledScope(puzzles.arraySize <= 0))
            {
                if (GUILayout.Button(new GUIContent("Remove Puzzle"), GUILayout.Height(72f)))
                {
                    puzzles.arraySize--;
                }
            }
            if (GUILayout.Button(new GUIContent("Add Puzzle"), GUILayout.Height(72f)))
            {
                puzzles.arraySize++;
                if (puzzles.arraySize > show.Length)
                {
                    CopyShowValues();
                }
                lever = puzzles.GetArrayElementAtIndex(puzzles.arraySize - 1).FindPropertyRelative("lever");
                lever.objectReferenceValue = null;
                obstacles = puzzles.GetArrayElementAtIndex(puzzles.arraySize - 1).FindPropertyRelative("obstacles");
                obstacles.arraySize = 1;
                obstacles.GetArrayElementAtIndex(obstacles.arraySize - 1).objectReferenceValue = null;
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    private bool HaveEmpty(SerializedProperty lever, SerializedProperty obstacles)
    {
        bool haveEmpty = false;
        if (lever.objectReferenceValue == null)
        {
            haveEmpty = true;
        }
        else
        {
            for (int nullCheckIndex = 0; nullCheckIndex < obstacles.arraySize; nullCheckIndex++)
            {
                haveEmpty = obstacles.GetArrayElementAtIndex(nullCheckIndex).objectReferenceValue == null;
                if (haveEmpty)
                {
                    break;
                }
            }
        }

        return haveEmpty;
    }

    private void CopyShowValues()
    {
        bool[] tempArray = new bool[puzzles.arraySize];
        tempArray[tempArray.Length - 1] = true;
        System.Array.Copy(show, tempArray, show.Length);
        show = tempArray;
    }
}
#endif