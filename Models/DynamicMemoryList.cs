using System.Linq;

namespace MemoryAlloc
{
    /// <summary>
    /// 动态分区状态下的内存状态表
    /// </summary>
    public class DynamicMemoryList : MemoryList
    {
        /// <summary>
        /// 指示分配方式的枚举类型
        /// FF - 首次适应算法
        /// BF - 最佳适应算法
        /// </summary>
        public enum Modes { FF, BF };

        /// <summary>
        /// 指示当前实例表使用的内存分配方式
        /// </summary>
        private Modes Mode { get; set; } = Modes.FF;

        public DynamicMemoryList(int memorySize, Modes mode)
        {
            Add(new MemoryBlock(memorySize));
            MemRest = memorySize;
            Mode = mode;
        }

        /// <summary>
        /// 使用给定空间大小和分配方式重置当前实例状态表
        /// </summary>
        /// <param name="memorySize">内存总空间</param>
        /// <param name="mode">内存分配方式</param>
        public void Reset(int memorySize, Modes mode)
        {
            Clear();
            Add(new MemoryBlock(memorySize));
            MemRest = memorySize;
            Mode = mode;
        }
        
        public override AllocState Alloc(string jobName, int size)
        {
            if (size > MemRest) return new AllocState { OutOfMemory = true };
            if (!this.All(_ => _.Name != jobName)) return new AllocState { IsJobExist = true };

            MemoryBlock mem = null;
            if (Mode == Modes.BF)
            {   //对空闲分区按容量进行升序排序，选择首个能容纳进程的分区分配给进程
                mem = FreeList.OrderBy(_ => _.Size).FirstOrDefault(_ => _.Size >= size);
            }
            else
            {   //从空闲分区中将第一个能容纳进程的分区分配给进程
                mem = FreeList.FirstOrDefault(_ => _.Size >= size);
            }
            var memIndex = IndexOf(mem);
            if (memIndex < 0) return new AllocState { NoAvailableUnit = true };
            if((mem.Size -= size) == 0) Remove(mem);
            Insert(memIndex, new MemoryBlock(jobName, size));
            MemRest -= size;
            return new AllocState { Success = true };
        }

        public override void Free(string jobName)
        {
            if (jobName == MemoryBlock.EmptyMemory) return;

            var memblock = this.FirstOrDefault(_ => _.Name == jobName);
            var memIndex = IndexOf(memblock);
            if (memIndex < 0) return;

            this[memIndex].Name = MemoryBlock.EmptyMemory;
            MemRest += this[memIndex].Size;

            bool merged = false; // 是否进行过空闲块合并
            if (memIndex + 1 < Count && this[memIndex + 1].Name == MemoryBlock.EmptyMemory)
            {   //释放并与后方相邻空闲内存合并
                this[memIndex].Size += this[memIndex + 1].Size;
                RemoveAt(memIndex + 1); //注：如果当前块与下一块合并，memIndex其实仍指向当前块
                merged = true;
            }
            if (memIndex - 1 >= 0 && this[memIndex - 1].Name == MemoryBlock.EmptyMemory)
            {   //释放并与前方相邻空闲内存合并
                this[memIndex].Size += this[memIndex - 1].Size;
                RemoveAt(--memIndex);
                merged = true;
            }
            if (!merged)
            {   //如果未进行合并，创建新块
                Insert(memIndex, new MemoryBlock(this[memIndex].Size));
                RemoveAt(memIndex);
            }
        }
    }
}
