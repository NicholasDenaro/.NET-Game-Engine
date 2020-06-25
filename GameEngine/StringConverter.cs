using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GameEngine
{
    public static class StringConverter
    {
        public static string Serialize<T>(IEnumerable<T> list)
        {
            if (!list.Any())
            {
                return "[]";
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("[");

            foreach (T t in list)
            {
                IDescription idt = t as IDescription;
                if (idt != null)
                {
                    sb.Append(idt.Serialize());
                }
                else
                {
                    sb.Append(t);
                }
                sb.Append(",");
            }

            sb.Remove(sb.Length - 1, 1);

            sb.Append("]");

            return sb.ToString();
        }

        public static string Serialize<K, V>(Dictionary<K, V> dict)
        {
            if(!dict.Any())
            {
                return "{}";
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("{");

            foreach (KeyValuePair<K, V> kvp in dict)
            {
                sb.Append("(");
                sb.Append(kvp.Key);
                sb.Append(",");
                IDescription idv = kvp.Value as IDescription;
                if (idv != null)
                {
                    sb.Append(idv.Serialize());
                }
                else
                {
                    sb.Append(kvp.Value);
                }
                sb.Append(")");
                sb.Append(",");
            }

            sb.Remove(sb.Length - 1, 1);

            sb.Append("}");

            return sb.ToString();
        }

        public static List<string> DeserializeTokens(string state)
        {
            List<string> list = new List<string>();

            int start = 1;
            int pos = start;
            do
            {
                if (state[pos] == '(')
                {
                    pos = GetClosingIndex(state, start, '(', ')');
                    list.Add(state.Substring(start, pos - start));
                    start = pos + 1;
                }
                else if (state[pos] == '[')
                {
                    pos = GetClosingIndex(state, start, '[', ']');
                    list.Add(state.Substring(start, pos - start));
                    start = pos + 1;
                }
                else if (state[pos] == '{')
                {
                    pos = GetClosingIndex(state, pos, '{', '}');
                    list.Add(state.Substring(start, pos - start));
                    start = pos + 1;
                }
                else if (state[pos] == ',')
                {
                    list.Add(state.Substring(start, pos - start));
                    start = pos + 1;
                }
                else if (state[pos] == ')' || state[pos] == ']' || state[pos] == '}')
                {
                    list.Add(state.Substring(start, pos - start));
                    start = pos + 1;
                }
            }
            while (++pos < state.Length);

            return list;
        }

        public static List<T> Deserialize<T>(string state, Func<string, T> f)
        {
            List<T> list = new List<T>();

            int start = 1;
            int pos = start;
            do
            {
                if (state[pos] == ':')
                {
                    string type = state.Substring(start, pos - start);
                    Type t = GetType(type);
                    object o = Activator.CreateInstance(t);
                    IDescription idescr = o as IDescription;
                    if (idescr != null)
                    {
                        int st = pos + 1;
                        int end = GetClosingIndex(state, st, '{', '}');
                        idescr.Deserialize(state.Substring(st, end - st));
                        list.Add((T)idescr);
                        pos = end;
                    }
                }
                else if (state[pos] == '(')
                {
                    pos = GetClosingIndex(state, start, '(', ')');
                    list.Add(f(state.Substring(start, pos - start)));
                    start = pos + 1;
                }
                else if (state[pos] == '[')
                {
                    pos = GetClosingIndex(state, start, '[', ']');
                    list.Add(f(state.Substring(start, pos - start)));
                    start = pos + 1;
                }
                else if (state[pos] == '{')
                {
                    pos = GetClosingIndex(state, start, '{', '}');
                    list.Add(f(state.Substring(start, pos - start)));
                    start = pos + 1;
                }
                else if (state[pos] == ',' || state[pos] == ')' || state[pos] == ']' || state[pos] == '}')
                {
                    if (state.Substring(start, pos - start).Any())
                    {
                        list.Add(f(state.Substring(start, pos - start)));
                    }
                    start = pos + 1;
                }
            }
            while (++pos < state.Length);

            return list;
        }

        public static Dictionary<K, V> Deserialize<K, V>(string state, Func<string, K> fkey, Func<string, V> fVal)
        {
            Dictionary<K, V> dict = new Dictionary<K, V>();
            List<string> tokens = DeserializeTokens(state);

            int start = 1;
            int pos = start;
            do
            {
                pos = GetClosingIndex(state, start, '(', ')');
                tokens = DeserializeTokens(state.Substring(start, pos - start));
                if (tokens[1].Contains(":"))
                {
                    int indextype = tokens[1].IndexOf(':');
                    string type = tokens[1].Substring(0, indextype);
                    Type t = GetType(type);
                    object o = Activator.CreateInstance(t);
                    IDescription idescr = o as IDescription;
                    if (idescr != null)
                    {
                        idescr.Deserialize(tokens[1].Substring(indextype + 1));
                        dict.Add(fkey(tokens[0]), (V)idescr);
                    }
                }
                else
                {
                    dict.Add(fkey(tokens[0]), fVal(tokens[1]));
                }
                start = pos + 1;
            }
            while (++pos < state.Length);

            return dict;
        }

        private static Type GetType(string type)
        {
            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            foreach (var assembly in assemblies)
            {
                Type t = assembly.GetType(type, false);
                if (t != null)
                    return t;
            }

            throw new ArgumentException("Type " + type + " doesn't exist in the current app domain");
        }

        ////public static Dictionary<K, V> Deserialize<K, V>(string state) where V : IDescription
        ////{
        ////    Dictionary<K, V> list = new Dictionary<K, V>();

        ////    int start = 1;
        ////    int pos = start;
        ////    do
        ////    {
        ////        if (state[pos] == '(')
        ////        {
        ////            pos = GetClosingIndex(state, start, '(', ')');
        ////            T t = new T();
        ////            t.Deserialize(state.Substring(start, pos - start));
        ////            list.Add(t);
        ////            start = pos + 1;
        ////        }
        ////        else if (state[pos] == '[')
        ////        {
        ////            pos = GetClosingIndex(state, start, '[', ']');
        ////            T t = new T();
        ////            t.Deserialize(state.Substring(start, pos - start));
        ////            list.Add(t);
        ////            start = pos + 1;
        ////        }
        ////        else if (state[pos] == '{')
        ////        {
        ////            pos = GetClosingIndex(state, start, '{', '}');
        ////            T t = new T();
        ////            t.Deserialize(state.Substring(start, pos - start));
        ////            list.Add(t);
        ////            start = pos + 1;
        ////        }
        ////        else if (state[pos] == ',' || state[pos] == ')' || state[pos] == ']' || state[pos] == '}')
        ////        {
        ////            T t = new T();
        ////            t.Deserialize(state.Substring(start, pos - start));
        ////            list.Add(t);
        ////            start = pos + 1;
        ////        }
        ////    }
        ////    while (++pos < state.Length);

        ////    return list;
        ////}

        public static int GetClosingIndex(string str, int start, char open, char close)
        {
            int end = start;
            int count = 0;
            do
            {
                char ch = str[end++];
                if (ch == open)
                {
                    count++;
                }
                else if (ch == close)
                {
                    count--;
                }
            }
            while (count != 0);

            return end;
        }


    }
}
