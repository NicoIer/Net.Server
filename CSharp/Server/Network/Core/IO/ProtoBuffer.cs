using System;
using Google.Protobuf;

namespace Nico
{
    /// <summary>
    ///  缓冲区
    /// </summary>
    public class ProtoBuffer : IDisposable
    {
        public const int DefaultCapacity = 1500; //MTU~=1500
        public const int GrowScale = 2;

        #region Pool

        private static readonly ConcurrentPool<ProtoBuffer> Pool =
            new ConcurrentPool<ProtoBuffer>(() => new ProtoBuffer(), 1000);


        public static ProtoBuffer Get()
        {
            ProtoBuffer buffer = Pool.Get();
            buffer.Reset();
            return buffer;
        }

        public static void Return(ProtoBuffer buffer) => Pool.Return(buffer);

        #endregion


        internal byte[] buffer = new byte[DefaultCapacity]; //1000*1500 byte ~= 1.5M

        public int Position { get; private set; }
        public int Capacity => buffer.Length;

        // 禁止外部实例化
        private ProtoBuffer()
        {
        }

        /// <summary>
        /// 把proto消息序列到缓冲区
        /// </summary>
        /// <param name="proto"></param>
        /// <typeparam name="T"></typeparam>
        public void WriteProto<T>(IMessage<T> proto) where T : IMessage<T>
        {
            int size = proto.CalculateSize();
            EnsureCapacity(Position + size);
            //使用数组段避免拷贝 进而避免GC
            //把消息写到缓冲区
            proto.WriteTo(new ArraySegment<byte>(buffer, Position, size));
            Position += size;
        }


        public unsafe void WriteBlittable<T>(T value)
            where T : unmanaged
        {
            EnsureCapacity(Position + sizeof(T));
            fixed (byte* ptr = &buffer[Position]) //获取当前位置的指针
            {
                *(T*)ptr = value; //把值写到缓冲区
            }

            Position += sizeof(T);
        }

        public void From(ByteString byteString)
        {
            Reset();
            int size = byteString.Length;
            EnsureCapacity(size);
            byteString.Span.CopyTo(buffer);
        }

        public void From(ArraySegment<byte> segment)
        {
            Reset();
            int size = segment.Count;
            EnsureCapacity(size);
            segment.CopyTo(buffer);
        }

        public unsafe T ReadBlittable<T>() where T : unmanaged
        {
            int size = sizeof(T);
            if (Position + size > Capacity)
            {
                throw new IndexOutOfRangeException();
            }

            T value;
            fixed (byte* ptr = &buffer[Position])
            {
                value = *(T*)ptr;
            }

            Position += size;
            return value;
        }

        public void Dispose() => Pool.Return(this);

        public void Reset()
        {
            Position = 0;
        }

        public void EnsureCapacity(int value)
        {
            if (buffer.Length < value)
            {
                int capacity = Math.Max(value, buffer.Length * GrowScale);
                Array.Resize(ref buffer, capacity);
            }
        }

        /// <summary>
        /// 0-Position的数组段
        /// </summary>
        /// <returns></returns>
        private ArraySegment<byte> ToArraySegment()
        {
            return new ArraySegment<byte>(buffer, 0, Position);
        }

        private ByteString ToByteString()
        {
            return ByteString.CopyFrom(new ArraySegment<byte>(buffer, 0, Position));
        }

        public static implicit operator ArraySegment<byte>(ProtoBuffer writer) => writer.ToArraySegment();
        public static implicit operator ByteString(ProtoBuffer writer) => writer.ToByteString();


        public override string ToString() =>
            $"[{ToArraySegment().ToHexString()} @ {Position}/{Capacity}]";
    }
}