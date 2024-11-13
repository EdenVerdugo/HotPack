using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// HotPack: => Crea un ciclo que ejecuta una funcion por cada elemento
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            foreach (var item in array)
            {
                action?.Invoke(item);
            }
        }

        /// <summary>
        /// HotPack: => Crea un ciclo que ejecuta una funcion para elemento y regresa una copia del elemento
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T[] Map<T>(this T[] array, Func<T, T> func)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = func(array[i]);
            }

            return array;
        }

        public static List<T> Map<T>(this List<T> array, Func<T, T> func)
        {
            for (int i = 0; i < array.Count; i++)
            {
                array[i] = func(array[i]);
            }

            return array;
        }
    }
}
