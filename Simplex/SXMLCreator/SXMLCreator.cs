using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace SXMLCreator
{
    public class SXMLCreator
    {

        public void CreateXSD(Type targetAssemblyType, string targetNameSpace)
        {
            List<Type> targetTypes = FilterTypes(targetAssemblyType, targetNameSpace);
            if(targetTypes == null || targetTypes.Count==0){
                 Console.WriteLine("could not find any type in assembly");
                 return;
            }
            FileStream xsdFile = File.Create(targetNameSpace + ".xsd");
            using (StreamWriter writer = new StreamWriter(xsdFile))
            {
                writer.WriteLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                writer.WriteLine($"<xs:schema xmlns:xs=\"{targetNameSpace}\">");
                foreach (Type type in targetTypes)
                {
                    if(type.Name.StartsWith("<"))//what are these?!?
                        continue;
                    writer.WriteLine(GetTypeDescription(type));
                }
                writer.WriteLine("</xs:schema>");
            }
        }

        private List<Type> FilterTypes(Type targetAssemblyType, string targetNameSpace)
        {
            Assembly targetAssembly = Assembly.GetAssembly(targetAssemblyType);
            Type[] containedTypes = targetAssembly.GetTypes();
            if(containedTypes.Length==0){
               
                return null;
            }
            IEnumerable<Type> tmp = containedTypes.Where<Type>(x => x.Namespace!=null && x.Namespace.Contains(targetNameSpace));
            List<Type> typesInNameSpace = tmp.ToList<Type>();
            List<Type> targetTypes = typesInNameSpace;//typesInNameSpace.Where<Type>(x => x.GetCustomAttribute<SerializableAttribute>() != null).ToList<Type>();
            return targetTypes;
        }

        string GetTypeDescription(Type type)
        {
            StringBuilder sb = new StringBuilder();
            
             
            //first checking for enum as enum is also a value type
            if (type.IsEnum)
            {
                Array values= Enum.GetValues(type);
                sb.AppendLine($"<xs:element name=\"{type.Name}\" nillable=\"true\" type=\"{type.Name}\" />");
                sb.AppendLine($"<xs:simpleType name=\"{type.Name}\">");
                sb.AppendLine("<xs:restriction base=\"xs:string\">");
                foreach(object value in values){
                    sb.AppendLine($"<xs:enumeration value=\"{value.ToString()}\"/>");
                }
                sb.AppendLine("</xs:restriction>");
                sb.AppendLine("</xs:simpleType>");
            }
            else if (type.IsClass||(type.IsValueType && !type.IsPrimitive))
            {
                sb.AppendLine($"<xs:element name=\"{type.Name}\" nillable=\"true\" type=\"{type.Name}\" />");
                sb.AppendLine($"<xs:complexType name=\"{type.Name}\">");
                PropertyInfo[] props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (PropertyInfo prop in props)
                {
                    string typeDesc = "";
                    if (prop.PropertyType.IsClass || prop.PropertyType.IsValueType)
                    {
                        typeDesc = prop.PropertyType.Name;
                    }
                    else
                    {

                        if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long))
                            typeDesc = "integer";
                        else if (prop.PropertyType == typeof(float) || prop.PropertyType == typeof(double))
                            typeDesc = "decimal";
                        else if (prop.PropertyType == typeof(string))
                            typeDesc = "string";
                        else if (prop.PropertyType == typeof(bool))
                            typeDesc = "boolean";
                    }
                    sb.AppendLine($"<xs:element name=\"{prop.Name}\" type=\"xs:{typeDesc}\"/>");
                }
                sb.AppendLine("</xs:complexType>");
            }
            
            
            return sb.ToString();
        }
    }
}