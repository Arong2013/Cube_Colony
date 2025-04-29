using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireMan))]
    public class RayfireManEditor : Editor
    {
        Texture2D  logo;
        Texture2D  icon;
        RayfireMan  man;
        
        /// /////////////////////////////////////////////////////////
        /// Static
        /// /////////////////////////////////////////////////////////
        
        static readonly int space = 3;
        
        // Expand
        static bool expandDemolition;
        static bool expandMatPresets;
        static bool expandMatHeavyMetal;
        static bool expandMatLightMetal;
        static bool expandMatDenseRock;
        static bool expandMatPorousRock;
        static bool expandMatConcrete;
        static bool expandMatBrick;
        static bool expandMatGlass;
        static bool expandMatRubber;
        static bool expandMatIce;
        static bool expandMatWood;
        
        static readonly GUIContent gui_ph_set   = new GUIContent ("Set Gravity",     "Sets custom gravity for simulated objects.");
        static readonly GUIContent gui_ph_mul   = new GUIContent ("    Multiplier",  "Custom gravity multiplier.");
        static readonly GUIContent gui_ph_int   = new GUIContent ("Interpolation",   "");
        static readonly GUIContent gui_ph_cok   = new GUIContent ("Cooking Options", "Mesh Collider cooking options.");
        static readonly GUIContent gui_ph_col   = new GUIContent ("Collider Size",   "Minimum object size to get collider.");
        static readonly GUIContent gui_ph_cop   = new GUIContent ("Vertices Amount", "Used only when Planar Check enabled. All meshes with vertices amount less than defined value will perform planar check. If all vertices lay ALMOST on a plane then object will not get collider to avoid convex hull generation errors.");
        static readonly GUIContent gui_col_mesh = new GUIContent ("Mesh",            "Collision detection which will be used for simulated mesh objects.");
        static readonly GUIContent gui_col_cls  = new GUIContent ("Cluster",         "Collision detection which will be used for Connected and Nested clusters.");
        static readonly GUIContent gui_mat_min = new GUIContent ("Minimum Mass", "Minimum mass value which will be assigned to simulated object" +
                                                                                 " if it's mass calculated by it's volume and density will be less than this value.");
        static readonly GUIContent gui_mat_max = new GUIContent ("Maximum Mass", "Maximum mass value which will be assigned to simulated object" + 
                                                                                 " if it's mass calculated by it's volume and density will be higher than this value.");
        static readonly GUIContent gui_mat_pres = new GUIContent ("Material Presets", "List of hardcoded materials with predefined simulation and demolition properties.");
        static readonly GUIContent gui_mat_dest = new GUIContent ("Demolishable",     "Makes object with this material demolishable in runtime.");
        static readonly GUIContent gui_mat_sol  = new GUIContent ("Solidity",         "Global material solidity multiplier which used at collision to calculate if object should be demolished or not.");
        static readonly GUIContent gui_mat_dens = new GUIContent ("Density",          "Object mass depends on picked material density and collider volume.");
        static readonly GUIContent gui_mat_drag = new GUIContent ("Drag",             "Allows to decrease position velocity over time.");
        static readonly GUIContent gui_mat_ang  = new GUIContent ("Angular Drag",     "Allows to decrease rotation velocity over time.");
        static readonly GUIContent gui_mat_mat = new GUIContent ("Material",         "Physic material which will be used for all objects with this material." + 
                                                                                     "If Material is not define then it will be created and defined here at Start using following Frictions and Bounciness properties.");
        static readonly GUIContent gui_mat_dyn  = new GUIContent ("Dynamic Friction", "");
        static readonly GUIContent gui_mat_stat = new GUIContent ("Static Friction",  "");
        static readonly GUIContent gui_mat_bnc  = new GUIContent ("Bounciness",       "");
        static readonly GUIContent gui_act_par  = new GUIContent ("Parent",           "Object which will become parent of activated object");
        static readonly GUIContent gui_dml_sol  = new GUIContent ("Global Solidity",  "Global Solidity multiplier. Affect solidity of all simulated objects.");
        static readonly GUIContent gui_dml_time = new GUIContent ("Time Quota", "Demolition time quota in milliseconds. Allows to prevent demolition at " +
                                                                                "the same frame if there was already another demolition " + 
                                                                                "at the same frame and it took more time than Time Quota value.");
        static readonly GUIContent gui_adv_expand  = new GUIContent ("Advanced Properties", "");
        static readonly GUIContent gui_adv_parent  = new GUIContent ("Parent",              "Defines parent for all new fragments.");
        static readonly GUIContent gui_adv_global  = new GUIContent ("    Global Parent",   "Defines parent for all new fragments.");
        static readonly GUIContent gui_adv_current = new GUIContent ("Current Amount",      "Amount of created fragments.");
        static readonly GUIContent gui_adv_amount = new GUIContent ("Maximum Amount", "Maximum amount of allowed fragments. Object won't be demolished if existing amount of fragments "+
                                                                                      "in scene higher that this value. Fading allows to decrease amount of fragments in scene.");
        static readonly GUIContent gui_adv_bad  = new GUIContent ("Bad Mesh Try",     "Defines parent for all new fragments.");
        static readonly GUIContent gui_adv_size = new GUIContent ("Size Threshold",   "Disable Shadow Casting for all objects with size less than this value.");
        static readonly GUIContent gui_pl_frg   = new GUIContent ("Fragments",        "Create gameobjects with MeshFilter, MeshRenderer and RayFireRigid components until Min Capacity value will be reached. Objects will be used for runtime fragments when needed");
        static readonly GUIContent gui_pl_prt   = new GUIContent ("Particles",        "Create gameobjects with Particle System until Min Capacity value will be reached. Objects will be used for Debris or Dust when needed.");
        static readonly GUIContent gui_pl_bck   = new GUIContent ("    Reuse",        "Do not destroy objects and send them back to pool until Max Capacity value will be reached");
        static readonly GUIContent gui_pl_min   = new GUIContent ("    Capacity Min", "");
        static readonly GUIContent gui_pl_max   = new GUIContent ("    Capacity Max", "");
        
        /// /////////////////////////////////////////////////////////
        /// Inspector
        /// /////////////////////////////////////////////////////////
        
        public override void OnInspectorGUI()
        {
            man = target as RayfireMan;
            if (man == null)
                return;
            
            // Set new static instance
            if (RayfireMan.inst == null)
                RayfireMan.inst = man;

            GUILayout.Space (8);

            if (Application.isPlaying == true)
            {
                if (GUILayout.Button ("Destroy Storage Fragments", GUILayout.Height (20)))
                    RayfireMan.inst.storage.DestroyAll();
                
                GUILayout.Space (space);
            }
            
            UI_Physics();
            
            GUILayout.Space (space);
            
            UI_Collision();
            
            GUILayout.Space (space);

            UI_Materials();

            GUILayout.Space (space);
            
            UI_Activation();

            GUILayout.Space (space);
            
            UI_Demolition();

            GUILayout.Space (space);

            UI_Pooling();
            
            GUILayout.Space (space);

            UI_Info();

            GUILayout.Space (space);
            
            UI_About();

            GUILayout.Space (8);
        }

        /// /////////////////////////////////////////////////////////
        /// Physics
        /// /////////////////////////////////////////////////////////

        void UI_Physics()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Physics", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool setGravity = EditorGUILayout.Toggle (gui_ph_set, man.setGravity);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_ph_set.text);
                man.setGravity = setGravity;
                SetDirty (man);
            }

            GUILayout.Space (space);
            
            if (man.setGravity == true)
            {
                EditorGUI.BeginChangeCheck();
                float multiplier = EditorGUILayout.Slider (gui_ph_mul, man.multiplier, 0f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ph_mul.text);
                    man.multiplier = multiplier;
                    SetDirty (man);
                }
                
                GUILayout.Space (space);
            }
            
            EditorGUI.BeginChangeCheck();
            RigidbodyInterpolation interpolation = (RigidbodyInterpolation)EditorGUILayout.EnumPopup (gui_ph_int, man.interpolation);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_ph_int.text);
                man.interpolation = interpolation;
                SetDirty (man);
            }

            GUILayout.Space (space);
            GUILayout.Label ("  Collider", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float colliderSize = EditorGUILayout.Slider (gui_ph_col, man.colliderSize, 0f, 1f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_ph_col.text);
                man.colliderSize = colliderSize;
                SetDirty (man);
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int coplanarVerts = EditorGUILayout.IntSlider (gui_ph_cop, man.coplanarVerts, 0, 999);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_ph_cop.text);
                man.coplanarVerts = coplanarVerts;
                SetDirty (man);
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            MeshColliderCookingOptions cookingOptions = (MeshColliderCookingOptions)EditorGUILayout.EnumFlagsField (gui_ph_cok, man.cookingOptions);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_ph_cok.text);
                man.cookingOptions = cookingOptions;
                SetDirty (man);
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Collision
        /// /////////////////////////////////////////////////////////
        
        void UI_Collision()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Collision Detection", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            CollisionDetectionMode meshCollision = (CollisionDetectionMode)EditorGUILayout.EnumPopup (gui_col_mesh, man.meshCollision);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_col_mesh.text);
                man.meshCollision = meshCollision;
                SetDirty (man);
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            CollisionDetectionMode clusterCollision = (CollisionDetectionMode)EditorGUILayout.EnumPopup (gui_col_cls, man.clusterCollision);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_col_cls.text);
                man.clusterCollision = clusterCollision;
                SetDirty (man);
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Materials
        /// /////////////////////////////////////////////////////////

        void UI_Materials()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Materials", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float minimumMass = EditorGUILayout.Slider (gui_mat_min, man.minimumMass, 0f, 1f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_mat_min.text);
                man.minimumMass = minimumMass;
                SetDirty (man);
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float maximumMass = EditorGUILayout.Slider (gui_mat_max, man.maximumMass, 0f, 4000f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_mat_max.text);
                man.maximumMass = maximumMass;
                SetDirty (man);
            }

            GUILayout.Space (space);
            
            UI_Materials_Presets();
        }

        void UI_Materials_Presets()
        {
            expandMatPresets = EditorGUILayout.Foldout (expandMatPresets, gui_mat_pres, true);
            if (expandMatPresets == true)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Space (space);
                
                UI_Material (man.materialPresets.heavyMetal, ref expandMatHeavyMetal, "Heavy Metal");
                
                GUILayout.Space (space);
                
                UI_Material (man.materialPresets.lightMetal, ref expandMatLightMetal, "Light Metal");
                
                GUILayout.Space (space);
                
                UI_Material (man.materialPresets.denseRock, ref expandMatDenseRock, "Dense Rock");
                
                GUILayout.Space (space);
                
                UI_Material (man.materialPresets.porousRock, ref expandMatPorousRock, "Porous Rock");
                
                GUILayout.Space (space);
                
                UI_Material (man.materialPresets.concrete, ref expandMatConcrete, "Concrete");
                
                GUILayout.Space (space);
                
                UI_Material (man.materialPresets.brick, ref expandMatBrick, "Brick");
                
                GUILayout.Space (space);
                
                UI_Material (man.materialPresets.glass, ref expandMatGlass, "Glass");
                
                GUILayout.Space (space);
                
                UI_Material (man.materialPresets.rubber, ref expandMatRubber, "Rubber");
                
                GUILayout.Space (space);
                
                UI_Material (man.materialPresets.ice, ref expandMatIce, "Ice");
                
                GUILayout.Space (space);
                
                UI_Material (man.materialPresets.wood, ref expandMatWood, "Wood");
                
                EditorGUI.indentLevel--;
            }
        }

        void UI_Material(RFMaterial mat, ref bool state, string cap)
        {
            state = EditorGUILayout.Foldout (state, cap, true);
            if (state == true)
            {
                GUILayout.Space (space);
                
                EditorGUI.indentLevel++;
                
                GUILayout.Label ("          Demolition", EditorStyles.boldLabel);
                
                EditorGUI.BeginChangeCheck();
                bool destructible = EditorGUILayout.Toggle (gui_mat_dest, mat.destructible);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_mat_dest.text);
                    mat.destructible = destructible;
                    SetDirty (man);
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                int solidity = EditorGUILayout.IntSlider (gui_mat_sol, mat.solidity, 0, 100);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_mat_sol.text);
                    mat.solidity = solidity;
                    SetDirty (man);
                }
                
                GUILayout.Label ("          Rigid Body", EditorStyles.boldLabel);
                
                EditorGUI.BeginChangeCheck();
                float density = EditorGUILayout.Slider (gui_mat_dens, mat.density, 0.01f, 100f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_mat_dens.text);
                    mat.density = density;
                    SetDirty (man);
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float drag = EditorGUILayout.Slider (gui_mat_drag, mat.drag, 0f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_mat_drag.text);
                    mat.drag = drag;
                    SetDirty (man);
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float angularDrag = EditorGUILayout.Slider (gui_mat_ang, mat.angularDrag, 0f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_mat_ang.text);
                    mat.angularDrag = angularDrag;
                    SetDirty (man);
                }
                
                GUILayout.Label ("          Physic Material", EditorStyles.boldLabel);
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                PhysicsMaterial material = (PhysicsMaterial)EditorGUILayout.ObjectField (gui_mat_mat, mat.material, typeof(PhysicsMaterial), true);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_mat_mat.text);
                    mat.material = material;
                    SetDirty (man);
                }
                
                EditorGUI.BeginChangeCheck();
                float dynamicFriction = EditorGUILayout.Slider (gui_mat_dyn, mat.dynamicFriction, 0.01f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_mat_dyn.text);
                    mat.dynamicFriction = dynamicFriction;
                    SetDirty (man);
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float staticFriction = EditorGUILayout.Slider (gui_mat_stat, mat.staticFriction, 0.01f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_mat_stat.text);
                    mat.staticFriction = staticFriction;
                    SetDirty (man);
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float bounciness = EditorGUILayout.Slider (gui_mat_bnc, mat.bounciness, 0.01f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_mat_bnc.text);
                    mat.bounciness = bounciness;
                    SetDirty (man);
                }
                
                EditorGUI.indentLevel--;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Activation
        /// /////////////////////////////////////////////////////////

        void UI_Activation()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Activation", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            GameObject parent = (GameObject)EditorGUILayout.ObjectField (gui_act_par, man.parent, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_act_par.text);
                man.parent = parent;
                SetDirty (man);
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Demolition
        /// /////////////////////////////////////////////////////////

        void UI_Demolition()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Demolition", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float globalSolidity = EditorGUILayout.Slider (gui_dml_sol, man.globalSolidity, 0f, 5f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_dml_sol.text);
                man.globalSolidity = globalSolidity;
                SetDirty (man);
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float timeQuota = EditorGUILayout.Slider (gui_dml_time, man.timeQuota, 0f, 0.1f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_dml_time.text);
                man.timeQuota = timeQuota;
                SetDirty (man);
            }
            
            GUILayout.Space (space);

            UI_Demolition_Adv();
        }

        void UI_Demolition_Adv()
        {
            expandDemolition = EditorGUILayout.Foldout (expandDemolition, gui_adv_expand, true);
            if (expandDemolition == true)
            {
                EditorGUI.indentLevel++;

                GUILayout.Space (space);
                GUILayout.Label ("  Fragments", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                RFManDemolition.FragmentParentType parent = (RFManDemolition.FragmentParentType)EditorGUILayout.EnumPopup
                    (gui_adv_parent, man.advancedDemolitionProperties.parent);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_adv_parent.text);
                    man.advancedDemolitionProperties.parent = parent;
                    SetDirty (man);
                }

                GUILayout.Space (space);

                if (man.advancedDemolitionProperties.parent == RFManDemolition.FragmentParentType.GlobalParent)
                {
                    EditorGUI.BeginChangeCheck();
                    Transform globalParent = (Transform)EditorGUILayout.ObjectField
                        (gui_adv_global, man.advancedDemolitionProperties.globalParent,  typeof(Transform), true);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_adv_global.text);
                        man.advancedDemolitionProperties.globalParent = globalParent;
                        SetDirty (man);
                    }
                    
                    GUILayout.Space (space);
                }

                EditorGUI.BeginChangeCheck();
                int currentAmount = EditorGUILayout.IntField
                    (gui_adv_current, man.advancedDemolitionProperties.currentAmount);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_adv_current.text);
                    man.advancedDemolitionProperties.currentAmount = currentAmount;
                    SetDirty (man);
                }
                
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int maximumAmount = EditorGUILayout.IntField
                    (gui_adv_amount, man.advancedDemolitionProperties.maximumAmount);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_adv_amount.text);
                    man.advancedDemolitionProperties.maximumAmount = maximumAmount;
                    SetDirty (man);
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int badMeshTry = EditorGUILayout.IntSlider
                    (gui_adv_bad, man.advancedDemolitionProperties.badMeshTry, 1, 10);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_adv_bad.text);
                    man.advancedDemolitionProperties.badMeshTry = badMeshTry;
                    SetDirty (man);
                }

                GUILayout.Space (space);
                GUILayout.Label ("  Shadows", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float sizeThreshold = EditorGUILayout.Slider
                    (gui_adv_size, man.advancedDemolitionProperties.sizeThreshold, 0, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_adv_size.text);
                    man.advancedDemolitionProperties.sizeThreshold = sizeThreshold;
                    SetDirty (man);
                }
                
                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Pooling
        /// /////////////////////////////////////////////////////////

        void UI_Pooling()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Pooling", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool enable = EditorGUILayout.Toggle (gui_pl_frg, man.fragments.enable);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_pl_frg.text);
                man.fragments.enable = enable;
                SetDirty (man);
            }
            
            if (man.fragments.enable == true)
            {
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool reuse = EditorGUILayout.Toggle (gui_pl_bck, man.fragments.reuse);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_pl_bck.text);
                    man.fragments.reuse = reuse;
                    SetDirty (man);
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                int minCap = EditorGUILayout.IntSlider (gui_pl_min, man.fragments.minCap, 0, 3000);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_pl_min.text);
                    man.fragments.minCap = minCap;
                    SetDirty (man);
                }
                
                if (man.fragments.reuse == true)
                {
                    GUILayout.Space (space);
                    
                    EditorGUI.BeginChangeCheck();
                    int maxCap = EditorGUILayout.IntSlider (gui_pl_max, man.fragments.maxCap, 0, 3000);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_pl_max.text);
                        man.fragments.maxCap = maxCap;
                        SetDirty (man);
                    }
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool en = EditorGUILayout.Toggle (gui_pl_prt, man.particles.enable);
            if (EditorGUI.EndChangeCheck() == true)
            {
                {
                    Undo.RecordObjects (targets, gui_pl_prt.text);
                    man.particles.enable = en;
                    SetDirty (man);
                }
                man.particles.Enable = man.particles.enable;
            }
                
            // Info
            if (man.particles.enable == true)
            {
                if (man.particles.emitters != null && man.particles.emitters.Count > 0)
                {
                    GUILayout.Space (space);
                    GUILayout.Label ("    Emitters: " + man.particles.emitters.Count,        EditorStyles.boldLabel);
                    GUILayout.Space (space);
                    GUILayout.Label ("    Particles: " + man.particles.GetTotalPoolAmount(), EditorStyles.boldLabel);
                    GUILayout.Space (space);
                    GUILayout.Label ("    Reused: " + man.particles.reused,                  EditorStyles.boldLabel);
                    GUILayout.Space (space);
                    GUILayout.Label ("    In Scene: " + man.particles.ResetCheck(),          EditorStyles.boldLabel);
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Info
        /// /////////////////////////////////////////////////////////

        void UI_Info()
        {
            if (Application.isPlaying == true)
            {
                GUILayout.Label ("  Info:", EditorStyles.boldLabel);

                if (man.fragments.enable == true && man.fragments.queue.Count > 0)
                    GUILayout.Label ("Rigid Pool: " + man.fragments.queue.Count);
                
                if (man.advancedDemolitionProperties.currentAmount > 0)
                    GUILayout.Label ("Fragments: " + man.advancedDemolitionProperties.currentAmount + "/" + man.advancedDemolitionProperties.maximumAmount);
            }
        }

        /// /////////////////////////////////////////////////////////
        /// About
        /// /////////////////////////////////////////////////////////
        
        void UI_About()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  About", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool debug = EditorGUILayout.Toggle ("Debug Messages", man.debug);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, "Debug");
                man.debug = debug;
                SetDirty (man);
            }
            
            GUILayout.Space (space);
            
            GUILayout.Label ("Plugin build: " + RayfireMan.buildMajor + '.' + RayfireMan.buildMinor.ToString ("D2"));

            GUILayout.Space (space);

            // Logo TODO remove if component removed
            if (logo == null)
                logo = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/RayFire/Info/Logo/logo_small.png", typeof(Texture2D));
            if (logo != null)
                GUILayout.Box (logo, GUILayout.Width ((int)EditorGUIUtility.currentViewWidth - 19f), GUILayout.Height (64));
            
            if (GUILayout.Button ("     Changelog     ", GUILayout.Height (20)))
                Application.OpenURL ("https://assetstore.unity.com/packages/tools/game-toolkits/rayfire-for-unity-148690#releases");
        }

        
        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////

        void SetDirty (RayfireMan scr)
        {
            if (Application.isPlaying == false)
            {
                EditorUtility.SetDirty (scr);
                EditorSceneManager.MarkSceneDirty (scr.gameObject.scene);
            }
        }
    }
}