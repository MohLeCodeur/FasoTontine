# FasoTontine 🌟

**FasoTontine** est un modèle d'application web de gestion d'épargne collective (tontine) en ligne. Ce dépôt fournit l'ensemble des maquettes visuelles, le design premium (CSS/Bootstrap) et l'architecture MVC propre (Modèles, Vues, Contrôleurs) prête à recevoir votre logique d'implémentation de base de données.

---

## 🚀 Comment lancer l'application localement

### Prérequis
- Avoir installé le **SDK .NET Core** (version 8.0, 9.0 ou 10.0+). Vous pouvez vérifier votre installation avec la commande :
  ```bash
  dotnet --version
  ```

### Étapes de lancement

1. **Cloner le dépôt** (si ce n'est pas déjà fait) :
   ```bash
   git clone https://github.com/MohLeCodeur/FasoTontine.git
   cd FasoTontine
   ```

2. **Restaurer les dépendances** :
   ```bash
   dotnet restore
   ```

3. **Lancer l'application** :
   ```bash
   dotnet run
   ```

4. **Accéder à l'application** :
   Une fois l'application démarrée, ouvrez votre navigateur et accédez à l'adresse locale indiquée dans votre console (généralement [http://localhost:5299](http://localhost:5299)).



## 📂 Structure principale du projet

- 📁 **Controllers/** : Les contrôleurs de routage (`AuthController`, `HomeController`, `TontineController`, `CotisationController`).
- 📁 **Models/** : Les entités et structures de données.
- 📁 **Views/** : Les templates Razor (HTML / C#) contenant le design premium.
- 📁 **wwwroot/** : Les fichiers statiques (images, feuilles de style globales et scripts js).
