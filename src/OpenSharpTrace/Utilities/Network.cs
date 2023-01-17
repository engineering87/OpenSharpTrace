// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace OpenSharpTrace.Utilities
{
    public static class Network
    {
        /// <summary>
        /// Clean the mixed notation ipv4 and ipv6 addresses.
        /// </summary>
        /// <param name="address">The ip adress with mixed notation</param>
        /// <returns></returns>
        public static string CleanNotationAddress(string address)
        {            
            if (address?.Substring(0, 7) == "::ffff:")
            {
                return address.Substring(7);
            }
            return address;
        }
    }
}
