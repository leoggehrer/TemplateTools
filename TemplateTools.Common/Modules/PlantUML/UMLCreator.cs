//@BaseCode


namespace TemplateTools.Common.Modules.PlantUML
{
    using TemplateTools.Common.Extensions;
    using System.Collections;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Provides methods for creating UML diagrams.
    /// </summary>
    public static partial class UMLCreator
    {
        #region skinparam
        /// <summary>
        /// Gets the default class skin parameters.
        /// </summary>
        /// <remarks>
        /// The default class skin parameters include background color, arrow color, and border color.
        /// </remarks>
        /// <returns>
        /// An enumerable collection of strings representing the default class skin parameters.
        /// </returns>
        public static IEnumerable<string> DefaultClassSkinparam => [
            "skinparam class {",
            " BackgroundColor whitesmoke",
            " ArrowColor grey",
            " BorderColor darkgrey",
            "}",
        ];
        /// <summary>
        /// Returns the default skin parameters for objects.
        /// </summary>
        /// <remarks>
        /// The skin parameters define the appearance of objects in a diagram.
        /// </remarks>
        /// <returns>
        /// An enumerable collection of strings representing the default skin parameters.
        /// </returns>
        public static IEnumerable<string> DefaultObjectSkinparam => [
            "skinparam object {",
            " BackgroundColor white",
            " ArrowColor grey",
            " BorderColor darkgrey",
            "}",
        ];
        #endregion skinparam

        #region diagram creators
        /// <summary>
        /// Creates a class diagram based on specified creation flags and types.
        /// </summary>
        /// <param name="diagramCreationFlags">The flags indicating the desired elements to include in the diagram.</param>
        /// <param name="types">The types used to generate the class diagram.</param>
        /// <returns>A collection of strings representing the class diagram.</returns>
        public static IEnumerable<string> CreateClassDiagram(DiagramCreationFlags diagramCreationFlags, params Type[] types)
        {
            var result = new List<string>();
            var allTypes = new List<Type>(types);

            if ((diagramCreationFlags & DiagramCreationFlags.TypeExtends) > 0)
            {
                foreach (var type in allTypes.Clone().Where(t => t.IsClass))
                {
                    allTypes.AddRange(type.GetClassHierarchy().Where(e => allTypes.Contains(e) == false));
                }
            }

            if ((diagramCreationFlags & DiagramCreationFlags.ImplementedInterfaces) > 0)
            {
                foreach (var type in allTypes.Clone())
                {
                    allTypes.AddRange(type.GetDeclaredInterfaces().Where(e => allTypes.Contains(e) == false));
                }
            }

            if ((diagramCreationFlags & DiagramCreationFlags.InterfaceExtends) > 0)
            {
                foreach (var type in allTypes.Clone().Where(t => t.IsInterface))
                {
                    allTypes.AddRange(type.GetClassHierarchy().Where(e => allTypes.Contains(e) == false));
                }
            }

            result.AddRange(CreateTypeDefinitions(allTypes, diagramCreationFlags));

            if ((diagramCreationFlags & DiagramCreationFlags.TypeExtends) > 0)
            {
                result.AddRange(CreateTypeHirarchies(allTypes.Where(t => t.IsClass)));
            }

            if ((diagramCreationFlags & DiagramCreationFlags.InterfaceExtends) > 0)
            {
                foreach (var type in allTypes.Where(t => t.IsInterface))
                {
                    var extend = type.GetInterfaceHierarchy();

                    extend.Extends.ForEach(e => result.AddRange(CreateTypeHierachy([extend.Type!, e.Type!])));
                }
            }

            if ((diagramCreationFlags & DiagramCreationFlags.ImplementedInterfaces) > 0)
            {
                foreach (var type in allTypes.Where(t => t.IsClass))
                {
                    result.AddRange(CreateTypeImplements(type));
                }
            }

            if ((diagramCreationFlags & DiagramCreationFlags.ClassRelations) > 0)
            {
                foreach (var type in allTypes.Where(t => t.IsClass))
                {
                    result.AddRange(CreateTypeRelations(type, 0));
                }
            }
            return result;
        }
        /// <summary>
        /// Creates the name of an object by combining the object's type name and hash code.
        /// </summary>
        /// <param name="obj">The object to create the name for.</param>
        /// <returns>The name of the object.</returns>
        private static string CreateObjectName(Object obj) => $"{obj.GetType().Name}_{obj.GetHashCode()}";
        /// <summary>
        /// Creates a collection name for the specified object.
        /// </summary>
        /// <param name="obj">The object for which a collection name is to be created.</param>
        /// <returns>A string representing the collection name.</returns>
        private static string CreateCollectionName(Object obj) => $"Colletion_{obj.GetHashCode()}";
        /// <summary>
        /// Creates an object diagram for the given objects up to a specified depth.
        /// </summary>
        /// <param name="maxDeep">The maximum depth of the object diagram.</param>
        /// <param name="objects">The objects to include in the object diagram.</param>
        /// <returns>The lines representing the object diagram.</returns>
        public static IEnumerable<string> CreateObjectDiagram(int maxDeep, params Object[] objects)
        {
            var result = new List<string>();
            var createdObjects = new List<object>();
            void CreateMapStateRec(Object[] objects, List<string> lines, int deep)
            {
                static void CreateMapObjectState(Object obj, List<string> lines)
                {
                    lines.Add($"map {CreateObjectName(obj)} " + "{");
                    lines.AddRange(CreateObjectState(obj).SetIndent(1));
                    lines.Add("}");
                }
                static void CreateMapCollectionState(IEnumerable collection, List<string> lines)
                {
                    lines.Add($"map {CreateCollectionName(collection)} " + "{");
                    lines.AddRange(CreateCollectionState(collection).SetIndent(1));
                    lines.Add("}");
                }

                foreach (var obj in objects)
                {
                    if (createdObjects.Contains(obj) == false)
                    {
                        createdObjects.Add(obj);
                        CreateMapObjectState(obj, lines);

                        if (deep + 1 <= maxDeep)
                        {
                            var relations = obj.GetType().GetRelations();
                            var relationFieldInfos = obj.GetType().GetAllClassFields()
                                                        .Where(fi => relations.Any(r => r == fi.FieldType));

                            foreach (var relFieldInfo in relationFieldInfos)
                            {
                                var value = GetFieldValue(obj, relFieldInfo);

                                if (value is IEnumerable collection)
                                {
                                    var counter = 0;

                                    CreateMapCollectionState(collection, lines);
                                    lines.Add($"{CreateObjectName(obj)}::{GetFieldName(relFieldInfo)} --> {CreateCollectionName(value)}");
                                    if (deep + 2 <= maxDeep)
                                    {
                                        foreach (var item in collection)
                                        {
                                            if (item != null)
                                            {
                                                CreateMapStateRec([item], lines, deep + 2);
                                                lines.Add($"{CreateCollectionName(collection)}::{counter} --> {CreateObjectName(item)}");
                                            }
                                            counter++;
                                        }
                                    }
                                }
                                else if (value != null)
                                {
                                    CreateMapStateRec([value], lines, deep + 1);
                                    lines.Add($"{CreateObjectName(obj)}::{GetFieldName(relFieldInfo)} --> {CreateObjectName(value)}");
                                }
                            }
                        }
                    }
                }
            }
            CreateMapStateRec(objects, result, 0);
            return result;
        }
        #endregion diagram creators

