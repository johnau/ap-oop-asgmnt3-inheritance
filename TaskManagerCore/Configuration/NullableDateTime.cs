using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagerCore.Configuration
{
    public class NullableDateTime : IComparable<NullableDateTime>, IComparable<DateTime>
    {
        public static NullableDateTime None = new NullableDateTime();

        public bool HasValue { get; private set; }
        private DateTime _dateTime = DateTime.MinValue;

        public DateTime Value
        {
            get
            {
                if (!HasValue)
                    throw new Exception("No value");
                
                return _dateTime;
            }
            set
            {
                SetValue(value);
            }
        }

        public NullableDateTime()
        {
            HasValue = false;
            _dateTime = DateTime.MinValue;
        }

        public NullableDateTime(DateTime dateTime)
        {
            HasValue = true;
            _dateTime = dateTime;
        }

        public void SetValue(DateTime value)
        {
            HasValue = true;
            _dateTime = value;
        }

        public void ClearValue()
        {
            HasValue = false;
            _dateTime = DateTime.MinValue;
        }

        public int CompareTo(NullableDateTime other)
        {
            if (!HasValue && !other.HasValue)
            {
                return 0;
            }
            else if (HasValue && other.HasValue)
            {
                return Value.CompareTo(other.Value);
            }
            else if (HasValue && !other.HasValue)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public int CompareTo(DateTime other)
        {
            if (HasValue)
            {
                return Value.CompareTo(other);
            }
            else
            {
                return -1;
            }
        }

        public bool IsGreaterThan(DateTime other)
        {
            if (!HasValue)
                return false; // This instance has no value, consider it not greater than other

            return _dateTime > other;
        }

        public bool IsLessThan(DateTime other)
        {
            if (!HasValue)
                return false; // This instance has no value, consider it not greater than other

            return _dateTime < other;
        }

        public bool IsEqualTo(DateTime other)
        {
            if (!HasValue)
                return false; // This instance has no value, consider it not greater than other

            return _dateTime == other;
        }
    }
}
