using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InspectorNavigationWindow : EditorWindow {

    bool _canPrev = false;

    Stack<HashSet<int>> _history = new Stack<HashSet<int>>();

    [MenuItem("Window/InspectorNavigation")]
    public static void ShowWindow()
    {
        var editor = GetWindow(typeof(InspectorNavigationWindow));
        editor.minSize = new Vector2(editor.minSize.x, 25);
        editor.maxSize = new Vector2(editor.maxSize.x, 25);
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        CreateUIElement(() => {
            if (GUILayout.Button("<-"))
            {
                GoPrev();
            }
        }, _canPrev);
        CreateUIElement(() => {
            if (GUILayout.RepeatButton("<<"))
            {
                GoPrev();
            }
        }, _canPrev);
        GUILayout.EndHorizontal();
    }

    void CreateUIElement(Action element, bool enabled)
    {
        if (!enabled)
        {
            EditorGUI.BeginDisabledGroup(true);
        }
        element();
        if (!enabled)
        {
            EditorGUI.EndDisabledGroup();
        }
    }

    void GoPrev()
    {
        if (_history.Count < 1)
        {
            _canPrev = false;
        }
        else if (_history.Count == 1)
        {
            _history.Pop();
            Selection.instanceIDs = new int[0];
            _canPrev = false;
            Repaint();
        }else if (_history.Count > 1) {
            _history.Pop();
            var set = _history.Peek();
            Selection.instanceIDs = ArrayFromSet(set);
            Repaint();
        }
    }

    private void OnSelectionChange()
    {
        if (Selection.instanceIDs.Length == 0 && _history.Count == 0) return;
        if (_history.Count <= 0)
        {
            _history.Push(SetFromArray(Selection.instanceIDs));
            _canPrev = true;
        }
        else
        {
            var newSelection = SetFromArray(Selection.instanceIDs);
            if (NewSelectionContainsLastSelection())
            {
                _history.Pop();
            }
            _canPrev = true;
            _history.Push(newSelection);
        }
        Repaint();
    }

    private bool NewSelectionContainsLastSelection()
    {
        return IsSubsetOf(_history.Peek(), SetFromArray(Selection.instanceIDs));
    }

    HashSet<int> SetFromArray(int[] array)
    {
        var set = new HashSet<int>();
        foreach (var v in array)
        {
            set.Add(v);
        }
        return set;
    }

    int[] ArrayFromSet(HashSet<int> set)
    {
        List<int> list = new List<int>();
        foreach (var v in set)
        {
            list.Add(v);
        }
        return list.ToArray();
    }

    bool IsSubsetOf(HashSet<int> subset, HashSet<int> set)
    {
        foreach (var v in subset)
        {
            if (!set.Contains(v))
            {
                return false;
            }
        }
        return true;
    }

}
