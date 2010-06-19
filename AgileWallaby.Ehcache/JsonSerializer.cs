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

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace AgileWallaby.Ehcache
{
    public class JsonSerializer : ISerializer
    {
        public const string JsonContentType = "application/json";
    
        public string ContentType
        {
            get { return JsonContentType; }
        }

        public object Deserialize(Stream s)
        {
            throw new NotImplementedException();
        }

        public void Serialize<T>(Stream s, T value) where T : class
        {
            var type = typeof(T);
            if (!IsDataContract(type))
            {
                throw new ArgumentException(string.Format("{0} is not a DataContract, cannot serialize to JSON.", type.FullName));
            }
            var ser = new DataContractJsonSerializer(type);
            ser.WriteObject(s, value);
        }


        public T Deserialize<T>(Stream s) where T : class
        {
            var type = typeof (T);
            if (!IsDataContract(type))
            {
                throw new ArgumentException(string.Format("{0} is not a DataContract, cannot deserialize from JSON.", type.FullName));
            }
            var ser = new DataContractJsonSerializer(typeof(T));
            return (T) ser.ReadObject(s);
        }

        private bool IsDataContract(Type type)
        {
            return type.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0;
        }

    }
}