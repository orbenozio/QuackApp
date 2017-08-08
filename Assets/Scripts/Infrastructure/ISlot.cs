using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public delegate void SlotDelegate(UnityEngine.Object obj);

namespace Assets.Scripts.Infrastructure
{
    public interface ISlot
    {
        event SlotDelegate SlotClickEvent;
    }
}
