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
        [SerializeField] private InputField iF_ipAddress;
        [SerializeField] private GameObject c_buttons;
        


        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            iF_ipAddress = GetComponentInChildren<InputField>();
            if (IS_NOT_NULL(iF_ipAddress))
            {
                iF_ipAddress.text = "192.168.";
                iF_ipAddress.onEndEdit.AddListener(OnInputFieldUpdated);
            }
        }

        //protected override void InitializeEventsCallbacks()
        //{
        //    base.InitializeEventsCallbacks();

        //    NCClient.Instance.StateUpdated += On_NCClient_StateUpdated;
        //}

        private void On_NCClient_StateUpdated(ENCClientState newState)
        {
            //if (newState == activeOnState)
            //{
            //    c_buttons.SetActive(true);
            //}
            //else
            //{
            //    c_buttons.SetActive(false);
            //}
        }

        private void OnInputFieldUpdated(String newInput)
        {
            IpAddress = newInput;
        }
    }
}