using Flex.Development.Instances;
using Flex.Misc.Utility;
using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Data.States
{
    public class ObjectSave
    {
        private Object _old;
        private Object _current;
        private Type _type;
        private PropertyInfo _subProperty;
        private Object _parent;

        public ObjectSave(Object old, Object current, Type type, PropertyInfo subProperty = null, Object parent = null)
        {
            _old = old;
            _current = current;
            _type = type;
            _subProperty = subProperty;
            _parent = parent;
        }

        public void Reset()
        {
            try
            {
                foreach (PropertyInfo property in _type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    //System.Diagnostics.Debug.WriteLine(property.Name + " " + property.PropertyType.Name);
                    if (property.PropertyType.Equals(typeof(UISafeObservableCollection<Instance>))) //For parent checks, special case! We'll assume the parent is Instance here
                    {
                        if (property.CanWrite)
                        {
                            if (_subProperty == null)
                            {
                                UISafeObservableCollection<Instance> oldInstances = property.GetValue(_old) as UISafeObservableCollection<Instance>;
                                UISafeObservableCollection<Instance> newInstances = property.GetValue(_current) as UISafeObservableCollection<Instance>;

                                List<Instance> list = newInstances.Where(x => !oldInstances.Any(y => x.Equals(y))).ToList();

                                //System.Diagnostics.Debug.WriteLine("New items = " + list.Count);
                                foreach (Instance instance in list)
                                {
                                    instance.RemoveFromParent();
                                    ActiveScene.RemoveInstance(instance);
                                }
                            }
                            else if (_parent != null)
                            {
                                Object subProp = _subProperty.GetValue(_parent);
                                if (subProp != null)
                                {
                                    UISafeObservableCollection<Instance> oldInstances = property.GetValue(subProp) as UISafeObservableCollection<Instance>;
                                    UISafeObservableCollection<Instance> newInstances = property.GetValue(_current) as UISafeObservableCollection<Instance>;

                                    IEnumerable<Instance> newElements = newInstances.Where(x => !oldInstances.Any(y => x.Equals(y)));

                                    foreach (Instance instance in newElements)
                                    {
                                        instance.RemoveFromParent();
                                        ActiveScene.RemoveInstance(instance);
                                    }
                                }
                            }
                        }
                        continue;
                    }
                    else if (property.PropertyType.IsValueType || property.PropertyType.Equals(typeof(String))) //String check because the String datatype is a special snowflake...
                    {
                        ScriptMemberAttribute scriptMemberAttribute = property.GetCustomAttribute<ScriptMemberAttribute>();
                        if (property.CanWrite)
                        {
                            if (_subProperty == null)
                            {
                                property.SetValue(_current, property.GetValue(_old));
                            }
                            else if (_parent != null)
                            {
                                Object subProp = _subProperty.GetValue(_parent);
                                if (subProp != null)
                                {
                                    Object val = property.GetValue(subProp);
                                    property.SetValue(_current, val);
                                }
                            }
                        }
                        continue;
                    }
                    else
                    {
                        if (property.GetCustomAttribute<NonSerializedAttribute>() != null)
                        {
                            continue;
                        }
                        ScriptMemberAttribute scriptMemberAttribute = property.GetCustomAttribute<ScriptMemberAttribute>();
                        if (scriptMemberAttribute != null && scriptMemberAttribute.Access.HasFlag(ScriptAccess.None))
                        {
                            continue;
                        }
                        Object oldValue = property.GetValue(_old);
                        Object newValue = property.GetValue(_current);
                        ObjectSave save = new ObjectSave(oldValue, newValue, property.PropertyType, property, _old);
                        save.Reset();
                    }
                }
            }
            catch
            {

            }
        }
    }
}
