using System.Globalization;

namespace CodeChallenge.Server.Helpers
{
    public class DataParser
    {

        public static int IntParser(int minAcceptedValue, string intText)
        {
            bool isInt = int.TryParse(intText, out int value);

            if(!isInt || value < minAcceptedValue)
            {
                return int.MinValue;
            }

            return value;
        }
        public static bool DateParser(string format, string dateText)
        {
            return DateOnly.TryParseExact(
                    dateText,
                    format,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var dt); 
        }

        public static double DoubleParser(double minAcceptedValue, string doubleText)
        {
            bool isDouble = double.TryParse(doubleText, out double value);

            if (!isDouble || value < minAcceptedValue)
            {
                return double.MinValue;
            }

            return value;
        }
    }
}
