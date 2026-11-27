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
                NoteContent = string.Empty
            };
            noteNames.Add(note);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(noteNames, options);

            string filePath = "notes.json";
            File.WriteAllText(filePath, jsonString);
            Debug.WriteLine("Saved successfully!");

            listBox1.Items.Add(note.NoteName);
            currentNote = note;
            listBox1.SelectedItem = note.NoteName;
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
                    break;
                }
            }
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
            SaveNote(currentNote);
            foreach (var note in noteNames)
            {
                if (note.NoteName == listBox1.SelectedItem.ToString())
                {
                    richTextBox1.Text = note.NoteContent;
                    currentNote = note;
                    break;
                }
            }
        }
    }
}
