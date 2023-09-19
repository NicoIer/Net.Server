namespace Nico
{
    public static class FrameSyncRpcCenter
    {
        public delegate void FrameRpcDelegate(FrameSyncBehavior behavior, ProtoBuffer @params, int channel);

        public static Dictionary<int, FrameRpcDelegate> frameRpcMap = new Dictionary<int, FrameRpcDelegate>();


        public static void RegisterFrameRpc<T>(int methodHash, Action<T, ProtoBuffer, int> onRpc)
            where T : FrameSyncBehavior
        {
            frameRpcMap[methodHash] = (behavior, @params, channel) => { onRpc(behavior as T, @params, channel); };
        }

        /// <summary>
        /// 当收到帧同步方法
        /// </summary>
        /// <param name="methodHash"></param>
        /// <param name="params"></param>
        /// <param name="channel"></param>
        public static void OnFrameRpc(FrameSyncBehavior behavior, int methodHash, ProtoBuffer @params, int channel)
        {
            //TODO warning
            if (frameRpcMap.TryGetValue(methodHash, out var action))
            {
                action(behavior, @params, channel);
            }
        }
    }
}