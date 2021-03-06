/*
 * Created by SharpDevelop.
 * User: lextm
 * Date: 2008/5/10
 * Time: 16:14
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
#pragma warning disable 1591
namespace Lextm.SharpSnmpLib.Tests
{
    [TestFixture]
    public class TestGauge32
    {
        [Test]
        public void TestConstructor()
        {
            byte[] buffer2 = new byte[] {01, 44};
            Gauge32 c2 = new Gauge32(buffer2);
            Assert.AreEqual(300, c2.ToUInt32());

            
            byte[] buffer1 = new byte[] {13};
            Gauge32 c1 = new Gauge32(buffer1);
            Assert.AreEqual(13, c1.ToUInt32());
            
            byte[] buffer3 = new byte[] {1, 17, 112};
            Gauge32 c3 = new Gauge32(buffer3);
            Assert.AreEqual(70000, c3.ToUInt32());
            
            byte[] buffer4 = new byte[] {1, 201, 195, 128};
            Gauge32 c4 = new Gauge32(buffer4);
            Assert.AreEqual(30000000, c4.ToUInt32());
            
            byte[] buffer5 = new byte[] {0, 255, 255, 255, 255};
            Gauge32 c5 = new Gauge32(buffer5);
            Assert.AreEqual(uint.MaxValue, c5.ToUInt32());
            
            byte[] buffer0 = new byte[] {0};
            Gauge32 c0 = new Gauge32(buffer0);
            Assert.AreEqual(uint.MinValue, c0.ToUInt32());
        }
        
        [Test]
        public void TestToBytes()
        {
            Gauge32 c0 = new Gauge32(0);
            Gauge32 r0 = (Gauge32)SnmpDataFactory.CreateSnmpData(c0.ToBytes());
            Assert.AreEqual(r0, c0);
            
            Gauge32 c5 = new Gauge32(uint.MaxValue);
            Gauge32 r5 = (Gauge32)SnmpDataFactory.CreateSnmpData(c5.ToBytes());
            Assert.AreEqual(r5, c5);
            
            Gauge32 c4 = new Gauge32(30000000);
            Gauge32 r4 = (Gauge32)SnmpDataFactory.CreateSnmpData(c4.ToBytes());
            Assert.AreEqual(r4, c4);
            
            Gauge32 c3 = new Gauge32(70000);
            Gauge32 r3 = (Gauge32)SnmpDataFactory.CreateSnmpData(c3.ToBytes());
            Assert.AreEqual(r3, c3);
            
            Gauge32 c1 = new Gauge32(13);
            Gauge32 r1 = (Gauge32)SnmpDataFactory.CreateSnmpData(c1.ToBytes());
            Assert.AreEqual(r1, c1);
            
            Gauge32 c2 = new Gauge32(300);
            Gauge32 r2 = (Gauge32)SnmpDataFactory.CreateSnmpData(c2.ToBytes());
            Assert.AreEqual(r2, c2);
        }
    }
}
#pragma warning restore 1591