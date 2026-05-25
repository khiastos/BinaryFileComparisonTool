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

        private void Border_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                List<string> binFiles = new List<string>();

                for (int i = 0; i < files.Length; i++)
                {
                    if (String.Equals(Path.GetExtension(files[i]), ".bin", StringComparison.OrdinalIgnoreCase))
                    {
                        binFiles.Add(files[i]);
                    }
                    else
                    {
                        MessageBox.Show("Uniquement les .bin sont acceptés.", "Mauvaise extension de fichier", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if (binFiles.Count >= 3)
                {
                    _loadedFiles.AddRange(binFiles);
                    DropTextBlock.Text = string.Empty;
                    DropTextBlock.Text += "Fichier chargés : \n";
                    CompareButton.IsEnabled = true;

                    foreach (string file in binFiles)
                    {
                        DropTextBlock.Text += $"{Path.GetFileName(file)}\n";
                    }
                }
                else
                {
                    MessageBox.Show("Trois fichiers .bin minimum sont requis pour la comparaison.", "Un seul fichier sélectionné", MessageBoxButton.OK, MessageBoxImage.Error);
                    _loadedFiles.Clear();
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

            // Récupère la taille totale du contenu
            int contentLength = allBytes[0].Length;

            // On itère jusqu'à atteindre la fin de la taille du contenu
            for (int i = 0; i < contentLength; i++)
            {
                // Lors d'une nouvelle itération, on prépare à l'avance un tableau pour relier chaque valeur de chaque adresse mémoire pour tous les fichiers
                byte[] allContentForCurrentIndex = new byte[allBytes.Length];

                // On itère le nombre de fois correspondant au nombre de fichiers
                for (int j = 0; j < allBytes.Length; j++)
                {
                    // On récupère tout le contenu du fichier sur lequel on itère
                    byte[] dump = allBytes[j];
                    allContentForCurrentIndex[j] = dump[i];
                }

                // On vérifie si il y a au moins un élément du tableau qui a une valeur, si c'est le cas alors on l'ajoute dans le dictionnaire 
                if (allContentForCurrentIndex.Distinct().Count() > 1)
                {
                    adresswithValue.Add(i, allContentForCurrentIndex);
                }
            }

            // Initialisation + ouverture de la fenêtre de conflit
            new CompareWindow(adresswithValue, _loadedFiles, allBytes).ShowDialog();
        }
    }
}