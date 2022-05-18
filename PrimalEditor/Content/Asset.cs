using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PrimalEditor.Content
{
    enum AssetType
    {
        Unknown,
        Animation,
        Audio,
        Material,
        Mesh,
        Skeleton,
        Texture,
    }
    abstract class Asset : ViewModeBase
    {
        public AssetType Type { get; private set; }
        public Asset(AssetType type)
        {
            Debug.Assert(type != AssetType.Unknown);
            Type = type;
        }
    }
}