        #region diagram helpers
        /// Creates the XML documentation for the CreateTypeDefinition method.
        /// @param type The type for which to create the type definition.
        /// @param diagramCreationFlags The flags to determine which members to include in the type definition.
        /// @return A collection of strings representing the XML documentation for the method.
        public static IEnumerable<string> CreateTypeDefinition(Type type, DiagramCreationFlags diagramCreationFlags)
        {
            var result = new List<string>();

            if (type.IsEnum)
            {
                result.Add($"enum {type.Name} #light" +
                $"blue " + "{");
                if ((diagramCreationFlags & DiagramCreationFlags.EnumMembers) > 0)
                {
                    foreach (var item in Enum.GetValues(type))
                    {
                        result.Add($" {item}");
                    }
                }
                result.Add("}");
            }
            else if (type.IsClass)
            {
                var prefix = type.IsPublic ? "+" : String.Empty;

                if (type.IsAbstract)
                    result.Add($"{prefix}abstract class {type.Name} #white " + "{");
                else
                    result.Add($"{prefix}class {type.Name} #whitesmoke " + "{");
                if ((diagramCreationFlags & DiagramCreationFlags.ClassMembers) > 0)
                {
                    result.AddRange(CreateItemMembers(type).SetIndent(1));
                }
                result.Add("}");
            }
            else if (type.IsInterface)
            {
                result.Add($"interface {type.Name} #lightgrey " + "{");
                if ((diagramCreationFlags & DiagramCreationFlags.InterfaceMembers) > 0)
                {
                    result.AddRange(CreateItemMembers(type).SetIndent(1));
                }
                result.Add("}");
            }
            return result;
        }
        /// <summary>
        /// Creates type hierarchies for the given collection of types.
        /// </summary>
        /// <param name="types">The collection of types.</param>
        /// <returns>A collection of string representing the type hierarchies.</returns>
        public static IEnumerable<string> CreateTypeHirarchies(IEnumerable<Type> types)
        {
            var result = new List<string>();

            foreach (var item in CreateDiagramHirarchies(types))
            {
                result.AddRange(CreateTypeHierachy(item));
            }
            return result.Distinct();
        }
        /// <summary>
        /// Creates a type hierarchy based on the input collection of types.
        /// </summary>
        /// <param name="types">The collection of types to create the hierarchy from.</param>
        /// <returns>An enumerable collection of strings representing the type hierarchy.</returns>
        public static IEnumerable<string> CreateTypeHierachy(IEnumerable<Type> types)
        {
            var result = new List<string>();
            var typeArray = types.ToArray();

            for (int i = 0; i < typeArray.Length - 1; i++)
            {
                if (typeArray[i + 1].IsInterface || typeArray[i].IsInterface)
                {
                    result.Add($"{typeArray[i + 1].Name} <|.. {typeArray[i].Name}");
                }
                else
                {
                    result.Add($"{typeArray[i + 1].Name} <|-- {typeArray[i].Name}");
                }
            }
            return result;
        }
        /// <summary>
        /// Retrieves a collection of type relations for the specified type up to the specified depth.
        /// </summary>
        /// <param name="type">The type for which to retrieve type relations.</param>
        /// <param name="deep">The depth up to which to retrieve type relations.</param>
        /// <returns>A collection of string representations of the type relations.</returns>
        public static IEnumerable<string> CreateTypeRelations(Type type, int deep)
        {
            var result = new List<string>();

            foreach (var item in type.GetRelations(deep))
            {
                if (item.IsArray)
                {
                    var elemType = item.UnderlyingSystemType;

                    if (item.IsNullableType())
                    {
                        result.Add($"{type.Name} --> \"0..1\" {elemType.Name}");
                    }
                    else
                    {
                        result.Add($"{type.Name} --> \"1\" {elemType.Name}");
                    }
                }
                else if (item.IsGenericCollectionType())
                {
                    var elemType = item.GetGenericArguments()[0];

                    if (item.IsNullableType())
                    {
                        result.Add($"{type.Name} \"*\" o-- \"0..1\" {elemType.Name}");
                    }
                    else
                    {
                        result.Add($"{type.Name} \"*\" *-- \"1\" {elemType.Name}");
                    }
                }
                else
                {
                    if (item.IsNullableType())
                    {
                        result.Add($"{type.Name} --> \"0..1\" {item.Name}");
                    }
                    else
                    {
                        result.Add($"{type.Name} --> \"1\" {item.Name}");
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Creates a collection of formatted string representations of interfaces implemented by the specified type.
        /// </summary>
        /// <param name="type">The type to check for implemented interfaces.</param>
        /// <returns>A collection of formatted string representations of interfaces implemented by the specified type.</returns>
        public static IEnumerable<string> CreateTypeImplements(Type type)
        {
            var result = new List<string>();
            var lolypop = "()-";

            foreach (var typeInfo in type.GetDeclaredInterfaces())
            {
                result.Add($"{typeInfo.Name} {lolypop} {type.Name}");
            }
            return result;
        }
        /// <summary>
        /// Creates type definitions for the specified collection of types.
        /// </summary>
        /// <param name="types">The collection of types to create type definitions for.</param>
        /// <param name="diagramCreationFlags">Flags specifying diagram creation options.</param>
        /// <returns>A collection of type definitions as strings.</returns>
        public static IEnumerable<string> CreateTypeDefinitions(IEnumerable<Type> types, DiagramCreationFlags diagramCreationFlags)
        {
            var result = new List<string>();

            foreach (var type in types)
            {
                result.AddRange(CreateTypeDefinition(type, diagramCreationFlags));
            }
            return result;
        }
        /// <summary>
        /// Retrieves a collection of item members for the specified type.
        /// </summary>
        /// <param name="type">The type to retrieve item members from.</param>
        /// <returns>A collection of item members.</returns>
        public static IEnumerable<string> CreateItemMembers(Type type)
        {
            var counter = 0;
            var result = new List<string>();

            #region fields
            BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (var item in type.GetFields(bindingFlags))
            {
                counter++;
                result.Add($"{(item.IsPublic ? "+" : "-")}" + " {static}" + $"{item.FieldType.GetSourceTypeName()} {GetFieldName(item)}");
            }
            if (counter > 0)
                result.Add("---");

            counter = 0;
            bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            foreach (var item in type.GetFields(bindingFlags))
            {
                var prefix = item.IsPrivate ? "-" : "+";

                counter++;
                result.Add($"{prefix}" + " " + $"{item.FieldType.GetSourceTypeName()} {GetFieldName(item)}");
            }
            if (counter > 0)
                result.Add("---");
            #endregion fields

            #region properties
            counter = 0;
            bindingFlags = BindingFlags.Static | BindingFlags.Public;
            foreach (var item in type.GetProperties(bindingFlags))
            {
                if (item.CanRead)
                {
                    counter++;
                    result.Add(" + {static}" + $"{item.PropertyType.GetSourceTypeName()} get{item.Name}()");
                }
                if (item.CanWrite)
                {
                    counter++;
                    result.Add(" + {static}" + $"set{item.Name}({item.PropertyType.GetSourceTypeName()} value)");
                }
            }
            if (counter > 0)
                result.Add("---");

            counter = 0;
            bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
            foreach (var item in type.GetProperties(bindingFlags))
            {
                if (item.CanRead)
                {
                    counter++;
                    result.Add($" + {item.PropertyType.GetSourceTypeName()} get{item.Name}()");
                }
                if (item.CanWrite)
                {
                    counter++;
                    result.Add($" + set{item.Name}({item.PropertyType.GetSourceTypeName()} value)");
                }
            }
            if (counter > 0)
                result.Add("---");
            #endregion

            #region methoden
            counter = 0;
            bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (var item in type.GetMethods(bindingFlags))
            {
                var prefix = item.IsPrivate ? "-" : "+";

                counter++;
                result.Add($"{prefix} " + "{static}" + $"{item.ReturnType.GetSourceTypeName()} {item.Name}({GetParameters(item)})");
            }
            if (counter > 0)
                result.Add("---");

            counter = 0;
            bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            foreach (var item in type.GetMethods(bindingFlags))
            {
                var prefix = item.IsPrivate ? "-" : "+";

                counter++;
                result.Add($"{prefix} {item.ReturnType.GetSourceTypeName()} {item.Name}({GetParameters(item)})");
            }
            //if (counter > 0)
            //    result.Add("---");
            #endregion methoden
            return result;
        }
        /// <summary>
        /// Creates an object state by retrieving the fields and their values of the specified object.
        /// </summary>
        /// <param name="obj">The object for which to create the state.</param>
        /// <returns>An enumerable collection of string representations of the object state.</returns>
        public static IEnumerable<string> CreateObjectState(Object obj)
        {
            var counter = 0;
            var result = new List<string>();

            #region fields
            BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (var item in obj.GetType().GetFields(bindingFlags))
            {
                counter++;
                // result.Add("{static}" + $"{item.FieldType.Name} {GetFieldName(item)} => {GetStateValue(obj, item)}");
                result.Add("{static}" + $"{GetFieldName(item)} => {GetStateValue(obj, item)}");
            }
            if (counter > 0)
                result.Add("---");

            counter = 0;
            foreach (var item in obj.GetType().GetAllClassFields())
            {
                counter++;
                //                result.Add($"{item.FieldType.Name} {GetFieldName(item)} => {GetStateValue(obj, item)}");
                result.Add($"{GetFieldName(item)} => {GetStateValue(obj, item)}");
            }
            //if (counter > 0)
            //    result.Add("---");
            #endregion fields

            return result;
        }
        /// <summary>
        /// Creates a collection state by iterating through the given collection and generating a string representation of each item in the collection.
        /// </summary>
        /// <param name="collection">The collection to iterate through.</param>
        /// <returns>An enumerable collection of strings representing the state of items in the collection.</returns>
        public static IEnumerable<string> CreateCollectionState(IEnumerable collection)
        {
            var counter = 0;
            var result = new List<string>();

            foreach (var item in collection)
            {
                if (item != null)
                {
                    result.Add($"{counter++} => {item.GetType().Name}_{item.GetHashCode()}");
                }
                else
                {
                    result.Add($"{counter++} => null");
                }
            }
            return result;
        }
        #endregion diagram helpers

        #region helpers
        /// <summary>
        /// Creates diagram hierarchies for the given types.
        /// </summary>
        /// <param name="types">The types for which to create diagram hierarchies.</param>
        /// <returns>An enumerable of enumerables of types representing the diagram hierarchies.</returns>
        public static IEnumerable<IEnumerable<Type>> CreateDiagramHirarchies(IEnumerable<Type> types)
        {
            var result = new List<IEnumerable<Type>>();

            foreach (var type in types)
            {
                var classHirarchy = type.GetClassHierarchy();

                if (classHirarchy.Count() > 1)
                {
                    result.Add(classHirarchy);
                }
            }
            var calculatedHirarchies = new List<IEnumerable<Type>>();

            for (int i = 0; i < result.Count - 1; i++)
            {
                var commonSet = result[i];

                for (int j = i + 1; j < result.Count; j++)
                {
                    commonSet = commonSet.Intersect(result[j]);
                }

                if (commonSet.Count() > 1)
                {
                    for (int j = 0; j < result.Count; j++)
                    {
                        var currentSet = result[j];
                        var exceptSet = currentSet.Except(commonSet);

                        if (commonSet.All(e => currentSet.Any(c => e == c)) && exceptSet.Any())
                        {
                            var intersectSet = result[j].Intersect([commonSet.First()]);
                            var createSet = exceptSet.Union(intersectSet);

                            calculatedHirarchies.Add(createSet);
                        }
                        else
                        {
                            calculatedHirarchies.Add(result[j]);
                        }
                    }
                }
                else
                {
                    calculatedHirarchies.AddRange(result);
                }
                result.Clear();
                result.AddRange(calculatedHirarchies);
                calculatedHirarchies.Clear();
            }
            return result.Distinct();
        }
        /// <summary>
        /// Returns the name of the field.
        /// </summary>
        /// <param name="fieldInfo">The fieldInfo object representing the field.</param>
        /// <returns>The name of the field.</returns>
        public static string GetFieldName(FieldInfo fieldInfo)
        {
            string? result;

            if (fieldInfo.Name.Contains("k__BackingField"))
            {
                result = "_" + fieldInfo.Name.Betweenstring("<", ">");
                result = string.Concat(result[..2].ToLower(), result.AsSpan(2));
            }
            else
            {
                result = fieldInfo.Name;
            }
            return result;
        }
        /// <summary>
        /// Retrieves the parameters of a method and returns them as a string.
        /// </summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> object representing the method.</param>
        /// <returns>A string representation of the parameters in the format "ParameterType parameterName, ..." (comma separated).</returns>
        public static string GetParameters(MethodInfo methodInfo)
        {
            var counter = 0;
            var result = new StringBuilder();

            foreach (var item in methodInfo.GetParameters())
            {
                if (counter++ > 0)
                    result.Append(", ");

                result.Append($"{item.ParameterType.Name} {item.Name}");
            }
            return result.ToString();
        }
        /// <summary>
        /// Retrieves the value of a specified field.
        /// </summary>
        /// <param name="obj">The object whose field value will be retrieved.</param>
        /// <param name="fieldInfo">The FieldInfo object representing the field.</param>
        /// <returns>The value of the specified field.</returns>
        public static Object? GetFieldValue(Object obj, FieldInfo fieldInfo)
        {
            object? value;

            if (fieldInfo.IsStatic)
            {
                value = fieldInfo.GetValue(null);
            }
            else
            {
                value = fieldInfo.GetValue(obj);
            }
            return value;
        }
        /// <summary>
        /// Retrieves the state value of an object's field using a specified FieldInfo.
        /// </summary>
        /// <param name="obj">The object whose state value is to be retrieved.</param>
        /// <param name="fieldInfo">The FieldInfo of the field to retrieve the state value from.</param>
        /// <returns>The state value of the object's field.</returns>
        public static string GetStateValue(Object obj, FieldInfo fieldInfo)
        {
            return GetStateValue(obj, fieldInfo, 15);
        }
        /// <summary>
        /// Retrieves the state value of a specified field.
        /// </summary>
        /// <param name="obj">The object to retrieve the state value from.</param>
        /// <param name="fieldInfo">The field information of the desired field.</param>
        /// <param name="maxLength">The maximum length of the returned value.</param>
        /// <returns>The state value of the specified field as a string.</returns>
        public static string GetStateValue(Object obj, FieldInfo fieldInfo, int maxLength)
        {
            string? result;
            var value = GetFieldValue(obj, fieldInfo);

            if (fieldInfo.FieldType.IsValueType)
            {
                result = value?.ToString() ?? string.Empty;
            }
            else if (fieldInfo.FieldType == typeof(string))
            {
                result = $"\"{value}\"";
            }
            else if (value == null)
            {
                result = "null";
            }
            else
            {
                result = $"_{value.GetHashCode()}";
            }
            return result.Length > maxLength - 3 ? result[..(maxLength - 2)] + "..." : result;
        }
        #endregion helpers
    }
}

