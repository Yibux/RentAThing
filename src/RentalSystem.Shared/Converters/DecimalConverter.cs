using Google.Cloud.Firestore;
using System;

namespace RentalSystem.Shared.Converters
{
    public class DecimalConverter : IFirestoreConverter<decimal>
    {
        public decimal FromFirestore(object value)
        {
            if (value is double d) return (decimal)d;
            if (value is long l) return (decimal)l;
            if (value is string s && decimal.TryParse(s, out var result)) return result;
            return 0m;
        }

        public object ToFirestore(decimal value)
        {
            return (double)value;
        }
    }
}