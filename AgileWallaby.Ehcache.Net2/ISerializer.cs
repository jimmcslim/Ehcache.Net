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

namespace AgileWallaby.Ehcache
{
    /// <summary>
    /// An interface for abstracting serialization and deserialization of objects to and from streams.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Indicates the content type that the implementation outputs.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Deserializes the generic type from the stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        T Deserialize<T>(Stream s) where T : class;

        object Deserialize(Stream s);

        /// <summary>
        /// Serializes the generic type to the stream.
        /// </summary>
        void Serialize<T>(Stream s, T value) where T: class;
    }
}