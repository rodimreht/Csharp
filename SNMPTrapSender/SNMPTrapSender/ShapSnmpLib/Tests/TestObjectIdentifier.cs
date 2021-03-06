﻿using System;
using System.IO;
using NUnit.Framework;
#pragma warning disable 1591
namespace Lextm.SharpSnmpLib.Tests
{
    [TestFixture]
    public class TestObjectIdentifier
    {
        [Test]
        public void TestToOid()
        {
            ObjectIdentifier oid = new ObjectIdentifier(new byte[] { 0x2B, 0x06, 0x01, 0x04, 0x01, 0x90, 0x72, 0x87, 0x68, 0x02 });
            Assert.AreEqual(new uint[] { 1, 3, 6, 1, 4, 1, 2162, 1000, 2 }, oid.ToNumerical());
        }
        
        [Test]
        public void TestToBytes()
        {
        	uint[] expected = new uint[] {1,3,6,1,4,1,2162,1000,2};
        	ObjectIdentifier oid = new ObjectIdentifier(expected);
        	byte[] result = oid.ToBytes();
        	MemoryStream m = new MemoryStream(result);
        	ISnmpData data = SnmpDataFactory.CreateSnmpData(m);
        	Assert.AreEqual(SnmpType.ObjectIdentifier, data.TypeCode);
        	ObjectIdentifier o = (ObjectIdentifier)data;
        	Assert.AreEqual(expected, o.ToNumerical());
        }        
    }
}
#pragma warning restore 1591

