using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace NetController
{
    public class NCUIContext : AbstractSingletonManager<NCUIContext>
    {
        public string IpAddress { get; private set; }

        [SerializeField] private ENCClientState activeOnState;

        private InputField inputField_ipAddress;
        
        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            inputField_ipAddress = GetComponentInChildren<InputField>();
            if (IS_NOT_NULL(inputField_ipAddress))
            {
                inputField_ipAddress.onEndEdit.AddListener(OnInputFieldUpdated);
            }
        }

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

        private void OnInputFieldUpdated(String newInput)
        {
            IpAddress = newInput;
        }
    }
}