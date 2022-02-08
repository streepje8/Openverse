using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Openverse.ScriptableObjects { 
    public class OpenverseSettings : ScriptableObject
    {
        public GameObject playerPrefab;
        public ushort ServerPort;
        public ushort playerLimit;

    }
}
