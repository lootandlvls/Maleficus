using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NetController
{
    public class NCUIContext : AbstractSingletonManager<NCUIContext>
    {
        [SerializeField] private ENCClientState activeOnState;

        protected override void InitializeEventsCallbacks()
        {
            base.InitializeEventsCallbacks();

            NCClient.Instance.StateUpdated += On_NCClient_StateUpdated;
        }

        private void On_NCClient_StateUpdated(ENCClientState newState)
        {
            if (newState == activeOnState)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}