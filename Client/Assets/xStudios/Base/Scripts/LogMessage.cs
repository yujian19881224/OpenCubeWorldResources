using System;
using UnityEngine;
using System.Collections.Generic;

namespace X
{
    public class LogMessage : MonoBehaviour
    {
        void Awake()
        {
            Application.logMessageReceived += OnLog;
        }


        Dictionary<string , int> dic = new Dictionary<string , int>();

        void OnLog( string message , string stacktrace , LogType type )
        {
            if ( type == LogType.Assert || type == LogType.Error || type == LogType.Exception )
            {
                if ( dic.ContainsKey( message ) )
                {
                    return;
                }

                dic[ message ] = 1;

                PHP.Instance.SaveLog( message + "\r\n" + stacktrace );
            }
        }
    }

}

