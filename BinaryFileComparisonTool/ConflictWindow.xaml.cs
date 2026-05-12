using System.Dynamic;
using System.IO;
using System.Windows;

namespace BinaryFileComparisonTool
{
    public partial class ConflictWindow : Window
    {
        private Dictionary<int, byte[]> _adresswithValue;
        private List<string> _binFileNames;
        public ConflictWindow(Dictionary<int, byte[]> adresswithValue, List<string> binFileNames)
        {
            _adresswithValue = adresswithValue;
            _binFileNames = binFileNames;
            InitializeComponent();

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
                ConflitResume.Columns.Add(new System.Windows.Controls.DataGridTextColumn
                {
                    // Header = nom du fichier, Binding = liaison de données pour afficher les valeurs des conflits pour chaque fichier
                    Header = Path.GetFileNameWithoutExtension(file),
                    Binding = new System.Windows.Data.Binding(Path.GetFileNameWithoutExtension(file))
                });
            }

            // Lier la liste des entrées de conflit à l'interface utilisateur pour afficher les adresses des conflits
            ConflitResume.ItemsSource = conflitEntries;
        }
    }
}
