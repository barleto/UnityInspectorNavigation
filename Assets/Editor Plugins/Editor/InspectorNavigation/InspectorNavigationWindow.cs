using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace InspectorNavigationToolbar
{
    [InitializeOnLoad]
    public static class InspectorNavigationToolbarButton
    {
        static private bool _canPrev = false;

        static Stack<HashSet<int>> _history = new Stack<HashSet<int>>();

        static InspectorNavigationToolbarButton()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }
        

        static void OnToolbarGUI()
        {
            CreateInspectorBackButton();
        }

        static void CreateInspectorBackButton()
        {
            var tex = EditorGUIUtility.IconContent(@"d_SpeedScale").image;

            GUI.changed = false;

            CreateUIElement(() => {
                if (GUI.Button(new Rect(0, 0, 100, 20), new GUIContent("Inspector", tex, "Back in inspector selection."))) {
                    GoPrev();
                }
            }, _canPrev);
        }

        static void CreateUIElement(Action element, bool enabled)
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

        static void GoPrev()
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
            }
            else if (_history.Count > 1)
            {
                _history.Pop();
                var set = _history.Peek();
                Selection.instanceIDs = ArrayFromSet(set);
            }
        }

        static private void OnSelectionChanged()
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
        }

        static private bool NewSelectionContainsLastSelection()
        {
            return IsSubsetOf(_history.Peek(), SetFromArray(Selection.instanceIDs));
        }

        static HashSet<int> SetFromArray(int[] array)
        {
            var set = new HashSet<int>();
            foreach (var v in array)
            {
                set.Add(v);
            }
            return set;
        }

        static int[] ArrayFromSet(HashSet<int> set)
        {
            List<int> list = new List<int>();
            foreach (var v in set)
            {
                list.Add(v);
            }
            return list.ToArray();
        }

        static bool IsSubsetOf(HashSet<int> subset, HashSet<int> set)
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
}
