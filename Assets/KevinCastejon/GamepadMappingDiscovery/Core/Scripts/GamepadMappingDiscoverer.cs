using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinCastejon.GamepadMappingDiscovery
{
    public class GamepadMappingDiscoverer : MonoBehaviour
    {
        [SerializeField] private float[] _axes = new float[28];
        [SerializeField] private bool[] _buttons = new bool[20];
        [SerializeField] private bool _showOnlyActives;

        private void Update()
        {
            for (int i = 0; i < 27; i++)
            {
                _axes[i] = Input.GetAxisRaw("GamepadMappingDiscovery_Axis" + (i + 1));
            }
            for (int i = 0; i < 20; i++)
            {
                _buttons[i] = Input.GetKey((KeyCode)(330 + i));
            }
        }
    }
}
