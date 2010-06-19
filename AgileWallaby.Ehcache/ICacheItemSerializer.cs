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
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

namespace AgileWallaby.Ehcache
{
    public interface ICacheItemSerializer
    {
        string Serialize<T>(T value);
        T Deserialize<T>(string serializedValue);
    }

    public class JsonCacheItemSerializer: ICacheItemSerializer
    {
        public string Serialize<T>(T value)
        {
            var ser = new DataContractJsonSerializer(typeof (T));
            var st = new MemoryStream();
            ser.WriteObject(st, value);
            return new StreamReader(st).ReadToEnd();
        }

        public T Deserialize<T>(string serializedValue)
        {
            var ser = new DataContractJsonSerializer(typeof (T));
            var st = new MemoryStream();
            var sr = new StreamWriter(st);
            sr.Write(serializedValue);
            sr.Close();
            return (T) ser.ReadObject(st);
        }
    }

    public class XmlCacheItemSerializer: ICacheItemSerializer
    {
        public string Serialize<T>(T value)
        {
            var ser = new XmlSerializer(typeof (T));
            TextWriter tw = new StringWriter();
            ser.Serialize(tw, value);
            tw.Close();
            return tw.ToString();
        }

        public T Deserialize<T>(string serializedValue)
        {
            var ser = new XmlSerializer(typeof (T));
            using (var sr = new StringReader(serializedValue))
            {
                return (T) ser.Deserialize(sr);
            }
        }
    }
}
