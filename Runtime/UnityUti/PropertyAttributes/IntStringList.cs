using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlugRMK.UnityUti
{
    [CreateAssetMenu(fileName = "IntStringList", menuName = "Scriptable Objects/IntStringList")]
    public class IntStringList : ScriptableObject
    {
        [Serializable]
        public class IntString
        {
            public int IntValue;
            public string StringValue;
        }

        [SerializeField]
        List<IntString> _intStrings = new();
        public List<IntString> IntStrings => _intStrings;
    }
}
