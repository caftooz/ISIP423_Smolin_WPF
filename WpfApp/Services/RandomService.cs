using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Services
{
    public sealed class RandomService
    {
        private static readonly Lazy<RandomService> _lazy =
            new Lazy<RandomService>(() => new RandomService());

        public static RandomService Instance => _lazy.Value;

        private readonly Random _random;

        private RandomService()
        {
            _random = new Random();
        }

        public int Next(int minValue, int maxValue) =>
            _random.Next(minValue, maxValue);

        public int Next(int maxValue) =>
            _random.Next(maxValue);

        public double NextDouble() =>
            _random.NextDouble();
    }
}
