using System;
using System.Collections.Generic;
using System.IO;

namespace Moba
{
    
    /// <summary>
    /// 网络中心
    /// 提供网络消息管理
    /// </summary>
    public class NetCenter
    {
        private Dictionary<int, Action<Stream>> _handlers;

        public NetCenter()
        {
            _handlers = new Dictionary<int, Action<Stream>>();
        }

        #region 消息管理

        public void Register<T>(Action<T> handler, Func<Stream, T> deSerialize)
        {
            var msgId = TypeId<T>.id;
            if (_handlers.ContainsKey(msgId))
            {
                throw new ArgumentException($"{msgId} already registered");
                return;
            }

            // warp一层，将Stream转换为T
            _handlers.Add(msgId, bytes =>
            {
                var msg = deSerialize(bytes);
                handler(msg);
            });
        }


        public void UnRegister<T>()
        {
            var msgId = TypeId<T>.id;
            if (_handlers.ContainsKey(msgId))
            {
                _handlers.Remove(msgId);
            }
        }

        #endregion
    }
}