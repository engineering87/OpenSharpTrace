// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using NUnit.Framework;
using OpenSharpTrace.Utilities;

namespace OpenSharpTrace.Test.Utilities
{
    public class NetworkTests
    {
        [Test]
        public void CleanNotationAddress()
        {
            var address = "::ffff:10.18.1.1";
            var cleanAddress = Network.CleanNotationAddress(address);

            Assert.That(cleanAddress, Is.Not.Null);
            Assert.That(cleanAddress, Is.EqualTo("10.18.1.1"));
        }
    }
}
