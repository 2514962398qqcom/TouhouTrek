using System.Linq;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

using ZMDFQ;
using ZMDFQ.Cards;

namespace Tests
{
    public class OtherTests
    {
        [UnityTest]
        public IEnumerator getTypeTest()
        {
            Assert.AreEqual(typeof(EV_E001), typeof(Card).Assembly.GetTypes().First(t => t.Name == nameof(EV_E001)));
            yield break;
        }
    }
}
