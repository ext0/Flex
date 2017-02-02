using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Misc.Tracker
{
    public class Tracker
    {
        private NotifyPropertyChangedObject _root;
        private Dictionary<NotifyPropertyChangedObject, List<PropertyInfo>> _nullValidTrackProperties;

        public event EventHandler<PropertyChangedEventArgs> OnChanged;

        public Tracker(NotifyPropertyChangedObject root)
        {
            _root = root;
            _root.PropertyChanged += TriggerEvent;
            _nullValidTrackProperties = new Dictionary<NotifyPropertyChangedObject, List<PropertyInfo>>();
            TraversePropertyEvents(_root);
        }

        private void TraversePropertyEvents(NotifyPropertyChangedObject obj)
        {
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                TrackMemberAttribute trackMember = property.GetCustomAttribute<TrackMemberAttribute>();
                if (trackMember == null)
                {
                    continue;
                }
                if (property.PropertyType.GetCustomAttribute<TrackClassAttribute>() != null)
                {
                    if (property.PropertyType.IsSubclassOf(typeof(NotifyPropertyChangedObject)))
                    {
                        Object val = property.GetValue(obj);
                        if (val != null)
                        {
                            (property.GetValue(obj) as NotifyPropertyChangedObject).PropertyChanged += TriggerEvent;
                            TraversePropertyEvents(obj);
                        }
                        else
                        {
                            if (!_nullValidTrackProperties.ContainsKey(obj))
                            {
                                _nullValidTrackProperties.Add(obj, new List<PropertyInfo>());
                            }
                            if (!_nullValidTrackProperties[obj].Contains(property))
                            {
                                _nullValidTrackProperties[obj].Add(property);
                            }
                        }
                    }
                }
            }
        }

        private void CheckNullifiedProperties()
        {
            List<Tuple<NotifyPropertyChangedObject, PropertyInfo>> toRemove = new List<Tuple<NotifyPropertyChangedObject, PropertyInfo>>();

            foreach (NotifyPropertyChangedObject obj in _nullValidTrackProperties.Keys)
            {
                foreach (PropertyInfo property in _nullValidTrackProperties[obj])
                {
                    Object val = property.GetValue(obj);
                    if (val != null)
                    {
                        (property.GetValue(obj) as NotifyPropertyChangedObject).PropertyChanged += TriggerEvent;
                        toRemove.Add(new Tuple<NotifyPropertyChangedObject, PropertyInfo>(obj, property));
                        //TraversePropertyEvents(obj);
                    }
                }
            }

            foreach (Tuple<NotifyPropertyChangedObject, PropertyInfo> tuple in toRemove)
            {
                if (_nullValidTrackProperties.ContainsKey(tuple.Item1))
                {
                    if (_nullValidTrackProperties[tuple.Item1].Contains(tuple.Item2))
                    {
                        _nullValidTrackProperties[tuple.Item1].Remove(tuple.Item2);
                    }
                    if (_nullValidTrackProperties[tuple.Item1].Count == 0)
                    {
                        _nullValidTrackProperties.Remove(tuple.Item1);
                    }
                }
            }
        }

        private void TriggerEvent(Object sender, PropertyChangedEventArgs e)
        {
            CheckNullifiedProperties();
            if (OnChanged != null)
            {
                OnChanged.Invoke(this, e);
            }
        }
    }
}
