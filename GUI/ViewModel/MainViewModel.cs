using Core.Entity;
using KaiserMVVMCore;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace GUI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private int selectedFileCount;
        public int SelectedFileCount { get => selectedFileCount; set => base.Set(ref selectedFileCount, value); }

        private List<string> listOfSelectedFiles = new();
        private List<string> ListOfSelectedFiles { get => listOfSelectedFiles; set => base.Set(ref listOfSelectedFiles, value); }

        public ObservableCollection<Replacement> ListOfReplacements { get; set; } = new ObservableCollection<Replacement>();


        public RelayCommand OpenFilesCommand { get; set; }
        public RelayCommand ReplaceValuesCommand { get; set; }

        public MainViewModel()
        {
            this.OpenFilesCommand = new RelayCommand(this.OpenFilesCommandAction);
            this.ReplaceValuesCommand = new RelayCommand(this.ReplaceValuesCommandAction);

            for (var i = 0; i < 3; i++)
            {
                this.ListOfReplacements.Add(new Replacement());
            }
        }

        private void OpenFilesCommandAction()
        {
            var fd = new OpenFileDialog();
            fd.CheckFileExists = true;
            fd.Multiselect = true;
            if (fd.ShowDialog() == true)
            {
                this.SelectedFileCount = fd.FileNames.Length;
                this.ListOfSelectedFiles.AddRange(fd.FileNames);
            }
        }

        private void ReplaceValuesCommandAction()
        {
            List<Replacement> replacements = this.ListOfReplacements.Where(
                e => 
                    e != null && 
                    !String.IsNullOrEmpty(e.OriginalValue) &&
                    !String.IsNullOrEmpty(e.ReplacementValue)
            ).ToList();

            try
            {
                foreach (var file in this.ListOfSelectedFiles)
                {
                    try
                    {
                        string fileContent = File.ReadAllText(file);
                        //fileContent = System.Text.RegularExpressions.Regex.Unescape(fileContent);
                        foreach (Replacement replacement in replacements)
                        {
                            fileContent = fileContent.Replace(replacement.OriginalValue, replacement.ReplacementValue);
                        }

                        File.WriteAllText(file + ".updated", fileContent);

                        fileContent = null;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"File {file} could not be read: {e.Message}!");
                        if (MessageBox.Show($"File {file} could not be read! Continue?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No)
                            return;
                    }
                }

                MessageBox.Show("All replacements done.", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch(Exception e)
            {
                Debug.WriteLine($"Error in foreach loop: {e.Message}!");
            }

            

            
        }
    }
}
