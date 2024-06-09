using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BulkFileTimeModifier
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //Create the OnPropertyChanged method to raise the event
        //The calling member's name will be used as the parameter.
        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<Fichier> fichiers
        {
            set
            {
                _fichiers = value;
                OnPropertyChanged("fichiers");
            }
            get
            {
                return _fichiers;
            }
        }
        ObservableCollection<Fichier> _fichiers;

        public string folder
        {
            set
            {
                if (_folder == value) return;
                _folder = value;
                OnPropertyChanged("folder");
                SetFichiers(folder);

                Properties.Settings.Default["_folder"] = folder;
                Properties.Settings.Default.Save();
            }
            get { return _folder; }
        }
        string _folder = Properties.Settings.Default["_folder"].ToString();

        public int seconds
        {
            set
            {
                if (_seconds != value)
                {
                    _seconds = value;
                    OnPropertyChanged("seconds");
                    PreviewChangeTime(seconds);
                    OnPropertyChanged("fichiers");
                }
            }
            get { return _seconds; }
        }
        int _seconds = 60 * 10;

        public int timeType
        {
            set
            {
                _timeType = value;
                OnPropertyChanged("timeType");
                LoadFilesAndPreview();
            }
            get
            {
                return _timeType;
            }
        }
        int _timeType = 1;

        public int timeOperation
        {
            set
            {
                _timeOperation = value;
                OnPropertyChanged("timeOperation");
                LoadFilesAndPreview();
            }
            get
            {
                return _timeOperation;
            }
        }
        int _timeOperation = 1;

        public DateTime fixedDateTimeValue
        {
            set
            {
                _fixedDateTimeValue = value;
                OnPropertyChanged("fixedDateTimeValue");
                PreviewChangeTime(0);
            }
            get
            {
                return _fixedDateTimeValue;
            }
        }
        DateTime _fixedDateTimeValue = DateTime.MinValue;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFilesAndPreview();
        }

        void LoadFilesAndPreview()
        {
            SetFichiers(folder);
            PreviewChangeTime(seconds);
        }

        void SetFichiers(string dir)
        {
            fichiers = new ObservableCollection<Fichier>();
            if (!System.IO.Directory.Exists(dir))
                return;

            string[] files = System.IO.Directory.GetFiles(dir);
            foreach (string filename in files)
            {
                Fichier f = new Fichier(filename);
                switch (timeType)
                {
                    case 1:
                        f.DateCurrent = System.IO.File.GetLastWriteTime(f.Filename);
                        break;
                    case 2:
                        f.DateCurrent = System.IO.File.GetLastAccessTime(f.Filename);
                        break;
                    case 3:
                        f.DateCurrent = System.IO.File.GetCreationTime(f.Filename);
                        break;
                }
                fichiers.Add(f);
            }

            if (fichiers.Count > 0 && _fixedDateTimeValue == DateTime.MinValue)
                fixedDateTimeValue = fichiers[0].DateCurrent;
        }

        void PreviewChangeTime(double sec)
        {
            TimeSpan ts = TimeSpan.FromSeconds(sec);
            List<string> fails = new List<string>();
            switch (timeOperation)
            {
                case 1:
                    foreach (Fichier f in fichiers)
                        f.DateAfter = f.DateCurrent + ts;
                    break;
                case 2:
                    foreach (Fichier f in fichiers)
                        f.DateAfter = fixedDateTimeValue;
                    break;
                case 3:
                    foreach (Fichier f in fichiers)
                    {
                        string s = f.FilenameSimple;
                        try
                        {
                            string _s = s.Substring(0, 4);
                            int YYYY = int.Parse(_s);
                            int MM = int.Parse(s.Substring(4, 2));
                            int DD = int.Parse(s.Substring(6, 2));
                            int HH = int.Parse(s.Substring(9, 2));
                            int mm = int.Parse(s.Substring(11, 2));
                            int ss = int.Parse(s.Substring(13, 2));

                            DateTime dt = new DateTime(YYYY, MM, DD, HH, mm, ss);
                            f.DateAfter = dt;
                        }
                        catch (Exception ex)
                        {
                            fails.Add(f.Filename);
                        }
                    }
                    break;
            }
            OnPropertyChanged("fichiers");

            if (fails.Count > 0)
            {
                MessageBox.Show("Erreur avec les fichiers suivants :\n" +
                  string.Join("\n", fails));
            }

        }

        void ChangeTime()
        {
            foreach (Fichier f in fichiers)
            {
                switch (timeType)
                {
                    case 1:
                        System.IO.File.SetLastWriteTime(f.Filename, f.DateAfter);
                        break;
                    case 2:
                        System.IO.File.SetLastAccessTime(f.Filename, f.DateAfter);
                        break;
                    case 3:
                        System.IO.File.SetCreationTime(f.Filename, f.DateAfter);
                        break;
                }
            }
            LoadFilesAndPreview();
        }

        void btn_Go_Click(object sender, RoutedEventArgs e)
        {
            ChangeTime();
        }
    }
}