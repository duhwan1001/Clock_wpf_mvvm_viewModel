using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VewModelSample.Model;

namespace VewModelSample.ViewModel
{
    internal class ViewLogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        private static ViewLogViewModel _instance = new ViewLogViewModel();
        public static ViewLogViewModel Instance
        {
            get { return _instance; }
        }

        private Model.ClockModel clockModel = null;

        public ViewLogViewModel()
        {
            clockModel = ClockModel.Instance;
        }

        public ObservableCollection<ClockModel.dataGridData> logDatas
        {
            get
            {
                if (clockModel._logDatas == null)
                {
                    clockModel._logDatas = new ObservableCollection<ClockModel.dataGridData>();
                }
                return clockModel._logDatas;
            }
            set
            {
                clockModel._logDatas = value;
                OnPropertyChanged("logDatas");
            }
        }

        public int LogSequence
        {
            get { return logDatas.Count + 1; }
        }

        public void AddData(String function, String AddedTime, String RecordText)
        {
            ClockModel.dataGridData dataGrid = new ClockModel.dataGridData();
            dataGrid.dataGridSequence = LogSequence;
            dataGrid.dataGridFunction = function;
            dataGrid.dataGridAddedTime = AddedTime;
            dataGrid.dataGridSimpleRecordText = RecordText;

            logDatas.Add(dataGrid);
        }

    }
}
