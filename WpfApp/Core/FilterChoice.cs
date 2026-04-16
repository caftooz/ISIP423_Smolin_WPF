using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    public class FilterChoice<T>
    {
        public T Item { get; }
        private bool _isActive;
        private readonly Action _onChanged;

        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; _onChanged?.Invoke(); }
        }

        public FilterChoice(T item, Action onChanged)
        {
            Item = item;
            _onChanged = onChanged;
        }
    }
}
