using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BL
{
    public class FileBrowserService
    {
        public bool GetWordFiles(out string[] files)
        {
            var dialog = GetFileDialog();
            dialog.Filter = "Word files|*.doc;*.docx";
            dialog.Multiselect = true;

            if(dialog.ShowDialog() == true)
            {
                files = dialog.FileNames;
                return true;
            }

            files = null;
            return false;
        }

        public bool GetImageFile(string start, out string file)
        {
            var dialog = GetFileDialog();
            dialog.InitialDirectory = start;
            dialog.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;";
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == true)
            {
                file = dialog.FileName;
                return true;
            }

            file = null;
            return false;
        }


        public bool GetPdfFile(out string file)
        {
            var dialog = GetFileDialog();
            dialog.Filter = "Pdf files|*.pdf;";

            if (dialog.ShowDialog() == true)
            {
                file = dialog.FileName;
                return true;
            }

            file = null;
            return false;
        }

        public bool GetPdfFiles(out string[] files)
        {
            var dialog = GetFileDialog();
            dialog.Filter = "Pdf files|*.pdf;";
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == true)
            {
                files = dialog.FileNames;
                return true;
            }

            files = null;
            return false;
        }

        OpenFileDialog GetFileDialog() => new OpenFileDialog
        {
            CheckFileExists = true,
            CheckPathExists = true,
            InitialDirectory = @"C:\Users\VIT\Desktop\Тесты",
        };
    }
}
