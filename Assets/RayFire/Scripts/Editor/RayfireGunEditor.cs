using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireGun))]
    public class RayfireGunEditor : Editor
    {
        RayfireGun   gun;
        List<string> layerNames;
        
        /// /////////////////////////////////////////////////////////
        /// Static
        /// /////////////////////////////////////////////////////////
        
        static int space = 3;
        
        static GUIContent gui_dir_show    = new GUIContent ("Show",             "Show shooting ray");
        static GUIContent gui_dir_axis    = new GUIContent ("Axis",             "Shooting direction if Target is not defined.");
        static GUIContent gui_dir_dist    = new GUIContent ("Distance",         "Maximum shooting distance.");
        static GUIContent gui_dir_targ    = new GUIContent ("Target",           "");
        static GUIContent gui_bur_rnd     = new GUIContent ("Rounds",           "");
        static GUIContent gui_bur_rate    = new GUIContent ("Rate",             "");
        static GUIContent gui_imp_show    = new GUIContent ("Show",             "Show impact position and radius. Visible when shooting ray intersects with collider.");
        static GUIContent gui_imp_tp      = new GUIContent ("Type",             "");
        static GUIContent gui_imp_str     = new GUIContent ("Strength",         "");
        static GUIContent gui_imp_rad     = new GUIContent ("Radius",           "");
        static GUIContent gui_imp_ofs     = new GUIContent ("Offset",           "Negative value offset Impact point towards Gun position. Positive farther from Gun position");
        static GUIContent gui_imp_cls     = new GUIContent ("Demolish Cluster", "");
        static GUIContent gui_imp_ina     = new GUIContent ("Affect Inactive",  "");
        static GUIContent gui_comp_rg     = new GUIContent ("Rigid",            "");
        static GUIContent gui_comp_rt     = new GUIContent ("RigidRoot",        "");
        static GUIContent gui_comp_rb     = new GUIContent ("RigidBody",        "");
        static GUIContent gui_dmg_val     = new GUIContent ("Damage",           "");
        static GUIContent gui_dmg_shtp    = new GUIContent ("Per Shard",        "Single Shard: Damage will be applied to single shard intersected by shooting ray. \n" +
                                                                                "Shards In Impact Radius: Damage will be applied to all shards in impact radius.");
        static GUIContent gui_vfx_debris  = new GUIContent ("Debris",           "");
        static GUIContent gui_vfx_dust    = new GUIContent ("Dust",             "");
        static GUIContent gui_vfx_flash   = new GUIContent ("Flash",            "");
        static GUIContent gui_fl_int_min  = new GUIContent ("Minimum",          "");
        static GUIContent gui_fl_int_max  = new GUIContent ("Maximum",          "");
        static GUIContent gui_fl_rng_min  = new GUIContent ("Minimum",          "");
        static GUIContent gui_fl_rng_max  = new GUIContent ("Maximum",          "");
        static GUIContent gui_fl_distance = new GUIContent ("Distance",         "");
        static GUIContent gui_fl_color    = new GUIContent ("Color",            "");
        
        /// /////////////////////////////////////////////////////////
        /// Inspector
        /// /////////////////////////////////////////////////////////
        
        public override void OnInspectorGUI()
        {
            gun = target as RayfireGun;
            if (gun == null)
                return;

            GUILayout.Space (8);

            UI_Buttons();
            
            GUILayout.Space (space);

            UI_Direction();
            
            GUILayout.Space (space);

            UI_Burst();
            
            GUILayout.Space (space);

            UI_Impact();
            
            GUILayout.Space (space);

            UI_Components();
            
            GUILayout.Space (space);

            UI_Damage();

            GUILayout.Space (space);

            UI_VFX();
            
            GUILayout.Space (space);

            UI_Filters();
            
            GUILayout.Space (8);
        }
        
        /// /////////////////////////////////////////////////////////
        /// Buttons
        /// /////////////////////////////////////////////////////////

        void UI_Buttons()
        {
            if (Application.isPlaying == true)
            {
                if (GUILayout.Toggle (gun.shooting, "Start Shooting", "Button", GUILayout.Height (25)) == true)
                    gun.StartShooting();
                else
                    gun.StopShooting();

                GUILayout.BeginHorizontal();

                if (GUILayout.Button ("Single Shot", GUILayout.Height (22)))
                    foreach (var targ in targets)
                        (targ as RayfireGun).Shoot();

                if (GUILayout.Button ("    Burst   ", GUILayout.Height (22)))
                    foreach (var targ in targets)
                        (targ as RayfireGun).Burst();

                EditorGUILayout.EndHorizontal();
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Direction
        /// /////////////////////////////////////////////////////////
        
        void UI_Direction()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Direction", EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool showRay = EditorGUILayout.Toggle (gui_dir_show, gun.showRay);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_dir_show.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.showRay = showRay;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            AxisType axis = (AxisType)EditorGUILayout.EnumPopup (gui_dir_axis, gun.axis);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_dir_axis.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.axis = axis;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            Transform targ = (Transform)EditorGUILayout.ObjectField (gui_dir_targ, gun.target, typeof(Transform), true);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_dir_targ.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.target = targ;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float maxDistance = EditorGUILayout.Slider (gui_dir_dist, gun.maxDistance, 0f, 100f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_dir_dist.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.maxDistance = maxDistance;
                    SetDirty (scr);
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Burst
        /// /////////////////////////////////////////////////////////
        
        void UI_Burst()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Burst", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int rounds = EditorGUILayout.IntSlider (gui_bur_rnd, gun.rounds, 2, 20);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_bur_rnd.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.rounds = rounds;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float rate = EditorGUILayout.Slider (gui_bur_rate, gun.rate, 0.01f, 5f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_bur_rate.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.rate = rate;
                    SetDirty (scr);
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Impact
        /// /////////////////////////////////////////////////////////

        void UI_Impact()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Impact", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool showHit = EditorGUILayout.Toggle (gui_imp_show, gun.showHit);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_imp_show.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.showHit = showHit;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            RayfireGun.ImpactType type = (RayfireGun.ImpactType)EditorGUILayout.EnumPopup (gui_imp_tp, gun.type);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_imp_tp.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.type = type;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float strength = EditorGUILayout.Slider (gui_imp_str, gun.strength, 0f, 20f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_imp_str.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.strength = strength;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float radius = EditorGUILayout.Slider (gui_imp_rad, gun.radius, 0f, 10f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_imp_rad.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.radius = radius;
                    SetDirty (scr);
                }
            }

            if (gun.type == RayfireGun.ImpactType.AddExplosionForce)
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float offset = EditorGUILayout.Slider (gui_imp_ofs, gun.offset, -5f, 5f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_imp_ofs.text);
                    foreach (RayfireGun scr in targets)
                    {
                        scr.offset = offset;
                        SetDirty (scr);
                    }
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool affectInactive = EditorGUILayout.Toggle (gui_imp_ina, gun.affectInactive);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_imp_ina.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.affectInactive = affectInactive;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool demolishCluster = EditorGUILayout.Toggle (gui_imp_cls, gun.demolishCluster);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_imp_cls.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.demolishCluster = demolishCluster;
                    SetDirty (scr);
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Components
        /// /////////////////////////////////////////////////////////

        void UI_Components()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Components", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool rigid = EditorGUILayout.Toggle (gui_comp_rg, gun.rigid);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_comp_rg.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.rigid = rigid;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool rigidRoot = EditorGUILayout.Toggle (gui_comp_rt, gun.rigidRoot);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_comp_rt.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.rigidRoot = rigidRoot;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool rigidBody = EditorGUILayout.Toggle (gui_comp_rb, gun.rigidBody);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_comp_rb.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.rigidBody = rigidBody;
                    SetDirty (scr);
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Damage
        /// /////////////////////////////////////////////////////////

        void UI_Damage()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Damage", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float damage = EditorGUILayout.Slider (gui_dmg_val, gun.damage, 0, 100f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_dmg_val.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.damage = damage;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            RayfireGun.PerShardType pShardTp = (RayfireGun.PerShardType)EditorGUILayout.EnumPopup (gui_dmg_shtp, gun.pShardTp);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_dmg_shtp.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.pShardTp = pShardTp;
                    SetDirty (scr);
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// VFX
        /// /////////////////////////////////////////////////////////

        void UI_VFX()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  VFX", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool debris = EditorGUILayout.Toggle (gui_vfx_debris, gun.debris);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_vfx_debris.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.debris = debris;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool dust = EditorGUILayout.Toggle (gui_vfx_dust, gun.dust);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_vfx_dust.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.dust = dust;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool flash = EditorGUILayout.Toggle (gui_vfx_flash, gun.flash);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_vfx_flash.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.flash = flash;
                    SetDirty (scr);
                }
            }

            if (gun.flash == true)
                UI_Flash();
        }

        void UI_Flash()
        {
            EditorGUI.indentLevel++;
            
            GUILayout.Space (space);
            GUILayout.Label ("      Intensity", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float intensityMin = EditorGUILayout.Slider (gui_fl_int_min, gun.Flash.intensityMin, 0.1f, 5f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_fl_int_min.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.Flash.intensityMin = intensityMin;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float intensityMax = EditorGUILayout.Slider (gui_fl_int_max, gun.Flash.intensityMax, 0.1f, 5f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_fl_int_max.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.Flash.intensityMax = intensityMax;
                    SetDirty (scr);
                }
            }
              
            GUILayout.Space (space);
            GUILayout.Label ("      Range", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float rangeMin = EditorGUILayout.Slider (gui_fl_rng_min, gun.Flash.rangeMin, 0.01f, 10f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_fl_rng_min.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.Flash.rangeMin = rangeMin;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float rangeMax = EditorGUILayout.Slider (gui_fl_rng_max, gun.Flash.rangeMax, 0.01f, 10f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_fl_rng_max.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.Flash.rangeMax = rangeMax;
                    SetDirty (scr);
                }
            }
                
            GUILayout.Space (space);
            GUILayout.Label ("      Other", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float distance = EditorGUILayout.Slider (gui_fl_distance, gun.Flash.distance, 0.01f, 2f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_fl_distance.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.Flash.distance = distance;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
                
            EditorGUI.BeginChangeCheck();
            Color color = EditorGUILayout.ColorField (gui_fl_color, gun.Flash.color);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_fl_color.text);
                foreach (RayfireGun scr in targets)
                {
                    scr.Flash.color = color;
                    SetDirty (scr);
                }
            }
                
            EditorGUI.indentLevel--;
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
            string tagFilter = EditorGUILayout.TagField ("Tag", gun.tagFilter);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, "Tag");
                foreach (RayfireGun scr in targets)
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
            int mask = EditorGUILayout.MaskField ("Layer", gun.mask, layerNames.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, "Layer");
                foreach (RayfireGun scr in targets)
                {
                    scr.mask = mask;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Draw
        /// /////////////////////////////////////////////////////////
        
        [DrawGizmo (GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        static void DrawGizmosSelected (RayfireGun gun, GizmoType gizmoType)
        {
            // Ray
            if (gun.showRay == true)
            {
                Gizmos.DrawRay (gun.transform.position, gun.ShootVector * gun.maxDistance);
            }

            // Hit
            if (gun.showHit == true)
            {
                RaycastHit hit;
                bool       hitState = Physics.Raycast (gun.transform.position, gun.ShootVector, out hit, gun.maxDistance, gun.mask);
                if (hitState == true)
                {
                    // TODO COLOR BY IMPACT STR

                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere (hit.point, gun.radius);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////
        
        void SetDirty (RayfireGun scr)
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