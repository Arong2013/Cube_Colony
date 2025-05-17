using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireUnyielding))]
    public class RayfireUnyieldingEditor : Editor
    {
        RayfireUnyielding uny;
        Vector3           centerWorldPos;
        BoxBoundsHandle   m_BoundsHandle = new BoxBoundsHandle();
        
        /// /////////////////////////////////////////////////////////
        /// Static
        /// /////////////////////////////////////////////////////////
        
        static readonly int space = 3;
        static readonly Color wireColor = new Color (0.58f, 0.77f, 1f);

        static readonly GUIContent gui_propUny     = new GUIContent ("Unyielding",      "Set Unyielding property for children Rigids and Shards.");
        static readonly GUIContent gui_propAct     = new GUIContent ("Activatable",     "Set Activatable property for children Rigids and Shards.");
        static readonly GUIContent gui_propSim     = new GUIContent ("Simulation Type", "Custom simulation type.");
        static readonly GUIContent gui_gizmoShow   = new GUIContent ("Show",            "");
        static readonly GUIContent gui_gizmoSize   = new GUIContent ("Size",            "Unyielding gizmo size.");
        static readonly GUIContent gui_gizmoCenter = new GUIContent ("Center",          "Unyielding gizmo center.");
        
        /// /////////////////////////////////////////////////////////
        /// Methods
        /// /////////////////////////////////////////////////////////
        
        [DrawGizmo (GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        static void DrawGizmosSelected (RayfireUnyielding targ, GizmoType gizmoType)
        {
            if (targ.enabled && targ.showGizmo == true)
            {
                Gizmos.color  = wireColor;
                Gizmos.matrix = targ.transform.localToWorldMatrix;
                Gizmos.DrawWireCube (targ.centerPosition, targ.size);
            }
        }

        private void OnSceneGUI()
        {
            // Get shatter
            uny = target as RayfireUnyielding;
            if (uny == null)
                return;

            if (uny.enabled && uny.showGizmo == true)
            {
                Transform transform      = uny.transform;
                centerWorldPos  = transform.TransformPoint (uny.centerPosition);

                // Point3 handle
                if (uny.showCenter == true)
                {
                    EditorGUI.BeginChangeCheck();
                    centerWorldPos = Handles.PositionHandle (centerWorldPos, Quaternion.identity);
                    if (EditorGUI.EndChangeCheck() == true)
                        Undo.RecordObject (uny, "Center Move");
                                    
                    uny.centerPosition = transform.InverseTransformPoint (centerWorldPos);
                }
                
                Handles.matrix = uny.transform.localToWorldMatrix;
                m_BoundsHandle.wireframeColor = wireColor;
                m_BoundsHandle.center         = uny.centerPosition;
                m_BoundsHandle.size           = uny.size;

                // Draw the handle
                EditorGUI.BeginChangeCheck();
                m_BoundsHandle.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject (uny, "Change Bounds");
                    uny.size = m_BoundsHandle.size;
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Inspector
        /// /////////////////////////////////////////////////////////

        public override void OnInspectorGUI()
        {
            uny = target as RayfireUnyielding;
            if (uny == null)
                return;

            GUILayout.Space (8);
            
            if (Application.isPlaying == true)
                if (GUILayout.Button ("   Activate   ", GUILayout.Height (25)))
                    foreach (var targ in targets)
                        if (targ as RayfireUnyielding != null)
                            (targ as RayfireUnyielding).Activate();
            
            GUILayout.Space (space);
            
            UI_Properties();
            
            GUILayout.Space (space);
            
            UI_Gizmo();
            
            GUILayout.Space (8);
        }
        
        /// /////////////////////////////////////////////////////////
        /// Gizmo
        /// /////////////////////////////////////////////////////////
        
        void UI_Gizmo()
        {
            GUILayout.Label ("  Gizmo", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            bool showGizmo = EditorGUILayout.Toggle (gui_gizmoShow, uny.showGizmo);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_gizmoShow.text);
                foreach (RayfireUnyielding scr in targets)
                {
                    scr.showGizmo = showGizmo;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            Vector3 size = EditorGUILayout.Vector3Field (gui_gizmoSize, uny.size);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_gizmoSize.text);
                foreach (RayfireUnyielding scr in targets)
                {
                    scr.size = size;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
                
            EditorGUI.BeginChangeCheck();
            Vector3 centerPosition = EditorGUILayout.Vector3Field (gui_gizmoCenter, uny.centerPosition);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_gizmoCenter.text);
                foreach (RayfireUnyielding scr in targets)
                {
                    scr.centerPosition = centerPosition;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUILayout.BeginHorizontal ();
            EditorGUI.BeginChangeCheck();
            bool showCenter = GUILayout.Toggle (uny.showCenter, "Show Center", "Button", GUILayout.Height (22));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, "Show Center");
                foreach (RayfireUnyielding scr in targets)
                {
                    scr.showCenter = showCenter;
                    SetDirty (scr);
                }
            }
            
            // Reset center
            if (GUILayout.Button ("   Reset   ", GUILayout.Height (22)))
            {
                Undo.RecordObjects (targets, "   Reset   ");
                foreach (RayfireUnyielding scr in targets)
                {
                    scr.centerPosition = Vector3.zero;
                    SetDirty (scr);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        
        /// /////////////////////////////////////////////////////////
        /// Properties
        /// /////////////////////////////////////////////////////////
        
        void UI_Properties()
        {
            GUILayout.Label ("  Properties", EditorStyles.boldLabel);

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool unyielding = EditorGUILayout.Toggle (gui_propUny, uny.unyielding);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_propUny.text);
                foreach (RayfireUnyielding scr in targets)
                {
                    scr.unyielding = unyielding;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool activatable = EditorGUILayout.Toggle (gui_propAct, uny.activatable);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_propAct.text);
                foreach (RayfireUnyielding scr in targets)
                {
                    scr.activatable = activatable;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            RayfireUnyielding.UnySimType simulationType = (RayfireUnyielding.UnySimType)EditorGUILayout.EnumPopup (gui_propSim, uny.simulationType);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_propSim.text);
                foreach (RayfireUnyielding scr in targets)
                {
                    scr.simulationType = simulationType;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////
        
        void SetDirty (RayfireUnyielding scr)
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