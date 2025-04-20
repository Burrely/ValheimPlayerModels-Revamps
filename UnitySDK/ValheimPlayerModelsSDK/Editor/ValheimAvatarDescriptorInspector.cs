﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ValheimPlayerModels;
using UnityEditor.SceneManagement;
using System.IO;
using NUnit.Framework.Api;
using UnityEditorInternal;
using System.ComponentModel;
using UnityEditor.Animations;
using System.Globalization;

[CustomEditor(typeof(ValheimAvatarDescriptor))]
public class ValheimAvatarDescriptorInspector : Editor
{
    
    private SerializedProperty avatarName;

    private SerializedProperty leftHand;
    private SerializedProperty rightHand;
    private SerializedProperty helmet;
    private SerializedProperty backShield;
    private SerializedProperty backMelee;
    private SerializedProperty backTwohandedMelee;
    private SerializedProperty backBow;
    private SerializedProperty backTool;
    private SerializedProperty backAtgeir;

    private SerializedProperty showHelmet;
    private SerializedProperty showCape;

    private SerializedProperty animatorParameters;

    private SerializedProperty actionMenuItems;

    private SerializedProperty controlName;
    private SerializedProperty controlTypes;
    private SerializedProperty controlParameterNames;
    private SerializedProperty controlValues;
    
    [SerializeField]
    private ReorderableList animParamList;

    [SerializeField]
    private ReorderableList actionMenuItemsList;


    // [SerializeField]
    // private ReorderableList intParamList;
    // [SerializeField]
    // private ReorderableList floatParamList;

