﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireShatter))]
    public class RayfireShatterEditor : Editor
    {
        RayfireShatter shat;
        Transform      transForm;
        Vector3        centerWorldPos;
        Quaternion     centerWorldQuat;

        SerializedProperty custTmProp;
        ReorderableList    custTmList;
        SerializedProperty custPointProp;
        ReorderableList    custPointList;
        SerializedProperty sliceTmProp;
        ReorderableList    sliceTmList;

        /// /////////////////////////////////////////////////////////
        /// Static
        /// /////////////////////////////////////////////////////////
        static readonly int space = 3;
        static bool exp_deb;
        static bool exp_lim;
        static bool exp_fil;

        static readonly GUIContent gui_tp              = new GUIContent ("Type",                "Defines fragmentation type for object.");
        static readonly GUIContent gui_tp_vor          = new GUIContent ("      Voronoi",       "Low poly, convex, physics friendly fragments.");
        static readonly GUIContent gui_tp_vor_amount   = new GUIContent ("Amount",              "Defines amount of points in point cloud, every point represent rough center of  fragment.");
        static readonly GUIContent gui_tp_vor_bias     = new GUIContent ("Center Bias",         "Defines offset of points in point cloud towards Center.");
        static readonly GUIContent gui_tp_spl          = new GUIContent ("      Splinters",     "Low poly, convex, physics friendly fragments, stretched along one axis.");
        static readonly GUIContent gui_tp_spl_axis     = new GUIContent ("Axis",                "Fragments will be stretched over defined axis.");
        static readonly GUIContent gui_tp_spl_str      = new GUIContent ("Strength",            "Defines sharpness of stretched fragments.");
        static readonly GUIContent gui_tp_slb          = new GUIContent ("      Slabs",         "Low poly, convex, physics friendly fragments, stretched along two axes.");
        static readonly GUIContent gui_tp_rad          = new GUIContent ("      Radial",        "Low poly, convex, physics friendly fragments, creates radial fragments pattern.");
        static readonly GUIContent gui_tp_rad_axis     = new GUIContent ("Center Axis",         "");
        static readonly GUIContent gui_tp_rad_radius   = new GUIContent ("Radius",              "");
        static readonly GUIContent gui_tp_rad_div      = new GUIContent ("Divergence",          "");
        static readonly GUIContent gui_tp_rad_rest     = new GUIContent ("  Restrict To Plane", "");
        static readonly GUIContent gui_tp_rad_rings    = new GUIContent ("Rings",               "");
        static readonly GUIContent gui_tp_rad_focus    = new GUIContent ("Focus",               "");
        static readonly GUIContent gui_tp_rad_str      = new GUIContent ("Focus Strength",      "");
        static readonly GUIContent gui_tp_rad_randRing = new GUIContent ("Random Rings",        "");
        static readonly GUIContent gui_tp_rad_rays     = new GUIContent ("Rays",                "");
        static readonly GUIContent gui_tp_rad_randRay  = new GUIContent ("Random Rays",         "");
        static readonly GUIContent gui_tp_rad_twist    = new GUIContent ("Twist",               "");
        static readonly GUIContent gui_tp_hex          = new GUIContent ("      Hex",           "");
        static readonly GUIContent gui_tp_hex_grd      = new GUIContent ("      Grid",          "");
        static readonly GUIContent gui_tp_hex_size     = new GUIContent ("Size",                "Hex size");
        static readonly GUIContent gui_tp_hex_grid     = new GUIContent ("Grid",                "");
        static readonly GUIContent gui_tp_hex_am       = new GUIContent ("Amount",              "Amount of hexes in grid in two axes");

        static readonly GUIContent gui_tp_cus         = new GUIContent ("      Custom",   "Low poly, convex, physics friendly fragments, allows to use custom point cloud for fragments distribution.");
        static readonly GUIContent gui_tp_cus_src     = new GUIContent ("Source",         "");
        static readonly GUIContent gui_tp_cus_use     = new GUIContent ("Use As",         "");
        static readonly GUIContent gui_tp_cus_am      = new GUIContent ("Amount",         "");
        static readonly GUIContent gui_tp_cus_rad     = new GUIContent ("Radius",         "");
        static readonly GUIContent gui_tp_cus_en      = new GUIContent ("Enable",         "");
        static readonly GUIContent gui_tp_cus_sz      = new GUIContent ("Size",           "");
        static readonly GUIContent gui_tp_mir         = new GUIContent ("      Mirrored", "Low poly, convex, physics friendly fragments, generate custom point cloud mirrored at the edges over defined axes.");
        static readonly GUIContent gui_tp_slc         = new GUIContent ("      Slice",    "Slice object by planes.");
        static readonly GUIContent gui_tp_slc_pl      = new GUIContent ("Plane",          "Slicing plane.");
        static readonly GUIContent gui_tp_brk         = new GUIContent ("      Bricks",   "");
        static readonly GUIContent gui_tp_brk_type    = new GUIContent ("Lattice",        "");
        static readonly GUIContent gui_tp_brk_mult    = new GUIContent ("Multiplier",     "");
        static readonly GUIContent gui_tp_brk_am_X    = new GUIContent ("X axis",         "");
        static readonly GUIContent gui_tp_brk_am_Y    = new GUIContent ("Y axis",         "");
        static readonly GUIContent gui_tp_brk_am_Z    = new GUIContent ("Z axis",         "");
        static readonly GUIContent gui_tp_brk_lock    = new GUIContent ("Lock",           "");
        static readonly GUIContent gui_tp_brk_sp_prob = new GUIContent ("Probability",    "");
        static readonly GUIContent gui_tp_brk_sp_offs = new GUIContent ("Offset",         "");
        static readonly GUIContent gui_tp_brk_sp_rot  = new GUIContent ("Rotation",       "");
        static readonly GUIContent gui_tp_vxl         = new GUIContent ("      Voxels",   "");
        static readonly GUIContent gui_tp_tet = new GUIContent ("      Tets", "Tetrahedron based fragments, this type is mostly useless as is and should be used with Gluing, " +
                                                                              "in this case it creates high poly concave fragments.");
        static readonly GUIContent gui_tp_tetDn        = new GUIContent ("Density",           "");
        static readonly GUIContent gui_tp_tetNs        = new GUIContent ("Noise",             "");
        static readonly GUIContent gui_pr_mode         = new GUIContent ("Mode",              "");
        static readonly GUIContent gui_mat_in          = new GUIContent ("Material",          "Defines material for fragment's inner surface.");
        static readonly GUIContent gui_mat_scl         = new GUIContent ("Mapping Scale",     "Defines mapping scale for inner surface.");
        static readonly GUIContent gui_mat_col         = new GUIContent ("Color",             "Set custom Vertex Color for all inner surface vertices.");
        static readonly GUIContent gui_mat_uve         = new GUIContent ("UV",                "Set custom UV coordinate for all inner surface vertices.");
        static readonly GUIContent gui_pr_exp          = new GUIContent ("  Export",          "Export fragments meshes to Unity Asset and reference to this asset.");
        static readonly GUIContent gui_pr_exp_src      = new GUIContent ("Source",            "");
        static readonly GUIContent gui_pr_exp_sfx      = new GUIContent ("Suffix",            "");
        static readonly GUIContent gui_pr_cls          = new GUIContent ("  Clusters",        "Allows to glue groups of fragments into single mesh by deleting shared faces.");
        static readonly GUIContent gui_pr_cls_en       = new GUIContent ("Enable",            "Allows to glue groups of fragments into single mesh by deleting shared faces.");
        static readonly GUIContent gui_pr_cls_cnt      = new GUIContent ("Count",             "Amount of clusters defined by random point cloud.");
        static readonly GUIContent gui_pr_cls_seed     = new GUIContent ("Seed",              "Random seed for clusters point cloud generator.");
        static readonly GUIContent gui_pr_cls_rel      = new GUIContent ("Relax",             "Smooth strength for cluster inner surface.");
        static readonly GUIContent gui_pr_cls_debris   = new GUIContent ("Debris",            "Preserve some fragments at the edges of clusters to create small debris around big chunks.");
        static readonly GUIContent gui_pr_cls_amount   = new GUIContent ("Amount",            "Amount of debris in last layer in percents relative to amount of fragments in cluster.");
        static readonly GUIContent gui_pr_cls_layers   = new GUIContent ("Layers",            "Amount of debris layers at cluster border.");
        static readonly GUIContent gui_pr_cls_scale    = new GUIContent ("Scale",             "Scale variation for inner debris.");
        static readonly GUIContent gui_pr_cls_min      = new GUIContent ("Minimum",           "Minimum amount of fragments in debris cluster.");
        static readonly GUIContent gui_pr_cls_max      = new GUIContent ("Maximum",           "Maximum amount of fragments in debris cluster.");
        static readonly GUIContent gui_pr_adv_seed     = new GUIContent ("Seed",              "Seed for point cloud generator. Set to 0 to get random point cloud every time.");
        static readonly GUIContent gui_pr_adv_dec      = new GUIContent ("Decompose",         "Check output fragments and separate not connected parts of meshes into separate fragments.");
        static readonly GUIContent gui_pr_adv_col      = new GUIContent ("Collinear",         "Remove vertices which lay on straight edge.");
        static readonly GUIContent gui_pr_adv_copy     = new GUIContent ("Copy",              "Copy components from original object to fragments");
        static readonly GUIContent gui_pr_adv_bake     = new GUIContent ("Bake",              "Prepares fragment meshes for use with a MeshCollider.");
        static readonly GUIContent gui_pr_adv_smooth   = new GUIContent ("Smooth",            "Smooth fragments inner surface.");
        static readonly GUIContent gui_pr_adv_combine  = new GUIContent ("Combine",           "Combine all children meshes into one mesh and fragment this mesh.");
        static readonly GUIContent gui_pr_adv_input    = new GUIContent ("Input Precap",      "Create extra triangles to connect open edges and close mesh volume for correct fragmentation.");
        static readonly GUIContent gui_pr_adv_output   = new GUIContent ("    Output Precap", "Keep fragment's faces created by Input Precap.");
        static readonly GUIContent gui_pr_adv_remove   = new GUIContent ("Double Faces",      "Delete faces which overlap with each other.");
        static readonly GUIContent gui_pr_adv_element  = new GUIContent ("Element Size",      "Input mesh will be separated to not connected mesh elements, every element will be fragmented separately." + "This threshold value measures in percentage relative to original objects size and prevent element from being fragmented if its size is less.");
        static readonly GUIContent gui_pr_adv_inner    = new GUIContent ("Inner",             "Do not output inner fragments which has no outer surface.");
        static readonly GUIContent gui_pr_adv_planar   = new GUIContent ("Planar",            "Do not output planar fragments which mesh vertices lie in the same plane.");
        static readonly GUIContent gui_pr_adv_rel      = new GUIContent ("Relative Size",     "Do not output small fragments. Measures is percentage relative to original object size.");
        static readonly GUIContent gui_pr_adv_abs      = new GUIContent ("Absolute Size",     "Do not output small fragments which size in world units is less than this value.");
        static readonly GUIContent gui_pr_adv_size_lim = new GUIContent ("Size",              "All fragments with size bigger than Max Size value will be fragmented to few more fragments.");
        static readonly GUIContent gui_pr_adv_size_am  = new GUIContent ("    Max Size",      "");
        static readonly GUIContent gui_pr_adv_vert_lim = new GUIContent ("Vertex",            "All fragments with vertex amount higher than Max Amount value will be fragmented to few more fragments.");
        static readonly GUIContent gui_pr_adv_vert_am  = new GUIContent ("    Max Amount",    "");
        static readonly GUIContent gui_pr_adv_tri_lim  = new GUIContent ("Triangle",          "All fragments with triangle amount higher than Max Amount value will be fragmented to few more fragments.");
        static readonly GUIContent gui_pr_adv_tri_am   = new GUIContent ("    Max Amount",    "");
        static readonly GUIContent gui_cn_pos = new GUIContent ("Position", "");
        static readonly GUIContent gui_cn_rot = new GUIContent ("Rotation", "");
        static readonly GUIContent gui_cn_res = new GUIContent ("Reset  ", "");
        static readonly GUIContent gui_int = new GUIContent ("Interactive", "Preview fragments as one mesh. WARNING: Do not forget to Restore Original Mesh.");
        
        /// /////////////////////////////////////////////////////////
        /// Enable
        /// /////////////////////////////////////////////////////////
        
        private void OnEnable()
        {
            custTmProp                     = serializedObject.FindProperty ("custom.transforms");
            custTmList                     = new ReorderableList (serializedObject, custTmProp, true, true, true, true);
            custTmList.drawElementCallback = DrawCustTmListItems;
            custTmList.drawHeaderCallback  = DrawCustTmHeader;
            custTmList.onAddCallback       = AddCustTm;
            custTmList.onRemoveCallback    = RemoveCustTm;

            custPointProp                     = serializedObject.FindProperty ("custom.vector3");
            custPointList                     = new ReorderableList (serializedObject, custPointProp, true, true, true, true);
            custPointList.drawElementCallback = DrawCustPointListItems;
            custPointList.drawHeaderCallback  = DrawCustPointHeader;
            custPointList.onAddCallback       = AddCustPoint;
            custPointList.onRemoveCallback    = RemoveCustPoint;

            sliceTmProp                     = serializedObject.FindProperty ("slice.sliceList");
            sliceTmList                     = new ReorderableList (serializedObject, sliceTmProp, true, true, true, true);
            sliceTmList.drawElementCallback = DrawSliceTmListItems;
            sliceTmList.drawHeaderCallback  = DrawSliceTmHeader;
            sliceTmList.onAddCallback       = AddSliceTm;
            sliceTmList.onRemoveCallback    = RemoveSliceTm;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Inspector
        /// /////////////////////////////////////////////////////////
        
        public override void OnInspectorGUI()
        {
            shat = target as RayfireShatter;
            if (shat == null)
                return;

            // Get inspector width
            // float width = EditorGUIUtility.currentViewWidth - 20f;

            // Space
            GUILayout.Space (8);
            
            UI_Fragment();
            UI_Interactive();
            
            GUILayout.Space (space);
            
            UI_Preview();

            // Reset scale if fragments were deleted
            shat.ResetScale (shat.previewScale);

            GUILayout.Space (space);
            
            UI_Types();

            GUILayout.Space (space);

            UI_Material();

            GUILayout.Space (space);

            UI_Cluster();

            GUILayout.Space (space);

            UI_Advanced();

            GUILayout.Space (space);

            UI_Export();

            GUILayout.Space (space);

            UI_Collider();

            GUILayout.Space (space);

            UI_Center();

            GUILayout.Space (space);

            InfoUI();

            GUILayout.Space (8);
        }

        /// /////////////////////////////////////////////////////////
        /// Types
        /// /////////////////////////////////////////////////////////
        
        void UI_Types()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Fragments", EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            FragType type = (FragType)EditorGUILayout.EnumPopup (gui_tp, shat.type);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.type = type;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.indentLevel++;

            if (shat.type == FragType.Voronoi)
                UI_Type_Voronoi();
            else if (shat.type == FragType.Splinters)
                UI_Type_Splinters();
            else if (shat.type == FragType.Slabs)
                UI_Type_Slabs();
            else if (shat.type == FragType.Radial)
                UI_Type_Radial();
            else if (shat.type == FragType.Hexagon)
                UI_Type_HexGrid();
            else if (shat.type == FragType.Custom)
                UI_Type_Custom();
            else if (shat.type == FragType.Slices)
                UI_Type_Slices();
            else if (shat.type == FragType.Bricks)
                UI_Type_Bricks();
            else if (shat.type == FragType.Voxels)
                UI_Type_Voxels();
            else if (shat.type == FragType.Tets)
                UI_Type_Tets();

            EditorGUI.indentLevel--;
        }

        /// /////////////////////////////////////////////////////////
        /// Voronoi
        /// /////////////////////////////////////////////////////////
        
        void UI_Type_Voronoi()
        {
            GUILayout.Space (space);
            GUILayout.Label (gui_tp_vor, EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int amount = EditorGUILayout.IntField (gui_tp_vor_amount, shat.voronoi.amount);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_vor_amount.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.voronoi.amount = amount;
                    SetDirty (scr);
                }
                shat.InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float centerBias = EditorGUILayout.Slider (gui_tp_vor_bias, shat.voronoi.centerBias, 0f, 1f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_vor_bias.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.voronoi.centerBias = centerBias;
                    SetDirty (scr);
                }
                InteractiveChange();
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Splinters
        /// /////////////////////////////////////////////////////////
        
        void UI_Type_Splinters()
        {
            GUILayout.Space (space);
            GUILayout.Label (gui_tp_spl, EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            AxisType axis = (AxisType)EditorGUILayout.EnumPopup (gui_tp_spl_axis, shat.splinters.axis);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_spl_axis.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.splinters.axis = axis;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int amount = EditorGUILayout.IntField (gui_tp_vor_amount, shat.splinters.amount);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_vor_amount.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.splinters.amount = amount;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float strength = EditorGUILayout.Slider (gui_tp_spl_str, shat.splinters.strength, 0f, 1f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_spl_str.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.splinters.strength = strength;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float centerBias = EditorGUILayout.Slider (gui_tp_vor_bias, shat.splinters.centerBias, 0f, 1f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_vor_bias.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.splinters.centerBias = centerBias;
                    SetDirty (scr);
                }
                InteractiveChange();
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Slabs
        /// /////////////////////////////////////////////////////////
        void UI_Type_Slabs()
        {
            GUILayout.Space (space);
            GUILayout.Label (gui_tp_slb, EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            AxisType axis = (AxisType)EditorGUILayout.EnumPopup (gui_tp_spl_axis, shat.slabs.axis);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_spl_axis.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.slabs.axis = axis;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int amount = EditorGUILayout.IntField (gui_tp_vor_amount, shat.slabs.amount);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_vor_amount.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.slabs.amount = amount;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float strength = EditorGUILayout.Slider (gui_tp_spl_str, shat.slabs.strength, 0f, 1f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_spl_str.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.slabs.strength = strength;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float centerBias = EditorGUILayout.Slider (gui_tp_vor_bias, shat.slabs.centerBias, 0f, 1f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_vor_bias.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.slabs.centerBias = centerBias;
                    SetDirty (scr);
                }
                InteractiveChange();
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Radial
        /// /////////////////////////////////////////////////////////
        
        void UI_Type_Radial()
        {
            GUILayout.Space (space);
            GUILayout.Label (gui_tp_rad, EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            AxisType centerAxis = (AxisType)EditorGUILayout.EnumPopup (gui_tp_rad_axis, shat.radial.centerAxis);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_rad_axis.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.radial.centerAxis = centerAxis;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float radius = EditorGUILayout.Slider (gui_tp_rad_radius, shat.radial.radius, 0.01f, 30f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_rad_radius.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.radial.radius = radius;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float divergence = EditorGUILayout.Slider (gui_tp_rad_div, shat.radial.divergence, 0f, 1f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_rad_div.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.radial.divergence = divergence;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            if (shat.radial.divergence > 0)
            {
                EditorGUI.BeginChangeCheck();
                bool restrictToPlane = EditorGUILayout.Toggle (gui_tp_rad_rest, shat.radial.restrictToPlane);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_tp_rad_rest.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.radial.restrictToPlane = restrictToPlane;
                        SetDirty (scr);
                    }
                    InteractiveChange();
                }
            }

            GUILayout.Space (space);
            GUILayout.Label ("      Rings", EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int rings = EditorGUILayout.IntSlider (gui_tp_rad_rings, shat.radial.rings, 3, 60);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_rad_rings.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.radial.rings = rings;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int randomRings = EditorGUILayout.IntSlider (gui_tp_rad_randRing, shat.radial.randomRings, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_rad_randRing.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.radial.randomRings = randomRings;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int focusStr = EditorGUILayout.IntSlider (gui_tp_rad_str, shat.radial.focusStr, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_rad_str.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.radial.focusStr = focusStr;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int focus = EditorGUILayout.IntSlider (gui_tp_rad_focus, shat.radial.focus, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_rad_focus.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.radial.focus = focus;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);
            GUILayout.Label ("      Rays", EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int rays = EditorGUILayout.IntSlider (gui_tp_rad_rays, shat.radial.rays, 3, 60);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_rad_rays.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.radial.rays = rays;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int randomRays = EditorGUILayout.IntSlider (gui_tp_rad_randRay, shat.radial.randomRays, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_rad_randRay.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.radial.randomRays = randomRays;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int twist = EditorGUILayout.IntSlider (gui_tp_rad_twist, shat.radial.twist, -90, 90);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_rad_twist.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.radial.twist = twist;
                    SetDirty (scr);
                }
                InteractiveChange();
            }
        }

        /// /////////////////////////////////////////////////////////
        /// HexGrid
        /// /////////////////////////////////////////////////////////
        
        void UI_Type_HexGrid()
        {
            GUILayout.Space (space);
            GUILayout.Label (gui_tp_hex, EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float size = EditorGUILayout.Slider (gui_tp_hex_size, shat.hexagon.size, 0.01f, 10f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_hex_size.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.hexagon.size = size;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool enable = EditorGUILayout.Toggle (gui_tp_cus_en, shat.hexagon.enable);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_cus_en.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.hexagon.enable = enable;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);

            GUILayout.Space (space);
            GUILayout.Label (gui_tp_hex_grd, EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            PlaneType plane = (PlaneType)EditorGUILayout.EnumPopup (gui_tp_slc_pl, shat.hexagon.plane);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_slc_pl.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.hexagon.plane = plane;
                    SetDirty (scr);
                }
                InteractiveChange();
            }
            
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int row = EditorGUILayout.IntSlider (gui_tp_hex_am, shat.hexagon.row, 3, 200);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_hex_am.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.hexagon.row = row;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int col = EditorGUILayout.IntSlider (" ", shat.hexagon.col, 3, 200);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, "Hexagon Col");
                foreach (RayfireShatter scr in targets)
                {
                    scr.hexagon.col = col;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float div = EditorGUILayout.Slider (gui_tp_rad_div, shat.hexagon.div, 0f, shat.hexagon.size);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_rad_div.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.hexagon.div = div;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            if (shat.hexagon.div > 0)
            {
                EditorGUI.BeginChangeCheck();
                bool rest = EditorGUILayout.Toggle (gui_tp_rad_rest, shat.hexagon.rest);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_tp_rad_rest.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.hexagon.rest = rest;
                        SetDirty (scr);
                    }
                    InteractiveChange();
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Custom
        /// /////////////////////////////////////////////////////////
        
        void UI_Type_Custom()
        {
            GUILayout.Space (space);
            GUILayout.Label (gui_tp_cus, EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            RFCustom.RFPointCloudSourceType source = (RFCustom.RFPointCloudSourceType)EditorGUILayout.EnumPopup (gui_tp_cus_src, shat.custom.source);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_cus_src.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.custom.source = source;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            RFCustom.RFPointCloudUseType useAs = (RFCustom.RFPointCloudUseType)EditorGUILayout.EnumPopup (gui_tp_cus_use, shat.custom.useAs);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_cus_use.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.custom.useAs = useAs;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            if (shat.custom.source == RFCustom.RFPointCloudSourceType.TransformList)
            {
                GUILayout.Label ("      List", EditorStyles.boldLabel);

                serializedObject.Update();
                custTmList.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
            }

            if (shat.custom.source == RFCustom.RFPointCloudSourceType.Vector3List)
            {
                GUILayout.Label ("      List", EditorStyles.boldLabel);

                serializedObject.Update();
                custPointList.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
            }

            GUILayout.Space (space);

            if (shat.custom.useAs == RFCustom.RFPointCloudUseType.VolumePoints)
            {
                GUILayout.Label ("      Volume", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();
                int amount = EditorGUILayout.IntSlider (gui_tp_cus_am, shat.custom.amount, 3, 999);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_tp_cus_am.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.custom.amount = amount;
                        SetDirty (scr);
                    }
                    InteractiveChange();
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float radius = EditorGUILayout.Slider (gui_tp_cus_rad, shat.custom.radius, 0.01f, 4f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_tp_cus_rad.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.custom.radius = radius;
                        SetDirty (scr);
                    }
                    InteractiveChange();
                }

                if (shat.custom.inBoundPoints.Count > 0)
                {
                    GUILayout.Space (space);
                    GUILayout.Label ("    In/Out points: " + shat.custom.inBoundPoints.Count + "/" + shat.custom.outBoundPoints.Count);
                }
            }

            GUILayout.Label ("      Preview", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            bool enable = EditorGUILayout.Toggle (gui_tp_cus_en, shat.custom.enable);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_cus_en.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.custom.enable = enable;
                    SetDirty (scr);
                }
            }

            if (shat.custom.enable == true)
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float size = EditorGUILayout.Slider (gui_tp_cus_sz, shat.custom.size, 0.01f, 0.4f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_tp_cus_sz.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.custom.size = size;
                        SetDirty (scr);
                    }
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Slices
        /// /////////////////////////////////////////////////////////
        
        void UI_Type_Slices()
        {
            GUILayout.Space (space);
            GUILayout.Label (gui_tp_slc, EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            PlaneType plane = (PlaneType)EditorGUILayout.EnumPopup (gui_tp_slc_pl, shat.slice.plane);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_slc_pl.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.slice.plane = plane;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            serializedObject.Update();
            sliceTmList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        /// /////////////////////////////////////////////////////////
        /// Bricks
        /// /////////////////////////////////////////////////////////
        
        void UI_Type_Bricks()
        {
            GUILayout.Space (space);
            GUILayout.Label (gui_tp_brk, EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            RFBricks.RFBrickType amountType = (RFBricks.RFBrickType)EditorGUILayout.EnumPopup (gui_tp_brk_type, shat.bricks.amountType);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_type.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.amountType = amountType;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float mult = EditorGUILayout.Slider (gui_tp_brk_mult, shat.bricks.mult, 0.1f, 10);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_mult.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.mult = mult;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            if (shat.bricks.amountType == RFBricks.RFBrickType.ByAmount)
                UI_Type_Bricks_Amount();
            else
                UI_Type_Bricks_Size();

            GUILayout.Space (space);

            UI_Type_Bricks_Size_Variation();

            GUILayout.Space (space);

            UI_Type_Bricks_Offset();

            GUILayout.Space (space);

            UI_Type_Bricks_Split();
        }

        void UI_Type_Bricks_Amount()
        {
            GUILayout.Space (space);
            GUILayout.Label ("      Amount", EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int amount_X = EditorGUILayout.IntSlider (gui_tp_brk_am_X, shat.bricks.amount_X, 0, 50);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_am_X.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.amount_X = amount_X;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int amount_Y = EditorGUILayout.IntSlider (gui_tp_brk_am_Y, shat.bricks.amount_Y, 0, 50);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_am_Y.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.amount_Y = amount_Y;
                    SetDirty (scr);
                }
                InteractiveChange();
            }
            
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int amount_Z = EditorGUILayout.IntSlider (gui_tp_brk_am_Z, shat.bricks.amount_Z, 0, 50);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_am_Z.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.amount_Z = amount_Z;
                    SetDirty (scr);
                }
                InteractiveChange();
            }
        }

        void UI_Type_Bricks_Size()
        {
            GUILayout.Space (space);
            GUILayout.Label ("      Size", EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float size_X = EditorGUILayout.Slider (gui_tp_brk_am_X, shat.bricks.size_X, 0.01f, 10);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_am_X.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.size_X = size_X;
                    if (shat.bricks.size_Lock == true)
                    {
                        scr.bricks.size_Z = size_X;
                        scr.bricks.size_Y = size_X;
                    }

                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float size_Y = EditorGUILayout.Slider (gui_tp_brk_am_Y, shat.bricks.size_Y, 0.01f, 10);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_am_Y.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.size_Y = size_Y;
                    if (shat.bricks.size_Lock == true)
                    {
                        scr.bricks.size_X = size_Y;
                        scr.bricks.size_Z = size_Y;
                    }
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float size_Z = EditorGUILayout.Slider (gui_tp_brk_am_Z, shat.bricks.size_Z, 0.01f, 10);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_am_Z.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.size_Z = size_Z;
                    if (shat.bricks.size_Lock == true)
                    {
                        scr.bricks.size_X = size_Z;
                        scr.bricks.size_Y = size_Z;
                    }
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool size_Lock = EditorGUILayout.Toggle (gui_tp_brk_lock, shat.bricks.size_Lock);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_tp_brk_lock.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.size_Lock = size_Lock;
                    SetDirty (scr);
                }
                InteractiveChange();
            }
        }

        void UI_Type_Bricks_Size_Variation()
        {
            GUILayout.Space (space);
            GUILayout.Label ("      Size Variation", EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int sizeVar_X = EditorGUILayout.IntSlider (gui_tp_brk_am_X, shat.bricks.sizeVar_X, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_am_X.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.sizeVar_X = sizeVar_X;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int sizeVar_Y = EditorGUILayout.IntSlider (gui_tp_brk_am_Y, shat.bricks.sizeVar_Y, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_am_Y.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.sizeVar_Y = sizeVar_Y;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int sizeVar_Z = EditorGUILayout.IntSlider (gui_tp_brk_am_Z, shat.bricks.sizeVar_Z, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_am_Z.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.sizeVar_Z = sizeVar_Z;
                    SetDirty (scr);
                }
                InteractiveChange();
            }
        }

        void UI_Type_Bricks_Offset()
        {
            GUILayout.Space (space);
            GUILayout.Label ("      Offset", EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float offset_X = EditorGUILayout.Slider (gui_tp_brk_am_X, shat.bricks.offset_X, 0f, 1f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_am_X.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.offset_X = offset_X;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float offset_Y = EditorGUILayout.Slider (gui_tp_brk_am_Y, shat.bricks.offset_Y, 0f, 1f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_am_Y.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.offset_Y = offset_Y;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float offset_Z = EditorGUILayout.Slider (gui_tp_brk_am_Z, shat.bricks.offset_Z, 0f, 1f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_am_Z.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.offset_Z = offset_Z;
                    SetDirty (scr);
                }
                InteractiveChange();
            }
        }

        void UI_Type_Bricks_Split()
        {
            GUILayout.Space (space);
            GUILayout.Label ("      Split", EditorStyles.boldLabel);
            GUILayout.Space (space);

            UI_Type_Bricks_Split_Axes();

            EditorGUI.BeginChangeCheck();
            int split_probability = EditorGUILayout.IntSlider (gui_tp_brk_sp_prob, shat.bricks.split_probability, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_sp_prob.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.split_probability = split_probability;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int split_rotation = EditorGUILayout.IntSlider (gui_tp_brk_sp_rot, shat.bricks.split_rotation, 0, 90);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_sp_rot.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.split_rotation = split_rotation;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float split_offset = EditorGUILayout.Slider (gui_tp_brk_sp_offs, shat.bricks.split_offset, 0f, 0.95f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_brk_sp_offs.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.split_offset = split_offset;
                    SetDirty (scr);
                }
                InteractiveChange();
            }
        }

        void UI_Type_Bricks_Split_Axes()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space (space);
            EditorGUILayout.PrefixLabel ("X Y Z");
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool split_X = EditorGUILayout.Toggle ("", shat.bricks.split_X, GUILayout.Width (40));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, "Split X");
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.split_X = split_X;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool split_Y = EditorGUILayout.Toggle ("", shat.bricks.split_Y, GUILayout.Width (40));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, "Split Y");
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.split_Y = split_Y;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool split_Z = EditorGUILayout.Toggle ("", shat.bricks.split_Z, GUILayout.Width (40));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, "Split Z");
                foreach (RayfireShatter scr in targets)
                {
                    scr.bricks.split_Z = split_Z;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            EditorGUILayout.EndHorizontal();
        }

        /// /////////////////////////////////////////////////////////
        /// Voxels
        /// /////////////////////////////////////////////////////////
        
        void UI_Type_Voxels()
        {
            GUILayout.Space (space);
            GUILayout.Label (gui_tp_vxl, EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float size = EditorGUILayout.Slider (gui_tp_cus_sz, shat.voxels.size, 0.05f, 10);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_cus_sz.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.voxels.size = size;
                    SetDirty (scr);
                }
                InteractiveChange();
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Tets
        /// /////////////////////////////////////////////////////////
        
        void UI_Type_Tets()
        {
            GUILayout.Space (space);
            GUILayout.Label (gui_tp_tet, EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int density = EditorGUILayout.IntSlider (gui_tp_tetDn, shat.tets.density, 1, 50);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_tetDn.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.tets.density = density;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int noise = EditorGUILayout.IntSlider (gui_tp_tetNs, shat.tets.noise, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_tp_tetNs.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.tets.noise = noise;
                    SetDirty (scr);
                }
                InteractiveChange();
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Properties
        /// /////////////////////////////////////////////////////////
        
        void UI_Material()
        {
            // Not for decompose
            if (shat.type == FragType.Decompose)
                return;

            GUILayout.Space (space);
            GUILayout.Label ("  Inner Surface", EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float mScl = EditorGUILayout.Slider (gui_mat_scl, shat.material.mScl, 0.01f, 5f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_mat_scl.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.material.mScl = mScl;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            Material iMat = (Material)EditorGUILayout.ObjectField (gui_mat_in, shat.material.iMat, typeof(Material), true);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_mat_in.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.material.iMat = iMat;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool cE = EditorGUILayout.Toggle (gui_mat_col, shat.material.cE);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_mat_col.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.material.cE = cE;
                    SetDirty (scr);
                }
            }

            if (shat.material.cE == true)
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                Color cC = EditorGUILayout.ColorField ("", shat.material.cC);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, "Color");
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.material.cC = cC;
                        SetDirty (scr);
                    }
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool uvE = EditorGUILayout.Toggle (gui_mat_uve, shat.material.uvE);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_mat_uve.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.material.uvE = uvE;
                    SetDirty (scr);
                }
            }

            if (shat.material.uvE == true)
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                Vector2 uvC = EditorGUILayout.Vector2Field ("", shat.material.uvC);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, "UV");
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.material.uvC = uvC;
                        SetDirty (scr);
                    }
                }
            }
        }

        void UI_Cluster()
        {
            // Not for bricks, slices and decompose
            if (shat.type == FragType.Bricks || shat.type == FragType.Decompose || shat.type == FragType.Voxels
                || shat.type == FragType.Slices)
                return;

            GUILayout.Space (space);
            GUILayout.Label (gui_pr_cls, EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool enable = EditorGUILayout.Toggle (gui_pr_cls_en, shat.clusters.enable);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_pr_cls_en.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.clusters.enable = enable;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            if (shat.clusters.enable == true)
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int count = EditorGUILayout.IntSlider (gui_pr_cls_cnt, shat.clusters.count, 2, 200);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_pr_cls_cnt.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.clusters.count = count;
                        SetDirty (scr);
                    }
                    InteractiveChange();
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int seed = EditorGUILayout.IntSlider (gui_pr_cls_seed, shat.clusters.seed, 0, 100);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_pr_cls_seed.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.clusters.seed = seed;
                        SetDirty (scr);
                    }
                    InteractiveChange();
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float relax = EditorGUILayout.Slider (gui_pr_cls_rel, shat.clusters.relax, 0f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_pr_cls_rel.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.clusters.relax = relax;
                        SetDirty (scr);
                    }
                    InteractiveChange();
                }

                GUILayout.Space (space);

                exp_deb = EditorGUILayout.Foldout (exp_deb, gui_pr_cls_debris, true);
                if (exp_deb == true)
                {
                    GUILayout.Space (space);

                    EditorGUI.indentLevel++;

                    EditorGUI.BeginChangeCheck();
                    int amount = EditorGUILayout.IntSlider (gui_pr_cls_amount, shat.clusters.amount, 0, 100);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_pr_cls_amount.text);
                        foreach (RayfireShatter scr in targets)
                        {
                            scr.clusters.amount = amount;
                            SetDirty (scr);
                        }
                        InteractiveChange();
                    }

                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    int layers = EditorGUILayout.IntSlider (gui_pr_cls_layers, shat.clusters.layers, 0, 5);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_pr_cls_layers.text);
                        foreach (RayfireShatter scr in targets)
                        {
                            scr.clusters.layers = layers;
                            SetDirty (scr);
                        }
                        InteractiveChange();
                    }

                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    float scale = EditorGUILayout.Slider (gui_pr_cls_scale, shat.clusters.scale, 0.1f, 1f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_pr_cls_scale.text);
                        foreach (RayfireShatter scr in targets)
                        {
                            scr.clusters.scale = scale;
                            SetDirty (scr);
                        }
                        InteractiveChange();
                    }

                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    int min = EditorGUILayout.IntSlider (gui_pr_cls_min, shat.clusters.min, 1, 20);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_pr_cls_min.text);
                        foreach (RayfireShatter scr in targets)
                        {
                            scr.clusters.min = min;
                            SetDirty (scr);
                        }
                        InteractiveChange();
                    }

                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    int max = EditorGUILayout.IntSlider (gui_pr_cls_max, shat.clusters.max, 1, 20);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_pr_cls_max.text);
                        foreach (RayfireShatter scr in targets)
                        {
                            scr.clusters.max = max;
                            SetDirty (scr);
                        }
                        InteractiveChange();
                    }

                    EditorGUI.indentLevel--;
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Advanced
        /// /////////////////////////////////////////////////////////
        
        void UI_Advanced()
        {
            UI_Advanced_Properties();

            GUILayout.Space (space);

            UI_Advanced_Filters();

            GUILayout.Space (space);

            if (shat.mode == FragmentMode.Editor)
                UI_Advanced_Editor();
        }

        void UI_Advanced_Properties()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Properties", EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            FragmentMode mode = (FragmentMode)EditorGUILayout.EnumPopup (gui_pr_mode, shat.mode);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_pr_mode.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.mode = mode;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int seed = EditorGUILayout.IntSlider (gui_pr_adv_seed, shat.advanced.seed, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_pr_adv_seed.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.advanced.seed = seed;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool copyComponents = EditorGUILayout.Toggle (gui_pr_adv_copy, shat.advanced.copyComponents);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_pr_adv_copy.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.advanced.copyComponents = copyComponents;
                    SetDirty (scr);
                }
            }

            /*
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool bake = EditorGUILayout.Toggle (gui_pr_adv_bake, shat.advanced.bake);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_pr_adv_bake.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.advanced.bake = bake;
                    SetDirty (scr);
                }
            }
            */

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool smooth = EditorGUILayout.Toggle (gui_pr_adv_smooth, shat.advanced.smooth);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_pr_adv_smooth.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.advanced.smooth = smooth;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool combineChildren = EditorGUILayout.Toggle (gui_pr_adv_combine, shat.advanced.combineChildren);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_pr_adv_combine.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.advanced.combineChildren = combineChildren;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool removeCollinear = EditorGUILayout.Toggle (gui_pr_adv_col, shat.advanced.removeCollinear);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_pr_adv_col.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.advanced.removeCollinear = removeCollinear;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool decompose = EditorGUILayout.Toggle (gui_pr_adv_dec, shat.advanced.decompose);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_pr_adv_dec.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.advanced.decompose = decompose;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool inputPrecap = EditorGUILayout.Toggle (gui_pr_adv_input, shat.advanced.inputPrecap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_pr_adv_input.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.advanced.inputPrecap = inputPrecap;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            if (shat.advanced.inputPrecap == true)
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                bool outputPrecap = EditorGUILayout.Toggle (gui_pr_adv_output, shat.advanced.outputPrecap);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_pr_adv_output.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.advanced.outputPrecap = outputPrecap;
                        SetDirty (scr);
                    }
                    InteractiveChange();
                }
            }

            GUILayout.Space (space);

            UI_Advanced_Limits();
        }

        void UI_Advanced_Limits()
        {
            exp_lim = EditorGUILayout.Foldout (exp_lim, "Limitations", true);
            if (exp_lim == true)
            {
                GUILayout.Space (space);

                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                bool sizeLimitation = EditorGUILayout.Toggle (gui_pr_adv_size_lim, shat.advanced.sizeLimitation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_pr_adv_size_lim.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.advanced.sizeLimitation = sizeLimitation;
                        SetDirty (scr);
                    }
                }

                if (shat.advanced.sizeLimitation == true)
                {
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    float sizeAmount = EditorGUILayout.Slider (gui_pr_adv_size_am, shat.advanced.sizeAmount, 0.1f, 100f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_pr_adv_size_am.text);
                        foreach (RayfireShatter scr in targets)
                        {
                            scr.advanced.sizeAmount = sizeAmount;
                            SetDirty (scr);
                        }
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                bool vertexLimitation = EditorGUILayout.Toggle (gui_pr_adv_vert_lim, shat.advanced.vertexLimitation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_pr_adv_vert_lim.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.advanced.vertexLimitation = vertexLimitation;
                        SetDirty (scr);
                    }
                }

                if (shat.advanced.vertexLimitation == true)
                {
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    int vertexAmount = EditorGUILayout.IntSlider (gui_pr_adv_vert_am, shat.advanced.vertexAmount, 100, 1900);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_pr_adv_vert_am.text);
                        foreach (RayfireShatter scr in targets)
                        {
                            scr.advanced.vertexAmount = vertexAmount;
                            SetDirty (scr);
                        }
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                bool triangleLimitation = EditorGUILayout.Toggle (gui_pr_adv_tri_lim, shat.advanced.triangleLimitation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_pr_adv_tri_lim.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.advanced.triangleLimitation = triangleLimitation;
                        SetDirty (scr);
                    }
                }

                if (shat.advanced.triangleLimitation == true)
                {
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    int triangleAmount = EditorGUILayout.IntSlider (gui_pr_adv_tri_am, shat.advanced.triangleAmount, 100, 1900);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_pr_adv_tri_am.text);
                        foreach (RayfireShatter scr in targets)
                        {
                            scr.advanced.triangleAmount = triangleAmount;
                            SetDirty (scr);
                        }
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        void UI_Advanced_Editor()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Editor", EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            int elementSizeThreshold = EditorGUILayout.IntSlider (gui_pr_adv_element, shat.advanced.elementSizeThreshold, 1, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_pr_adv_element.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.advanced.elementSizeThreshold = elementSizeThreshold;
                    SetDirty (scr);
                }
                InteractiveChange();
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool removeDoubleFaces = EditorGUILayout.Toggle (gui_pr_adv_remove, shat.advanced.removeDoubleFaces);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_pr_adv_remove.text);
                foreach (RayfireShatter scr in targets)
                {
                    scr.advanced.removeDoubleFaces = removeDoubleFaces;
                    SetDirty (scr);
                }
                InteractiveChange();
            }
        }

        void UI_Advanced_Filters()
        {
            exp_fil = EditorGUILayout.Foldout (exp_fil, "Filters", true);
            if (exp_fil == true)
            {
                GUILayout.Space (space);

                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                bool inner = EditorGUILayout.Toggle (gui_pr_adv_inner, shat.advanced.inner);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_pr_adv_inner.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.advanced.inner = inner;
                        SetDirty (scr);
                    }
                    InteractiveChange();
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                bool planar = EditorGUILayout.Toggle (gui_pr_adv_planar, shat.advanced.planar);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_pr_adv_planar.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.advanced.planar = planar;
                        SetDirty (scr);
                    }
                    InteractiveChange();
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int relativeSize = EditorGUILayout.IntSlider (gui_pr_adv_rel, shat.advanced.relativeSize, 0, 10);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_pr_adv_rel.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.advanced.relativeSize = relativeSize;
                        SetDirty (scr);
                    }
                    InteractiveChange();
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float absoluteSize = EditorGUILayout.Slider (gui_pr_adv_abs, shat.advanced.absoluteSize, 0, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_pr_adv_abs.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.advanced.absoluteSize = absoluteSize;
                        SetDirty (scr);
                    }
                    InteractiveChange();
                }

                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Fragment
        /// /////////////////////////////////////////////////////////
        
        void UI_Fragment()
        {
            if (GUILayout.Button ("Fragment", GUILayout.Height (25)))
            {
                foreach (var targ in targets)
                {
                    if (targ as RayfireShatter != null)
                    {
                        if ((targ as RayfireShatter).interactive == false)
                            (targ as RayfireShatter).Fragment();
                        else
                            (targ as RayfireShatter).InteractiveFragment();

                        // TODO APPLY LOCAL SHATTER PREVIEW PROPS TO ALL SELECTED
                    }
                }
                // Scale preview if preview turn on
                if (shat.previewScale > 0 && shat.scalePreview == true)
                    ScalePreview (shat);
            }

            GUILayout.Space (1);

            GUILayout.BeginHorizontal();

            // Delete last
            if (shat.fragmentsLast.Count > 0) // TODO SUPPORT MASS CHECK
            {
                if (GUILayout.Button ("Fragment to Last", GUILayout.Height (22)))
                {
                    foreach (var targ in targets)
                        if (targ as RayfireShatter != null)
                        {
                            (targ as RayfireShatter).DeleteFragmentsLast (1);
                            (targ as RayfireShatter).resetState = true;
                            (targ as RayfireShatter).Fragment (RayfireShatter.FragLastMode.ToLast);

                            // Scale preview if preview turn on
                            if ((targ as RayfireShatter).previewScale > 0 && (targ as RayfireShatter).scalePreview == true)
                                ScalePreview (targ as RayfireShatter);
                        }
                }

                if (GUILayout.Button ("    Delete Last    ", GUILayout.Height (22)))
                {
                    foreach (var targ in targets)
                        if (targ as RayfireShatter != null)
                        {
                            (targ as RayfireShatter).DeleteFragmentsLast();
                            (targ as RayfireShatter).resetState = true;
                            (targ as RayfireShatter).ResetScale (0f);
                        }
                }
            }

            // Delete all fragments
            if (shat.fragmentsAll.Count > 0 && shat.fragmentsAll.Count > shat.fragmentsLast.Count)
            {
                if (GUILayout.Button (" Delete All ", GUILayout.Height (22)))
                {
                    foreach (var targ in targets)
                        if (targ as RayfireShatter != null)
                        {
                            (targ as RayfireShatter).DeleteFragmentsAll();
                            (targ as RayfireShatter).resetState = true;
                            (targ as RayfireShatter).ResetScale (0f);
                        }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        void UI_Interactive()
        {
            EditorGUI.BeginChangeCheck();
            shat.interactive = GUILayout.Toggle (shat.interactive, gui_int, "Button", GUILayout.Height (25));
            if (EditorGUI.EndChangeCheck() == true)
            {
                // Toggle mode
                if (shat.interactive == true)
                {
                    shat.InteractiveStart();
                }
                else
                {
                    shat.InteractiveStop();
                }

                SetDirty (shat);
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Info
        /// /////////////////////////////////////////////////////////
        
        void InfoUI()
        {
            if (shat.fragmentsLast.Count > 0 || shat.fragmentsAll.Count > 0)
            {
                GUILayout.Space (3);
                GUILayout.Label ("  Info", EditorStyles.boldLabel);
                GUILayout.Space (3);

                GUILayout.BeginHorizontal();

                GUILayout.Label ("Roots: " + shat.rootChildList.Count);
                GUILayout.Label ("Last Fragments: " + shat.fragmentsLast.Count);
                GUILayout.Label ("Total Fragments: " + shat.fragmentsAll.Count);

                EditorGUILayout.EndHorizontal();
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Export
        /// /////////////////////////////////////////////////////////
        
        void UI_Export()
        {
            if (CanExport() == true)
            {
                GUILayout.Space (3);
                GUILayout.Label (gui_pr_exp, EditorStyles.boldLabel);
                GUILayout.Space (3);

                EditorGUI.BeginChangeCheck();
                RFMeshExport.MeshExportType source = (RFMeshExport.MeshExportType)EditorGUILayout.EnumPopup (gui_pr_exp_src, shat.export.source);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_pr_exp_src.text);
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.export.source = source;
                        SetDirty (scr);
                    }
                }

                if (HasToExport() == true)
                {
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    string suffix = EditorGUILayout.TextField (gui_pr_exp_sfx, shat.export.suffix);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_pr_exp_sfx.text);
                        foreach (RayfireShatter scr in targets)
                        {
                            scr.export.suffix = suffix;
                            SetDirty (scr);
                        }
                    }
                }

                GUILayout.Space (space);

                // Export Last fragments
                if (shat.export.source == RFMeshExport.MeshExportType.LastFragments && shat.fragmentsLast.Count > 0)
                    if (GUILayout.Button ("Export Last Fragments", GUILayout.Height (25)))
                        RFMeshAsset.SaveFragments (shat, null);

                // Export children
                if (shat.export.source == RFMeshExport.MeshExportType.Children && shat.transform.childCount > 0)
                    if (GUILayout.Button ("Export Children", GUILayout.Height (25)))
                        RFMeshAsset.SaveFragments (shat, null);
            }
        }

        bool CanExport()
        {
            if (shat.fragmentsLast.Count > 0 || shat.transform.childCount > 0)
                return true;
            return false;
        }

        bool HasToExport()
        {
            if (shat.export.source == RFMeshExport.MeshExportType.LastFragments && shat.fragmentsLast.Count > 0)
                return true;
            if (shat.export.source == RFMeshExport.MeshExportType.Children && shat.transform.childCount > 0)
                return true;
            return false;
        }

        /// /////////////////////////////////////////////////////////
        /// Center
        /// /////////////////////////////////////////////////////////
        
        void UI_Center()
        {
            if ((int)shat.type <= 5)
            {
                GUILayout.Space (3);
                GUILayout.Label ("  Center", EditorStyles.boldLabel);
                GUILayout.Space (3);

                GUILayout.BeginHorizontal();
                
                EditorGUI.BeginChangeCheck();
                bool showCenter = GUILayout.Toggle (shat.showCenter, " Show   ", "Button");
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, "Show Center");
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.showCenter = showCenter;
                        SetDirty (scr);
                    }
                    SceneView.RepaintAll();
                }
                
                if (GUILayout.Button (gui_cn_res))
                {
                    foreach (var targ in targets)
                    {
                        if (targ as RayfireShatter != null)
                        {
                            (targ as RayfireShatter).ResetCenter();
                            SetDirty (targ as RayfireShatter);
                        }
                    }
                    InteractiveChange();
                    SceneView.RepaintAll();
                }

                EditorGUILayout.EndHorizontal();

                GUILayout.Space (3);
                
                EditorGUI.BeginChangeCheck();
                shat.centerPosition = EditorGUILayout.Vector3Field (gui_cn_pos, shat.centerPosition);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (RayfireShatter scr in targets)
                    {
                        scr.centerPosition = shat.centerPosition;
                        SetDirty (scr);
                    }

                    InteractiveChange();
                    SceneView.RepaintAll();
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Preview
        /// /////////////////////////////////////////////////////////
        
        void UI_Preview()
        {
            // Preview
            if (shat.fragmentsLast.Count == 0 && shat.interactive == false)
                return;

            GUILayout.Space (3);
            GUILayout.Label ("  Preview", EditorStyles.boldLabel);
            GUILayout.Space (3);

            GUILayout.BeginHorizontal();

            // Start check for scale toggle change
            EditorGUI.BeginChangeCheck();
            shat.scalePreview = GUILayout.Toggle (shat.scalePreview, "Scale", "Button");
            if (EditorGUI.EndChangeCheck() == true)
            {
                if (shat.scalePreview == true)
                    ScalePreview (shat);
                else
                {
                    shat.resetState = true;
                    shat.ResetScale (0f);
                }

                SetDirty (shat);
                InteractiveChange();
            }
            
            // Color preview toggle
            if (shat.fragmentsLast.Count > 0)
                shat.colorPreview = GUILayout.Toggle (shat.colorPreview, "Color", "Button");

            EditorGUILayout.EndHorizontal();

            GUILayout.Space (3);

            GUILayout.BeginHorizontal();

            GUILayout.Label ("Scale Preview", GUILayout.Width (90));

            // Start check for slider change
            EditorGUI.BeginChangeCheck();
            shat.previewScale = GUILayout.HorizontalSlider (shat.previewScale, 0f, 0.99f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                if (shat.scalePreview == true)
                    ScalePreview (shat);
                SetDirty (shat);
                InteractiveChange(); // TODO only change scale, do not refrag. LIB update
            }

            EditorGUILayout.EndHorizontal();
        }

        static void ColorPreview (RayfireShatter scr)
        {
            if (scr.fragmentsLast.Count > 0)
            {
                Random.InitState (1);
                foreach (Transform root in scr.rootChildList)
                {
                    if (root != null)
                    {
                        MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>();
                        foreach (var mf in meshFilters)
                        {
                            Gizmos.color = new Color (Random.Range (0.2f, 0.8f), Random.Range (0.2f, 0.8f), Random.Range (0.2f, 0.8f));
                            Gizmos.DrawMesh (mf.sharedMesh, mf.transform.position, mf.transform.rotation, mf.transform.lossyScale * 1.01f);
                        }
                    }
                }
            }
        }

        static void ScalePreview (RayfireShatter scr)
        {
            if (scr.fragmentsLast.Count > 0 && scr.previewScale > 0f)
            {
                // Do not scale
                if (scr.skinnedMeshRend != null)
                    scr.skinnedMeshRend.enabled = false;
                if (scr.meshRenderer != null)
                    scr.meshRenderer.enabled = false;

                foreach (GameObject fragment in scr.fragmentsLast)
                    if (fragment != null)
                        fragment.transform.localScale = Vector3.one * scr.PreviewScale();
                scr.resetState = true;
            }

            if (scr.previewScale == 0f)
                scr.ResetScale (0f);
        }

        /// /////////////////////////////////////////////////////////
        /// Colliders
        /// /////////////////////////////////////////////////////////
        
        void UI_Collider()
        {
            if (shat.fragmentsLast.Count == 0)
                return;

            GUILayout.Space (3);
            GUILayout.Label ("  Colliders", EditorStyles.boldLabel);
            GUILayout.Space (3);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button ("Add Mesh Colliders"))
            {
                foreach (var targ in targets)
                    if (targ as RayfireShatter != null)
                        AddColliders (targ as RayfireShatter);
                SceneView.RepaintAll();
            }

            if (GUILayout.Button (" Remove Colliders "))
            {
                foreach (var targ in targets)
                    if (targ as RayfireShatter != null)
                        RemoveColliders (targ as RayfireShatter);
                SceneView.RepaintAll();
            }

            EditorGUILayout.EndHorizontal();
        }

        static void AddColliders (RayfireShatter scr)
        {
            if (scr.fragmentsLast.Count > 0)
                foreach (var frag in scr.fragmentsLast)
                {
                    MeshCollider mc = frag.GetComponent<MeshCollider>();
                    if (mc == null)
                        mc = frag.AddComponent<MeshCollider>();
                    mc.convex = true;
                }
        }

        static void RemoveColliders (RayfireShatter scr)
        {
            if (scr.fragmentsLast.Count > 0)
                foreach (var frag in scr.fragmentsLast)
                {
                    MeshCollider mc = frag.gameObject.GetComponent<MeshCollider>();
                    if (mc != null)
                        DestroyImmediate (mc);
                }
        }

        /// /////////////////////////////////////////////////////////
        /// Draw
        /// /////////////////////////////////////////////////////////
        [DrawGizmo (GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        static void DrawGizmosSelected (RayfireShatter shatter, GizmoType gizmoType)
        {
            // Color preview
            if (shatter.colorPreview == true)
                ColorPreview (shatter);

            // HexGrid cloud preview
            if (shatter.type == FragType.Hexagon && shatter.hexagon.enable == true)
            {
                // Get bounds for preview
                Bounds bound = shatter.GetBound();
                if (bound.size.magnitude > 0)
                {
                    // Center position from local to global
                    Vector3 centerPos = shatter.transform.TransformPoint (shatter.centerPosition);

                    // Collect point cloud and draw
                    RFHexagon.GetHexPointCLoud (shatter.hexagon, shatter.transform, centerPos, shatter.centerDirection, shatter.advanced.seed, bound);
                    DrawSpheres (shatter.hexagon.pcBndIn, shatter.hexagon.pcBndOut, shatter.hexagon.size / 4f);
                }
            }

            // Custom point cloud preview
            if (shatter.type == FragType.Custom && shatter.custom.enable == true)
            {
                // Get bounds for preview
                Bounds bound = shatter.GetBound();
                if (bound.size.magnitude > 0)
                {
                    // Collect point cloud and draw
                    RFCustom.GetCustomPointCLoud (shatter.custom, shatter.transform, shatter.advanced.seed, bound);
                    DrawSpheres (shatter.custom.inBoundPoints, shatter.custom.outBoundPoints, shatter.custom.size);
                }
            }
        }

        // Draw In/Out points
        static void DrawSpheres(List<Vector3> inBoundPoints, List<Vector3> outBoundPoints, float size)
        {
            if (inBoundPoints != null && inBoundPoints.Count > 0)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < inBoundPoints.Count; i++)
                    Gizmos.DrawSphere (inBoundPoints[i], size);
            }
            if (outBoundPoints != null && outBoundPoints.Count > 0)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < outBoundPoints.Count; i++)
                    Gizmos.DrawSphere (outBoundPoints[i], size / 2f);
            }
        }

        // Show center move handle
        private void OnSceneGUI()
        {
            // Get shatter
            shat = target as RayfireShatter;
            if (shat == null)
                return;

            transForm       = shat.transform;
            centerWorldPos  = transForm.TransformPoint (shat.centerPosition);
            centerWorldQuat = transForm.rotation * shat.centerDirection;

            // Point3 handle
            if (shat.showCenter == true)
            {
                EditorGUI.BeginChangeCheck();
                centerWorldPos = Handles.PositionHandle (centerWorldPos, centerWorldQuat.RFNormalize());
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObject (shat, "Center Move");
                    InteractiveChange();
                    SetDirty (shat);
                }

                EditorGUI.BeginChangeCheck();
                centerWorldQuat = Handles.RotationHandle (centerWorldQuat, centerWorldPos);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObject (shat, "Center Rotate");
                    InteractiveChange();
                    SetDirty (shat);
                }
            }

            shat.centerDirection = Quaternion.Inverse (transForm.rotation) * centerWorldQuat;
            shat.centerPosition  = transForm.InverseTransformPoint (centerWorldPos);
        }

        /// /////////////////////////////////////////////////////////
        /// ReorderableList Custom Transform
        /// /////////////////////////////////////////////////////////
        
        void DrawCustTmListItems (Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = custTmList.serializedProperty.GetArrayElementAtIndex (index);
            EditorGUI.PropertyField (new Rect (rect.x, rect.y + 2, EditorGUIUtility.currentViewWidth - 80f, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }

        void DrawCustTmHeader (Rect rect)
        {
            rect.x += 10;
            EditorGUI.LabelField (rect, "Transform List");
        }

        void AddCustTm (ReorderableList list)
        {
            if (shat.custom.transforms == null)
                shat.custom.transforms = new List<Transform>();
            shat.custom.transforms.Add (null);
            list.index = list.count;
            InteractiveChange();
        }

        void RemoveCustTm (ReorderableList list)
        {
            if (shat.custom.transforms != null)
            {
                shat.custom.transforms.RemoveAt (list.index);
                list.index = list.index - 1;
                InteractiveChange();
            }
        }

        /// /////////////////////////////////////////////////////////
        /// ReorderableList Custom Point 3
        /// /////////////////////////////////////////////////////////
        void DrawCustPointListItems (Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = custPointList.serializedProperty.GetArrayElementAtIndex (index);
            EditorGUI.PropertyField (new Rect (rect.x, rect.y + 2, EditorGUIUtility.currentViewWidth - 80f, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }

        void DrawCustPointHeader (Rect rect)
        {
            rect.x += 10;
            EditorGUI.LabelField (rect, "Vector3 List");
        }

        void AddCustPoint (ReorderableList list)
        {
            if (shat.custom.vector3 == null)
                shat.custom.vector3 = new List<Vector3>();
            shat.custom.vector3.Add (Vector3.zero);
            list.index = list.count;
            InteractiveChange();
        }

        void RemoveCustPoint (ReorderableList list)
        {
            if (shat.custom.vector3 != null)
            {
                shat.custom.vector3.RemoveAt (list.index);
                list.index = list.index - 1;
                InteractiveChange();
            }
        }

        /// /////////////////////////////////////////////////////////
        /// ReorderableList Slice Transform
        /// /////////////////////////////////////////////////////////
        void DrawSliceTmListItems (Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = sliceTmList.serializedProperty.GetArrayElementAtIndex (index);
            EditorGUI.PropertyField (new Rect (rect.x, rect.y + 2, EditorGUIUtility.currentViewWidth - 80f, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }

        void DrawSliceTmHeader (Rect rect)
        {
            rect.x += 10;
            EditorGUI.LabelField (rect, "Transform List");
        }

        void AddSliceTm (ReorderableList list)
        {
            if (shat.slice.sliceList == null)
                shat.slice.sliceList = new List<Transform>();
            shat.slice.sliceList.Add (null);
            list.index = list.count;
            InteractiveChange();
        }

        void RemoveSliceTm (ReorderableList list)
        {
            if (shat.slice.sliceList != null)
            {
                shat.slice.sliceList.RemoveAt (list.index);
                list.index = list.index - 1;
                InteractiveChange();
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////

        // Property change
        void InteractiveChange()
        {
            if (shat != null && shat.interactive == true)
                shat.InteractiveChange();
        }
        
        // Set dirty
        void SetDirty (RayfireShatter scr)
        {
            if (Application.isPlaying == false)
            {
                EditorUtility.SetDirty (scr);
                EditorSceneManager.MarkSceneDirty (scr.gameObject.scene);
                SceneView.RepaintAll();
            }
        }
    }

    // Normalize quat in order to support Unity 2018.1
    public static class RFQuaternionExtension
    {
        public static Quaternion RFNormalize (this Quaternion q)
        {
            float f = 1f / Mathf.Sqrt (q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            return new Quaternion (q.x * f, q.y * f, q.z * f, q.w * f);
        }
    }
}

/*
public class ExampleClass: EditorWindow
{
    GameObject gameObject;
    Editor     gameObjectEditor;

    [MenuItem("Example/GameObject Editor")]
    static void ShowWindow()
    {
        GetWindowWithRect<ExampleClass>(new Rect(0, 0, 256, 256));
    }

    void OnGUI()
    {
        gameObject = (GameObject) EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

        GUIStyle bgColor = new GUIStyle();
        bgColor.normal.background = EditorGUIUtility.whiteTexture;

        if (gameObject != null)
        {
            if (gameObjectEditor == null)
                gameObjectEditor = Editor.CreateEditor(gameObject);

            gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), bgColor);
        }
    }
}


[CustomPreview(typeof(GameObject))]
public class MyPreview : ObjectPreview
{
    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        GUI.Label(r, target.name + " is being previewed");
    }
}
*/