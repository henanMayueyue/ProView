﻿using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace ProView
{
	public partial class PVMainForm : Form
	{
		public PVMainForm()
		{
			InitializeComponent();
			pvDataViewer.pvImageViewer = this.pvImageViewer;
			pvImageViewer.pvMainForm = this;
			InitUserSettings();
		}
		
		#region Events
		void MainForm2FormClosing(object sender, FormClosingEventArgs e)
		{
			pvDataViewer.MainFormClosing(e);
			SaveUserSettings();
		}
		#endregion
		
		#region User Settings
		
		void InitUserSettings()
		{
			WindowState = (FormWindowState)Properties.MainForm.Default.WindowState;
			Size = Properties.MainForm.Default.WindowSize;
			StartPosition = FormStartPosition.Manual;
			Location = Properties.MainForm.Default.WindowPosition;

			splitContainer1.Orientation = (Orientation)Properties.MainForm.Default.SplitterOrientation;
			if (splitContainer1.Orientation == Orientation.Vertical)
			{
				ctxSplitterVert.Checked = true;
				splitContainer1.SplitterDistance = (int)(Properties.MainForm.Default.SplitterDistance * ClientSize.Width);
			}
			else
			{
				ctxSplitterHoriz.Checked = true;
				splitContainer1.SplitterDistance = (int)(Properties.MainForm.Default.SplitterDistance * ClientSize.Height);
			}
			
			PDF.resolution = Properties.ImageViewer.Default.PDFResolution;
			pvDataViewer.InitUserSettings();
		}

		void SaveUserSettings()
		{
			if (WindowState == FormWindowState.Minimized)
				WindowState = FormWindowState.Normal;
			if (WindowState != FormWindowState.Maximized)
			{
				Properties.MainForm.Default.WindowSize = Size;
				Properties.MainForm.Default.WindowPosition = Location;
			}
			Properties.MainForm.Default.WindowState = (int)WindowState;
			if (splitContainer1.Orientation == Orientation.Vertical)
				Properties.MainForm.Default.SplitterDistance = splitContainer1.SplitterDistance / (double)ClientSize.Width;
			else
				Properties.MainForm.Default.SplitterDistance = splitContainer1.SplitterDistance / (double)ClientSize.Height;
			Properties.MainForm.Default.SplitterOrientation = (int)splitContainer1.Orientation;
			Properties.MainForm.Default.Save();
		}
		#endregion
		
		
		#region Drag and Drop Files
		void MainForm2DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
		}
		
		void MainForm2DragDrop(object sender, DragEventArgs e)
		{
			var dropItems = (string[])e.Data.GetData(DataFormats.FileDrop);
			var dropFiles = new List<string>();
			foreach (var dropItem in dropItems)
			{
				try
				{
					var attr = File.GetAttributes(dropItem);
					if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
						dropFiles.AddRange(Directory.EnumerateFiles(dropItem, "*.*", SearchOption.AllDirectories).OrderBy(x => x));
					else
						dropFiles.Add(dropItem);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
					var result = MessageBox.Show(dropItem + "\n\n" + ex.Message + "\n\nContinue loading other files?", "Problem", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
					if (result == DialogResult.OK)
						continue;
					return;
				}
			}
			foreach (var dropFile in dropFiles)
			{
				PVFile pvFile = null;
				try
				{
					pvFile = PVFile.Load(dropFile);
					pvDataViewer.AddItem(pvFile);
				}
				catch (Exception ex)
				{
					var result = MessageBox.Show(dropFile + "\n\n" + ex.Message + "\n\nContinue loading other files?", "Problem", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
					if (result == DialogResult.OK)
						continue;
					return;
				}
			}
		}
		
		/*
		void MainForm2DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		ProgressWindow pw;
		
		void MainForm2DragDrop(object sender, DragEventArgs e)
		{
			string[] dropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

			pw = new ProgressWindow();
			pw.Text = "Analyzing " + dropFiles.Length + " file(s)";

			BackgroundWorker addFilesBgWorker = new BackgroundWorker();
			addFilesBgWorker.WorkerReportsProgress = true;
			addFilesBgWorker.WorkerSupportsCancellation = true;
			addFilesBgWorker.DoWork += addFilesBgWorker_DoWork;
			addFilesBgWorker.ProgressChanged += addFilesBgWorker_ProgressChanged;
			addFilesBgWorker.RunWorkerCompleted += addFilesBgWorker_RunWorkerCompleted;
			addFilesBgWorker.RunWorkerAsync(dropFiles);

			pw.BtnCancel.Click += delegate { addFilesBgWorker.CancelAsync(); };
			pw.ShowDialog(this);
		}

		void addFilesBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			
			if (e.Error != null)
			{
				MessageBox.Show(e.Error.Message);
			}
			else if (e.Cancelled)
			{
				// Next, handle the case where the user canceled
				// the operation.
				// Note that due to a race condition in
				// the DoWork event handler, the Cancelled
				// flag may not have been set, even though
				// CancelAsync was called.
				//MessageBox.Show("Worker was canceled!");
			}
			else
			{
				// Finally, handle the case where the operation
				// succeeded.
				pw.ProgressBar.Value = 100;
				AddFilesToList(e.Result as List<PVFile>);
			}
			pw.Close();
			pw.Dispose();
		}

		void AddFilesToList(List<PVFile> pvFiles)
		{
			foreach (PVFile pvFile in pvFiles)
			{
				if (!pvDataViewer.AddItem(pvFile))
				{
					var result = MessageBox.Show(pvFile.FileName + "\n\nThe file is already in the list.", "Problem", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
					if (result == DialogResult.Cancel)
						return;
				}
			}
		}

		void addFilesBgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			pw.ProgressBar.Value = e.ProgressPercentage;
		}

		void addFilesBgWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var dropFiles = e.Argument as string[];
			var worker = sender as BackgroundWorker;
			var files = new List<PVFile>();
			for (int i = 0; i < dropFiles.Length; i++)
			{
				worker.ReportProgress((i + 1) * 100 / dropFiles.Length);
				if (worker.CancellationPending)
				{
					e.Cancel = true;
					break;
				}

				PVFile pvFile = null;
				try
				{
					switch (Path.GetExtension(dropFiles[i]).ToLower())
					{
						case ".pdf":
							pvFile = new PDF(dropFiles[i]);
							break;
						case ".tif":
							pvFile = new TIF(dropFiles[i]);
							break;
						default:
							throw new Exception("Cannot open this kind of file.");
					}
				}
				catch (Exception ex)
				{
					var result = MessageBox.Show(dropFiles[i] + "\n\n" + ex.Message, "Problem", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
					if (result == DialogResult.OK)
						continue;
					e.Cancel = true;
					return;
				}
				if (pvFile != null)
					files.Add(pvFile);
			}
			e.Result = files;
		}
		 */
		#endregion
		
		#region Keyboard shortcuts
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Control | Keys.F:
					pvImageViewer.ZoomToFit();
					return true;
				case Keys.Control | Keys.R:
					pvDataViewer.RenameFiles();
					return true;
				case Keys.PageDown:
					pvImageViewer.SetPageNext();
					return true;
				case Keys.PageUp:
					pvImageViewer.SetPagePrev();
					return true;
				case Keys.F1:
					var settingsDialog = new PVSettingsDialog();
					settingsDialog.PVDataViewer = pvDataViewer;
					settingsDialog.ShowDialog(this);
					//settingsDialog.Dispose();
					return true;
				default:
					return base.ProcessCmdKey(ref msg, keyData);
			}
		}
		#endregion
				
		#region Splitter Context Menu
		void cMenuSplitterVert_Click(object sender, EventArgs e)
		{
			double percent = splitContainer1.SplitterDistance / (double)ClientSize.Height;
			splitContainer1.Orientation = Orientation.Vertical;
			splitContainer1.SplitterDistance = (int)(ClientSize.Width * percent);
			ctxSplitterHoriz.Checked = false;
			ctxSplitterVert.Checked = true;
		}

		void cMenuSplitterHoriz_Click(object sender, EventArgs e)
		{
			double percent = splitContainer1.SplitterDistance / (double)ClientSize.Width;
			splitContainer1.Orientation = Orientation.Horizontal;
			splitContainer1.SplitterDistance = (int)(ClientSize.Height * percent);
			ctxSplitterHoriz.Checked = true;
			ctxSplitterVert.Checked = false;
		}

		void splitContainer1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
				ctxSplitter.Show(this, e.Location);
		}
		#endregion
	}
}
