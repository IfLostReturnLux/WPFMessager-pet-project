using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CrystalMessagerWPF
{
    internal class Utils
    {
        public void OpenNewForm(Window newForm, Window closeForm)
        {
            newForm.Show();
            closeForm.Close();
        }
    }
}
