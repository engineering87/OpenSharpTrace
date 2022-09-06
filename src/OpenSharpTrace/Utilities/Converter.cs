// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace OpenSharpTrace.Utilities
{
    public static class Converter
    {
        public static DateTime? ToDateTime(this object value)
        {
            if (value is DateTime)
                return (DateTime)value;
            return null;
        }
    }
}
