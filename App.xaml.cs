using System.Data;
using System.Linq;
using System.Windows;

namespace MemoryAlloc
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static string[] _sequence = new string[] 
        {
            "@FF 640",
            "//@(模式) (总内存空间) [各分区大小(仅限固定分区)]",
            "//@FF : 动态分区 首次适应",
            "//例：@FF 640",
            "//@BF ：动态分区 最佳适应",
            "//例：@BF 640",
            "//@FIX：固定分区 另加各分区块大小",
            "//例：@FIX 640 224,128,160,64,64",
            "ALLOC 1,130",
            "FREE 1",
            "//指令ALLOC 作业名,内存空间",
            "//为指定作业分配内存空间，如：ALLOC 1,100",
            "//指令FREE 作业名",
            "//释放指定作业占用的内存空间，如：FREE 1",
            "//使用//添加注释",
        };
        public static string[] RawSequence
        {
            get => _sequence;
            set => _sequence = value;
        }
        public static string[] Sequence
        {
            get => _sequence.Where(t => !t.StartsWith("//")).ToArray();
        }
    }
}