    private void OnEnable()
    {
        avatarName = serializedObject.FindProperty("avatarName");

        leftHand = serializedObject.FindProperty("leftHand");
        rightHand = serializedObject.FindProperty("rightHand");
        helmet = serializedObject.FindProperty("helmet");
        backShield = serializedObject.FindProperty("backShield");
        backMelee = serializedObject.FindProperty("backMelee");
        backTwohandedMelee = serializedObject.FindProperty("backTwohandedMelee");
        backBow = serializedObject.FindProperty("backBow");
        backTool = serializedObject.FindProperty("backTool");
        backAtgeir = serializedObject.FindProperty("backAtgeir");

        showHelmet = serializedObject.FindProperty("showHelmet");
        showCape = serializedObject.FindProperty("showCape");

        animatorParameters = serializedObject.FindProperty("animatorParameters");
        actionMenuItems = serializedObject.FindProperty("actionMenuItems");

        controlName = serializedObject.FindProperty("controlName");
        controlTypes = serializedObject.FindProperty("controlTypes");
        controlParameterNames = serializedObject.FindProperty("controlParameterNames");
        controlValues = serializedObject.FindProperty("controlValues");
        
        animParamList = new ReorderableList(serializedObject, animatorParameters, true, true, true, true);
        actionMenuItemsList = new ReorderableList(serializedObject, actionMenuItems, true, true, true, true);
        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.HelpBox("It's recommended to use the \"Standard\" shader\non the avatar for best results.", MessageType.Info);

        ValheimAvatarDescriptor descriptor = (ValheimAvatarDescriptor)target;
        Animator animator = descriptor.GetComponent<Animator>();

        bool valid = true;
        valid = valid && !string.IsNullOrEmpty(descriptor.avatarName);
        valid = valid && descriptor.leftHand != null;
        valid = valid && descriptor.rightHand != null;
        valid = valid && descriptor.helmet != null;
        valid = valid && descriptor.backShield != null;
        valid = valid && descriptor.backMelee != null;
        valid = valid && descriptor.backTwohandedMelee != null;
        valid = valid && descriptor.backBow != null;
        valid = valid && descriptor.backTool != null;
        valid = valid && descriptor.backAtgeir != null;
        valid = valid && animator != null;

        GUI.enabled = false;

        if (animator && !animator.avatar.isHuman)
        {
            EditorGUILayout.HelpBox("Avatar is not Humanoid, no animation will play if kept this way.", MessageType.Warning);
        }

        if (animator && animator.avatar.isHuman) GUI.enabled = true;
        if (GUILayout.Button("Auto-Setup"))
        {
            Undo.SetCurrentGroupName("Auto-created avatar objects");

            animator.transform.localScale = Vector3.one;
            float headHeight = animator.GetBoneTransform(HumanBodyBones.Head).position.y - animator.transform.position.y;
            float pmScale = 1.669556f / headHeight;
            animator.transform.localScale = new Vector3(pmScale, pmScale, pmScale);

            if (!descriptor.leftHand)
            {
                descriptor.leftHand = new GameObject("LeftHand_Attach").transform;
                descriptor.leftHand.SetParent(animator.GetBoneTransform(HumanBodyBones.LeftHand), false);
                descriptor.leftHand.localEulerAngles = new Vector3(0, 0, -180);
                descriptor.leftHand.gameObject.AddComponent<ForceSelection>();
                AddPreview(descriptor.leftHand,"BowPreview");
                Undo.RegisterCreatedObjectUndo(descriptor.leftHand.gameObject, "added attach point");
            }

            if (!descriptor.rightHand)
            {
                descriptor.rightHand = new GameObject("RightHand_Attach").transform;
                descriptor.rightHand.SetParent(animator.GetBoneTransform(HumanBodyBones.RightHand), false);
                descriptor.rightHand.localEulerAngles = new Vector3(0, 0, 0);
                descriptor.rightHand.gameObject.AddComponent<ForceSelection>();
                AddPreview(descriptor.rightHand, "OneHandedPreview");
                Undo.RegisterCreatedObjectUndo(descriptor.rightHand.gameObject, "added attach point");
            }

            if (!descriptor.helmet)
            {
                descriptor.helmet = new GameObject("Helmet_attach").transform;
                descriptor.helmet.SetParent(animator.GetBoneTransform(HumanBodyBones.Head), false);
                descriptor.helmet.gameObject.AddComponent<ForceSelection>();
                AddPreview(descriptor.helmet, "HelmetPreview");
                Undo.RegisterCreatedObjectUndo(descriptor.helmet.gameObject, "added attach point");
            }

            if (!descriptor.backShield)
            {
                descriptor.backShield = new GameObject("BackShield_attach").transform;
                descriptor.backShield.SetParent(animator.GetBoneTransform(HumanBodyBones.Chest), false);
                descriptor.backShield.localEulerAngles = new Vector3(260, -75, -130);
                descriptor.backShield.gameObject.AddComponent<ForceSelection>();
                AddPreview(descriptor.backShield, "ShieldPreview");
                Undo.RegisterCreatedObjectUndo(descriptor.backShield.gameObject, "added attach point");
            }

            if (!descriptor.backMelee)
            {
                descriptor.backMelee = new GameObject("BackOneHanded_attach").transform;
                descriptor.backMelee.SetParent(animator.GetBoneTransform(HumanBodyBones.Chest), false);
                descriptor.backMelee.localEulerAngles = new Vector3(120, 90, 95);
                descriptor.backMelee.gameObject.AddComponent<ForceSelection>();
                AddPreview(descriptor.backMelee, "OneHandedPreview");
                Undo.RegisterCreatedObjectUndo(descriptor.backMelee.gameObject, "added attach point");
            }

            if (!descriptor.backTwohandedMelee)
            {
                descriptor.backTwohandedMelee = new GameObject("BackTwohanded_attach").transform;
                descriptor.backTwohandedMelee.SetParent(animator.GetBoneTransform(HumanBodyBones.Chest), false);
                descriptor.backTwohandedMelee.localEulerAngles = new Vector3(125, 90, 100);
                descriptor.backTwohandedMelee.gameObject.AddComponent<ForceSelection>();
                AddPreview(descriptor.backTwohandedMelee, "TwoHandedPreview");
                Undo.RegisterCreatedObjectUndo(descriptor.backTwohandedMelee.gameObject, "added attach point");
            }

            if (!descriptor.backBow)
            {
                descriptor.backBow = new GameObject("BackBow_attach").transform;
                descriptor.backBow.SetParent(animator.GetBoneTransform(HumanBodyBones.Chest), false);
                descriptor.backBow.localEulerAngles = new Vector3(-115, -45, 136);
                descriptor.backBow.gameObject.AddComponent<ForceSelection>();
                AddPreview(descriptor.backBow, "BowPreview");
                Undo.RegisterCreatedObjectUndo(descriptor.backBow.gameObject, "added attach point");
            }

            if (!descriptor.backTool)
            {
                descriptor.backTool = new GameObject("BackTool_attach").transform;
                descriptor.backTool.SetParent(animator.GetBoneTransform(HumanBodyBones.Hips), false);
                descriptor.backTool.localEulerAngles = new Vector3(100, -90, -180);
                descriptor.backTool.gameObject.AddComponent<ForceSelection>();
                AddPreview(descriptor.backTool, "ToolPreview");
                Undo.RegisterCreatedObjectUndo(descriptor.backTool.gameObject, "added attach point");
            }

            if (!descriptor.backAtgeir)
            {
                descriptor.backAtgeir = new GameObject("BackAtgeir_attach").transform;
                descriptor.backAtgeir.SetParent(animator.GetBoneTransform(HumanBodyBones.Chest), false);
                descriptor.backAtgeir.localEulerAngles = new Vector3(-75, 45, -225);
                descriptor.backAtgeir.gameObject.AddComponent<ForceSelection>();
                Undo.RegisterCreatedObjectUndo(descriptor.backAtgeir.gameObject, "added attach point");
            }
            Undo.IncrementCurrentGroup();
        }

        GUI.enabled = valid;

        if (GUILayout.Button("Export"))
        {
            string path = EditorUtility.SaveFilePanel("Save object file", "", descriptor.avatarName + ".valavtr", "valavtr");

            if (path != "")
            {
                string fileName = Path.GetFileName(path);
                string folderPath = Path.GetDirectoryName(path);

                Selection.activeObject = descriptor.gameObject;
                EditorUtility.SetDirty(descriptor);
                EditorSceneManager.MarkSceneDirty(descriptor.gameObject.scene);
                EditorSceneManager.SaveScene(descriptor.gameObject.scene);

                GameObject avatarClone = Instantiate(descriptor.gameObject);
                foreach (Transform child in avatarClone.GetComponentsInChildren<Transform>())
                {
                    if(child != null && child.CompareTag("EditorOnly")) DestroyImmediate(child.gameObject);
                }

                foreach (ForceSelection child in avatarClone.GetComponentsInChildren<ForceSelection>())
                {
                    if (child != null) DestroyImmediate(child);
                }

                PrefabUtility.SaveAsPrefabAsset(avatarClone, "Assets/_avatar.prefab");
                DestroyImmediate(avatarClone);
                AssetBundleBuild assetBundleBuild = default(AssetBundleBuild);
                assetBundleBuild.assetNames = new string[] {
                    "Assets/_avatar.prefab"
                };

                assetBundleBuild.assetBundleName = fileName;

                BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

                BuildPipeline.BuildAssetBundles(Application.temporaryCachePath, new AssetBundleBuild[] { assetBundleBuild }, 0, EditorUserBuildSettings.activeBuildTarget);
                EditorPrefs.SetString("currentBuildingAssetBundlePath", folderPath);
                EditorUserBuildSettings.SwitchActiveBuildTarget(selectedBuildTargetGroup, activeBuildTarget);
                AssetDatabase.DeleteAsset("Assets/_avatar.prefab");
                if(File.Exists(path))
                    File.Delete(path);
                File.Move(Application.temporaryCachePath + "/" + fileName, path);
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Exportation Successful!", "Exportation Successful!", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Exportation Failed!", "Path is invalid.", "OK");
            }

        }

