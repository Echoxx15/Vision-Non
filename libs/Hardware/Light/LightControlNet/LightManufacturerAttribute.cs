using System;

namespace LightControlNet
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class LightManufacturerAttribute : Attribute
    {
        public LightControllerType Type { get; }
        public string ManufacturerName { get; }

        public LightManufacturerAttribute(LightControllerType type, string manufacturerName)
        {
            Type = type;
            ManufacturerName = manufacturerName;
        }
    }
}
