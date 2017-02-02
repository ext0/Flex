using Flex.Development.Execution.Runtime.Attributes;
using Flex.Development.Instances;
using NiL.JS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Runtime
{
    public class DynamicJS : JSValue
    {
        protected Object _object;

        private sealed class WriteAccessor : JSValue
        {
            private string _name;
            private Object _object;

            public WriteAccessor(string name, Object obj)
            {
                _object = obj;
                _name = name;
            }

            public override void Assign(JSValue value)
            {
                foreach (PropertyInfo property in _object.GetType().GetProperties())
                {
                    DynamicExposedPropertyAttribute propertyAttribute = property.GetCustomAttribute<DynamicExposedPropertyAttribute>(false);

                    if (propertyAttribute == null)
                    {
                        continue;
                    }

                    String name = (propertyAttribute.HasCustomName) ? (propertyAttribute.Name) : (property.Name.ToLower());

                    if (name.Equals(_name))
                    {
                        if (propertyAttribute.IsReadOnly)
                        {
                            return;
                        }

                        Type propertyType = property.PropertyType;

                        if (propertyType.Equals(typeof(String)))
                        {
                            property.SetValue(_object, value.ToString());
                        }
                        else if (propertyType.Equals(typeof(int)))
                        {
                            property.SetValue(_object, (int)value);
                        }
                        else if (propertyType.Equals(typeof(double)))
                        {
                            property.SetValue(_object, (double)value);
                        }
                        else if (propertyType.Equals(value.GetType()))
                        {
                            property.SetValue(_object, value as Object);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Not a supported type!");
                        }
                        return;
                    }
                }
            }
        }

        public DynamicJS(Object obj)
        {
            if (obj.GetType().GetCustomAttribute<DynamicExposedClassAttribute>() == null)
            {
                throw new Exception("TypeError: Cannot convert " + obj.GetType().Name + " to a DynamicJS object - not marked as DynamicExposedClass.");
            }
            ValueType = JSValueType.Object;
            Value = this;
            _object = obj;
        }

        public Object Object
        {
            get
            {
                return _object;
            }
        }

        public override JSValue toString(Arguments args)
        {
            return Wrap(_object.ToString());
        }

        public override JSValue toLocaleString()
        {
            return toString(null);
        }

        public override string ToString()
        {
            return _object.ToString();
        }

        protected override JSValue GetProperty(JSValue key, bool forWrite, PropertyScope propertyScope)
        {
            if (key.ValueType == JSValueType.String)
            {
                foreach (PropertyInfo property in _object.GetType().GetProperties())
                {
                    DynamicExposedPropertyAttribute propertyAttribute = property.GetCustomAttribute<DynamicExposedPropertyAttribute>(false);

                    if (propertyAttribute == null)
                    {
                        continue;
                    }

                    String name = (propertyAttribute.HasCustomName) ? (propertyAttribute.Name) : (property.Name.ToLower());

                    if (name.Equals(key.ToString()))
                    {
                        if (forWrite)
                        {
                            return new WriteAccessor(key.ToString(), _object);
                        }
                        else
                        {
                            Object val = property.GetValue(_object);

                            if (property.PropertyType.Equals(typeof(int)))
                            {
                                return (int)val;
                            }
                            else if (property.PropertyType.Equals(typeof(String)))
                            {
                                return val.ToString();
                            }
                            else if (property.PropertyType.Equals(typeof(Double)))
                            {
                                return (double)val;
                            }
                            else if (property.PropertyType.IsSubclassOf(typeof(Object)) || property.PropertyType.Equals(typeof(Object)))
                            {
                                return new DynamicJS(val);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Unsupported type!");
                            }
                        }
                    }
                }
                foreach (MethodInfo method in _object.GetType().GetMethods())
                {
                    DynamicExposedMethodAttribute methodAttribute = method.GetCustomAttribute<DynamicExposedMethodAttribute>(false);

                    if (methodAttribute == null)
                    {
                        continue;
                    }

                    String name = (methodAttribute.HasCustomName) ? (methodAttribute.Name) : (method.Name.ToLower());

                    if (name.Equals(key.ToString()))
                    {

                        Type methodReturnType = method.ReturnType;

                        if (forWrite)
                        {
                            //Don't let the user redefine methods...
                            return null;
                        }

                        //Hacky code ahead

                        Delegate del = Delegate.CreateDelegate(methodAttribute.DelegateType, _object, method);

                        return JSValue.Marshal(del);
                    }
                }
                if (_object.GetType().IsSubclassOf(typeof(Instance)) || _object.GetType().Equals(typeof(Instance)))
                {
                    foreach (Instance child in (_object as Instance).GetChildren(false))
                    {
                        if (child.DisplayName.Equals(key.ToString()))
                        {
                            return new DynamicJS(child);
                        }
                    }
                }
            }
            return null;
        }
    }
}
