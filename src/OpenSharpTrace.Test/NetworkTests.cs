// (c) 2022-2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using NUnit.Framework;
using OpenSharpTrace.Utilities;

namespace OpenSharpTrace.Test
{
    [TestFixture]
    public class NetworkTests
    {
        [Test]
        public void CleanNotationAddress_Null_ReturnsNull()
        {
            Assert.That(Network.CleanNotationAddress(null), Is.Null);
        }

        [Test]
        public void CleanNotationAddress_Empty_ReturnsEmpty()
        {
            Assert.That(Network.CleanNotationAddress(string.Empty), Is.EqualTo(string.Empty));
        }

        [TestCase("127.0.0.1", "127.0.0.1")]
        [TestCase("::ffff:192.168.0.5", "192.168.0.5")]
        [TestCase("::FFFF:10.0.0.1", "10.0.0.1")]
        public void CleanNotationAddress_NormalizesIPv6Mapped(string input, string expected)
        {
            Assert.That(Network.CleanNotationAddress(input), Is.EqualTo(expected));
        }
    }
}