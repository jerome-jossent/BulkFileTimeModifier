using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkFileTimeModifier
{
   public class Fichier : System.ComponentModel.INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public string Filename { get; set; }
        public string FilenameSimple { get; set; }

        public DateTime DateCurrent
        {
            set
            {
                _DateCurrent = value;
                OnPropertyChanged("DateCurrent");
            }
            get { return _DateCurrent; }
        }
        DateTime _DateCurrent;


        public DateTime DateAfter
        {
            set
            {
                _DateAfter = value;
                OnPropertyChanged("DateAfter");
            }
            get { return _DateAfter; }
        }
        DateTime _DateAfter;

        public Fichier(string filename)
        {
            Filename = filename;

            FileInfo fileInfo = new FileInfo(Filename);
            FilenameSimple = fileInfo.Name;
        }
    }
}