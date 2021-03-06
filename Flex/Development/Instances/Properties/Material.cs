﻿using Mogre;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Instances.Properties
{
    [TypeConverter(typeof(MaterialConverter))]
    public enum Material
    {
        [Description("Grass")]
        GRASS,
        [Description("Wood Planks")]
        WOOD_PLANKS,
        [Description("Dirt")]
        DIRT,
        [Description("Concrete")]
        CONCRETE,
        [Description("Plain")]
        PLAIN,
        [Description("Metal")]
        METAL,
        [Description("Rock")]
        ROCK,
        [Description("Water")]
        WATER,
        [Description("Bricks")]
        BRICKS,
        [Description("Sand")]
        SAND,
        [Description("Cobblestone")]
        COBBLESTONE,
        [Description("Ice")]
        ICE,
        [Description("Corroded Metal")]
        CORRODED_METAL
    }

    public class MaterialConverter : EnumConverter
    {
        private Type enumType;

        public MaterialConverter(Type type) : base(type)
        {
            enumType = type;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                         object value, Type destType)
        {
            FieldInfo fi = enumType.GetField(Enum.GetName(enumType, value));
            DescriptionAttribute dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                                        typeof(DescriptionAttribute));
            if (dna != null)
                return dna.Description;
            else
                return value.ToString();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
        {
            return srcType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture,
                                           object value)
        {
            foreach (FieldInfo fi in enumType.GetFields())
            {
                DescriptionAttribute dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                                            typeof(DescriptionAttribute));
                if ((dna != null) && ((string)value == dna.Description))
                    return Enum.Parse(enumType, fi.Name);
            }
            return Enum.Parse(enumType, (string)value);
        }
    }

    public static class MaterialExtensions
    {
        public static MaterialPtr GetMaterial(this Material material)
        {
            switch (material)
            {
                case Material.GRASS:
                    return ((MaterialPtr)MaterialManager.Singleton.GetByName("Part/Grass"));
                case Material.WOOD_PLANKS:
                    return ((MaterialPtr)MaterialManager.Singleton.GetByName("Part/WoodPlanks"));
                case Material.DIRT:
                    return ((MaterialPtr)MaterialManager.Singleton.GetByName("Part/Dirt"));
                case Material.CONCRETE:
                    return ((MaterialPtr)MaterialManager.Singleton.GetByName("Part/Concrete"));
                case Material.METAL:
                    return ((MaterialPtr)MaterialManager.Singleton.GetByName("Part/Metal"));
                case Material.ROCK:
                    return ((MaterialPtr)MaterialManager.Singleton.GetByName("Part/Rock"));
                case Material.WATER:
                    return ((MaterialPtr)MaterialManager.Singleton.GetByName("Part/Water"));
                case Material.BRICKS:
                    return ((MaterialPtr)MaterialManager.Singleton.GetByName("Part/Bricks"));
                case Material.SAND:
                    return ((MaterialPtr)MaterialManager.Singleton.GetByName("Part/Sand"));
                case Material.COBBLESTONE:
                    return ((MaterialPtr)MaterialManager.Singleton.GetByName("Part/Cobblestone"));
                case Material.CORRODED_METAL:
                    return ((MaterialPtr)MaterialManager.Singleton.GetByName("Part/CorrodedMetal"));
                case Material.ICE:
                    return ((MaterialPtr)MaterialManager.Singleton.GetByName("Part/Ice"));
                case Material.PLAIN:
                    return ((MaterialPtr)MaterialManager.Singleton.GetByName("Part/Plain"));
                default:
                    return ((MaterialPtr)MaterialManager.Singleton.GetByName("Part/Placeholder"));
            }
        }

        public static Vector2 TextureScaling(this Material material)
        {
            switch (material)
            {
                case Material.GRASS:
                    return new Vector2(16, 16);
                case Material.WOOD_PLANKS:
                    return new Vector2(16, 16);
                case Material.DIRT:
                    return new Vector2(16, 16);
                case Material.CONCRETE:
                    return new Vector2(8, 8);
                case Material.METAL:
                    return new Vector2(16, 16);
                case Material.ROCK:
                    return new Vector2(16, 16);
                case Material.WATER:
                    return new Vector2(16, 16);
                case Material.BRICKS:
                    return new Vector2(4, 4);
                case Material.SAND:
                    return new Vector2(16, 16);
                case Material.COBBLESTONE:
                    return new Vector2(20, 20);
                case Material.CORRODED_METAL:
                    return new Vector2(16, 16);
                case Material.ICE:
                    return new Vector2(16, 16);
                case Material.PLAIN:
                default:
                    return new Vector2(16, 16);
            }
        }
    }
}
