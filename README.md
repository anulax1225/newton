# Projet des orbites

## Prérequis et Nécessaire

#### Matériel et logiciel 

* Un éditeur de texte
* Un clavier et une souri 

#### Librairie 

* Raylib.h ``` dotnet add package Raylib```

## Contexte 

Se dépôt est un projet de d'atelier informatique au CPNE, il dure 85 périodes. Je travaille tout seul dessus toute les semaines depuis le 16.08.23 a raison 10h30m en cours et 2h à 4h cher moi. Le sujet est la gravitation universelle de Newton énoncé en 1687 et sont implémentation dans une simulation informatique. 

## But

Le but de se projet est de développé une simulation paramétrer de la gravitation universelle. Ceci se base sur le fait que dans l'espace sans frottement et sans autre réaction, il s'agit de la seul chose qui influence la trajectoire des planètes. Le projet sera fait en 2D mais une implémentation en 3D est totalement possible. L'idée serait de permettre à l'utilisateur de créer son propre "système solaire" en observant puis modifiant les différents paramètre qui gère les mouvements des corps (masse, vitesse, position). Le but étant de créer des orbites stables. 

## Interface graphique
Sur mon interface les corps seront de simple boule de couleur qui bouge en fonction d'une vitesse initial et l'attraction au autre corps :

<img src="img/orbites.png">

 Si l'on presse V l'interface nous les vecteurs vitesse associé à chaque corps :
 
<img src="img/orbitesV.png">

Si l'on click sur une planète on obtiens des informations complémentaire sur elle :

<img src="img/orbitesInfo.png">

Si l'on presse sur A après avoir clicker sur un astre et l'on sera dans son référentielle :

![Changement de réferentielle](video/ref.gif)

## Théorie Physique

### Trouver le déplacement d'un corp

<img  src="img/EXEMPLE_GRAVITATION.png"><br>

Ici nous avons un bonne exemple avec deux corp en attraction. Les flèches montrent les vecteurs force. Ces deux vecteur ont une norme identique, c'est à dire les grandeurs de ceci sont les mêmes. De plus l'on remarque que le sens de ces vecteurs est opposé. *d* indique la distance qui sépare nos astres et Ma, Mb sont les masses de nos corps.

  

#### Trouver la force d'attraction en fonction de la gravitation universelle

Avec toute ces informations il est possible de calculé la force d'attraction des corps selon cette équation :

*Fa/b = G * Ma * Mb / d^2*

G la constante gravitationnelle à été placé à 1 (dans le cadre de la simulation) donc l'influe pas sur l'équation.

  

#### Trouver l'accélération en fonction de la force d'attraction

Il existe un lien direct entre la force et l'accélération et il s'agit de l'équation :

*→F = Ma * →a*  

Mais le problème est que l'équation de la gravitation noté plus haut, donne la magnitude de ce vecteur et n'en est pas un. Ce qui veux dire que allons devoir trouver le vecteur force avant. Heureusement cet opération est assez haïssez car il suffit de faire le produit scalaire de la magnitude et d'un vecteur de valeur (1,1). Une fois *→F* obtenu, nous réarrangeons l'équation de l'accélération pour l'isoler ce qui nous donne :

*→a = →F/Ma*

  

#### Trouver la vitesse en fonction de l'accélération

La vitesse est obtenu en faisant de la cinétique :

*V1 = V0 + →a * Δt*

Comme Δt vaut toujours 1 (décidé ainsi car la mesure de la gravité est fait à des temps similaire et régulier pour tout les corps).

Se qui nous laisse avec : 

*V1 = V0 + →a*  

Cet équation nous informe que avec les conditions cité au dessus, la vitesse est égal à la vitesse précédente + l'accélération.

  

### Conséquence

#### Inertie
Si l'on reprends la formule *V1 = V0 + →a* l'on remarquera que si la vitesse est nulle au départ les corp ne pourront que s'écraser l'un dans l'autre. Il sera donc nécessaire pour obtenir une belle simulation de donner une vitesse initial à nos corps.

#### Distance nul
De plus si deux astres on exactement la même position, la division  *G * Ma * Mb / d^2* donnera une division par zéro ce qui nous donnera l'infinie. Curieusement cela ne pose pas de problème en pratique quand lim x => 0, la vitesse augmente exponentiellement vers l'infinie se qui la propulse loin du  Δd = 0. Mais cela veux aussi dire qu'il est impossible de rassembler en un point infinitésimal d'espace, se qui est le principe d'un troue noir.

