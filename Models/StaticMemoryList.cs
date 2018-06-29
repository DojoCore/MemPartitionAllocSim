using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoryAlloc
{
    /// <summary>
    /// 固定分区状态下的内存状态表
    /// </summary>
    public class StaticMemoryList : MemoryList
    {
        public StaticMemoryList(IEnumerable<Int32> initSeq)
        {
            foreach(var size in initSeq.OrderBy(i => i))
            {
                Add(new MemoryBlock(size));
                MemRest += size;
            }
        }

        public override AllocState Alloc(string jobName, int size)
        {
            if (size > MemRest) return new AllocState { OutOfMemory = true };
            if (!this.All(_ => _.Name != jobName)) return new AllocState { IsJobExist = true };

            var toAlloc = FreeList.FirstOrDefault(_ => _.Size >= size);
            if(toAlloc != null)
            {
                toAlloc.Name = jobName;
                MemRest -= toAlloc.Size;
                return new AllocState { Success = true };
            }
            return new AllocState { NoAvailableUnit = true };
        }

        public override void Free(string jobName)
        {
            if (jobName == MemoryBlock.EmptyMemory) return;

            var toFree = this.FirstOrDefault(_ => _.Name == jobName);
            if(toFree != null)
            {
                MemRest += toFree.Size;
                toFree.Name = MemoryBlock.EmptyMemory;
            }
        }
    }
}
