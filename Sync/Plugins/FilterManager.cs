﻿using Sync.MessageFilter;
using Sync.Source;
using Sync.Tools;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
namespace Sync.Plugins
{
    /// <summary>
    /// 提供对消息的发送、管理、过滤功能
    /// </summary>
    public class FilterManager
    {
        Dictionary<Type, List<IFilter>> filters;
<<<<<<< HEAD
        SyncConnector parent;

        public FilterManager(SyncConnector p)
        {
            parent = p;
			filters = new Dictionary<Type, List<IFilter>>();
			
            //filters.Add(typeof(IOsu), new List<FilterBase>());
            //filters.Add(typeof(IDanmaku), new List<FilterBase>());

            //为弹幕创建消息管理器，管控发往irc的消息
            MessageManager.SetSendMessageAction((string userName,string message) =>{
                parent.GetIRC().sendRawMessage(userName, message);
            });

            MessageManager.LimitLevel = 3;

            MessageManager.Init(this);
			
            
=======
        public FilterManager()
        {
            filters = new Dictionary<Type, List<IFilter>>();
>>>>>>> deliay
            AddSource<ISourceOsu>();
            AddSource<ISourceDanmaku>();
            AddSource<ISourceOnlineChange>();
            AddSource<ISourceGift>();
        }

        private void AddSource<T>()
        {
            filters.Add(typeof(T), new List<IFilter>());
        }

        public IEnumerable<KeyValuePair<Type, IFilter>> GetFiltersEnum()
        {
            foreach (var list in filters)
            {
                foreach(var item in list.Value)
                {
                    yield return new KeyValuePair<Type, IFilter>(list.Key, item);
                }
            }
        }


        public int Count { get { return filters.Sum(p => p.Value.Count); } }

        internal void PassFilterDanmaku(ref MessageBase msg)
        {
            PassFilter<ISourceDanmaku>(ref msg);
        }

        internal void PassFilterOSU(ref MessageBase msg)
        {
            PassFilter<ISourceOsu>(ref msg);
        }

        internal void PassFilterGift(ref MessageBase msg)
        {
            PassFilter<ISourceGift>(ref msg);
        }

        internal void PassFilterOnlineChange(ref MessageBase msg)
        {
            PassFilter<ISourceGift>(ref msg);
        }

        private void PassFilter<T>(ref MessageBase msg)
        {
            PassFilter(typeof(T), ref msg);
        }

        private void PassFilter(Type identify, ref MessageBase msg)
        {
            foreach (var filter in filters[identify])
            {
                filter.onMsg(ref msg);
            }
        }

        public void AddFilter(IFilter filter)
        {
            foreach (var i in filter.GetType().GetInterfaces())
            {
                if(filters.ContainsKey(i))
                {
                    filters[i].Add(filter);
                }
            }
        }

        public void deleteFilter(IFilter filter)
        {
            foreach (var i in filter.GetType().GetInterfaces())
            {
                if (filters.ContainsKey(i))
                {
                    filters[i].Remove(filter);
                }
            }
        }

        public void AddFilters(params IFilter[] filters)
        {
            foreach (IFilter filter in filters)
            {
                AddFilter(filter);
            }
        }

<<<<<<< HEAD
        public void RaiseMessage<Source>(MessageBase msg)
        {
            RaiseMessage(typeof(Source), msg);
        }

        /// <summary>
        /// 产生一个消息  
        /// 该消息会被按顺序编译
        /// </summary>
        /// <param name="msgType">消息类型，此处传IOsu(来自IRC)和IDanmaku(来自弹幕)</param>
        /// <param name="msg">具体消息实例</param>
        private void RaiseMessage(Type msgType, MessageBase msg)
        {
            MessageBase newMsg = msg;

            //消息来自弹幕
            if (msgType == typeof(ISourceDanmaku))
            {
                PassFilterDanmaku(ref newMsg);
                //将消息过滤一遍插件之后，判断是否取消消息（消息是否由插件自行处理拦截）
                if (newMsg.cancel) return;
                else
                {
                    //parent.GetIRC().sendRawMessage(Configuration.TargetIRC, newMsg.user + newMsg.message.RawText);
                    MessageManager.PostIRCMessage(Configuration.TargetIRC, newMsg);
                }
                return;
            }

            //消息来自osu!IRC
            else if(msgType == typeof(ISourceOsu))
            {
                PassFilterOSU(ref newMsg);
                //同上
                if (newMsg.cancel) return;
                else
                {
                    //发信用户为设置的目标IRC
                    if(newMsg.user.RawText == Configuration.TargetIRC)
                    {
                        if(parent.GetSource() is ISendable)
                        {
                            ISendable sender = parent.GetSource() as ISendable;
                            if(sender.LoginStauts())
                            {
                                sender.Send(newMsg.message);
                            }
                        }
                    }
                    //其他用户则转发到目标IRC
                    else
                    {
                        //parent.GetIRC().sendRawMessage(Configuration.TargetIRC, newMsg.user + newMsg.message.RawText);
                        MessageManager.PostIRCMessage(Configuration.TargetIRC, newMsg);
                    }
                    
                }
                return;
            }

            //消息来自弹幕礼物
            else if(msgType == typeof(ISourceGift))
            {
                PassFilterGift(ref newMsg);
                if (newMsg.cancel) return;
                else
                {
                    parent.GetIRC().sendRawMessage(Configuration.TargetIRC, IRC.IRCClient.CONST_ACTION_FLAG + newMsg.message);
                }
            }

            //观看人数变化
            else if(msgType == typeof(ISourceOnlineChange))
            {
                PassFilterOnlineChange(ref newMsg);
                if (newMsg.cancel) return;
                else
                {
                    parent.GetIRC().sendRawMessage(Configuration.TargetIRC, IRC.IRCClient.CONST_ACTION_FLAG + newMsg.message);
                }
            }

        }
=======
>>>>>>> deliay
    }

