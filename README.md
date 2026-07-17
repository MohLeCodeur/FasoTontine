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

---

## 🛠️ État actuel et architecture

L'application est configurée comme un **gabarit de développement (template)** :
- **Design & UI intacts** : Le design CSS personnalisé, l'intégration Bootstrap et les vues Razor (`.cshtml`) sont entièrement fonctionnels et affichent des pages d'accueil, de tontines, de formulaires de paiement et d'authentification.
- **Logique épurée (Stubs)** : La logique métier et l'accès aux données ont été nettoyés et marqués par des commentaires `// TODO: ...`. Les contrôleurs renvoient pour le moment des ensembles de données vides et effectuent des redirections pour permettre de naviguer sur l'application de façon fluide.
- **Prêt pour base de données** : Les classes de modèles (comme `User`, `Tontine`, `Cotisation`) sont prêtes à être utilisées pour générer vos migrations Entity Framework Core ou être connectées à votre API.

---

## 📂 Structure principale du projet

- 📁 **Controllers/** : Les contrôleurs de routage (`AuthController`, `HomeController`, `TontineController`, `CotisationController`).
- 📁 **Models/** : Les entités et structures de données.
- 📁 **Views/** : Les templates Razor (HTML / C#) contenant le design premium.
- 📁 **wwwroot/** : Les fichiers statiques (images, feuilles de style globales et scripts js).