        GUI.enabled = true;

        EditorGUILayout.LabelField("Infos", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(avatarName);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Attachment points", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(leftHand);
        EditorGUILayout.PropertyField(rightHand);
        EditorGUILayout.PropertyField(helmet);
        EditorGUILayout.PropertyField(backShield);
        EditorGUILayout.PropertyField(backMelee);
        EditorGUILayout.PropertyField(backTwohandedMelee);
        EditorGUILayout.PropertyField(backBow);
        EditorGUILayout.PropertyField(backTool);
        EditorGUILayout.PropertyField(backAtgeir);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Show Features", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(showHelmet);
        EditorGUILayout.PropertyField(showCape);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Action Menu - Parameters", EditorStyles.boldLabel);
        if (GUILayout.Button("Generate parameters from attached Animator")) { PopulateParameterListFromAnimator(animParamList, animatorParameters); }
        DrawParameterList(animParamList, animatorParameters);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Action Menu", EditorStyles.boldLabel);
        DrawMenuItemsList(actionMenuItemsList, actionMenuItems, animatorParameters);

        //DrawCombinedLists(new string[]{"Name","Type","Parameter name","Value"}, 75,controlName,controlTypes,controlParameterNames,controlValues);

        serializedObject.ApplyModifiedProperties();
    }

