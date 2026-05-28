using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;

namespace BinaryFileComparisonTool
{
    public partial class CompareWindow : Window
    {
        private Dictionary<int, byte[]> _conflicts;
        private List<string> _binFileNames;
        private byte[][] _allBytes;
        public CompareWindow(Dictionary<int, byte[]> conflicts, List<string> binFileNames, byte[][] allBytes)
        {
            _conflicts = conflicts;
            _binFileNames = binFileNames;
            _allBytes = allBytes;
            InitializeComponent();

            ErrorNumber.Text = "Nombre de conflits : " + _conflicts.Count;

            // Création de la colonne "Adresse"
            ComparaisonResume.Columns.Add(new DataGridTextColumn
            {
                Header = "Adresse",
                Binding = new Binding("Key")
                {
                    StringFormat = "0x{0:X8}"
                }
            });


            // Création de la colonne "Valeur"
            for (int i = 0; i < _binFileNames.Count; i++)
            {
                string file = _binFileNames[i];
                ComparaisonResume.Columns.Add(new DataGridTextColumn
                {
                    Header = Path.GetFileNameWithoutExtension(file),
                    Binding = new Binding($"Value[{i}]")
                    {
                        StringFormat = "0x{0:X2}"
                    }
                });
            }

            // Lier la liste des entrées de conflit à l'interface utilisateur pour afficher les adresses des conflits
            ComparaisonResume.ItemsSource = _conflicts;
        }

        public void Merge_OnClick(object sender, RoutedEventArgs e)
        {
            byte[] mergedBytes = _allBytes[0];

            foreach (KeyValuePair<int, byte[]> conflict in _conflicts)
            {
                Dictionary<byte, int> countsValue = new Dictionary<byte, int>();

                foreach (byte value in conflict.Value)
                {
                    if (countsValue.ContainsKey(value))
                    {
                        countsValue[value]++;
                    }
                    else
                    {
                        countsValue.Add(value, 1);
                    }
                }

                mergedBytes[conflict.Key] = countsValue
                    .Where(unprogrammedByte => unprogrammedByte.Key != 0xFF)
                    .OrderByDescending(pair => pair.Value)
                    .First().Key;
            }

            // Enregistrer le fichier fusionné
            SaveFileDialog mergedFilePath = new SaveFileDialog
            {
                Filter = "Fichiers binaires (*.bin)|*.bin",
                Title = "Enregistrer le fichier fusionné"
            };

            if (mergedFilePath.ShowDialog() == true)
            {
                File.WriteAllBytes(mergedFilePath.FileName, mergedBytes);
                MessageBox.Show($"Fichier fusionné créé avec succès : {mergedFilePath.FileName}", "Fusion réussie", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Enregistrement annulé. Le fichier fusionné n'a pas été créé.", "Enregistrement annulé", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


    }
}
