﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Net;
#pragma warning disable 1591
namespace Lextm.SharpSnmpLib.Tests
{
    [TestFixture]
    public class TestTrapMessage
    {
        [Test]
        public void TestSendTrap()
        {
            TrapV1Message message = new TrapV1Message(VersionCode.V1, 
                                                      IPAddress.Parse("127.0.0.1"),
                                                      "public",
                                                      new ObjectIdentifier(new uint[] {1,3,6}),
                                                      GenericCode.AuthenticationFailure,
                                                      0, 
                                                      0,
                                                      new List<Variable>());
            byte[] bytes = message.ToBytes();
            ISnmpMessage parsed = MessageFactory.ParseMessage(bytes);
            Assert.AreEqual(SnmpType.TrapV1Pdu, parsed.TypeCode);
            TrapV1Message m = (TrapV1Message)parsed;
            Assert.AreEqual(GenericCode.AuthenticationFailure, m.Generic);
            Assert.AreEqual(0, m.Specific);
            Assert.AreEqual("public", m.Community);
            Assert.AreEqual(IPAddress.Parse("127.0.0.1"), m.AgentAddress);
            Assert.AreEqual(new uint[] {1,3,6}, m.Enterprise.ToNumerical());
            Assert.AreEqual(0, m.TimeStamp);
            Assert.AreEqual(0, m.Variables.Count);
        }
        
        [Test]
        public void TestParseNoVarbind()
        {
            byte[] buffer = Resource.novarbind;
            ISnmpMessage m = MessageFactory.ParseMessage(buffer);
            TrapV1Message message = (TrapV1Message)m;
            Assert.AreEqual(GenericCode.EnterpriseSpecific, message.Generic);
            Assert.AreEqual(12, message.Specific);
            Assert.AreEqual("public", message.Community);
            Assert.AreEqual(IPAddress.Parse("127.0.0.1"), message.AgentAddress);
            Assert.AreEqual(new uint[] { 1,3 }, message.Enterprise.ToNumerical());
            Assert.AreEqual(16352, message.TimeStamp);
            Assert.AreEqual(0, message.Variables.Count);            
        }
        
        [Test]
        public void TestParseOneVarbind()
        {
            byte[] buffer = Resource.onevarbind;
            TrapV1Message message = (TrapV1Message)MessageFactory.ParseMessage(buffer);
            Assert.AreEqual(1, message.Variables.Count);
            Assert.AreEqual(new uint[] { 1, 3, 6, 1, 4, 1, 2162, 1000, 2 }, message.Enterprise.ToNumerical());
            Assert.AreEqual("TrapTest", message.Variables[0].Data.ToString());
            Assert.AreEqual(new uint[] {1,3,6,1,4,1,2162,1001,21,0}, message.Variables[0].Id.ToNumerical());
        }
        
        [Test]
        public void TestParseTwoVarbinds()
        {
            byte[] buffer = Resource.twovarbinds;
            TrapV1Message message = (TrapV1Message)MessageFactory.ParseMessage(buffer);
            Assert.AreEqual(2, message.Variables.Count);
            Assert.AreEqual("TrapTest", message.Variables[0].Data.ToString());
            Assert.AreEqual(new uint[] {1,3,6,1,4,1,2162,1001,21,0}, (uint[])message.Variables[0].Id.ToNumerical());
            Assert.AreEqual("TrapTest", message.Variables[1].Data.ToString());
            Assert.AreEqual(new uint[] {1,3,6,1,4,1,2162,1001,22,0}, (uint[])message.Variables[1].Id.ToNumerical());
        }

        [Test]
        public void TestParseFiveVarbinds()
        {
            byte[] buffer = Resource.fivevarbinds;
            TrapV1Message message = (TrapV1Message)MessageFactory.ParseMessage(buffer);
            Assert.AreEqual(5, message.Variables.Count);
            Assert.AreEqual("TrapTest5", message.Variables[4].Data.ToString());
            Assert.AreEqual(new uint[] { 1, 3, 6, 1, 4, 1, 2162, 1001, 25, 0 }, (uint[])message.Variables[4].Id.ToNumerical());
        }
    }
}
#pragma warning restore 1591

