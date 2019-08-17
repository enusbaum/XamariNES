using System.Collections.Generic;

namespace XamariNES.Cartridge.Mappers.impl
{
    /// <summary>
    ///     Base Class for Mappers
    /// </summary>
    public abstract class MapperBase
    {
        //Interceptor Delegates
        public delegate byte ReadInterceptor(int offset);
        public delegate void WriteInterceptor(int offset, byte value);

        //Dictionary of Interceptors
        protected readonly Dictionary<int, ReadInterceptor> ReadInterceptors = new Dictionary<int, ReadInterceptor>();
        protected readonly Dictionary<int, WriteInterceptor> WriteInterceptors = new Dictionary<int, WriteInterceptor>();

        //Cached Interceptors
        protected ReadInterceptor currentReadInterceptor;
        protected WriteInterceptor currentWriteInterceptor;

        protected MapperBase() { }

        /// <summary>
        ///     Registers a Read Interceptor at the specified offset
        /// </summary>
        /// <param name="readInterceptor"></param>
        /// <param name="offset"></param>
        public void RegisterReadInterceptor(ReadInterceptor readInterceptor, int offset)
        {
            ReadInterceptors.Add(offset, readInterceptor);
        }

        /// <summary>
        ///     Registers a Read Interceptor at the specified offset range
        /// </summary>
        /// <param name="readInterceptor"></param>
        /// <param name="offsetStart"></param>
        /// <param name="offsetEnd"></param>
        public void RegisterReadInterceptor(ReadInterceptor readInterceptor, int offsetStart, int offsetEnd)
        {
            for (var i = offsetStart; i <= offsetEnd; i++)
                RegisterReadInterceptor(readInterceptor, i);
        }

        /// <summary>
        ///     Registers a Write Interceptor at the specified offset
        /// </summary>
        /// <param name="writeInterceptor"></param>
        /// <param name="offset"></param>
        public void RegisterWriteInterceptor(WriteInterceptor writeInterceptor, int offset)
        {
            WriteInterceptors.Add(offset, writeInterceptor);
        }

        /// <summary>
        ///     Registers a Write Interceptor at the specified offset range
        /// </summary>
        /// <param name="writeInterceptor"></param>
        /// <param name="offsetStart"></param>
        /// <param name="offsetEnd"></param>
        public void RegisterWriteInterceptor(WriteInterceptor writeInterceptor, int offsetStart, int offsetEnd)
        {
            for (var i = offsetStart; i <= offsetEnd; i++)
                RegisterWriteInterceptor(writeInterceptor, i);
        }

        
    }
}
