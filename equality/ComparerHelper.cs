using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace equality
{
    public static class ComparerHelper
    {
        /// <summary>
        /// List of System Defined Types.
        /// </summary>
        public static readonly Type[] List;

        /// <summary>
        /// Static Constructor to initialize static List of Types.
        /// </summary>
        static ComparerHelper()
        {
            var types = new[]
                          {
                              typeof (Enum),
                              typeof (String),
                              typeof (Char),
                              typeof (Guid),

                              typeof (Boolean),
                              typeof (Byte),
                              typeof (Int16),
                              typeof (Int32),
                              typeof (Int64),
                              typeof (Single),
                              typeof (Double),
                              typeof (Decimal),

                              typeof (SByte),
                              typeof (UInt16),
                              typeof (UInt32),
                              typeof (UInt64),

                              typeof (DateTime),
                              typeof (DateTimeOffset),
                              typeof (TimeSpan),
                          };


            var nullTypes = from t in types
                            where t.IsValueType
                            select typeof(Nullable<>).MakeGenericType(t);

            List = types.Concat(nullTypes).ToArray();
        }

        /// <summary>
        /// Extension on Type to verify if current Type is System defined type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsSystemDefined(this Type type)
        {
            if (type.IsPrimitive || List.Any(x => x.IsAssignableFrom(type)))
                return true;

            var nut = Nullable.GetUnderlyingType(type);
            return nut != null && nut.IsEnum;
        }


        /// <summary>
        ///  True if all values and complex values match.Else False.
        /// </summary>
        /// <param name="first">this current Enumerable/List</param>
        /// <param name="second">Enumerable/List to be comapared with.</param>
        /// <returns>True/False.</returns>
        public static bool CompareEnumerableByValue(this IEnumerable<object> first, IEnumerable<object> second)
        {
            bool isEqual = true;
            bool isExitedInner = false;
            bool isSystemType = false;
            bool ifSystemType = false;

            Type firsttype = first.GetType();
            Type secondtype = second.GetType();

            PropertyInfo[] PropertyInfo = GetPropDetails(first, firsttype, ref ifSystemType);
            PropertyInfo[] PropertyInfos = GetPropDetails(second, secondtype, ref isSystemType);
            

            if (first.Count() == second.Count())
            {
                var count = first.Count();
                for (int i = 0; i < count; i++)
                {
                    if (isSystemType && ifSystemType)
                    {
                        object Tpropvalue = first.ElementAt(i);
                        object Upropvalue = second.ElementAt(i);
                        if (!Tpropvalue.Equals(Upropvalue))
                        {
                            isEqual = false;

                            Console.WriteLine("Difference found for Value :{0} and {1}", first.ElementAt(i).ToString(), second.ElementAt(i).ToString());
                        }
                    }
                    else
                    {
                        foreach (PropertyInfo prop in PropertyInfo)
                        {
                            Type ftypeofprop = first.ElementAt(i).GetType().GetProperty(prop.Name).PropertyType;
                            Type stypeofprop = second.ElementAt(i).GetType().GetProperty(prop.Name).PropertyType;
                            //modification may be needed here for types other than list need to make more generic.
                            if (ftypeofprop.IsGenericType && stypeofprop.IsGenericType && (ftypeofprop.GetGenericTypeDefinition() == typeof(List<>)))
                            {
                                Type typ = typeof(object);
                                IEnumerable<object> Tpropvalue = ((IEnumerable)first.ElementAt(i).GetType().GetProperty(prop.Name).GetValue(first.ElementAt(i))).Cast<object>();
                                IEnumerable<object> Upropvalue = ((IEnumerable)second.ElementAt(i).GetType().GetProperty(prop.Name).GetValue(second.ElementAt(i))).Cast<object>();
                                if ((Tpropvalue != null && Upropvalue == null) || (Tpropvalue == null && Upropvalue != null))
                                    isEqual = false;
                                else
                                    isEqual = CompareEnumerableByValue(Tpropvalue, Upropvalue);
                            }

                            else
                            {
                                Type typ = typeof(object);
                                if (typeof(IEnumerable).IsAssignableFrom(ftypeofprop) && typeof(IEnumerable).IsAssignableFrom(stypeofprop) && ftypeofprop != typeof(string))
                                {
                                    object[] Tapropvalue = ((IEnumerable)first.ElementAt(i).GetType().GetProperty(prop.Name).GetValue(first.ElementAt(i))).Cast<object>().Select(x => x == null ? x : x.ToString()).ToArray();
                                    object[] Uapropvalue = ((IEnumerable)second.ElementAt(i).GetType().GetProperty(prop.Name).GetValue(second.ElementAt(i))).Cast<object>().Select(x => x == null ? x : x.ToString()).ToArray();
                                    if ((Tapropvalue != null && Uapropvalue == null) || (Tapropvalue == null && Uapropvalue != null))
                                    {
                                        isEqual = false;
                                        Console.WriteLine("Difference found for Value :{0} and {1}", Tapropvalue, Uapropvalue);
                                    }
                                    else if (Tapropvalue != null && !Tapropvalue.SequenceEqual(Uapropvalue))
                                    {
                                        isEqual = false;
                                        Console.WriteLine("Difference found for Value :{0} and {1}", Tapropvalue, Uapropvalue);
                                    }

                                }
                                else
                                {
                                    object Tpropvalue = Convert.ChangeType(first.ElementAt(i).GetType().GetProperty(prop.Name).GetValue(first.ElementAt(i)), typ);
                                    object Upropvalue = Convert.ChangeType(second.ElementAt(i).GetType().GetProperty(prop.Name).GetValue(second.ElementAt(i)), typ);
                                    if ((Tpropvalue != null && Upropvalue == null) || (Tpropvalue == null && Upropvalue != null))
                                    {
                                        isEqual = false;
                                        Console.WriteLine("Difference found for Value :{0} and {1}", Tpropvalue, Upropvalue);
                                    }

                                    else if (Tpropvalue != null && !Tpropvalue.Equals(Upropvalue))
                                    {
                                        isEqual = false;
                                        Console.WriteLine("Difference found for Value :{0} and {1}", Tpropvalue, Upropvalue);
                                    }
                                }

                            }
                            if (!isEqual)
                            {
                                Console.WriteLine("Difference found for Property :" + prop.Name);
                                isExitedInner = true; break;
                            }
                        }
                    }

                    if (!isEqual || isExitedInner)
                    {
                        break;
                    }
                }
            }
            else
            {
                isEqual = false;
            }

            return isEqual;
        }

        /// <summary>
        /// Gets the list of Properties 
        /// </summary>
        /// <param name="first">List of object</param>
        /// <param name="firsttype">Type Param</param>
        /// <param name="ifSystemType">ifSystemType flag to set value.</param>
        /// <returns>array of PropertyInfo[] </returns>
        private static System.Reflection.PropertyInfo[] GetPropDetails(IEnumerable<object> first, Type firsttype, ref bool ifSystemType)
        {
            PropertyInfo[] PropertyInfo;
            if (firsttype.IsGenericType && (firsttype.GetGenericTypeDefinition() == typeof(List<>)))
            {
                Type firtype = first.GetType().GetGenericArguments()[0];
                ifSystemType = firtype.IsSystemDefined();
                PropertyInfo = firtype.GetProperties();
            }
            else
            {
                var temp = firsttype;
                PropertyInfo = temp.GetProperties();
            }
            return PropertyInfo;
        }

    }
}
