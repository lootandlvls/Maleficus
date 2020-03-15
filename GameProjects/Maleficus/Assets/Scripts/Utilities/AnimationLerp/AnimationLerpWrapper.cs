using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class AnimationLerpWrapper<A> : BNJMOBehaviour
    {
        public A Value { get; set; }
    }
}