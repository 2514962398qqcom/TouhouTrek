using System.Linq;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
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
        [UnityTest]
        public IEnumerator asyncTest()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                while (true)
                {
                    if (cts.IsCancellationRequested)
                        return;
                    Debug.Log(Time.time);
                }
            }, cts.Token);
            yield return new WaitForSeconds(10);
            cts.Cancel();
        }
    }
}
