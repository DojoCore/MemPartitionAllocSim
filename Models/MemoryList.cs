using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MemoryAlloc
{
    /// <summary>
    /// 内存列表的抽象基类，每种内存列表应继承并实现该类的功能
    /// </summary>
    public abstract class MemoryList : ObservableCollection<MemoryBlock>
    {
        /// <summary>
        /// 该结构体指示内存分配状态结果
        /// Success - 成功
        /// IsJobExist - 作业已经在内存中
        /// OutOfMemory - 内存空间耗尽
        /// NoAvailableUnit - 没有可以装入作业的分区
        /// </summary>
        public struct AllocState
        {
            public bool Success;
            public bool IsJobExist;
            public bool OutOfMemory;
            public bool NoAvailableUnit;
        }

        /// <summary>
        /// 剩余内存空间
        /// </summary>
        public int MemRest { get; protected set; }
        /// <summary>
        /// 空闲分区表
        /// </summary>
        protected IEnumerable<MemoryBlock> FreeList
        {
            get => this.Where(_ => _.Name == MemoryBlock.EmptyMemory);
        }

        /// <summary>
        /// 为指定作业分配内存
        /// </summary>
        /// <param name="jobName">作业名称</param>
        /// <param name="size">作业请求内存大小</param>
        /// <returns>分配状态</returns>
        public abstract AllocState Alloc(string jobName, int size);
        /// <summary>
        /// 释放指定进程占用的内存空间
        /// </summary>
        /// <param name="jobName">进程名称</param>
        public abstract void Free(string jobName);
    }
}
