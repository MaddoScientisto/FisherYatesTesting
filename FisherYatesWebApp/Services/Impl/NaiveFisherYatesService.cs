using System;

namespace FisherYatesWebApp.Services.Impl
{
    public class NaiveFisherYatesService : IFisherYatesService
    {
        /// <summary>
        /// Used in Shuffle(T).
        /// </summary>
        private Random _random;// = new Random();

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
            for (int i = 0; i < n; i++)
            {

                int r = _random.Next(n);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }
    }
}
