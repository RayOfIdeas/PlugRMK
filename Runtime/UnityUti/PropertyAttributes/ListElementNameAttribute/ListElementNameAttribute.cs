using System;
using UnityEngine;

namespace PlugRMK.UnityUti
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ListElementNameAttribute : PropertyAttribute
    {
        public string ElementName { get; }

        public ListElementNameAttribute(string elementName)
        {
            ElementName = elementName;
        }
    }
}
