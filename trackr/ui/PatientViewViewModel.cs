﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trackr.core;

namespace trackr.ui
{
    public class PatientViewViewModel : ViewModelBase
    {
        private static readonly Lazy<PatientViewViewModel> Lazy =
        new Lazy<PatientViewViewModel>(() => new PatientViewViewModel());
        
        public static PatientViewViewModel Instance => Lazy.Value;
        private PatientViewViewModel() {}
        
        public event EventHandler Closing;

        public System.Collections.ObjectModel.ObservableCollection<List<string>> strings { get; set; }

        #region Properties

        public TherapyPatient ActivePatient
        {
            get => Workspace.Instance.ActivePatient;
            set => Workspace.Instance.ActivePatient = value;
        }

        public TherapySession ActiveSession
        {
            get => Workspace.Instance.ActivePatient.GetActiveSession();
        }
        
        #endregion

        public void SendNoteToWorkspace(string rawNote)
        {
            var tokens = rawNote.Split(new[] {'[', ']'}, 2, StringSplitOptions.RemoveEmptyEntries);
            if (!tokens.Any()) return;
            try
            {
                var timeStamp = DateTime.ParseExact(tokens[0], "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                var noteContent = tokens[1];
                ActivePatient.GetActiveSession().InsertNote(timeStamp, noteContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Close()
        {
            if (ActiveSession.SessionRunning)
            {
                ActivePatient.EndSession();
            }
            Closing?.Invoke(this, EventArgs.Empty);
        }
        
        
        
    }
}
