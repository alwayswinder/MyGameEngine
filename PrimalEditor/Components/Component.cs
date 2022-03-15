using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace PrimalEditor.Components
{
    interface IMSComponent { }
    [DataContract]
    abstract public class Component : ViewModeBase
    {
        [DataMember]
        public GameEntity Owner { get; private set; }
        public Component(GameEntity entity)
        {
            Debug.Assert(entity != null);
            Owner = entity;
        }
    }
    abstract class MSComponent<T>:ViewModeBase, IMSComponent where T:Component
    { }
}
