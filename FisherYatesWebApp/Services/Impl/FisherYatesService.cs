using System;

namespace FisherYatesWebApp.Services.Impl
{
    public class FisherYatesService : IFisherYatesService
    {
        /// <summary>
        /// Used in Shuffle(T).
        /// </summary>
        private Random _random;// = new Random();

        /// <summary>
        /// Shuffle the array.
        /// </summary>
        /// <typeparam name="T">Array element type.</typeparam>
        /// <param name="array">Array to shuffle.</param>
        public void Shuffle<T>(T[] array, int? seed = null)
        {
            if (_random == null)
            {
                if (seed.HasValue)
                {
                    _random = new Random(seed.Value);
                }
                else
                {
                    _random = new Random();
                }
                
            }
            

            int n = array.Length;
            for (int i = 0; i < (n - 1); i++)
            {
                // Use Next on random instance with an argument.
                // ... The argument is an exclusive bound.
                //     So we will not go past the end of the array.
                int r = i + _random.Next(n - i);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }
        public string Shuffle(string input, int? seed = null)
        {
            var array = input.Split('-');
            Shuffle(array);
            return string.Join('-', array);
        }

    }
}
