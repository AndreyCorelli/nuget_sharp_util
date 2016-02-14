using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SharpExtensionsUtil.Logging;

namespace SharpExtensionsUtil.Reflection
{
    /// <summary>
    /// при сериализации записывает собственный тип в JSON
    /// при десериализации распознает собственный тип
    /// </summary>
    public abstract class JsonTypedObject
    {
        private static readonly Regex commandNameRegex = new Regex("(?<=\"ClassName\":\")[a-z,A-Z,_,0-9]+");

        public string ClassName { get; protected set; }

        protected JsonTypedObject()
        {
            ClassName = GetType().Name;
            var attr = GetType().GetCustomAttributes(typeof(JsonClassNameAttribute), true).FirstOrDefault() as
                        JsonClassNameAttribute;
            if (attr != null) ClassName = attr.Name;
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        protected static JsonTypedObject Deserialize(string commandJson, Dictionary<string, Type> commandByName)
        {
            if (string.IsNullOrEmpty(commandJson))
                return null;

            var nameMatch = commandNameRegex.Match(commandJson);
            if (string.IsNullOrEmpty(nameMatch.Value))
                return null;

            var cmdName = nameMatch.Value;
            Type commandType;
            if (!commandByName.TryGetValue(cmdName, out commandType))
                return null; // команда не распознана

            try
            {
                var cmd = (JsonTypedObject)JsonConvert.DeserializeObject(commandJson, commandType);
                return cmd;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Error parsing \"{0}\": {1}", commandJson, ex);
                return null;
            }
        }

        protected static Dictionary<string, Type> ReadTypeByName(Type inheretedType)
        {
            var typeByName = new Dictionary<string, Type>();
            var asm = inheretedType.Assembly;
            foreach (var type in asm.GetTypes())
            {
                if (!inheretedType.IsAssignableFrom(type)) continue;

                var typeName = type.Name;
                var nameAttr = type.GetCustomAttributes(typeof(JsonClassNameAttribute), true).FirstOrDefault() as
                        JsonClassNameAttribute;
                if (nameAttr != null)
                    typeName = nameAttr.Name;
                typeByName.Add(typeName, type);
            }
            return typeByName;
        }
    }
}
