using System;

namespace Core.Editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class TitleAttribute : Attribute
    {
        public string[] title;
        public TitleAttribute(params string[] _title) => this.title = _title;
    }
}