    private void PopulateParameterListFromAnimator(ReorderableList parameterList, SerializedProperty serializedParameters) {

        var animator = (UnityEditor.Animations.AnimatorController)((ValheimAvatarDescriptor)target).GetComponent<Animator>().runtimeAnimatorController;

        // Clear parameter list data.
        serializedParameters.arraySize = 0;

        //
        foreach (var parameter in animator.parameters) {
            
            //Only proceed if the parameter.type is valid for our context.
            if (parameter.type == UnityEngine.AnimatorControllerParameterType.Bool && parameter.type == UnityEngine.AnimatorControllerParameterType.Int && parameter.type == UnityEngine.AnimatorControllerParameterType.Float) break;
            
            serializedParameters.arraySize++;
            var index = serializedParameters.arraySize-1;
            var element = parameterList.serializedProperty.GetArrayElementAtIndex(index);
            switch (parameter.type) {
                case UnityEngine.AnimatorControllerParameterType.Bool:
                    
                    element.FindPropertyRelative("type").intValue = (int)ValheimAvatarParameterType.Bool;
                    element.FindPropertyRelative("name").stringValue = parameter.name;
                    element.FindPropertyRelative("defaultValue").floatValue = Convert.ToSingle(parameter.defaultBool);

                    
                break;
                case UnityEngine.AnimatorControllerParameterType.Int:

                    element.FindPropertyRelative("type").intValue = (int)ValheimAvatarParameterType.Int;
                    element.FindPropertyRelative("name").stringValue = parameter.name;
                    element.FindPropertyRelative("defaultValue").floatValue = parameter.defaultInt;
                    

                break;
                case UnityEngine.AnimatorControllerParameterType.Float:

                    element.FindPropertyRelative("type").intValue = (int)ValheimAvatarParameterType.Float;
                    element.FindPropertyRelative("name").stringValue = parameter.name;
                    element.FindPropertyRelative("defaultValue").floatValue = parameter.defaultFloat;
                    
                break;
            }

        }

    }

    private void AddPreview(Transform parent, string previewName)
    {
        GameObject preview = Instantiate(Resources.Load<GameObject>(previewName));
        preview.transform.SetParent(parent,true);
        preview.transform.localPosition = Vector3.zero;
        preview.transform.localRotation = Quaternion.identity;
    }

