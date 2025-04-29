using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireBlade))]
    public class RayfireBladeEditor : Editor
    {

        RayfireBlade blade;
        List<string> layerNames;
        
        SerializedProperty targetListProp;
        ReorderableList    targetList; 
        
        /// /////////////////////////////////////////////////////////
        /// Static
        /// /////////////////////////////////////////////////////////
        
        // Static
        static int   space    = 3;
        
        static GUIContent gui_filtCool = new GUIContent ("Cooldown", "Allows to temporary disable Blade component for defined time to prevent constant slicing.");
        static GUIContent gui_propAction = new GUIContent ("Action", "Slice: Object will be Sliced accordingly to Slice Type plane.\n"+
                                                                          "Demolish: Object will demolished accordingly to it's Mesh or Cluster Demolition properties.");
        static GUIContent gui_propTrigger = new GUIContent ("On Trigger", " Enter: Object will be sliced when Blade's trigger collider enter object's collider.\n" +
                                                                          " Exit: Object will be sliced when Blade's trigger collider exit object's collider.\n" +
                                                                          " Enter Exit: Object will be sliced when Blade's trigger collider exit object's collider and angle for slicing plane will be average angle between enter and exit. " +
                                                                          " This type should be used if object with Blade will be rotated while it is inside sliced object so slicing plane at least will have average angle.");
        
        static GUIContent gui_propSlice  = new GUIContent ("Slice Plane", "Defines slicing plane which will be used to slice target object.");
        static GUIContent gui_propDamage = new GUIContent ("Damage",      "Applies damage to sliced object.");
        static GUIContent gui_propSkin   = new GUIContent ("Skin",      "In order to detect collider collision one of the objects has to have RigidBody component and " +
                                                                        "Skinned Mesh object may not have RigidBody. " +
                                                                        "when this property enabled Blade object will get its own kinematic RigidBody component" +
                                                                        "to detect collision with skinned mesh objects.");
        
        
        static GUIContent gui_forceVal = new GUIContent ("Force", "Add to sliced fragments additional velocity impulse to separate them.");
        static GUIContent gui_forceIna = new GUIContent ("Affect Inactive", "Force will be applied to Inactive objects as well.");

        static GUIContent gui_targets = new GUIContent ("Target List", "Slicing also can be initiated by Slice Target button or by public SliceTarget() method. " +
                                                                       "In this case object with Blade doesn't have to enter or exit sliced object collider, but you need to define Target Gameobject for slice.");
      

        
        /// /////////////////////////////////////////////////////////
        /// Enable
        /// /////////////////////////////////////////////////////////
        
        private void OnEnable()
        {
            targetListProp                 = serializedObject.FindProperty("targets");
            targetList                     = new ReorderableList(serializedObject, targetListProp, true, true, true, true);
            targetList.drawElementCallback = DrawInitListItems;
            targetList.drawHeaderCallback  = DrawInitHeader;
            targetList.onAddCallback       = AddInit;
            targetList.onRemoveCallback    = RemoveInit;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Inspector
        /// /////////////////////////////////////////////////////////

        public override void OnInspectorGUI()
        {
            blade = target as RayfireBlade;
            if (blade == null)
                return;
            
            GUILayout.Space (8);
            
            if (Application.isPlaying == true)
            {
                // Cooldown
                if (blade.coolDownState == true)
                    GUILayout.Label ("  Cooldown...");
            }
            
            GUILayout.Space (space);

            UI_Props();
            
            GUILayout.Space (space);

            UI_Force();
            
            GUILayout.Space (space);
            
            UI_Filters();
            
            GUILayout.Space (space);

            UI_Targets();
            
            GUILayout.Space (8);
        }
        
        /// /////////////////////////////////////////////////////////
        /// Properties
        /// /////////////////////////////////////////////////////////

        void UI_Props()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Properties", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            RayfireBlade.ActionType actionType = (RayfireBlade.ActionType)EditorGUILayout.EnumPopup (gui_propAction, blade.actionType);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_propAction.text);
                foreach (RayfireBlade scr in targets)
                {
                    scr.actionType = actionType;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            RayfireBlade.CutType onTrigger = (RayfireBlade.CutType)EditorGUILayout.EnumPopup (gui_propTrigger, blade.onTrigger);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_propTrigger.text);
                foreach (RayfireBlade scr in targets)
                {
                    scr.onTrigger  = onTrigger;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            PlaneType sliceType = (PlaneType)EditorGUILayout.EnumPopup (gui_propSlice, blade.sliceType);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_propSlice.text);
                foreach (RayfireBlade scr in targets)
                {
                    scr.sliceType  = sliceType;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float damage = EditorGUILayout.Slider (gui_propDamage, blade.damage, 0.01f, 50f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_propDamage.text);
                foreach (RayfireBlade scr in targets)
                {
                    scr.damage = damage;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool skin = EditorGUILayout.Toggle (gui_propSkin, blade.skin);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_propSkin.text);
                foreach (RayfireBlade scr in targets)
                {
                    scr.skin = skin;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Force
        /// /////////////////////////////////////////////////////////

        void UI_Force()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Force", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float force = EditorGUILayout.Slider (gui_forceVal, blade.force, 0f, 10f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_forceVal.text);
                foreach (RayfireBlade scr in targets)
                {
                    scr.force = force;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool affectInactive = EditorGUILayout.Toggle (gui_forceIna, blade.affectInactive);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_forceIna.text);
                foreach (RayfireBlade scr in targets)
                {
                    scr.affectInactive = affectInactive;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Filters
        /// /////////////////////////////////////////////////////////
        
        void UI_Filters()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Filters", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float cooldown = EditorGUILayout.Slider (gui_filtCool, blade.cooldown, 0f, 10f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_filtCool.text);
                foreach (RayfireBlade scr in targets)
                {
                    scr.cooldown = cooldown;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            string tagFilter = EditorGUILayout.TagField ("Tag", blade.tagFilter);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, "Tag");
                foreach (RayfireBlade scr in targets)
                {
                    scr.tagFilter = tagFilter;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);

            if (layerNames == null)
            {
                layerNames = new List<string>();
                for (int i = 0; i <= 31; i++)
                    layerNames.Add (i + ". " + LayerMask.LayerToName (i));
            }
            
            EditorGUI.BeginChangeCheck();
            int mask = EditorGUILayout.MaskField ("Layer", blade.mask, layerNames.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, "Layer");
                foreach (RayfireBlade scr in targets)
                {
                    scr.mask = mask;
                    SetDirty (scr);
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Targets
        /// /////////////////////////////////////////////////////////

        void UI_Targets()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Targets", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            serializedObject.Update();
            targetList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying == true && blade.HasTargets == true)
            {
                if (GUILayout.Button (" Slice Target ", GUILayout.Height (25)))
                {
                    foreach (var bl in targets)
                        if (bl as RayfireBlade != null)
                            (bl as RayfireBlade).SliceTarget();
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// ReorderableList draw
        /// /////////////////////////////////////////////////////////
        
        void DrawInitListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = targetList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y+2, EditorGUIUtility.currentViewWidth - 80f, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }
        
        void DrawInitHeader(Rect rect)
        {
            rect.x += 10;
            EditorGUI.LabelField(rect, gui_targets);
        }

        void AddInit(ReorderableList list)
        {
            if (blade.targets == null)
                blade.targets = new List<GameObject>();
            blade.targets.Add (null);
            list.index = list.count;
        }
        
        void RemoveInit(ReorderableList list)
        {
            if (blade.HasTargets == true)
            {
                blade.targets.RemoveAt (list.index);
                list.index = list.index - 1;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////
        
        void SetDirty (RayfireBlade scr)
        {
            if (Application.isPlaying == false)
            {
                EditorUtility.SetDirty (scr);
                EditorSceneManager.MarkSceneDirty (scr.gameObject.scene);
            }
        }
    }
}