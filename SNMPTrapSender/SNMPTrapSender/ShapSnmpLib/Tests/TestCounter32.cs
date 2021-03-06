﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
#pragma warning disable 1591
namespace Lextm.SharpSnmpLib.Tests
{
    [TestFixture]
    public class TestCounter32
    {
        [Test]
        public void TestConstructor()
        {
            byte[] buffer2 = new byte[] {01, 44};
            Counter32 c2 = new Counter32(buffer2);
            Assert.AreEqual(300, c2.ToUInt32());

            
            byte[] buffer1 = new byte[] {13};
            Counter32 c1 = new Counter32(buffer1);
            Assert.AreEqual(13, c1.ToUInt32());
            
            byte[] buffer3 = new byte[] {1, 17, 112};
            Counter32 c3 = new Counter32(buffer3);
            Assert.AreEqual(70000, c3.ToUInt32());
            
            byte[] buffer4 = new byte[] {1, 201, 195, 128};
            Counter32 c4 = new Counter32(buffer4);
            Assert.AreEqual(30000000, c4.ToUInt32());
            
            byte[] buffer5 = new byte[] {0, 255, 255, 255, 255};
            Counter32 c5 = new Counter32(buffer5);
            Assert.AreEqual(uint.MaxValue, c5.ToUInt32());
            
            byte[] buffer0 = new byte[] {0};
            Counter32 c0 = new Counter32(buffer0);
            Assert.AreEqual(uint.MinValue, c0.ToUInt32());
        }
        
        [Test]
        public void TestToBytes()
        {
            Counter32 c0 = new Counter32(0);
            Counter32 r0 = (Counter32)SnmpDataFactory.CreateSnmpData(c0.ToBytes());
            Assert.AreEqual(r0, c0);
            
            Counter32 c5 = new Counter32(uint.MaxValue);
            Counter32 r5 = (Counter32)SnmpDataFactory.CreateSnmpData(c5.ToBytes());
            Assert.AreEqual(r5, c5);
            
            Counter32 c4 = new Counter32(30000000);
            Counter32 r4 = (Counter32)SnmpDataFactory.CreateSnmpData(c4.ToBytes());
            Assert.AreEqual(r4, c4);
            
            Counter32 c3 = new Counter32(70000);
            Counter32 r3 = (Counter32)SnmpDataFactory.CreateSnmpData(c3.ToBytes());
            Assert.AreEqual(r3, c3);
            
            Counter32 c1 = new Counter32(13);
            Counter32 r1 = (Counter32)SnmpDataFactory.CreateSnmpData(c1.ToBytes());
            Assert.AreEqual(r1, c1);
            
            Counter32 c2 = new Counter32(300);
            Counter32 r2 = (Counter32)SnmpDataFactory.CreateSnmpData(c2.ToBytes());
            Assert.AreEqual(r2, c2);
        }
    }
}
#pragma warning restore 1591