    private void DrawParameterList(ReorderableList parameterList, SerializedProperty serializedParameters)
    {
        parameterList.DoLayoutList();

        //
        (Rect, Rect, Rect) CalcLayout(Rect rect) {

            Rect rect1 = rect;
            Rect rect2 = rect;
            Rect rect3 = rect;

            var padding = 6;

            rect1.width = 52;
            rect3.width = 60;

            rect1.x = rect.x;
            rect3.x = rect.x + rect.width - rect3.width;

            rect2.x = rect1.x + rect1.width + padding;
            rect2.width = rect3.x - rect2.x - padding;

            return (rect1, rect2, rect3);
        }


        parameterList.drawHeaderCallback = (Rect rect) =>
        {
            
            rect.xMin += ReorderableList.Defaults.dragHandleWidth - ReorderableList.Defaults.padding + 1;
            rect.xMax -= 1;
            // rect.xMax -= ReorderableList.Defaults.padding;

            var (rect1, rect2, rect3) = CalcLayout(rect);
            EditorGUI.LabelField(rect1, "Type");
            EditorGUI.LabelField(rect2, "Name");
            EditorGUI.LabelField(rect3, "Default");

        };

        parameterList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            
            var oldHeight = rect.height;
            rect.height = GUI.skin.label.CalcHeight(new GUIContent(""), rect.width) + 1;

            rect.y += (oldHeight - rect.height) / 2;

            var (rect1, rect2, rect3) = CalcLayout(rect);


            //
            var element = parameterList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect1, element.FindPropertyRelative("type"), GUIContent.none);
            EditorGUI.PropertyField(rect2, element.FindPropertyRelative("name"), GUIContent.none);
            
