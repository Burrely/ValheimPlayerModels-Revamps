﻿#if PLUGIN
using System;
using SoftReferenceableAssets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ValheimPlayerModels.Loaders
{
    public class ValavtrLoader : AvatarLoaderBase
    {
        private AssetBundle avatarBundle;
        private SoftReference<Shader> shaderRef;

        public override IEnumerator LoadFile(string file)
        {
            AssetBundleCreateRequest bundleRequest =
                AssetBundle.LoadFromFileAsync(file);
            yield return bundleRequest;

            avatarBundle = bundleRequest.assetBundle;
            if (!avatarBundle)
            {
                Plugin.Log.LogError("Avatar Bundle " + file + " couldn't load!");
                LoadedSuccessfully = false;
                yield break;
            }

            LoadedSuccessfully = true;
        }

        public override IEnumerator LoadAvatar(PlayerModel playerModel)
        {
            if (!avatarBundle) {
                Plugin.Log.LogError("Cannot load avatar since bundle is null");
                yield break;
            }
            
            AvatarInstance avatarInstance = new AvatarInstance(playerModel);

            var avatarAssetBundleRequest = avatarBundle.LoadAssetAsync<GameObject>("_avatar");
            // await the request to finish
            yield return avatarAssetBundleRequest;
            var avatarAsset = avatarAssetBundleRequest.asset as GameObject;
            if (!avatarAsset)
            {
                Plugin.Log.LogError("Couldn't find avatar prefab");
                yield break;
            }

            avatarInstance.AvatarObject = Object.Instantiate(avatarAsset);
            avatarInstance.AvatarDescriptor = avatarInstance.AvatarObject.GetComponent<ValheimAvatarDescriptor>();
            avatarInstance.Animator = avatarInstance.AvatarObject.GetComponent<Animator>();

            avatarInstance.Transform = avatarInstance.AvatarObject.transform;
            avatarInstance.Transform.SetParent(playerModel.transform, false);

            avatarInstance.LeftFoot = avatarInstance.Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            avatarInstance.RightFoot = avatarInstance.Animator.GetBoneTransform(HumanBodyBones.RightFoot);
            avatarInstance.Hips = avatarInstance.Animator.GetBoneTransform(HumanBodyBones.Hips);


            avatarInstance.lodGroup = avatarInstance.AvatarObject.AddComponent<LODGroup>();
            var lodMeshes = new LOD(0.1f, avatarInstance.AvatarObject.GetComponentsInChildren<SkinnedMeshRenderer>());
            avatarInstance.lodGroup.SetLODs(new LOD[] { lodMeshes });
            avatarInstance.lodGroup.RecalculateBounds();

            var playerLodGroup = playerModel.player.GetVisual().GetComponent<LODGroup>();
            avatarInstance.lodGroup.fadeMode = playerLodGroup.fadeMode;
            avatarInstance.lodGroup.animateCrossFading = playerLodGroup.animateCrossFading;

            #region Convert Material Shaders

            Renderer[] renderers = avatarInstance.AvatarObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                foreach (Material mat in renderer.sharedMaterials)
                {
                    if (mat && mat.shader.name == "Valheim/Standard")
                    {
                        if (TryGetPlayerShader(out var shader))
                        {
                            mat.shader = shader;
                        }

                        var mainTex = mat.HasProperty("_MainTex") ? mat.GetTexture("_MainTex") as Texture2D : null;
                        var bumpMap = mat.HasProperty("_BumpMap") ? mat.GetTexture("_BumpMap") : null;

                        mat.SetTexture("_MainTex", mainTex);
                        mat.SetTexture("_SkinBumpMap", bumpMap);
                        mat.SetTexture("_ChestTex", mainTex);
                        mat.SetTexture("_ChestBumpMap", bumpMap);
                        mat.SetTexture("_LegsTex", mainTex);
                        mat.SetTexture("_LegsBumpMap", bumpMap);
                    }
                }
            }

            #endregion

            #region Import Parameters

            avatarInstance.AvatarDescriptor.Validate();

            avatarInstance.Parameters = new Dictionary<int, AvatarInstance.AvatarParameter>();
            
            if (avatarInstance.AvatarDescriptor.animatorParameters != null)
            {
                foreach (var element in avatarInstance.AvatarDescriptor.animatorParameters)
                {
                    var name = element.name;
                    var type = element.type;
                    var defaultValue = element.defaultValue;
                    var hash = Animator.StringToHash(name);
                    if (avatarInstance.Parameters.ContainsKey(hash)) continue;
                    switch (type)
                    {
                        case ValheimAvatarParameterType.Bool:
                            avatarInstance.Parameters.Add(hash, new AvatarInstance.AvatarParameter { name = name, type = AvatarInstance.ParameterType.Bool, boolValue = defaultValue > 0 });
                            avatarInstance.Animator.SetBool(hash, defaultValue > 0);
                            break;
                        case ValheimAvatarParameterType.Int:
                            avatarInstance.Parameters.Add(hash, new AvatarInstance.AvatarParameter { name = name, type = AvatarInstance.ParameterType.Int, intValue = (int)defaultValue });
                            avatarInstance.Animator.SetInteger(hash, (int)defaultValue);
                            break;
                        case ValheimAvatarParameterType.Float:
                            avatarInstance.Parameters.Add(hash, new AvatarInstance.AvatarParameter { name = name, type = AvatarInstance.ParameterType.Float, floatValue = defaultValue });
                            avatarInstance.Animator.SetFloat(hash, defaultValue);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            // if (avatarInstance.AvatarDescriptor.boolParameters != null)
            // {
            //     for (int i = 0; i < avatarInstance.AvatarDescriptor.boolParameters.Count; i++)
            //     {
            //         var name = avatarInstance.AvatarDescriptor.boolParameters[i];
            //         int hash = Animator.StringToHash(name);
            //         if (!avatarInstance.Parameters.ContainsKey(hash))
            //         {
            //             avatarInstance.Parameters.Add(hash, new AvatarInstance.AvatarParameter { name = name, type = AvatarInstance.ParameterType.Bool, boolValue = avatarInstance.AvatarDescriptor.boolParametersDefault[i] });
            //             avatarInstance.Animator.SetBool(hash, avatarInstance.AvatarDescriptor.boolParametersDefault[i]);
            //         }
            //     }
            // }
            //
            // if (avatarInstance.AvatarDescriptor.intParameters != null)
            // {
            //     for (int i = 0; i < avatarInstance.AvatarDescriptor.intParameters.Count; i++)
            //     {
            //         var name = avatarInstance.AvatarDescriptor.intParameters[i];
            //         int hash = Animator.StringToHash(name);
            //         if (!avatarInstance.Parameters.ContainsKey(hash))
            //         {
            //             avatarInstance.Parameters.Add(hash, new AvatarInstance.AvatarParameter { name = name, type = AvatarInstance.ParameterType.Int, intValue = avatarInstance.AvatarDescriptor.intParametersDefault[i] });
            //             avatarInstance.Animator.SetInteger(hash, avatarInstance.AvatarDescriptor.intParametersDefault[i]);
            //         }
            //     }
            // }
            //
            // if (avatarInstance.AvatarDescriptor.floatParameters != null)
            // {
            //     for (int i = 0; i < avatarInstance.AvatarDescriptor.floatParameters.Count; i++)
            //     {
            //         var name = avatarInstance.AvatarDescriptor.floatParameters[i];
            //         int hash = Animator.StringToHash(name);
            //         if (!avatarInstance.Parameters.ContainsKey(hash))
            //         {
            //             avatarInstance.Parameters.Add(hash, new AvatarInstance.AvatarParameter { name = name, type = AvatarInstance.ParameterType.Float, floatValue = avatarInstance.AvatarDescriptor.floatParametersDefault[i] });
            //             avatarInstance.Animator.SetFloat(hash, avatarInstance.AvatarDescriptor.floatParametersDefault[i]);
            //         }
            //     }
            // }

            #endregion

            #region Load Menu

            avatarInstance.MenuControls = new List<AvatarInstance.MenuControl>();

            if (avatarInstance.AvatarDescriptor.controlName != null)
            {
                for (int i = 0; i < avatarInstance.AvatarDescriptor.controlName.Length; i++)
                {
                    avatarInstance.MenuControls.Add(new AvatarInstance.MenuControl
                    {
                        name = avatarInstance.AvatarDescriptor.controlName[i],
                        type = avatarInstance.AvatarDescriptor.controlTypes[i],
                        parameter = avatarInstance.AvatarDescriptor.controlParameterNames[i],
                        value = avatarInstance.AvatarDescriptor.controlValues[i]
                    });
                }
            }

            #endregion

            LoadedAvatarInstance = avatarInstance;
        }

        public override IEnumerator Unload()
        {
            if (avatarBundle) {
                yield return avatarBundle.UnloadAsync(true);
            }
            if (referencedShader)
            {
                referencedShader = false;
                shaderRef.Release();
            }
        }

        private bool referencedShader = false;

        private bool TryGetPlayerShader(out Shader shader) {
            if (!referencedShader)
            {
                // The asset ID here corresponds to Valheim's "Player" shader.
                if (AssetID.TryParse("0ddedf6492e674317b18255c4db06013", out AssetID playerShaderAssetID))
                {
                    shaderRef = new SoftReference<Shader>(playerShaderAssetID);
                    shaderRef.Load();
                    referencedShader = true;
                    shader = shaderRef.Asset;
                    return true;
                }
                Plugin.Log.LogWarning("Failed to find player shader; unable to swap for Valheim/Standard");
                shader = null;
                return false;
            }
            else if (shaderRef.IsLoaded) {
                shader = shaderRef.Asset;
                return true;
            }

            Plugin.Log.LogError("Shader referenced but not loaded? Something is wrong.");
            shader = null;
            return false;
        }
    }
}
#endif