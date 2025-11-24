using System;
using UnityEngine;
namespace PlugRMK.UnityUti
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class LockAttribute : PropertyAttribute { }
}