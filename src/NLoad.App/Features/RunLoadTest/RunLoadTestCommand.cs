﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace NLoad.App.Features.RunLoadTest
{
    internal class RunLoadTestCommand : ICommand
    {
        #region Fields

        private bool _canExecute = true;
        private BackgroundWorker _worker;
        private readonly LoadTestViewModel _loadTestViewModel;

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Ctor

        public RunLoadTestCommand(LoadTestViewModel loadTestViewModel)
        {
            if (loadTestViewModel == null)
            {
                throw new ArgumentNullException("loadTestViewModel");
            }

            _loadTestViewModel = loadTestViewModel;

            NumberOfThreads = 100;
            Duration = TimeSpan.FromSeconds(10);
            DeleyBetweenThreadStart = TimeSpan.Zero;
        }

        #endregion

        #region Properties

        public int NumberOfThreads { get; set; }

        public TimeSpan Duration { get; set; }

        public TimeSpan DeleyBetweenThreadStart { get; set; }

        #endregion

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {

            SetCanExecute(false);

            _worker = CreateBackgroundWorker();

            _worker.RunWorkerAsync();
        }

        #region Helpers

        private void RunLoadTest(object sender, DoWorkEventArgs e)
        {
            var loadTest = NLoad.Test<InMemoryTest>()
                .WithNumberOfThreads(NumberOfThreads)
                .WithDurationOf(Duration)
                .WithDeleyBetweenThreadStart(DeleyBetweenThreadStart)
                .OnHeartbeat(UpdateThroughput)
                .Build();

            e.Result = loadTest.Run();
        }

        private BackgroundWorker CreateBackgroundWorker()
        {
            var worker = new BackgroundWorker();

            worker.DoWork += RunLoadTest;
            worker.RunWorkerCompleted += LoadTestCompleted;

            return worker;
        }

        private void SetCanExecute(bool canExecute)
        {
            _canExecute = canExecute;

            if (CanExecuteChanged != null)
            {
                Application.Current.Dispatcher.Invoke(() => CanExecuteChanged(this, EventArgs.Empty));
            }
        }

        private void LoadTestCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (LoadTestResult)e.Result;

            _loadTestViewModel.Elapsed = FormatElapsed(result.TotalRuntime);
            _loadTestViewModel.Iterations = result.TotalIterations;

            _loadTestViewModel.MinThroughput = result.MinThroughput;
            _loadTestViewModel.MaxThroughput = result.MaxThroughput;
            _loadTestViewModel.AverageThroughput = result.AverageThroughput;

            UnSubscribeWorkerEvents();

            _worker = null;

            SetCanExecute(true);
        }

        private void UnSubscribeWorkerEvents()
        {
            _worker.DoWork -= RunLoadTest;
            _worker.RunWorkerCompleted -= LoadTestCompleted;
        }

        private void UpdateThroughput(object sender, Heartbeat e)
        {
            if (e != null)
            {
                _loadTestViewModel.CurrentThroughput = Math.Round(e.Throughput, 2, MidpointRounding.AwayFromZero);
                _loadTestViewModel.Elapsed = FormatElapsed(e.Elapsed);
                _loadTestViewModel.Iterations = e.Iterations;
            }
        }

        private static string FormatElapsed(TimeSpan elapsed)
        {
            return string.Format("{0}:{1}:{2}", elapsed.ToString("hh"), elapsed.ToString("mm"), elapsed.ToString("ss"));
        }

        #endregion
    }
}