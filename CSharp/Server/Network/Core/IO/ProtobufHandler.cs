using System;
using Google.Protobuf;

namespace Nico
{
    public static class ProtobufHandler
    {
        /// <summary>
        /// 从缓冲区解包
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void UnPack<T>(ref T msg, ByteString data) where T : IMessage<T>, new()
        {
            msg.MergeFrom(data);
        }

        public static void UnPack<T>(ref T msg, ArraySegment<byte> data) where T : IMessage<T>, new()
        {
            msg.MergeFrom(data);
        }
        
        /// <summary>
        /// 将消息体包装到消息头中
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="msg"></param>
        /// <typeparam name="T"></typeparam>
        public static void PackHeader<T>(this ProtoBuffer buffer, T msg) where T : IMessage<T>
        {
            using (ProtoBuffer body = ProtoBuffer.Get())//拿两个buffer 一个用来写头 一个用来写body
            {
                PacketHeader header = Get<PacketHeader>();
                header.Id = TypeId<T>.ID;
                body.WriteProto(msg); //写入body
                header.Body = body;
                buffer.WriteProto(header); //写入头
                header.Return();
            }
        }

        public static T Get<T>() where T : IMessage<T>, new()
        {
            return ProtoPool<T>.Pool.Get();
        }

        public static void Return<T>(this T msg) where T : IMessage<T>, new()
        {
            ProtoPool<T>.Pool.Return(msg);
        }

        private static class ProtoPool<T> where T : IMessage<T>, new()
        {
            public static ConcurrentPool<T> Pool = new ConcurrentPool<T>(() => new T(), 10);
        }
    }
}