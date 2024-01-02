//Copyright (C) Silver Wolf 2023-2024, All Rights Reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using PluginAPI.Core.Attributes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginAPI.Events;
using MEC;
using CommandSystem.Commands.RemoteAdmin.Warhead;
using PluginAPI.Core;
using System.Diagnostics.Tracing;

namespace AutoNuke
{
    public class AutoNuke
    {
        public static AutoNuke Instance;
        [PluginConfig] public MainConfig config;
        [PluginEntryPoint("AutoNuke","1.0.0","AutoNuke","Silver Wolf")]
        public void OnEnabled()
        {
            Instance = this;
            EventManager.RegisterEvents<EventHandler>(this);
        }
    }
    public class EventHandler
    {
        public CoroutineHandle Sw;
        [PluginEvent(PluginAPI.Enums.ServerEventType.RoundRestart)]
        public void OnRoundRestart()
        {
            Timing.KillCoroutines(Sw);
        }

        [PluginEvent(PluginAPI.Enums.ServerEventType.RoundStart)]
        public void OnRoundStart(RoundStartEvent ev)
        {
            Sw = Timing.RunCoroutine(Detonate());
        }

        public IEnumerator<float> Detonate()
        {
            yield return Timing.WaitForSeconds(AutoNuke.Instance.config.Time);
            if (Warhead.IsDetonated)
            {
                yield break;
            }
            if (!Warhead.IsDetonationInProgress)
            {
                Warhead.Start();
            }
            Warhead.IsLocked = true;
            Warhead.LeverStatus = true;
            Warhead.WarheadUnlocked = true;
            Server.SendBroadcast(AutoNuke.Instance.config.ActivateBroadcast, 10, Broadcast.BroadcastFlags.Normal, true);
        }
    }
    public class MainConfig
    {
        [Description("系统核弹引爆时间（以秒为单位）")]
        public int Time { get; set; } = 1200;
        [Description("系统核弹启动公告")]
        public string ActivateBroadcast { get; set; } = "注意，系统核弹已经启动";
    }
}
