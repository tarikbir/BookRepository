using System;
using System.ComponentModel;
using System.Windows;

namespace BookRepository.AdminPanel
{
    public partial class UpdateWeightProgress : Window
    {
        public UpdateWeightProgress()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var response = SqlHandler.UpdateAllWeights((sender as BackgroundWorker));
            if (response.Success) DialogResult = true;
            else DialogResult = false;
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }
    }
}
