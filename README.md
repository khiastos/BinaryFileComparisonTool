# Tool : Comparateur de fichier .bin 

Dans le cadre de mon projet de récupération de firmware d'un Tamagotchi Pix, j'avais besoin d'un outil me permettant de comparer différents fichiers binaires entre eux.

Ainsi, cet outil me permet de charger au moins trois fichiers .bin, de détecter les adresses mémoire où les valeurs diffèrent entre les dumps et de générer un fichier fusionné en appliquant un vote majoritaire : c'est-à-dire en retenant la valeur qui apparaît le plus souvent à chaque adresse parmi tous les fichiers comparés.

---
## Outils et technologies utilisés

- **C#**
- **WPF**

---
## Fonctionnement
<p align="center">
  <img width="783" height="442" alt="Capture d&#39;écran 2026-07-03 183534" src="https://github.com/user-attachments/assets/917d56e0-958c-45cc-a444-270ed0577152" />
  <br/><em>Ouverture du tool</em>
</p>

---
 
<p align="center">
  <img width="785" height="437" alt="Fichiers chargés" src="https://github.com/user-attachments/assets/71265208-2081-4ec6-8bed-de5d6738bb74" />
  <br/><em>Glisser-déposer de trois .bin, les noms des fichiers chargés s'affichent dans la zone de dépôt</em>
</p>

---
 
<p align="center">
  <img width="833" height="487" alt="Fenêtre de comparaison" src="https://github.com/user-attachments/assets/d17c5f9d-53de-44f9-a6f8-a28aa892d833" />
  <br/><em>Fenêtre de comparaison : chaque ligne correspond à une adresse mémoire où les dumps ont une valeur différente</em>
</p>

---
 
<p align="center">
  <img width="344" height="150" alt="Capture d&#39;écran 2026-07-03 183640" src="https://github.com/user-attachments/assets/0b1ce028-2c00-4110-93f2-c37a320ca37a" />
  <br/><em>Après avoir cliqué sur "Merger", le vote majoritaire est appliqué et le fichier .bin fusionné est exporté à l'emplacement choisi</em>
</p>

---
 
<p align="center">
  <img width="787" height="444" alt="Erreur taille différente" src="https://github.com/user-attachments/assets/b7b4f3d8-0f56-4b43-ae3f-c2ea35e34704" />
  <br/><em>Message d'erreur si les fichiers n'ont pas la même taille, des dumps de tailles différentes indiquent une lecture incomplète ou corrompue, la comparaison n'aurait pas de sens</em>
</p>

---
 
<p align="center">
  <img width="401" height="150" alt="Erreur fichiers insuffisants" src="https://github.com/user-attachments/assets/a011578d-a6da-4d3d-aafe-6f2d2d86907a" />
  <br/><em>Message d'erreur s'il n'y a pas au moins trois fichiers .bin chargés</em>
</p>

---
 
<p align="center">
  <img width="783" height="437" alt="Fichiers identiques" src="https://github.com/user-attachments/assets/33420d5b-665a-4b5c-88d7-4de4d27e4cf3" />
  <br/><em>Lorsque les fichiers sont identiques, aucun conflit n'est détecté</em>
</p>
