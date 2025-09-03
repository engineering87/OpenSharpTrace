// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;

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
            if (string.IsNullOrEmpty(address)) return address;
            return address.StartsWith("::ffff:", StringComparison.OrdinalIgnoreCase)
                ? address.Substring(7)
                : address;
        }
    }
}
