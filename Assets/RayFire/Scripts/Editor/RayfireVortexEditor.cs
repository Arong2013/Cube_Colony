using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;


namespace RayFire
{
    
  
    
    
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireVortex))]
    public class RayfireVortexEditor : Editor
    {
        
        
        RayfireVortex vortex;
        List<string>  layerNames;

        /// /////////////////////////////////////////////////////////
        /// Static
        /// /////////////////////////////////////////////////////////
        
        // Static
        static int space = 3;
        
        static GUIContent gui_ancShow     = new GUIContent ("Show Handle",   "");
        static GUIContent gui_ancTop      = new GUIContent ("Top Point",     "");
        static GUIContent gui_ancBot      = new GUIContent ("Bottom Point",  "");
        static GUIContent gui_gizmoShow   = new GUIContent ("Show",          "");
        static GUIContent gui_gizmoTop    = new GUIContent ("Top Radius",    "");
        static GUIContent gui_gizmoBot    = new GUIContent ("Bottom Radius", "");
        static GUIContent gui_eye         = new GUIContent ("Eye",           "");
        static GUIContent gui_strStiff    = new GUIContent ("Stiffness",     "");
        static GUIContent gui_strSwirl    = new GUIContent ("Swirl",         "");
        static GUIContent gui_strFrc      = new GUIContent ("Force By Mass", "");
        static GUIContent gui_torEnable   = new GUIContent ("Enable",        "");
        static GUIContent gui_torStr      = new GUIContent ("Strength",      "");
        static GUIContent gui_torVar      = new GUIContent ("Variation",     "");
        static GUIContent gui_heiEnable   = new GUIContent ("Enable",        "");
        static GUIContent gui_heiSpeed    = new GUIContent ("Speed",         "");
        static GUIContent gui_heiSpread   = new GUIContent ("Spread",        "");
        static GUIContent gui_seed        = new GUIContent ("Seed",          "");
        static GUIContent gui_prevCircles = new GUIContent ("Circles ",      "");
        
        /// /////////////////////////////////////////////////////////
        /// Inspector
        /// /////////////////////////////////////////////////////////   
        
        public override void OnInspectorGUI()
        {
            vortex = target as RayfireVortex;
            if (vortex == null)
                return;

            GUILayout.Space (8);
            
            UI_Anchor();

            GUILayout.Space (space);

            UI_Gizmo();
            
            GUILayout.Space (space);

            UI_Eye();
            
            GUILayout.Space (space);

            UI_Strength();
            
            GUILayout.Space (space);

            UI_Torque();

            GUILayout.Space (space);

            UI_Height();

            GUILayout.Space (space);

            UI_Seed();
            
            GUILayout.Space (space);

            UI_Preview();
            
            GUILayout.Space (space);

            UI_Filters();
            
            GUILayout.Space (8);
        }

        /// /////////////////////////////////////////////////////////
        /// Anchor
        /// /////////////////////////////////////////////////////////  

        void UI_Anchor()
        {
            GUILayout.Label ("  Anchor", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            bool topHandle = EditorGUILayout.Toggle (gui_ancShow, vortex.topHandle);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_ancShow.text);
                foreach (RayfireVortex scr in targets)
                {
                    scr.topHandle = topHandle;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            Vector3 topAnchor = EditorGUILayout.Vector3Field (gui_ancTop, vortex.topAnchor);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_ancTop.text);
                foreach (RayfireVortex scr in targets)
                {
                    scr.topAnchor = topAnchor;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            Vector3 bottomAnchor = EditorGUILayout.Vector3Field (gui_ancBot, vortex.bottomAnchor);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_ancBot.text);
                foreach (RayfireVortex scr in targets)
                {
                    scr.bottomAnchor = bottomAnchor;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Gizmo
        /// /////////////////////////////////////////////////////////  

        void UI_Gizmo()
        {
            GUILayout.Label ("  Gizmo", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            bool showGizmo = EditorGUILayout.Toggle (gui_gizmoShow, vortex.showGizmo);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_gizmoShow.text);
                foreach (RayfireVortex scr in targets)
                {
                    scr.showGizmo = showGizmo;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float topRadius = EditorGUILayout.Slider (gui_gizmoTop, vortex.topRadius, 0.1f, 50f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_gizmoTop.text);
                foreach (RayfireVortex scr in targets)
                {
                    scr.topRadius = topRadius;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float bottomRadius = EditorGUILayout.Slider (gui_gizmoBot, vortex.bottomRadius, 0.0f, 50f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_gizmoBot.text);
                foreach (RayfireVortex scr in targets)
                {
                    scr.bottomRadius = bottomRadius;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Eye
        /// /////////////////////////////////////////////////////////  

        void UI_Eye()
        {
            GUILayout.Label ("  Eye", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            float eye = EditorGUILayout.Slider (gui_eye, vortex.eye, 0.05f, 0.9f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_eye.text);
                foreach (RayfireVortex scr in targets)
                {
                    scr.eye = eye;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Strength
        /// /////////////////////////////////////////////////////////  

        void UI_Strength()
        {
            GUILayout.Label ("  Strength", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            float stiffness = EditorGUILayout.Slider (gui_strStiff, vortex.stiffness, 1f, 10f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_strStiff.text);
                foreach (RayfireVortex scr in targets)
                {
                    scr.stiffness = stiffness;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float swirlStrength = EditorGUILayout.Slider (gui_strSwirl, vortex.swirlStrength, -40f, 40f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_strSwirl.text);
                foreach (RayfireVortex scr in targets)
                {
                    scr.swirlStrength = swirlStrength;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool forceByMass = EditorGUILayout.Toggle (gui_strFrc, vortex.forceByMass);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_strFrc.text);
                foreach (RayfireVortex scr in targets)
                {
                    scr.forceByMass = forceByMass;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Torque
        /// /////////////////////////////////////////////////////////  

        void UI_Torque()
        {
            GUILayout.Label ("  Torque", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            bool enableTorque = EditorGUILayout.Toggle (gui_torEnable, vortex.enableTorque);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_torEnable.text);
                foreach (RayfireVortex scr in targets)
                {
                    scr.enableTorque = enableTorque;
                    SetDirty (scr);
                }
            }

            if (vortex.enableTorque == true)
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float torqueStrength = EditorGUILayout.Slider (gui_torStr, vortex.torqueStrength, -1f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_torStr.text);
                    foreach (RayfireVortex scr in targets)
                    {
                        scr.torqueStrength = torqueStrength;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float torqueVariation = EditorGUILayout.Slider (gui_torVar, vortex.torqueVariation, 0f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_torVar.text);
                    foreach (RayfireVortex scr in targets)
                    {
                        scr.torqueVariation = torqueVariation;
                        SetDirty (scr);
                    }
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Height
        /// /////////////////////////////////////////////////////////  

        void UI_Height()
        {
            GUILayout.Label ("  Height Bias", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            bool enableHeightBias = EditorGUILayout.Toggle (gui_heiEnable, vortex.enableHeightBias);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_heiEnable.text);
                foreach (RayfireVortex scr in targets)
                {
                    scr.enableHeightBias = enableHeightBias;
                    SetDirty (scr);
                }
            }

            if (vortex.enableHeightBias == true)
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float biasSpeed = EditorGUILayout.Slider (gui_heiSpeed, vortex.biasSpeed, 0f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_heiSpeed.text);
                    foreach (RayfireVortex scr in targets)
                    {
                        scr.biasSpeed = biasSpeed;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float biasSpread = EditorGUILayout.Slider (gui_heiSpread, vortex.biasSpread, 0f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_heiSpread.text);
                    foreach (RayfireVortex scr in targets)
                    {
                        scr.biasSpread = biasSpread;
                        SetDirty (scr);
                    }
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Seed
        /// /////////////////////////////////////////////////////////  

        void UI_Seed()
        {
            GUILayout.Label ("  Seed", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            int seed = EditorGUILayout.IntSlider (gui_seed, vortex.seed, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_seed.text);
                foreach (RayfireVortex scr in targets)
                {
                    scr.seed = seed;
                    SetDirty (scr);
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Preview
        /// /////////////////////////////////////////////////////////  

        void UI_Preview()
        {
            GUILayout.Label ("  Preview", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            int circles = EditorGUILayout.IntSlider (gui_prevCircles, vortex.circles, 2, 10);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_prevCircles.text);
                foreach (RayfireVortex scr in targets)
                {
                    scr.circles = circles;
                    SetDirty (scr);
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Filters
        /// /////////////////////////////////////////////////////////  
        
        void UI_Filters()
        {
            GUILayout.Label ("  Filters", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            string tagFilter = EditorGUILayout.TagField ("Tag", vortex.tagFilter);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, "Tag");
                foreach (RayfireVortex scr in targets)
                {
                    scr.tagFilter = tagFilter;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);

            // Layer mask
            if (layerNames == null)
            {
                layerNames = new List<string>();
                for (int i = 0; i <= 31; i++)
                    layerNames.Add (i + ". " + LayerMask.LayerToName (i));
            }
            
            EditorGUI.BeginChangeCheck();
            int mask = EditorGUILayout.MaskField ("Layer", vortex.mask, layerNames.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, "Layer");
                foreach (RayfireVortex scr in targets)
                {
                    scr.mask = mask;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Draw
        /// /////////////////////////////////////////////////////////
        
        [DrawGizmo (GizmoType.Selected | GizmoType.NonSelected)]
        void OnSceneGUI()
        {
            if (vortex.showGizmo == true)
            {
                Transform transForm = vortex.transform;

                // Start check for changes and record undo
                EditorGUI.BeginChangeCheck();

                // Top Bottom circles
                Handles.DrawWireDisc (transForm.TransformPoint (vortex.topAnchor),    transForm.up, vortex.topRadius);
                Handles.DrawWireDisc (transForm.TransformPoint (vortex.bottomAnchor), transForm.up, vortex.bottomRadius);

                // Top Bottom radius handles
                vortex.topRadius    = Handles.RadiusHandle (transForm.rotation, transForm.TransformPoint (vortex.topAnchor),    vortex.topRadius,    true);
                vortex.bottomRadius = Handles.RadiusHandle (transForm.rotation, transForm.TransformPoint (vortex.bottomAnchor), vortex.bottomRadius, true);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObject (vortex, "Change Gizmo");
                }

                // Top point handle
                if (vortex.topHandle == true)
                {
                    vortex.topAnchor = transForm.InverseTransformPoint (Handles.PositionHandle (transForm.TransformPoint (vortex.topAnchor), transForm.rotation));
                    if (vortex.topAnchor.x > 20)
                        vortex.topAnchor.x = 20;
                    else if (vortex.topAnchor.z > 20)
                        vortex.topAnchor.z = 20;
                    if (vortex.topAnchor.x < -20)
                        vortex.topAnchor.x = -20;
                    else if (vortex.topAnchor.z < -20)
                        vortex.topAnchor.z = -20;
                }
            }
        }
        
        [DrawGizmo (GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        static void DrawGizmosSelected (RayfireVortex vortex, GizmoType gizmoType)
        {
            if (vortex.showGizmo)
            {
                // Vars
                Vector3 previousPoint = Vector3.zero;
                Vector3 nextPoint     = Vector3.zero;
                Color   wireColor     = new Color (0.58f, 0.77f, 1f);

                // Gizmo properties
                Gizmos.color  = wireColor;
                Gizmos.matrix = vortex.transform.localToWorldMatrix;

                // Gizmo center line
                Gizmos.DrawLine (vortex.topAnchor, vortex.bottomAnchor);

                // Draw main circles
                DrawCircle (vortex.topAnchor,    vortex.topRadius,    previousPoint, nextPoint);
                DrawCircle (vortex.bottomAnchor, vortex.bottomRadius, previousPoint, nextPoint);

                // Draw main eyes circles
                DrawCircle (vortex.topAnchor,    vortex.topRadius * vortex.eye,    previousPoint, nextPoint);
                DrawCircle (vortex.bottomAnchor, vortex.bottomRadius * vortex.eye, previousPoint, nextPoint);

                // Draw additional circles
                //if (vortex.circles > 2)
                //{
                //    float step = 1f / (vortex.circles - 1);
                //    for (int i = 1; i < vortex.circles - 1; i++)
                //    {
                //        Vector3 midPoint = Vector3.Lerp(vortex.bottomAnchor, vortex.topAnchor, step *i);
                //        float rad = Mathf.Lerp(vortex.bottomRadius, vortex.topRadius, step * i);
                //        DrawCircle(midPoint, rad);
                //        DrawCircle(midPoint, (vortex.topRadius + vortex.bottomRadius) / 2f * vortex.eye);
                //    }
                //}

                // Selectable sphere
                float sphereSize = (vortex.topRadius + vortex.bottomRadius) * 0.03f;
                if (sphereSize < 0.1f)
                    sphereSize = 0.1f;
                Gizmos.color = new Color (1.0f, 0.60f, 0f);
                Gizmos.DrawSphere (new Vector3 (vortex.bottomRadius,  0f, 0f),                   sphereSize);
                Gizmos.DrawSphere (new Vector3 (-vortex.bottomRadius, 0f, 0f),                   sphereSize);
                Gizmos.DrawSphere (new Vector3 (0f,                   0f, vortex.bottomRadius),  sphereSize);
                Gizmos.DrawSphere (new Vector3 (0f,                   0f, -vortex.bottomRadius), sphereSize);

                Gizmos.DrawSphere (new Vector3 (vortex.topRadius,  0f, 0f) + vortex.topAnchor,                sphereSize);
                Gizmos.DrawSphere (new Vector3 (-vortex.topRadius, 0f, 0f) + vortex.topAnchor,                sphereSize);
                Gizmos.DrawSphere (new Vector3 (0f,                0f, vortex.topRadius) + vortex.topAnchor,  sphereSize);
                Gizmos.DrawSphere (new Vector3 (0f,                0f, -vortex.topRadius) + vortex.topAnchor, sphereSize);

                //// Draw circle gizmo
                //void DrawHelix()
                //{
                //    float detalization = 200f;
                //    // Starting position from bottom to top on vortex axis
                //    Vector3 bottomStartPos = vortex.bottomAnchor;
                //    Vector3 vectorToTop = vortex.topAnchor - vortex.bottomAnchor;
                //    Vector3 vectorToTopStep = vectorToTop / detalization;
                //    float swirlNow = 0f;
                //    float swirlRate = 0.1f;
                //    float heightRateNow = 0f;
                //    previousPoint = bottomStartPos;
                //    nextPoint = Vector3.zero;
                //    float heightRateStep = 1f / detalization;
                //    while (heightRateNow < 1f)
                //    {
                //        // Next swirl rate
                //        swirlNow += swirlRate;

                //        // Increase current rate for lerp
                //        heightRateNow += heightRateStep;

                //        // Get average radius by height
                //        float radius = Mathf.Lerp(vortex.bottomRadius, vortex.topRadius, heightRateNow);

                //        // Get next point on vortex axis
                //        bottomStartPos += vectorToTopStep;

                //        // Get local helix point
                //        Vector3 point = Vector3.zero;
                //        point.x = Mathf.Cos(swirlNow) * radius;
                //        point.z = Mathf.Sin(swirlNow) * radius;

                //        // Get final vortex point
                //        point += bottomStartPos;

                //        // Gizmos.DrawWireSphere(point, 0.1f);
                //        Gizmos.DrawLine(point, previousPoint);
                //        // Gizmos.DrawWireSphere(point, 0.1f);
                //        previousPoint = point;
                //    }
                //}
            }
        }
        
        static void DrawCircle (Vector3 point, float radius, Vector3 previousPoint, Vector3 nextPoint)
        {
            // Draw top eye
            const int size  = 45;
            float     rate  = 0f;
            float     scale = 1f / size;
            nextPoint.y     = point.y;
            previousPoint.y = point.y;
            previousPoint.x = radius * Mathf.Cos (rate) + point.x;
            previousPoint.z = radius * Mathf.Sin (rate) + point.z;
            for (int i = 0; i < size; i++)
            {
                rate        += 2.0f * Mathf.PI * scale;
                nextPoint.x =  radius * Mathf.Cos (rate) + point.x;
                nextPoint.z =  radius * Mathf.Sin (rate) + point.z;

                Gizmos.DrawLine (previousPoint, nextPoint);
                previousPoint = nextPoint;
            }
        }
                
        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////

        void SetDirty (RayfireVortex scr)
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