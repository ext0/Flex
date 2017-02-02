using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Flex.Misc.Utility
{
    public static class FlexUtility
    {
        public static String PropertyToCleanString(String property)
        {
            return Regex.Replace(property, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }

        public static Point3D SnapToGrid(Point3D point, double snapSize)
        {
            return new Point3D(Math.Round(point.X / snapSize) * snapSize, Math.Round(point.Y / snapSize) * snapSize, Math.Round(point.Z / snapSize) * snapSize);
        }

        public static Vector3D SnapToGrid(Vector3D point, double snapSize)
        {
            return new Vector3D(Math.Round(point.X / snapSize) * snapSize, Math.Round(point.Y / snapSize) * snapSize, Math.Round(point.Z / snapSize) * snapSize);
        }

        public static void RunPrivateMethod<T>(T obj, String methodName)
        {
            MethodInfo method = typeof(T).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(obj, new object[] { });
        }

        public static void SetPrivateField<S>(S obj, String fieldName, Object value)
        {
            FieldInfo field = typeof(S).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(obj, value);
        }

        public static T GetPrivateField<S, T>(S obj, String fieldName)
        {
            FieldInfo field = typeof(S).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field.GetValue(obj);
        }

        public static System.Windows.Media.Media3D.Quaternion FromYawPitchRoll(float yaw, float pitch, float roll)
        {
            SharpDX.Quaternion quaternion = SharpDX.Quaternion.RotationYawPitchRoll(MathUtil.DegreesToRadians(yaw), MathUtil.DegreesToRadians(pitch), MathUtil.DegreesToRadians(roll));
            return new System.Windows.Media.Media3D.Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        public static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        public static byte[] SerializeToBinary(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static Object DeserializeToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        public static void RunWindowAction(Action action, DispatcherPriority priority, bool async = true)
        {
            try
            {
                if (async)
                {
                    Application.Current.Dispatcher.BeginInvoke(priority, action);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(priority, action);
                }
            }
            catch (NullReferenceException)
            {
                //Ignore this, because it's probably just the closing of the application
            }
        }

        public static double ConstrainValue(double value, double lower, double upper)
        {
            return Math.Min(upper, Math.Max(lower, value));
        }

        public static DependencyObject RecursiveFindDependencyObjectFromItem(TreeView view, Object obj, Func<Object, Object, bool> equalityComparator)
        {
            return RecursiveFindDependencyObjectFromItemHelper(view, view.Items, obj, equalityComparator);
        }

        private static DependencyObject RecursiveFindDependencyObjectFromItemHelper(Object view, ItemCollection collection, Object obj, Func<Object, Object, bool> equalityComparator)
        {
            int i = 0;
            foreach (Object item in collection)
            {
                TreeViewItem treeViewItem = null;
                if (view is TreeView)
                {
                    treeViewItem = (TreeViewItem)((view as TreeView).ItemContainerGenerator.ContainerFromIndex(i));
                }
                else if (view is TreeViewItem)
                {
                    treeViewItem = (TreeViewItem)((view as TreeViewItem).ItemContainerGenerator.ContainerFromIndex(i));
                }

                if (treeViewItem != null)
                {
                    if (equalityComparator(treeViewItem.DataContext, obj))
                    {
                        return treeViewItem as DependencyObject;
                    }
                    else
                    {
                        DependencyObject result = RecursiveFindDependencyObjectFromItemHelper(treeViewItem, treeViewItem.Items, obj, equalityComparator);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
                i++;
            }
            return null;
        }

        public static String CutLastDelimiter(String str, String delimiter)
        {
            if ((str.LastIndexOf(delimiter) != -1) && (str.LastIndexOf(delimiter) != (str.Length - 1)))
            {
                return str.Substring(str.LastIndexOf(delimiter) + 1);
            }
            return str;
        }
    }
}
