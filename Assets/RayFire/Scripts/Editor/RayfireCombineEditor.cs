using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine.Rendering;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireCombine))]
    public class RayfireCombineEditor : Editor
    {
        RayfireCombine     combine;
        SerializedProperty sourceListProp;
        ReorderableList    sourceList; 
        
        /// /////////////////////////////////////////////////////////
        /// Static
        /// /////////////////////////////////////////////////////////
        
        static int space = 3;
        static bool expand;
        
        static GUIContent gui_sourceType = new GUIContent ("Type",             "");
        static GUIContent gui_meshFilt   = new GUIContent ("Mesh Filters",     "");
        static GUIContent gui_meshSkin   = new GUIContent ("Skinned Meshes",   "");
        static GUIContent gui_meshPart   = new GUIContent ("Particle Systems", "");
        static GUIContent gui_threshSize = new GUIContent ("Size",             "Do not combine meshes with size less than defined value");
        static GUIContent gui_threshVert = new GUIContent ("Vertices",         "Do not combine meshes with amount of vertices less than defined value");
        static GUIContent gui_ind        = new GUIContent ("Index Format",     "Mesh with more than 65k vertices should use 32 bit Index Format");
        
        /// /////////////////////////////////////////////////////////
        /// Enable
        /// /////////////////////////////////////////////////////////
        
        private void OnEnable()
        {
            sourceListProp                 = serializedObject.FindProperty("objects");
            sourceList                     = new ReorderableList(serializedObject, sourceListProp, true, true, true, true);
            sourceList.drawElementCallback = DrawInitListItems;
            sourceList.drawHeaderCallback  = DrawInitHeader;
            sourceList.onAddCallback       = AddInit;
            sourceList.onRemoveCallback    = RemoveInit;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Inspector
        /// /////////////////////////////////////////////////////////
        
        public override void OnInspectorGUI()
        {
            combine = target as RayfireCombine;
            if (combine == null)
                return;

            GUILayout.Space (8);
            
            if (GUILayout.Button ("Combine", GUILayout.Height (25)))
                combine.Combine();
            
            GUILayout.Space (space);
            
            UI_Source();
            
            GUILayout.Space (space);

            UI_Mesh();

            GUILayout.Space (space);

            UI_Filters();

            GUILayout.Space (space);
            
            UI_Comb();

            GUILayout.Space (space);
            
            UI_Export();
            
            GUILayout.Space (8);
        }
        
        /// /////////////////////////////////////////////////////////
        /// Source
        /// /////////////////////////////////////////////////////////
        
        void UI_Source()
        {
            GUILayout.Space (space); 
            GUILayout.Label ("  Source", EditorStyles.boldLabel);
            GUILayout.Space (space); 
            
            EditorGUI.BeginChangeCheck();
            RayfireCombine.CombType type = (RayfireCombine.CombType)EditorGUILayout.EnumPopup (gui_sourceType, combine.type);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_sourceType.text);
                foreach (RayfireCombine scr in targets)
                {
                    scr.type = type;
                    SetDirty (scr);
                }
            }
            
            if (combine.type == RayfireCombine.CombType.ObjectsList)
            {
                GUILayout.Space (space);

                serializedObject.Update();
                sourceList.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
            }
        }

        void UI_Mesh()
        {
            GUILayout.Space (space); 
            GUILayout.Label ("  Mesh Source", EditorStyles.boldLabel);
            GUILayout.Space (space); 
            
            EditorGUI.BeginChangeCheck();
            bool meshFilters = EditorGUILayout.Toggle (gui_meshFilt, combine.meshFilters);
            GUILayout.Space (space);
            bool skinnedMeshes = EditorGUILayout.Toggle (gui_meshSkin, combine.skinnedMeshes);
            GUILayout.Space (space);
            bool particleSystems = EditorGUILayout.Toggle (gui_meshPart, combine.particleSystems);
            GUILayout.Space (space);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, "Mesh Source");
                foreach (RayfireCombine scr in targets)
                {
                    scr.meshFilters     = meshFilters;
                    scr.skinnedMeshes   = skinnedMeshes;
                    scr.particleSystems = particleSystems;
                    SetDirty (scr);
                }
            }
        }

        void UI_Filters()
        {
            GUILayout.Space (space); 
            GUILayout.Label ("  Filters", EditorStyles.boldLabel);
            GUILayout.Space (space); 
            
            EditorGUI.BeginChangeCheck();
            float sizeThreshold = EditorGUILayout.Slider (gui_threshSize, combine.sizeThreshold, 0, 10f);
            GUILayout.Space (space);
            int vertexThreshold = EditorGUILayout.IntSlider (gui_threshVert, combine.vertexThreshold, 0, 100);
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, "Filters");
                foreach (RayfireCombine scr in targets)
                {
                    scr.sizeThreshold   = sizeThreshold;
                    scr.vertexThreshold = vertexThreshold;
                    SetDirty (scr);
                }
            }
        }

        void UI_Comb()
        {
            GUILayout.Space (space); 
            GUILayout.Label ("  Combined Mesh", EditorStyles.boldLabel);
            GUILayout.Space (space); 
            
            EditorGUI.BeginChangeCheck();
            IndexFormat indexFormat = (IndexFormat)EditorGUILayout.EnumPopup(gui_ind, combine.indexFormat);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_ind.text);
                foreach (RayfireCombine scr in targets)
                {
                    scr.indexFormat = indexFormat;
                    SetDirty (scr);
                }
            }
        }

        void UI_Export()
        {
            GUILayout.Space (space); 
            GUILayout.Label ("  Export", EditorStyles.boldLabel);
            GUILayout.Space (space); 
            
            if (GUILayout.Button ("Export Mesh", GUILayout.Height (25)))
            {
                MeshFilter mf = combine.GetComponent<MeshFilter>();
                RFMeshAsset.SaveMesh (mf, combine.name);
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// ReorderableList draw
        /// /////////////////////////////////////////////////////////
        
        void DrawInitListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = sourceList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y+2, EditorGUIUtility.currentViewWidth - 80f, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }
        
        void DrawInitHeader(Rect rect)
        {
            rect.x += 10;
            EditorGUI.LabelField(rect, "Objects List");
        }

        void AddInit(ReorderableList list)
        {
            if (combine.objects == null)
                combine.objects = new List<GameObject>();
            combine.objects.Add (null);
            list.index = list.count;
        }
        
        void RemoveInit(ReorderableList list)
        {
            if (combine.objects != null)
            {
                combine.objects.RemoveAt (list.index);
                list.index = list.index - 1;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////

        void SetDirty (RayfireCombine scr)
        {
            if (Application.isPlaying == false)
            {
                EditorUtility.SetDirty (scr);
                EditorSceneManager.MarkSceneDirty (scr.gameObject.scene);
                SceneView.RepaintAll();
            }
        }
    }
}