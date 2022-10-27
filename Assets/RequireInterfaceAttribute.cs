using UnityEngine;
using System;

namespace KK.Utility
{
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        public Type requiredType { get; private set; }
        public RequireInterfaceAttribute(Type type)
        {
            requiredType = type;
        }
    }
}