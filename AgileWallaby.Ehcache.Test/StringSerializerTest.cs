#region License
/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is Ehcache .Net Wrapper..
 *
 * The Initial Developer of the Original Code is
 * James Webster.
 * Portions created by the Initial Developer are Copyright (C) 2010
 * the Initial Developer. All Rights Reserved.
 *
 * Contributor(s):
 *
 * ***** END LICENSE BLOCK ***** */
#endregion License

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgileWallaby.Ehcache.Test
{
    [TestClass]
    public class StringSerializerTest
    {
        [TestMethod]
        public void Content_Type_Is_Text_Plain()
        {
            var ser = (ISerializer) new StringSerializer();
            Assert.AreEqual("text/plain", ser.ContentType);
        }

        [TestMethod]
        public void Serialize_To_String()
        {
            var ser = new StringSerializer();
            var value = new object();
            MemoryStream str = new MemoryStream();
            ser.Serialize(str, value);
            str.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(str);
            Assert.AreEqual(value.ToString(), sr.ReadToEnd());
        }

        [TestMethod]
        public void Deserialize_From_String_Always_Returns_String()
        {
            var ser = new StringSerializer();
            MemoryStream st = new MemoryStream();
            StreamWriter str = new StreamWriter(st);
            str.Write(true);
            str.Write(false);
            str.Write(10);
            str.Write("hello");
            str.Flush();

            st.Seek(0, SeekOrigin.Begin);
            var valueString = ser.Deserialize<string>(st);
            Assert.AreEqual("TrueFalse10hello", valueString);

            st.Seek(0, SeekOrigin.Begin);
            var valueObject = ser.Deserialize(st);
            Assert.IsInstanceOfType(valueObject, typeof(string));
            Assert.AreEqual("TrueFalse10hello", valueObject);
        }

    }
}