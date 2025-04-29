using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEditor.IMGUI.Controls;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireBomb))]
    public class RayfireBombEditor : Editor
    {
        RayfireBomb        bomb;
        List<string>       layerNames;
        SerializedProperty obstColProp;
        ReorderableList    obstColList;
        BoxBoundsHandle    boxHandle;
        SphereBoundsHandle sphereHandle;
        Vector3            boxSize;
        
        /// /////////////////////////////////////////////////////////
        /// Static
        /// /////////////////////////////////////////////////////////
        
        // Static
        static readonly Color wireColor = new Color (0.58f, 0.77f, 1f);
        static readonly Color sphColor = new Color (1.0f, 0.50f, 0f);
        static readonly int   space    = 3;
        
        static bool exp_rng = true;
        static bool exp_imp = true;
        static bool exp_act = true;
        static bool exp_det;
        static bool exp_obs;
        static bool exp_dmg;
        static bool exp_aud;
        static bool exp_fil = true;
        
        // Gui
        static readonly GUIContent gui_rng = new GUIContent ("Range",      "");
        static readonly GUIContent gui_imp = new GUIContent ("Impulse", "");
        static readonly GUIContent gui_act = new GUIContent ("Activation", "");
        static readonly GUIContent gui_det = new GUIContent ("Detonation", "");
        static readonly GUIContent gui_obs = new GUIContent ("Obstacles",  "");
        static readonly GUIContent gui_dmg = new GUIContent ("Damage",     "");
        static readonly GUIContent gui_aud = new GUIContent ("Audio",      "");
        static readonly GUIContent gui_fil = new GUIContent ("Filters",    "");
        
        static readonly GUIContent gui_rangeShow     = new GUIContent ("Show",          "");
        static readonly GUIContent gui_rangeType     = new GUIContent ("Type",          "Explosion direction.");
        static readonly GUIContent gui_rangeFade     = new GUIContent ("Fade",          "Explosion strength decay over distance.");
        static readonly GUIContent gui_rangeRange    = new GUIContent ("Range",         "Only objects in Range distance will be affected by explosion.");
        static readonly GUIContent gui_rangeBox      = new GUIContent ("Size",          "Directional explosion area size."); 
        static readonly GUIContent gui_rangeDeletion = new GUIContent ("Deletion",      "Destroy objects close to Bomb. Measures in percentage relative to Range value.");
        static readonly GUIContent gui_impulseStr    = new GUIContent ("Strength",      "Maximum explosion impulse which will be applied to objects.");
        static readonly GUIContent gui_impulseCrv    = new GUIContent ("Curve",         "");
        static readonly GUIContent gui_impulseVar    = new GUIContent ("Variation",     "Random variation to final explosion strength for every object in percents relative to Strength value.");
        static readonly GUIContent gui_impulseChaos  = new GUIContent ("Chaos",         "Random rotation velocity to exploded objects.");
        static readonly GUIContent gui_impulseForce  = new GUIContent ("Force By Mass", "Add different final explosion impulse to objects with different mass.");
        static readonly GUIContent gui_impulseIna    = new GUIContent ("Inactive",      "Activate Inactive objects and explode them as well.");
        static readonly GUIContent gui_impulseKin    = new GUIContent ("Kinematic",     "Activate Kinematic objects and explode them as well.");
        static readonly GUIContent gui_detonHeight   = new GUIContent ("Height Offset", "Allows to offset downward Explosion position over global Y axis.");
        static readonly GUIContent gui_detonDelay    = new GUIContent ("Delay",         "Explosion delay in seconds.");
        static readonly GUIContent gui_detonStart    = new GUIContent ("At Start",      "Automatically explode Bomb at Gameobject activation.");
        static readonly GUIContent gui_detonDestroy  = new GUIContent ("Destroy",       "Destroy Gameobject after explosion.");
        static readonly GUIContent gui_obstEn        = new GUIContent ("Enable",        "Enable other colliders in scene as obstacles for explosion.");
        static readonly GUIContent gui_obstSta       = new GUIContent ("Static",        "Use all colliders without RigidBody component as obstacle.");
        static readonly GUIContent gui_obstKin       = new GUIContent ("Kinematik",     "Use all colliders with Kinematik RigidBody as obstacles for explosion.");
        static readonly GUIContent gui_damageApply   = new GUIContent ("Apply",         "Apply damage to objects with Rigid component in case they have enabled Damage.");
        static readonly GUIContent gui_damageValue   = new GUIContent ("Value",         "Damage value  which will take object at explosion.");
        static readonly GUIContent gui_audioPlay     = new GUIContent ("Play",          "Play audio clip at explosion.");
        static readonly GUIContent gui_audioVolume   = new GUIContent ("Volume",        "");
        static readonly GUIContent gui_audioClip     = new GUIContent ("Clip",          "Audio Clip to play at explosion.");

        static readonly string colListName = "Obstacle Colliders List";
        
        /// /////////////////////////////////////////////////////////
        /// Enable
        /// /////////////////////////////////////////////////////////
        
        private void OnEnable()
        {
            // Obstacle list
            obstColProp                     = serializedObject.FindProperty ("obstacleCollidersList");
            obstColList                     = new ReorderableList (serializedObject, obstColProp, true, true, true, true)
            {
                drawElementCallback = DrawObstColListItems,
                drawHeaderCallback  = DrawObstColHeader,
                onAddCallback       = AddObstCol,
                onRemoveCallback    = RemoveObstCol
            };

            // Box handle
            boxHandle                = new BoxBoundsHandle
            {
                wireframeColor = wireColor,
                handleColor    = sphColor
            };

            // Sphere handle
            sphereHandle                = new SphereBoundsHandle
            {
                wireframeColor = wireColor,
                handleColor    = sphColor
            };

            // Foldouts
            if (EditorPrefs.HasKey ("rf_br") == true) exp_rng = EditorPrefs.GetBool ("rf_br");
            if (EditorPrefs.HasKey ("rf_bi") == true) exp_imp = EditorPrefs.GetBool ("rf_bi");
            if (EditorPrefs.HasKey ("rf_ba") == true) exp_act = EditorPrefs.GetBool ("rf_ba");
            if (EditorPrefs.HasKey ("rf_bd") == true) exp_det = EditorPrefs.GetBool ("rf_bd");
            if (EditorPrefs.HasKey ("rf_bo") == true) exp_obs = EditorPrefs.GetBool ("rf_bo");
            if (EditorPrefs.HasKey ("rf_bm") == true) exp_dmg = EditorPrefs.GetBool ("rf_bm");
            if (EditorPrefs.HasKey ("rf_bu") == true) exp_aud = EditorPrefs.GetBool ("rf_bu");
            if (EditorPrefs.HasKey ("rf_bf") == true) exp_fil = EditorPrefs.GetBool ("rf_bf");
        }
        
        /// /////////////////////////////////////////////////////////
        /// Methods
        /// /////////////////////////////////////////////////////////

        void OnSceneGUI()
        {
            bomb = target as RayfireBomb;
            if (bomb == null)
                return;

            if (bomb.enabled == true)
            {
                // Set matrix
                Matrix4x4 mat = bomb.transform.localToWorldMatrix;
                mat.SetTRS (bomb.transform.position, bomb.transform.rotation, Vector3.one);
                Handles.matrix = mat;

                // Draw handles
                if (bomb.rangeType == RayfireBomb.RangeType.Spherical)
                    SphericalHandles(bomb);
                else if (bomb.rangeType == RayfireBomb.RangeType.Directional)
                    BoxHandles(bomb);
            }
        }
        
        void SphericalHandles (RayfireBomb bmb)
        {
            Vector3 ho = Vector3.zero;
            ho.y += bmb.heightOffset;

            /*
           EditorGUI.BeginChangeCheck();
           bomb.range = Handles.RadiusHandle (Quaternion.identity, ho, bomb.range);
           if (EditorGUI.EndChangeCheck() == true)
           {
               Undo.RecordObject (bomb, "Change Range");
               SetDirty (bomb);
           }
           */
            
           sphereHandle.radius = bmb.range;
           sphereHandle.center = ho;
           EditorGUI.BeginChangeCheck();
           sphereHandle.DrawHandle();
           if (EditorGUI.EndChangeCheck())
           {
               Undo.RecordObject (bmb, "Change Bounds");
               bmb.range          = sphereHandle.radius;
               sphereHandle.center = ho;
               SetDirty (bmb);
           }
        }
        
        void BoxHandles (RayfireBomb bmb)
        {
            // TODO Height offset support

            // Set box handle size
            boxSize.x             = bmb.boxSize.x;
            boxSize.y             = bmb.boxSize.y;
            boxSize.z             = bmb.range;
            boxHandle.size   = boxSize;
            boxHandle.center = Vector3.zero + new Vector3 (0, 0, bmb.range / 2f);
            
            EditorGUI.BeginChangeCheck();
            boxHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject (bmb, "Change Bounds");
                bmb.boxSize     = boxHandle.size;
                bmb.range       = boxHandle.size.z;
                boxHandle.center = Vector3.zero + new Vector3 (0, 0, bmb.range / 2f);
                SetDirty (bmb);
            }
            
            // TODO draw arrow
        }

        /// /////////////////////////////////////////////////////////
        /// Inspector
        /// /////////////////////////////////////////////////////////
        
        public override void OnInspectorGUI()
        {
            bomb = target as RayfireBomb;
            if (bomb == null)
                return;
            
            GUILayout.Space (8);

            UI_Actions();
            
            GUILayout.Space (space);

            UI_Range();

            GUILayout.Space (space);

            UI_Impulse();

            GUILayout.Space (space);

            UI_Activation();            

            GUILayout.Space (space);

            UI_Detonation();

            GUILayout.Space (space);

            UI_Obstacles();

            GUILayout.Space (space);

            UI_Damage();
            
            GUILayout.Space (space);

            UI_Audio();
            
            GUILayout.Space (space);

            UI_Filters();

            GUILayout.Space (8);
        }

        /// /////////////////////////////////////////////////////////
        /// Buttons
        /// /////////////////////////////////////////////////////////

        void UI_Actions()
        {
            if (Application.isPlaying == true)
            {
                GUILayout.Space (space);
                GUILayout.Label ("  Actions", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                GUILayout.BeginHorizontal();
                
                if (GUILayout.Button ("Explode", GUILayout.Height (25)))
                {
                    foreach (RayfireBomb script in targets)
                    {
                        script.Explode (script.delay);
                        SetDirty (script);
                    }
                }
                
                if (GUILayout.Button ("Restore", GUILayout.Height (25)))
                {
                    foreach (RayfireBomb script in targets)
                    {
                        script.Restore ();
                        SetDirty (script);
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Range
        /// /////////////////////////////////////////////////////////
        
        void UI_Range()
        {
            SetFoldoutPref (ref exp_rng, "rf_br", gui_rng, true);
            if (exp_rng == true)
            {
                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                bool showGizmo = EditorGUILayout.Toggle (gui_rangeShow, bomb.showGizmo);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_rangeShow.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.showGizmo = showGizmo;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                RayfireBomb.RangeType rangeType = (RayfireBomb.RangeType)EditorGUILayout.EnumPopup (gui_rangeType, bomb.rangeType);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_rangeType.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.rangeType = rangeType;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float range = EditorGUILayout.FloatField (gui_rangeRange, bomb.range);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_rangeRange.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        if (bomb.range < 0.1f)
                            bomb.range = 0.1f;
                        scr.range = range;
                        SetDirty (scr);
                    }
                }
            
                if (bomb.rangeType == RayfireBomb.RangeType.Directional)
                {
                    GUILayout.Space (space);
                
                    EditorGUI.BeginChangeCheck();
                    Vector2 boxSize = EditorGUILayout.Vector2Field (gui_rangeBox, bomb.boxSize);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObjects (targets, gui_rangeBox.text);
                        if (bomb.boxSize.x < 0.01f)
                            bomb.boxSize.x = 0.01f;
                        if (bomb.boxSize.y < 0.01f)
                            bomb.boxSize.y = 0.01f;
                        foreach (RayfireBomb script in targets)
                        {
                            script.boxSize = boxSize;
                            SetDirty (script);
                        }
                    }
                }
            
                GUILayout.Space (space);
            
                EditorGUI.BeginChangeCheck();
                int deletion = EditorGUILayout.IntSlider (gui_rangeDeletion, bomb.deletion, 0, 100);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_rangeDeletion.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.deletion = deletion;
                        SetDirty (scr);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Impulse
        /// /////////////////////////////////////////////////////////
        
        void UI_Impulse()
        {
            SetFoldoutPref (ref exp_imp, "rf_bi", gui_imp, true);
            if (exp_imp == true)
            {
                EditorGUI.indentLevel++;

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                RayfireBomb.FadeType fadeType = (RayfireBomb.FadeType)EditorGUILayout.EnumPopup (gui_rangeFade, bomb.fadeType);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_rangeFade.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.fadeType = fadeType;
                        SetDirty (scr);
                    }
                }

                if (bomb.fadeType == RayfireBomb.FadeType.ByCurve)
                {
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    AnimationCurve curve = EditorGUILayout.CurveField (gui_impulseCrv, bomb.curve);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_impulseCrv.text);
                        foreach (RayfireBomb scr in targets)
                        {
                            scr.curve = curve;
                            SetDirty (scr);
                        }
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float strength = EditorGUILayout.FloatField (gui_impulseStr, bomb.strength);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_impulseStr.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.strength = strength;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int variation = EditorGUILayout.IntSlider (gui_impulseVar, bomb.variation, 0, 100);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_impulseVar.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.variation = variation;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int chaos = EditorGUILayout.IntSlider (gui_impulseChaos, bomb.chaos, 0, 90);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_impulseChaos.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.chaos = chaos;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                bool forceByMass = EditorGUILayout.Toggle (gui_impulseForce, bomb.forceByMass);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_impulseForce.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.forceByMass = forceByMass;
                        SetDirty (scr);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Activation
        /// /////////////////////////////////////////////////////////

        void UI_Activation()
        {
            SetFoldoutPref (ref exp_act, "rf_ba", gui_act, true);
            if (exp_act == true)
            {
                EditorGUI.indentLevel++;

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                bool affectInactive = EditorGUILayout.Toggle (gui_impulseIna, bomb.affectInactive);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_impulseIna.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.affectInactive = affectInactive;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                bool affectKinematic = EditorGUILayout.Toggle (gui_impulseKin, bomb.affectKinematic);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_impulseKin.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.affectKinematic = affectKinematic;
                        SetDirty (scr);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Detonation
        /// /////////////////////////////////////////////////////////
        
        void UI_Detonation()
        {
            SetFoldoutPref (ref exp_det, "rf_bd", gui_det, true);
            if (exp_det == true)
            {
                EditorGUI.indentLevel++;

                GUILayout.Space (space);

                // Height offset only for spherical bomb
                if (bomb.rangeType == RayfireBomb.RangeType.Spherical)
                {
                    EditorGUI.BeginChangeCheck();
                    float heightOffset = EditorGUILayout.Slider (gui_detonHeight, bomb.heightOffset, -10f, 10f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_detonHeight.text);
                        foreach (RayfireBomb scr in targets)
                        {
                            scr.heightOffset = heightOffset;
                            SetDirty (scr);
                        }
                    }
                    
                    GUILayout.Space (space);
                }

                EditorGUI.BeginChangeCheck();
                float delay = EditorGUILayout.Slider (gui_detonDelay, bomb.delay, 0, 10f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_detonDelay.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.delay = delay;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                bool atStart = EditorGUILayout.Toggle (gui_detonStart, bomb.atStart);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_detonStart.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.atStart = atStart;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                bool destroy = EditorGUILayout.Toggle (gui_detonDestroy, bomb.destroy);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_detonDestroy.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.destroy = destroy;
                        SetDirty (scr);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Obstacles
        /// /////////////////////////////////////////////////////////
        
        void UI_Obstacles()
        {
            SetFoldoutPref (ref exp_obs, "rf_bo", gui_obs, true);
            if (exp_obs == true)
            {
                EditorGUI.indentLevel++;
            
                /*
                GUILayout.Space (space);
                GUILayout.Label ("  Obstacles", EditorStyles.boldLabel);
                GUILayout.Space (space);
                */
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool obst_enable = EditorGUILayout.Toggle (gui_obstEn, bomb.obst_enable);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_obstEn.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.obst_enable = obst_enable;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                bool obst_static = EditorGUILayout.Toggle (gui_obstSta, bomb.obst_static);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_obstSta.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.obst_static = obst_static;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                bool obst_kinematik = EditorGUILayout.Toggle (gui_obstKin, bomb.obst_kinematik);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_obstKin.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.obst_kinematik = obst_kinematik;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                serializedObject.Update();
                obstColList.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
                
                
                EditorGUI.indentLevel--;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Damage
        /// /////////////////////////////////////////////////////////
        
        void UI_Damage()
        {
            SetFoldoutPref (ref exp_dmg, "rf_bm", gui_dmg, true);
            if (exp_dmg == true)
            {
                EditorGUI.indentLevel++;

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                bool applyDamage = EditorGUILayout.Toggle (gui_damageApply, bomb.applyDamage);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_damageApply.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.applyDamage = applyDamage;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float damageValue = EditorGUILayout.Slider (gui_damageValue, bomb.damageValue, 0, 100f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_damageApply.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.damageValue = damageValue;
                        SetDirty (scr);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Audio
        /// /////////////////////////////////////////////////////////
        
        void UI_Audio()
        {
            SetFoldoutPref (ref exp_aud, "rf_bu", gui_aud, true);
            if (exp_aud == true)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool play = EditorGUILayout.Toggle (gui_audioPlay, bomb.play);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_audioPlay.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.play = play;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float volume = EditorGUILayout.Slider (gui_audioVolume, bomb.volume, 0.01f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_audioVolume.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.volume = volume;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                AudioClip clip = (AudioClip)EditorGUILayout.ObjectField (gui_audioClip, bomb.clip, typeof(AudioClip), true);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_audioClip.text);
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.clip = clip;
                        SetDirty (scr);
                    }
                }

                EditorGUI.indentLevel--;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Filters
        /// /////////////////////////////////////////////////////////
        
        void UI_Filters()
        {
            SetFoldoutPref (ref exp_fil, "rf_bf", gui_fil, true);
            if (exp_fil == true)
            {
                EditorGUI.indentLevel++;

                GUILayout.Space (space);
 
                EditorGUI.BeginChangeCheck();
                string tagFilter = EditorGUILayout.TagField ("Tag", bomb.tagFilter);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, "Tag");
                    foreach (RayfireBomb scr in targets)
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
                int mask = EditorGUILayout.MaskField ("Layer", bomb.mask, layerNames.ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, "Layer");
                    foreach (RayfireBomb scr in targets)
                    {
                        scr.mask = mask;
                        SetDirty (scr);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Draw
        /// /////////////////////////////////////////////////////////
        
        [DrawGizmo (GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        static void DrawGizmosSelected (RayfireBomb bomb, GizmoType gizmoType)
        {
            if (bomb.showGizmo == true)
            {
                // Gizmo properties
                Matrix4x4 mat = bomb.transform.localToWorldMatrix;
                mat.SetTRS (bomb.transform.position, bomb.transform.rotation, Vector3.one);
                Gizmos.matrix = mat;
                Gizmos.color  = wireColor; 
                
                if (bomb.rangeType == RayfireBomb.RangeType.Spherical)
                    DrawGizmoSphere (bomb);
                else if (bomb.rangeType == RayfireBomb.RangeType.Directional)
                    DrawGizmoBox (bomb);
            }
        }
        
        static void DrawGizmoSphere (RayfireBomb bomb)
        {
            // Vars
            const int   size          = 45;
            const float scale         = 1f / size;
            Vector3     previousPoint = Vector3.zero;
            Vector3     nextPoint     = Vector3.zero;
            float       h             = bomb.heightOffset;

            // Draw top eye
            float rate            = 0f;
            nextPoint.y     = h;
            previousPoint.y = h;
            previousPoint.x = bomb.range * Mathf.Cos (rate);
            previousPoint.z = bomb.range * Mathf.Sin (rate);
            for (int i = 0; i < size; i++)
            {
                rate        += 2.0f * Mathf.PI * scale;
                nextPoint.x =  bomb.range * Mathf.Cos (rate);
                nextPoint.z =  bomb.range * Mathf.Sin (rate);
                Gizmos.DrawLine (previousPoint, nextPoint);
                previousPoint = nextPoint;
            }

            // Draw top eye
            rate            = 0f;
            nextPoint.x     = 0f;
            previousPoint.x = 0f;
            previousPoint.y = bomb.range * Mathf.Cos (rate) + h;
            previousPoint.z = bomb.range * Mathf.Sin (rate);
            for (int i = 0; i < size; i++)
            {
                rate        += 2.0f * Mathf.PI * scale;
                nextPoint.y =  bomb.range * Mathf.Cos (rate) + h;
                nextPoint.z =  bomb.range * Mathf.Sin (rate);
                Gizmos.DrawLine (previousPoint, nextPoint);
                previousPoint = nextPoint;
            }

            // Draw top eye
            rate            = 0f;
            nextPoint.z     = 0f;
            previousPoint.z = 0f;
            previousPoint.y = bomb.range * Mathf.Cos (rate) + h;
            previousPoint.x = bomb.range * Mathf.Sin (rate);
            for (int i = 0; i < size; i++)
            {
                rate        += 2.0f * Mathf.PI * scale;
                nextPoint.y =  bomb.range * Mathf.Cos (rate) + h;
                nextPoint.x =  bomb.range * Mathf.Sin (rate);
                Gizmos.DrawLine (previousPoint, nextPoint);
                previousPoint = nextPoint;
            }

            // Selectable sphere
            float sphereSize = bomb.range * 0.07f;
            if (sphereSize < 0.1f)
                sphereSize = 0.1f;
            Gizmos.color = sphColor;
            Gizmos.DrawSphere (new Vector3 (0f,          bomb.range + h,  0f),          sphereSize);
            Gizmos.DrawSphere (new Vector3 (0f,          -bomb.range + h, 0f),          sphereSize);
            Gizmos.DrawSphere (new Vector3 (bomb.range,  h,               0f),          sphereSize);
            Gizmos.DrawSphere (new Vector3 (-bomb.range, h,              0f),          sphereSize);
            Gizmos.DrawSphere (new Vector3 (0f,          h,              bomb.range),  sphereSize);
            Gizmos.DrawSphere (new Vector3 (0f,          h,              -bomb.range), sphereSize);

            // Center helper
            Gizmos.color = Color.red;
            Gizmos.DrawSphere (new Vector3 (0f, 0f, 0f), sphereSize / 3f);

            // Height offset helper
            if (bomb.heightOffset != 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere (new Vector3 (0f, bomb.heightOffset, 0f), sphereSize / 3f);
            }
        }

        static void DrawGizmoBox(RayfireBomb bomb)
        {
            // Draw cube
            Vector3 center = Vector3.zero;
            center.z += bomb.range / 2f;
            Vector3 size = new Vector3(bomb.boxSize.x, bomb.boxSize.y, bomb.range);
            Gizmos.DrawWireCube (center, size);
            
            // Draw arrow
            Gizmos.DrawLine (Vector3.zero, center);
            Gizmos.DrawLine (center,       new Vector3 (center.x - center.z*0.1f, 0, center.z*0.8f));
            Gizmos.DrawLine (center,       new Vector3 (center.x + center.z*0.1f, 0, center.z*0.8f));
            
            // Draw center
            float sphereSize = bomb.range * 0.05f;
            if (sphereSize < 0.1f)
                sphereSize = 0.1f;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere (new Vector3 (0f, 0f, 0f), sphereSize / 2f);
            
            // Selectable sphere
            Gizmos.color = sphColor;
            Gizmos.DrawSphere (new Vector3 (0,          0,          bomb.range),   sphereSize);
            Gizmos.DrawSphere (new Vector3 (size.x/2f,  0,          bomb.range/2), sphereSize);
            Gizmos.DrawSphere (new Vector3 (-size.x/2f, 0,          bomb.range/2), sphereSize);
            Gizmos.DrawSphere (new Vector3 (0f,         size.y/2f,  bomb.range/2), sphereSize);
            Gizmos.DrawSphere (new Vector3 (0f,         -size.y/2f, bomb.range/2), sphereSize);
        }

        /// /////////////////////////////////////////////////////////
        /// ReorderableList obstacles
        /// /////////////////////////////////////////////////////////
        
        void DrawObstColListItems (Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = obstColList.serializedProperty.GetArrayElementAtIndex (index);
            EditorGUI.PropertyField (new Rect (rect.x, rect.y + 2, EditorGUIUtility.currentViewWidth - 80f, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }

        void DrawObstColHeader (Rect rect)
        {
            rect.x += 10;
            EditorGUI.LabelField (rect, colListName);
        }

        void AddObstCol (ReorderableList list)
        {
            if (bomb.obstacleCollidersList == null)
                bomb.obstacleCollidersList = new List<Collider>();
            bomb.obstacleCollidersList.Add (null);
            list.index = list.count;
        }

        void RemoveObstCol (ReorderableList list)
        {
            if (bomb.obstacleCollidersList != null)
            {
                bomb.obstacleCollidersList.RemoveAt (list.index);
                list.index = list.index - 1;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////
        
        void SetDirty (RayfireBomb scr)
        {
            if (Application.isPlaying == false)
            {
                EditorUtility.SetDirty (scr);
                EditorSceneManager.MarkSceneDirty (scr.gameObject.scene);
                SceneView.RepaintAll();
            }
        }
        
        void SetFoldoutPref (ref bool val, string pref, GUIContent caption, bool state) 
        {
            EditorGUI.BeginChangeCheck();
            val = EditorGUILayout.Foldout (val, caption, state);
            if (EditorGUI.EndChangeCheck() == true)
                EditorPrefs.SetBool (pref, val);
        }
    }
}