namespace Moba
{
    public static class StringExtensions
    {
        /// <summary>
        /// 这个是消息通信的基础 需要版本不同编程语言 不同平台下 对同一个字符串的hash值都是一样的
        /// 65535个类型够用了
        /// Hash冲突的概率是1/65535
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static ushort GetStableHash(this string str)
        {
            unchecked
            {
                ushort hash = 23;
                foreach (char c in str)
                    hash = (ushort)(hash * 31 + c);
                return hash;
            }
        }

        
    }
}