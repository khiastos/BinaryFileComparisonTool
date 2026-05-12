using System.IO;
using System.Windows;

namespace BinaryFileComparisonTool
{
    public partial class MainWindow : Window
    {
        List<string> _loadedFiles = new List<string>();
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
                    CompareButton.IsEnabled = true;

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

        // Logique de comparaison des fichiers .bin
        private async void Button_OnClick(object sender, RoutedEventArgs e)
        {

            Dictionary<int, byte[]> adresswithValue = new Dictionary<int, byte[]>();
            // Stocke chaque chemin de fichier en Task<byte[]>
            var fileReadTasks = _loadedFiles.Select(f => File.ReadAllBytesAsync(f));
            // Permet de lire le contenu de chaque fichier, stocke le résultat dans un tableau de tableaux de bytes (byte[][]) en gros un tableau qui stocke les données de chaque fichier
            byte[][] allBytes = await Task.WhenAll(fileReadTasks);

            /* Vérifie si tous les fichiers ont la même taille en comparant la longueur de chaque tableau de bytes
            Select = donne chaque taille de chaque fichier, Distinct = regroupe les tailles uniques, Count = compte le nombre de tailles uniques
            Ex : 3 fichiers de 16mb, Select = [16777216, 16777216, 16777216], Distinct = [16777216], Count = 1
            Donc si 2 fichiers de 16mb et un corrompu : Select [16777216, 16777216, 14680064], Distinct = [16777216, 14680064], Count = 2 */
            if (allBytes.Select(bytesLenght => bytesLenght.Length).Distinct().Count() != 1)
            {
                MessageBox.Show("Les fichiers n'ont pas la même taille, impossible de les comparer.", "Taille différente des .bin", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                // Vérifie chacun des octets dans chaque .bin
                for (int i = 0; i < allBytes[0].Length; i++)
                {
                    // Si une valeur est différente dans la même adresse
                    if (allBytes[1][i] != allBytes[0][i] && (allBytes[0][i] != 0xFF && allBytes[1][i] != 0xFF))
                    {
                        // Alors l'ajouter dans le dictionnaire
                        adresswithValue.Add(i, new byte[] { allBytes[0][i], allBytes[1][i] });
                    }
                }
                // Initialisation + ouverture de la fenêtre de conflit
                new CompareWindow(adresswithValue, _loadedFiles, allBytes).ShowDialog();
            }
        }
    }
}