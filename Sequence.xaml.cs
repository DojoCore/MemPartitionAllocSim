using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MemoryAlloc
{
    /// <summary>
    /// Sequence.xaml 的交互逻辑
    /// </summary>
    public partial class Sequence : Window
    {
        public Sequence()
        {
            InitializeComponent();
            StringBuilder sb = new StringBuilder();
            Array.ForEach(App.RawSequence, t => sb.AppendLine(t));
            txtSequence.Text = sb.ToString();
        }

        private void txtSequence_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.RawSequence = txtSequence.Text.Split(new char[] { '\r', '\n' }, 
                StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
