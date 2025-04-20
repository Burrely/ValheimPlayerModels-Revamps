﻿#if PLUGIN
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ValheimPlayerModels.Loaders
{
    public class AvatarInstance
    {
        public enum ParameterType
        {
            Bool,
            Int,
            Float
        }

        public class AvatarParameter
        {
            public string name;
            public ParameterType type;
            public bool boolValue;
            public int intValue;
            public float floatValue;
        }

        public class MenuControl
        {
            public string name;
            public ControlType type;
            public string parameter;
            public float value;
        }

        private PlayerModel Owner;
        public GameObject AvatarObject;
        public Transform Transform;
        public Transform LeftFoot;
        public Transform RightFoot;
        public Transform Hips;
        public Animator Animator;
        public LODGroup lodGroup;
        public ValheimAvatarDescriptor AvatarDescriptor;
        public Dictionary<int, AvatarParameter> Parameters;
        public List<MenuControl> MenuControls;

        public AvatarInstance(PlayerModel owner) => Owner = owner;

        #region Animator Params Methods

        public void SetParameterValue(string name, float value)
        {
            int hash = Animator.StringToHash(name);
            SetParameterValue(hash, value);
        }

        public void SetParameterValue(int hash, float value)
        {
            if (!Parameters.ContainsKey(hash)) return;
            switch (Parameters[hash].type)
            {
                case ParameterType.Bool:
                    SetBool(hash, value != 0);
                    break;
                case ParameterType.Int:
                    SetInt(hash, (int)value);
                    break;
                case ParameterType.Float:
                    SetFloat(hash, value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public float GetParameterValue(string name)
        {
            int hash = Animator.StringToHash(name);
            return GetParameterValue(hash);
        }

        public float GetParameterValue(int hash)
        {
            if (!Parameters.ContainsKey(hash)) return 0;

            switch (Parameters[hash].type)
            {
                case ParameterType.Bool:
                    return Parameters[hash].boolValue ? 1 : 0;
                case ParameterType.Int:
                    return Parameters[hash].intValue;
                case ParameterType.Float:
                    return Parameters[hash].floatValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool SetBool(string name, bool value)
        {
            int hash = Animator.StringToHash(name);
            return SetBool(hash, value);
        }

        public bool SetBool(int hash, bool value)
        {
            if (!Parameters.ContainsKey(hash)) return false;

            Parameters[hash].boolValue = value;
            Animator.SetBool(hash, value);

            if (Owner.zNetView.GetZDO() != null && Owner.zNetView.IsOwner())
                Owner.zNetView.GetZDO().Set(438569 + hash, value);
            return true;
        }

        public bool SetInt(string name, int value)
        {
            int hash = Animator.StringToHash(name);
            return SetInt(hash, value);
        }

        public bool SetInt(int hash, int value)
        {
            if (!Parameters.ContainsKey(hash)) return false;

            Parameters[hash].intValue = value;
            Animator.SetInteger(hash, value);

            if (Owner.zNetView.GetZDO() != null && Owner.zNetView.IsOwner())
                Owner.zNetView.GetZDO().Set(438569 + hash, value);
            return true;
        }

        public bool SetFloat(string name, float value)
        {
            int hash = Animator.StringToHash(name);
            return SetFloat(hash, value);
        }

        public bool SetFloat(int hash, float value)
        {
            if (!Parameters.ContainsKey(hash)) return false;

            Parameters[hash].floatValue = value;
            Animator.SetFloat(hash, value);

            if (Owner.zNetView.GetZDO() != null && Owner.zNetView.IsOwner())
                Owner.zNetView.GetZDO().Set(438569 + hash, value);
            return true;
        }

        /// <summary>
        /// Loads parameters from a dictionary of string to float
        /// ignores parameters that are in the dictionary but not defined in the avatar
        /// leaves parameters that are defined in the avatar but not in the dictionary unchanged
        /// </summary>
        public void LoadParametersFromDict(Dictionary<string, float> dict) {
            // load parameters, they are all floats since that's what the animator uses
            // so it is easier than trying to deal with dynamic types
            foreach (var kvp in dict)
            {
                SetParameterValue(kvp.Key, kvp.Value);
            }
        }
        
        /// <summary>
        /// Saves parameters to a dictionary of string to float
        /// </summary>
        public Dictionary<string, float> SaveParametersToDict() {
            var dict = new Dictionary<string, float>();
            foreach (var kvp in Parameters)
            {
                dict.Add(kvp.Value.name, GetParameterValue(kvp.Key));
            }
            return dict;
        }
        #endregion
    }

    public abstract class AvatarLoaderBase
    {
        public bool LoadedSuccessfully { get; protected set; }
        public AvatarInstance LoadedAvatarInstance { get; protected set; }

        public abstract IEnumerator LoadFile(string file);
        public abstract IEnumerator LoadAvatar(PlayerModel playerModel);
        public abstract IEnumerator Unload();
    }
}
#endif