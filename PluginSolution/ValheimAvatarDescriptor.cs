using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ValheimPlayerModels
{
    public enum ControlType
    {
        Button,
        Toggle,
        Slider
    }

    public enum ValheimAvatarParameterType
    {
        Bool,
        Int,
        Float
    }

    [Serializable]
    public struct ValheimAvatarParameter
    {
        public string name;
        public ValheimAvatarParameterType type;
        public float defaultValue;
    }
    
    public class ValheimAvatarDescriptor : MonoBehaviour, ISerializationCallbackReceiver
    {
        public string avatarName = "player";

        public Transform leftHand;
        public Transform rightHand;
        public Transform helmet;
        public Transform backShield;
        public Transform backMelee;
        public Transform backTwohandedMelee;
        public Transform backBow;
        public Transform backTool;
        public Transform backAtgeir;

        public bool showHelmet;
        public bool showCape;
        
        public List<ValheimAvatarParameter> animatorParameters = [];

        // unfortunately, BepInEx plugin structs & classes do not work properly with Unity's serialization system
        // we could use https://github.com/xiaoxiao921/FixPluginTypesSerialization to fix this, or maybe JsonUtility
        // but this is a simple workaround that works fine, so we'll use it for now
        public List<string> animatorParameterNames;
        public List<ValheimAvatarParameterType> animatorParameterTypes;
        public List<float> animatorParameterDefaultValues;
        
        // legacy parameter lists
        public List<string> boolParameters;
        public List<bool> boolParametersDefault;
        public List<string> intParameters;
        public List<int> intParametersDefault;
        public List<string> floatParameters;
        public List<float> floatParametersDefault;

        public string[] controlName;
        public ControlType[] controlTypes;
        public string[] controlParameterNames;
        public float[] controlValues;

        private void Awake()
        {
            Validate();
        }

        public void Validate()
        {
            
            // if (boolParametersDefault.Count != boolParameters.Count)
            //     boolParametersDefault.Resize(boolParameters.Count);
            //
            // if (intParametersDefault.Count != intParameters.Count)
            //     intParametersDefault.Resize(intParameters.Count);
            //
            // if (floatParametersDefault.Count != floatParameters.Count)
            //     floatParametersDefault.Resize(floatParameters.Count);

            if (controlTypes.Length != controlName.Length)
                Array.Resize(ref controlTypes, controlName.Length);

            if (controlParameterNames.Length != controlName.Length)
                Array.Resize(ref controlParameterNames, controlName.Length);

            if (controlValues.Length != controlName.Length)
                Array.Resize(ref controlValues, controlName.Length);
        }

        public void OnBeforeSerialize() {
            // null out the legacy parameter lists
            boolParameters = null;
            boolParametersDefault = null;
            intParameters = null;
            intParametersDefault = null;
            floatParameters = null;
            floatParametersDefault = null;
            
            animatorParameterNames = [];
            animatorParameterTypes = [];
            animatorParameterDefaultValues = [];
            foreach (var parameter in animatorParameters)
            {
                animatorParameterNames.Add(parameter.name);
                animatorParameterTypes.Add(parameter.type);
                animatorParameterDefaultValues.Add(parameter.defaultValue);
            }
        }
        public void OnAfterDeserialize() {
            #if PLUGIN
            // only fill in the true parameter list from the individual lists in the plugin
            animatorParameters = [];
            for (var i = 0; i < animatorParameterNames.Count; i++)
            {
                animatorParameters.Add(new ValheimAvatarParameter
                {
                    name = animatorParameterNames[i],
                    type = animatorParameterTypes[i],
                    defaultValue = animatorParameterDefaultValues[i]
                });
            }
            // also fill in from the legacy parameter lists
            if (boolParameters == null) return;
            Plugin.Log.LogInfo($"Avatar {name} has legacy parameters, transferring to new system");
            for (var i = 0; i < boolParameters.Count; i++)
            {
                animatorParameters.Add(new ValheimAvatarParameter
                {
                    name = boolParameters[i],
                    type = ValheimAvatarParameterType.Bool,
                    defaultValue = boolParametersDefault[i] ? 1 : 0
                });
            }
            for (var i = 0; i < intParameters.Count; i++)
            {
                animatorParameters.Add(new ValheimAvatarParameter
                {
                    name = intParameters[i],
                    type = ValheimAvatarParameterType.Int,
                    defaultValue = intParametersDefault[i]
                });
            }
            for (var i = 0; i < floatParameters.Count; i++)
            {
                animatorParameters.Add(new ValheimAvatarParameter
                {
                    name = floatParameters[i],
                    type = ValheimAvatarParameterType.Float,
                    defaultValue = floatParametersDefault[i]
                });
            }
            #endif
        }
    }
}