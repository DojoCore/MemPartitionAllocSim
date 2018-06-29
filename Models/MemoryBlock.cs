using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MemoryAlloc
{
    /// <summary>
    /// 内存中的进程单元，指示该单元占用的内存空间和分配状态
    /// </summary>
    public class MemoryBlock : INotifyPropertyChanged
    {
        /// <summary>
        /// 空闲单元标记
        /// </summary>
        public static string EmptyMemory { get; } = "FREE_MEMORY";

        private string _name;
        /// <summary>
        /// 进程标记
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if(value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _size;
        /// <summary>
        /// 进程所占用的空间
        /// </summary>
        public int Size
        {
            get => _size;
            set
            {
                if(value != _size)
                {
                    _size = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public MemoryBlock(string name, int size)
        {
            Name = name;
            Size = size;
        }

        public MemoryBlock(int size)
        {
            Name = EmptyMemory;
            Size = size;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
