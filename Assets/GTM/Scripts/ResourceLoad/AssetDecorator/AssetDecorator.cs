using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gtm
{
    /// <summary>
    /// Asset decorator.
    /// </summary>
    public interface AssetDecorator
    {
        /// <summary>
        /// Input the origin asset key reference and type reference.
        /// </summary>
        /// <param name="key">The asset key.</param>
        /// <param name="type">The asset type</param>
        void BeforeLoad(ref string key, ref Type type);

        /// <summary>
        /// Input origin asset key and type, and the asset reference.
        /// </summary>
        /// <param name="key">The asset key.</param>
        /// <param name="type">The asset type.</param>
        /// <param name="asset">The loaded asset.</param>
        void AfterLoad(string key, Type type, ref object asset);
    }
}
