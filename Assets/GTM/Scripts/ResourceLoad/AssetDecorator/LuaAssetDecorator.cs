using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gtm
{
    public class LuaAssetDecorator : AssetDecorator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public void BeforeLoad(ref string name, ref Type type)
        {
            if (type == typeof(string) || type == typeof(byte[]))
            {
                type = typeof(TextAsset);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="asset"></param>
        public void AfterLoad(string name, Type type, ref object asset)
        {
            if (asset == null)
                return;

            if (type == typeof(string))
            {
                asset = (asset as TextAsset).text;
            }
            else if (type == typeof(byte[]))
            {
                asset = (asset as TextAsset).bytes;
            }
        }
    }
}

