using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Globalization;
using VäderAppProd.Models;

namespace VäderAppProd
{
    public class VäderDataMap : ClassMap<WeatherRecord>
    {
        public VäderDataMap()
        {
            Map(m => m.Date).Name("Datum").TypeConverterOption.Format("yyyy-MM-dd H:mm");
            Map(m => m.Location).Name("Plats");
            Map(m => m.Temperature).Name("Temp").TypeConverter<NullableDoubleConverter>();
            Map(m => m.Humidity).Name("Luftfuktighet").TypeConverter<NullableDoubleConverter>();
        }
    }

    public class NullableDoubleConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, CsvHelper.IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            return double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? (double?)result : null;
        }
    }
}