using System.Dynamic;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace BinaryFileComparisonTool
{
    public partial class CompareWindow : Window
    {
        private Dictionary<int, byte[]> _adresswithValue;
        private List<string> _binFileNames;
        private byte[][] _allBytes;
        public CompareWindow(Dictionary<int, byte[]> adresswithValue, List<string> binFileNames, byte[][] allBytes)
        {
            _adresswithValue = adresswithValue;
            _binFileNames = binFileNames;
            _allBytes = allBytes;
            InitializeComponent();

            ErrorNumber.Text = "Nombre de conflits : " + _adresswithValue.Count;

            // Liste qui stocke les adresses des conflits
            List<dynamic> conflitEntries = new List<dynamic>();

            // Pour chaque entrée dans le dictionnaire contenant les adresses et les valeurs différentes
            foreach (var entry in _adresswithValue)
            {
                // Créer un objet dynamique pour stocker l'adresse et les valeurs des conflits
                dynamic row = new ExpandoObject();
                // Cast l'objet dynamique en dictionnaire pour pouvoir ajouter des propriétés dynamiques
                var dict = (IDictionary<string, object>)row;

                // Ajouter l'adresse du conflit à l'objet dynamique
                dict["Adress"] = $"0x{entry.Key:X8}";

                for (int i = 0; i < entry.Value.Length; i++)
                {
                    // Ajouter les valeurs des conflits pour chaque fichier à l'objet dynamique
                    dict[Path.GetFileNameWithoutExtension(_binFileNames[i])] = $"0x{entry.Value[i]:X2}";
                }

                // Ajouter l'objet dynamique à la liste des entrées de conflit
                conflitEntries.Add(row);
            }

            foreach (var file in _binFileNames)
            {
                // Ajouter une colonne pour chaque fichier dans le DataGrid
                ComparaisonResume.Columns.Add(new System.Windows.Controls.DataGridTextColumn
                {
                    // Header = nom du fichier, Binding = liaison de données pour afficher les valeurs des conflits pour chaque fichier
                    Header = Path.GetFileNameWithoutExtension(file),
                    Binding = new System.Windows.Data.Binding(Path.GetFileNameWithoutExtension(file))
                });
            }

            // Lier la liste des entrées de conflit à l'interface utilisateur pour afficher les adresses des conflits
            ComparaisonResume.ItemsSource = conflitEntries;
        }

        public void Merge_OnClick(object sender, RoutedEventArgs e)
        {
            // Permet de fusionner les fichiers .bin en utilisant les valeurs des conflits pour créer un nouveau fichier fusionné
            byte[] mergedBytes = new byte[_allBytes[0].Length];
            Dictionary<int, byte[]> mergedConflicts = new Dictionary<int, byte[]>();

            // Pour chaque position dans les fichiers .bin, comparer les valeurs des conflits et choisir une valeur pour le fichier fusionné
            for (int i = 0; i < mergedBytes.Length; i++)
            {
                Dictionary<byte, int> counts = new Dictionary<byte, int>();

                // Compter le nombre d'occurrences de chaque valeur à la position i dans les fichiers .bin
                for (int j = 0; j < _allBytes.Length; j++)
                {
                    byte value = _allBytes[j][i];
                    if (!counts.ContainsKey(value))
                    {
                        counts[value] = 0;
                    }
                    counts[value]++;
                }

                // Choisir la valeur la plus fréquente à la position i pour le fichier fusionné
                byte mostFrequentValue = counts.OrderByDescending(kv => kv.Value).First().Key;

                if (counts[mostFrequentValue] > _allBytes.Length / 2)
                {
                    mergedBytes[i] = mostFrequentValue;
                }
                else
                {
                    // Stocker les conflits pour cette adresse dans un dictionnaire
                    mergedConflicts[i] = _allBytes.Select(bytes => bytes[i]).ToArray();
                }
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
