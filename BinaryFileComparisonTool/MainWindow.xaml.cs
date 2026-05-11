using System.IO;
using System.Windows;

namespace BinaryFileComparisonTool
{
    public partial class MainWindow : Window
    {
        new List<string> _loadedFiles = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Drop = traite les fichiers déposés et vérifie si c'est bien des .bin
        private void Border_Drop(object sender, DragEventArgs e)
        {
            // GetDataPresent vérifie si les données sont des fichiers (FileDrop) avant de les traiter
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Cast des données en tableau de chaînes (string[]) pour obtenir les chemins des fichiers déposés
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string[] binFiles =
                    (from file in files
                     where String.Equals(Path.GetExtension(file), ".bin", StringComparison.OrdinalIgnoreCase)
                     // ToArray() convertit le résultat de la requête LINQ en un tableau de chaînes (string[])
                     select file).ToArray();
                if (binFiles.Length == 0)
                {
                    MessageBox.Show("Uniquement les .bin sont acceptés.", "Mauvaise extension de fichier", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (binFiles.Length < 2)
                {
                    MessageBox.Show("Deux fichiers .bin minimum sont requis pour la comparaison.", "Un seul fichier sélectionné", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                // Vide la liste précédente et stocke les nouveaux fichiers
                else
                {
                    _loadedFiles.Clear();
                    _loadedFiles.AddRange(binFiles);
                    // Supprime le contenu précédent de la TextBox avant d'afficher les nouveaux fichiers chargés
                    DropTextBlock.Text = "";
                    DropTextBlock.Text += $"Fichiers chargés : \n";

                    foreach (string file in binFiles)
                    {
                        // Path.GetFileName(file) extrait le nom du fichier à partir de son chemin complet, et l'ajoute à la TextBox pour affichage
                        DropTextBlock.Text += $"{Path.GetFileName(file)}\n";
                    }
                }
            }
        }

        // DragOver = vérifie si les données sont des fichiers et affiche un curseur de copie ou de non-autorisé
        private void Border_DragOver(object sender, DragEventArgs e)
        {
            // GetDataPresent vérifie si les données sont des fichiers (FileDrop) et ajuste l'effet de glisser-déposer en conséquence
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            // Handled = true indique que l'événement a été traité et empêche la propagation de l'événement à d'autres éléments de l'interface utilisateur
            e.Handled = true;
        }
    }
}