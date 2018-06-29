using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MemoryAlloc
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int instructionIndex = 1;

        private MemoryList _memory;

        private int Memsize
        {
            get
            {
                try
                {
                    return Int32.Parse(txtMemSize.Text);
                }
                catch
                {
                    MessageBox.Show("输入有误，请重试");
                    txtMemSize.Text = 640.ToString();
                    return 640;
                }
            }
        }
        
        public MainWindow()
        {
            InitializeComponent();
            _memory = new DynamicMemoryList(Memsize, DynamicMemoryList.Modes.FF);
            lstMemory.ItemsSource = _memory;
            RefreshFullMem();
            RefreshRestMem();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (lstMemory.SelectedItem != null)
            {
                var procedure = lstMemory.SelectedItem as MemoryBlock;
                FreeMemory(procedure.Name);
            }
        }

        private void FreeMemory(string name)
        {
            _memory.Free(name);
            RefreshRestMem();
        }

        private void btnMemSize_Click(object sender, RoutedEventArgs e)
        {
            if (rdoFF.IsChecked == true)
            {
                rdoFF_Checked(null, null);
            }
            else if (rdoFF.IsChecked == true)
            {
                rdoFF_Checked(null, null);
            }
            RefreshFullMem();
        }

        private void btnSetBlocks_Click(object sender, RoutedEventArgs e)
        {
            int[] blocks = GetBlocks(txtBlocks.Text);
            if (blocks.Sum() > Memsize)
            {
                MessageBox.Show(this, "分区总大小超出总内存大小，请重新分配", "参数错误");
                return;
            }
            _memory = new StaticMemoryList(blocks);
            lstMemory.ItemsSource = _memory;
            RefreshFullMem();
            RefreshRestMem();
        }

        private void rdoFF_Checked(object sender, RoutedEventArgs e)
        {
            if (_memory is DynamicMemoryList)
            {
                ((DynamicMemoryList)_memory).Reset(Memsize, DynamicMemoryList.Modes.FF);
            }
            else
            {
                _memory = new DynamicMemoryList(Memsize, DynamicMemoryList.Modes.FF);
                lstMemory.ItemsSource = _memory;
            }
            RefreshRestMem();
        }

        private void rdoBF_Checked(object sender, RoutedEventArgs e)
        {
            if (_memory is DynamicMemoryList)
            {
                ((DynamicMemoryList)_memory).Reset(Memsize, DynamicMemoryList.Modes.BF);
            }
            else
            {
                _memory = new DynamicMemoryList(Memsize, DynamicMemoryList.Modes.BF);
                lstMemory.ItemsSource = _memory;
            }
            RefreshRestMem();
        }

        private void btnAlloc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string jobName = txtJobName.Text;
                int size = Int32.Parse(txtJobSize.Text);
                var state = _memory.Alloc(jobName, size);
                SwitchState(state);
            }
            catch
            {
                MessageBox.Show(this, "输入有误，请重试", "分配失败");
            }
        }

        private void btnEditSeq_Click(object sender, RoutedEventArgs e)
        {
            Sequence seqWin = new Sequence();
            seqWin.Owner = this;
            seqWin.ShowDialog();
            btnResetSeq_Click(null, null);
        }

        private void btnBeginSeq_Click(object sender, RoutedEventArgs e)
        {
            if (instructionIndex >= App.Sequence.Count())
            {
                MessageBox.Show(this, "序列已经执行完毕", "操作序列");
                return;
            }
            var instruction = App.Sequence[instructionIndex];
            var splited = instruction.Split(new char[] { ' ', ',' },
                StringSplitOptions.RemoveEmptyEntries);
            try
            {
                string op = splited[0], name = splited[1];
                switch (op.ToUpper())
                {
                    case "ALLOC":
                        int size = Int32.Parse(splited[2]);
                        var state = _memory.Alloc(name, size);
                        SwitchState(state);
                        break;
                    case "FREE":
                        FreeMemory(name);
                        break;
                    default:
                        MessageBox.Show(this, "输入了未知指令，已跳过", "指令错误");
                        break;
                }
            }
            catch(IndexOutOfRangeException)
            {
                MessageBox.Show(this, "指令格式有误，已跳过", "指令错误");
            }
            catch(FormatException)
            {
                MessageBox.Show(this, "指令参数有误，已跳过", "指令错误");
            }
            instructionIndex++;
            if (instructionIndex < App.Sequence.Length)
            {
                txtNextInst.Text = App.Sequence[instructionIndex].ToUpper();
            }
            else { txtNextInst.Text = "END"; }
        }

        private void SwitchState(MemoryList.AllocState state)
        {
            if (state.Success)
            {
                RefreshRestMem();
            }
            else if (state.OutOfMemory)
            {
                MessageBox.Show(this, "可用内存不足", "分配失败");
            }
            else if (state.IsJobExist)
            {
                MessageBox.Show(this, "该作业已经存在于内存中", "分配失败");
            }
            else if (state.NoAvailableUnit)
            {
                MessageBox.Show(this, "没有可用的内存块", "分配失败");
            }
            else
            {
                MessageBox.Show(this, "未知错误", "分配失败");
            }
        }

        private void btnResetSeq_Click(object sender, RoutedEventArgs e)
        {
            instructionIndex = 1;
            try
            {
                var memstr = App.Sequence[0].Split(' ');
                string kind = memstr[0];
                int size = Int32.Parse(memstr[1]);
                switch (kind.Trim('@').ToUpper())
                {
                    case "FF":
                        _memory = new DynamicMemoryList(size, DynamicMemoryList.Modes.FF);
                        lstMemory.ItemsSource = _memory;
                        break;
                    case "BF":
                        _memory = new DynamicMemoryList(size, DynamicMemoryList.Modes.BF);
                        lstMemory.ItemsSource = _memory;
                        break;
                    case "FIX":
                        int[] blocks = GetBlocks(memstr[2]);
                        _memory = new StaticMemoryList(blocks);
                        lstMemory.ItemsSource = _memory;
                        break;
                    default:
                        MessageBox.Show(this, "未检测到内存分区模式，请重新编写序列", "指令错误");
                        return;
                }
            }
            catch(IndexOutOfRangeException)
            {
                MessageBox.Show(this, "序列中没有内容", "指令错误");
            }
            if(instructionIndex < App.Sequence.Length)
            {
                txtNextInst.Text = App.Sequence[instructionIndex].ToUpper();
            }
            else { txtNextInst.Text = "END"; }
        }

        private int[] GetBlocks(string blockstr)
        {
            string[] values = blockstr.Split(',');
            List<int> result = new List<int>();
            Array.ForEach(values, _ => result.Add(Int32.Parse(_.Trim(' '))));
            return result.ToArray();
        }

        private void RefreshRestMem() =>
            txtRestMem.Text = "可用内存空间：" + _memory.MemRest.ToString() + "KB";

        private void RefreshFullMem() =>
            txtFullMem.Text = "总内存空间：" + Memsize.ToString() + "KB";
    }
}
