/*
 * Created by SharpDevelop.
 * User: lextm
 * Date: 2008/5/3
 * Time: 20:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
#pragma warning disable 1591
namespace Lextm.SharpSnmpLib.Tests
{
    [TestFixture]
    public class TestNull
    {
        [Test]
        public void TestMethod()
        {
            Assert.AreEqual(false, new Null().Equals(null));
        }
    }
}
#pragma warning restore 1591