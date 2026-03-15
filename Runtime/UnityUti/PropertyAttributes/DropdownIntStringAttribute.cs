using System;
using UnityEngine;

namespace PlugRMK.UnityUti
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class DropdownIntStringAttribute : PropertyAttribute
    {
        public string ListName { get; }

        public DropdownIntStringAttribute(string listAssetName)
        {
            ListName = listAssetName;
        }
    }
}
