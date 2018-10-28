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
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            worker.RunWorkerAsync();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var response = SqlHandler.UpdateAllWeights((sender as BackgroundWorker));
            if(response.Success)
                MessageBox.Show("Database successfully updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("There was an error while updating the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }
    }
}
