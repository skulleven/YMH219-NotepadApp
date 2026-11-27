using System.Diagnostics;
using System.Text.Json;

namespace Notepad
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ReadJson();
            this.KeyPreview = true;
            this.KeyDown += KeyDownHandler;
        }

        public class NoteData
        {
            public string NoteName { get; set; }
            public string NoteContent { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime EditedAt { get; set; }

        }

        private List<NoteData> noteNames = new List<NoteData>();
        NoteData currentNote;

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // Handle text changed event here
        }

        private void SaveFile()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                saveFileDialog.Title = "Save File";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    System.IO.File.WriteAllText(filePath, richTextBox1.Text);
                }
            }
        }
        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.T)
            {
                SaveFile();
                e.SuppressKeyPress = true;
            }
            if (e.Control && e.KeyCode == Keys.S)
            {
                SaveNote(currentNote);
                e.SuppressKeyPress = true;
            }
        }
        private void ReadJson()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string filePath = "notes.json";
            if (System.IO.File.Exists(filePath))
            {
                string jsonString = System.IO.File.ReadAllText(filePath);
                noteNames = JsonSerializer.Deserialize<List<NoteData>>(jsonString, options);
                foreach (var note in noteNames)
                {
                    listBox1.Items.Add(note.NoteName);
                }
            }
        }
        private void CreateFile()
        {
            var note = new NoteData
            {
                NoteName = $"Note {noteNames.Count + 1}",
                NoteContent = string.Empty,
                CreatedAt = DateTime.Now,
                EditedAt = DateTime.Now
            };
            noteNames.Add(note);
            WriteListToJson();

            listBox1.Items.Add(note.NoteName);
            currentNote = note;
            listBox1.SelectedItem = note.NoteName;
            label1.Text = $"Created At: {note.CreatedAt.ToString("g")} | Edited At: {note.EditedAt.ToString("g")}";
        }
        private void SaveNote(NoteData? noteData)
        {
            if (noteData == null)
            {
                return;
            }

            noteData.NoteContent = richTextBox1.Text;

            foreach (var note in noteNames)
            {
                if (note.NoteName != null && note.NoteName == noteData.NoteName)
                {
                    note.NoteContent = richTextBox1.Text;
                    note.EditedAt = DateTime.Now;
                    label1.Text = $"Created At: {note.CreatedAt.ToString("g")} | Edited At: {note.EditedAt.ToString("g")}";
                    break;
                }
            }
            WriteListToJson();
        }

        private void WriteListToJson()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(noteNames, options);

            string filePath = "notes.json";
            File.WriteAllText(filePath, jsonString);
            Debug.WriteLine("Saved successfully!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            CreateFile();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Text != currentNote?.NoteContent)
            {
                SaveNote(currentNote);
            }
            if(listBox1.SelectedItem == null)
            {
                return;
            }
            foreach (var note in noteNames)
            {
                if (note.NoteName == listBox1.SelectedItem.ToString())
                {
                    richTextBox1.Text = note.NoteContent;
                    currentNote = note;
                    label1.Text = $"Created At: {note.CreatedAt.ToString("g")} | Edited At: {note.EditedAt.ToString("g")}";
                    break;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(currentNote == null)
            {
                return;
            }
            foreach(var note in noteNames)
            {
                if (note.NoteName == currentNote.NoteName)
                {
                    noteNames.Remove(note);
                    WriteListToJson();
                    listBox1.Items.Remove(note.NoteName);
                    richTextBox1.Text = string.Empty;
                    break;
                }
            }

        }
    }
}
