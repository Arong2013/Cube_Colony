using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireRestriction))]
    public class RayfireRestrictionEditor : Editor
    {
        // Target
        RayfireRestriction rest;

        /// /////////////////////////////////////////////////////////
        /// Static
        /// /////////////////////////////////////////////////////////

        static int space = 3;
        
        static GUIContent gui_rigid      = new GUIContent ("RayFire Rigid", "");
        static GUIContent gui_propEnable = new GUIContent ("Enable",        "Allows to Reset, Fade or perform Post demolition action when Rigid object breaks restriction.");
        static GUIContent gui_propAction = new GUIContent ("Action",        "");
        static GUIContent gui_propDelay  = new GUIContent ("Delay",         "Action delay in seconds.");
        static GUIContent gui_propInter  = new GUIContent ("Interval",      "How often component will check if object broke restriction.");
        static GUIContent gui_distVal    = new GUIContent ("Distance",      "Object will break restriction if will be moved for this distance.");
        static GUIContent gui_distPos    = new GUIContent ("Position",      "Restriction will break if distance between object and Initial/Target position will be higher than this value.");
        static GUIContent gui_distTarget = new GUIContent ("Target",        "");
        static GUIContent gui_trigReg    = new GUIContent ("Region",        "");
        static GUIContent gui_trigCol    = new GUIContent ("Collider",      "");
        
        /// /////////////////////////////////////////////////////////
        /// Inspector
        /// /////////////////////////////////////////////////////////

        public override void OnInspectorGUI()
        {
            // Get target
            rest = target as RayfireRestriction;
            if (rest == null)
                return;
            
            GUILayout.Space (8);
            
            EditorGUI.BeginChangeCheck();
            rest.rigid = (RayfireRigid)EditorGUILayout.ObjectField (gui_rigid, rest.rigid, typeof(RayfireRigid), true);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_rigid.text);
                foreach (RayfireRestriction scr in targets)
                {
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);

            UI_Prop();
            
            GUILayout.Space (space);

            UI_Dist();
            
            GUILayout.Space (space);

            UI_Trig();
            
            GUILayout.Space (8);
        }

        /// /////////////////////////////////////////////////////////
        /// Properties
        /// /////////////////////////////////////////////////////////

        void UI_Prop()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Properties", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool enable = EditorGUILayout.Toggle (gui_propEnable, rest.enable);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_propEnable.text);
                foreach (RayfireRestriction scr in targets)
                {
                    scr.enable = enable;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            RayfireRestriction.RFBoundActionType breakAction = (RayfireRestriction.RFBoundActionType)EditorGUILayout.EnumPopup (gui_propAction, rest.breakAction);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_propAction.text);
                foreach (RayfireRestriction scr in targets)
                {
                    scr.breakAction = breakAction;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float actionDelay = EditorGUILayout.Slider (gui_propDelay, rest.actionDelay, 0f, 60f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_propDelay.text);
                foreach (RayfireRestriction scr in targets)
                {
                    scr.actionDelay = actionDelay;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float checkInterval = EditorGUILayout.Slider (gui_propInter, rest.checkInterval, 0.1f, 60f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_propInter.text);
                foreach (RayfireRestriction scr in targets)
                {
                    scr.checkInterval = checkInterval;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Distance
        /// /////////////////////////////////////////////////////////

        void UI_Dist()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Distance", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            RayfireRestriction.RFDistanceType position = (RayfireRestriction.RFDistanceType)EditorGUILayout.EnumPopup (gui_distPos, rest.position);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_distPos.text);
                foreach (RayfireRestriction scr in targets)
                {
                    scr.position = position;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float distance = EditorGUILayout.Slider (gui_distVal, rest.distance, 0f, 99f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_distVal.text);
                foreach (RayfireRestriction scr in targets)
                {
                    scr.distance = distance;
                    SetDirty (scr);
                }
            }
            
            if (rest.position == RayfireRestriction.RFDistanceType.TargetPosition)
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                Transform tar = (Transform)EditorGUILayout.ObjectField (gui_distTarget, rest.target, typeof(Transform), true);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_distTarget.text);
                    foreach (RayfireRestriction scr in targets)
                    {
                        scr.target = tar;
                        SetDirty (scr);
                    }
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Trigger
        /// /////////////////////////////////////////////////////////

        void UI_Trig()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Trigger", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            RayfireRestriction.RFBoundTriggerType region = (RayfireRestriction.RFBoundTriggerType)EditorGUILayout.EnumPopup (gui_trigReg, rest.region);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_trigReg.text);
                foreach (RayfireRestriction scr in targets)
                {
                    scr.region = region;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            Collider Coll = (Collider)EditorGUILayout.ObjectField (gui_trigCol, rest.Collider, typeof(Collider), true);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_trigCol.text);
                foreach (RayfireRestriction scr in targets)
                {
                    scr.Collider = Coll;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////

        void SetDirty (RayfireRestriction scr)
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