    /// <summary>
    /// 消息管理器,管制消息的发送
    /// </summary>
    public class MessageManager
    {
        class SendFilter : IFilter, ISourceDanmaku
        {
            public new void onMsg(ref MessageBase msg)
            {
                if (!msg.message.RawText.StartsWith("?send"))
                    msg.cancel = true;
            }
        }

        public enum PeekOption
        {
            Force_All,
            Only_Send_Command,
            Auto
        }

        private static SendFilter sendFilter = new SendFilter();

        private static PeekOption option = PeekOption.Auto;
        public static PeekOption Option
        {
            set
            {
                option = value;
                if (option != value)
                {
                    if (option == PeekOption.Only_Send_Command)
                        manager.deleteFilter(sendFilter);
                    else if(value==PeekOption.Only_Send_Command)
                        manager.AddFilter(sendFilter);
                }
                isLimit = false;
            }
            get
            {
                return option;
            }
        }

        static System.Threading.Timer timer = null;
        static FilterManager manager = null;

        public static void Init(FilterManager refFilterManager)
        {
            timer = new System.Threading.Timer(runThread, null, 0, Convert.ToInt32(time_inv));
            manager = refFilterManager;
        }

        static float sendLimit_pre = 0;

        static float time_inv = 1000;
        static float time = 0;

        static float recoverTime = 60000;
        public static float RecoverTime
        {
            private set { }
            get { return recoverTime; }
        }

        public static  int CurrentQueueCount
        {
            private set { }
            get { return MessageQueue.Count; }
        }

        static float currentRecoverTime = 0;

        static Object lockObj = new object();

        static volatile int sendCount = 0;
        public int CurrentCount
        {
            private set { }
            get
            {
                return sendCount;
            }
        }

        static int limitCount = 5;
        public static int LimitLevel
        {
            get{return limitCount;}
            set
            {
                limitCount = value;
                sendLimit_pre = (float)limitCount / (60000/time_inv);
            }
        }

        private static volatile bool isLimit = false;
        public static bool IsLimit {
            private set { } get
            {
                return isLimit;
            }
        }

        static List<MessageBase> MessageQueue = new List<MessageBase>();

        public static void PostIRCMessage(string target,MessageBase message)
        {
            lock(lockObj)
            {
                MessageQueue.Add(message);
                sendCount++;

                if (isLimit)
                    return;

                SendMessage(target, message.user + message.message.RawText);
                MessageQueue.Remove(message);
            }
        }

        public delegate void SendMessageAction(string userName, string message);

        private static  SendMessageAction action;

        public static void SetSendMessageAction(SendMessageAction action)
        {
            MessageManager.action = action;
        }

        public static void SendMessage(string userName,string message)
        {
            action(userName, message);
        }

        private static void runThread(object state)
        {
            time = time + time_inv;
            StringBuilder sb;

            if (option == PeekOption.Auto)
            {
                //判断是否超出限定
                if (!isLimit&&(float)sendCount / (60000.0f / time_inv) >= sendLimit_pre)
                {
                    isLimit = true;
                    ConsoleWriter.WriteColor("isLimit is true now", ConsoleColor.Yellow);
                    currentRecoverTime = 0;
                }

                //解禁时间
                currentRecoverTime += isLimit?time_inv:0;
                if (currentRecoverTime >= recoverTime)
                {
                    currentRecoverTime = 0;
                   isLimit = false;
                   ConsoleWriter.WriteColor("isLimit is false now", ConsoleColor.Yellow);
                }
            }

            sendCount = 0;

            if (isLimit)
            {
                sb = new StringBuilder();
                MessageBase message = null;
                lock (lockObj)
                {
                    while (MessageQueue.Count != 0)
                    {
                        message = MessageQueue[0];
                        MessageQueue.RemoveAt(0);
                        if (message == null)
                            break;
                        sb.AppendFormat("{0}:{1} || ", message.user, message.message.RawText);
                    }
                }
                if(sb.Length!=0)
                    SendMessage("",sb.ToString());
            }
        }

    }
}