            element.FindPropertyRelative("defaultValue").floatValue = AvatarParameterField(rect3, (ValheimAvatarParameterType)element.FindPropertyRelative("type").enumValueIndex, element.FindPropertyRelative("defaultValue").floatValue);

        };

        // Add button - Adds item.
        parameterList.onAddCallback = (parameterList) => {

            serializedParameters.arraySize++;
            parameterList.Select(serializedParameters.arraySize-1);

        };

    }

    private void DrawMenuItemsList(ReorderableList menuList, SerializedProperty serializedMenuItems, SerializedProperty serializedParameters)
    {

        bool missingParameter = false;
        
        //
        (Rect, Rect, Rect, Rect) CalcLayout(Rect rect) {

            Rect rectName = rect; // Item name
            Rect rectParam = rect; // Item parameter
            Rect rectType = rect; // Item type
            Rect rectValue = rect; // Item value
            
            // Rect for containing menu item data (besides item name).
            Rect rectDataContainer = rect;

            // Padding
            var padding = 6;

            // Setting width
            rectParam.width = 80;
            rectType.width = 64;
            rectValue.width = 60;
            // Make name rect fill up the remaining space.
            rectName.width = rect.width - (rectParam.width + rectType.width + rectValue.width + padding * 3);

            // Set rect positions.
            rectName.x = rect.x;
            rectParam.x = rectName.xMax + padding;
            rectType.x = rectParam.xMax + padding;
            rectValue.x = rectType.xMax + padding;

            return (rectName, rectParam, rectType, rectValue);
        }
        var paramNames = new string[serializedParameters.arraySize];
        for (var i = 0; i < serializedParameters.arraySize; i++) {
            paramNames[i] = serializedParameters.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
        }
        
        menuList.drawHeaderCallback = (Rect rect) =>
        {
            
            rect.xMin += ReorderableList.Defaults.dragHandleWidth - ReorderableList.Defaults.padding + 1;
            rect.xMax -= 1;

            var (rectName, rectParam, rectType, rectValue) = CalcLayout(rect);
            EditorGUI.LabelField(rectName, "Name");
            EditorGUI.LabelField(rectParam, "Parameter");
            EditorGUI.LabelField(rectType, "Type");
            EditorGUI.LabelField(rectValue, "Value");

        };

        menuList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            
            var oldHeight = rect.height;
            rect.height = GUI.skin.label.CalcHeight(new GUIContent(""), rect.width) + 1;

            rect.y += (oldHeight - rect.height) / 2;

            var (rectName, rectParam, rectType, rectValue) = CalcLayout(rect);
            //
            var element = menuList.serializedProperty.GetArrayElementAtIndex(index);
            var paramNameProp = element.FindPropertyRelative("parameterName");

            // Create fields
            EditorGUI.PropertyField(rectName, element.FindPropertyRelative("name"), GUIContent.none);
            var popup = EditorGUI.Popup(rectParam, Array.IndexOf(paramNames, paramNameProp.stringValue), paramNames);
            EditorGUI.PropertyField(rectType, element.FindPropertyRelative("type"), GUIContent.none);
            if (element.FindPropertyRelative("type").enumValueIndex != (int)ControlType.Slider && popup != -1) {
                element.FindPropertyRelative("value").floatValue = AvatarParameterField(rectValue, (ValheimAvatarParameterType)serializedParameters.GetArrayElementAtIndex(popup).FindPropertyRelative("type").enumValueIndex, element.FindPropertyRelative("value").floatValue);
            } else {
                GUI.color = Color.grey;
                GUI.Label(rectValue, "n/a");
                GUI.color = Color.white;
            }
            
            if (popup != -1) {
                paramNameProp.stringValue = paramNames[popup];
            } else {
                missingParameter = true;
            }
            

        };

        // Add button - Adds item.
        menuList.onAddCallback = (menuList) => {

            serializedMenuItems.arraySize++;
            var newMenuItem = menuList.serializedProperty.GetArrayElementAtIndex(serializedMenuItems.arraySize-1);
            var newMenuItemIndex = serializedMenuItems.arraySize-1;

            // If parameter list is not empty, fetch parameterName from there, otherwise return empty string.
            var parameterName = (serializedParameters.arraySize != 0) ?
                (serializedMenuItems.arraySize >= serializedParameters.arraySize) ? // Takes the name from the parameter list at the same, or highest index if this lists' length precedes it.
                    serializedParameters.GetArrayElementAtIndex(serializedParameters.arraySize-1).FindPropertyRelative("name").stringValue :
                    serializedParameters.GetArrayElementAtIndex(newMenuItemIndex).FindPropertyRelative("name").stringValue
                    : "";


            newMenuItem.FindPropertyRelative("name").stringValue = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parameterName);
            newMenuItem.FindPropertyRelative("parameterName").stringValue = parameterName;
            newMenuItem.FindPropertyRelative("value").floatValue = 1;
            menuList.Select(newMenuItemIndex);
        };
        
        menuList.DoLayoutList();
        if (missingParameter) {
            EditorGUILayout.HelpBox("Menu items are missing parameters.", MessageType.Warning);
        }

    }

    private float AvatarParameterField(Rect rect, ValheimAvatarParameterType type, float value) {
        
        switch(type) {
            case ValheimAvatarParameterType.Bool:
                value = EditorGUI.Toggle(rect, value > 0) ? 1 : 0;
                break;
            case ValheimAvatarParameterType.Int:
                value = EditorGUI.IntField(rect, (int)value);
                break;
            case ValheimAvatarParameterType.Float:
                value = EditorGUI.FloatField(rect, value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return value;
        
    }

    private void DrawCombinedLists(string[] labels, float labelWidth, params SerializedProperty[] lists)
    {
        EditorGUILayout.PropertyField(lists[0], false);

        if (!lists[0].isExpanded)
        {
            EditorGUI.indentLevel += 1;

            for (int i = 0; i < lists[0].arraySize; i++)
            {
                EditorGUILayout.BeginVertical("Box");

                for (int j = 0; j < lists.Length; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(labels[j], GUILayout.MaxWidth(labelWidth));
                    EditorGUILayout.PropertyField(lists[j].GetArrayElementAtIndex(i),GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUI.indentLevel -= 1;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+")) // Create Add button
            {
                for (int j = 0; j < lists.Length; j++)
                {
                    lists[j].InsertArrayElementAtIndex(lists[j].arraySize);
                }
            }

            if (GUILayout.Button("-")) // Create Remove button
            {
                if (lists[0].arraySize > 0)
                {
                    for (int j = 0; j < lists.Length; j++)
                    {
                        lists[j].DeleteArrayElementAtIndex(lists[j].arraySize-1);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
