//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************

using System;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Diagnostics;
using Cliver;
using System.Configuration;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Cliver
{
    abstract public class BaseService
    {
        public BaseService(int pulse_delay_in_secs = 60)
        {
            this.pulse_delay_in_secs = pulse_delay_in_secs;
        }
        readonly int pulse_delay_in_secs;

        public delegate void OnStateChanged();
        public event OnStateChanged StateChanged = null;

        public bool Running
        {
            set
            {
                if (value)
                {
                    if (service_t == null || !service_t.IsAlive)
                    {
                        service_t = Cliver.ThreadRoutines.StartTry(
                            service,
                            null,
                            () =>
                            {
                                service_t = null;
                                StateChanged?.Invoke();
                            }
                            );
                    }
                }
                else
                {
                    if (service_t != null && service_t.IsAlive)
                    {
                        Thread exiting_service_t = service_t;
                        service_t = null;
                        //exiting_service_t.Join();
                        while (exiting_service_t.IsAlive)
                            //Application.DoEvents();
                            Thread.Sleep(50);
                    }
                }
            }
            get
            {
                return service_t != null && service_t.IsAlive;
            }
        }
        Thread service_t = null;

        void service()
        {
            StateChanged?.Invoke();

            Starting();

            while (service_t != null)
            {
                DateTime next_check_time = DateTime.Now.AddSeconds(pulse_delay_in_secs);
                Pulse();
                while (service_t != null && next_check_time > DateTime.Now)
                    Thread.Sleep(100);
            }

            Exiting();
        }

        virtual protected void Starting()
        {

        }

        abstract protected void Pulse();

        virtual protected void Exiting()
        {

        }
    }

    public abstract class BaseService2
    {
        public delegate void OnStateChanged();
        public event OnStateChanged StateChanged = null;

        public bool Running
        {
            set
            {
                running = value;

                SetRunning(value);

                StateChanged?.Invoke();
            }
            get
            {
                return running;
            }
        }
        bool running = false;
        
        abstract protected void SetRunning(bool run);
    }
}