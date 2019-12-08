using System;

namespace CommandLine
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class GroupAttribute : Attribute
    {
        public GroupAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
