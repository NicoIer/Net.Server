using System;

namespace Nico
{
    public class BindableProperty<T> where T : new()
    {
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value))
                {
                    return;
                }

                var tempValue = _value;
                _value = value;
                OnValueChanged?.Invoke(tempValue, _value);
            }
        }

        public T ValueWithoutNotify
        {
            get => _value;
            set
            {
                if (Equals(_value, value))
                {
                    return;
                }

                _value = value;
            }
        }

        public event Action<T, T> OnValueChanged;

        public BindableProperty(T value)
        {
            _value = value;
        }

        public BindableProperty()
        {
            _value = new T();
        }
        

        public override string ToString()
        {
            return _value.ToString();
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        //重写 T x = property; 的隐式转换 
        public static implicit operator T(BindableProperty<T> property)
        {
            return property.Value;
        }
        
        
    }
}