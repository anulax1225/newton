# Projet des orbites
Le but de se projet est de développé une simulation maniable de la gravitation universelle. Ceci se base sur le fait que dans l'espace sans frottement et sans réaction autre, il s'agit de la seul chose qui influence la trajectoire d'une planète.   

## Théorie
### Trouver le déplacement d'un corp 
<img src="img/EXEMPLE_GRAVITATION.png"><br>
Ici nous avons un bonne exemple avec deux corp en attraction. Les flèches montrent les vecteurs force. Ces deux vecteur ont une norme identique, c'est à dire les grandeurs de ceci sont les mêmes. De plus l'on remarque que le sens de ces vecteurs est opposé. *d* indique la distance qui sépare nos astres et Ma, Mb sont les masses de nos corps.

#### Trouver la force d'attraction en fonction de la gravitation universelle
Avec toute ces informations il est possible de calculé la force d'attraction des corps selon cette équation :<br>
*Fa/b = G * Ma * Mb / d^2*<br>
G la constante gravitationnelle à été placé à 1 (dans le cadre de la simulation) donc l'influe pas sur l'équation.

#### Trouver l'accélération en fonction de la force d'attraction 
Il existe un lien direct entre la force et l'accélération et il s'agit de l'équation :<br>
*→F = Ma * →a* <br>
Mais le problème est que l'équation de la gravitation noté plus haut, donne la magnitude de ce vecteur et n'en est pas un. Ce qui veux dire que allons devoir trouver le vecteur force avant. Heureusement cet opération est assez haïssez car il suffit de faire le produit scalaire de la magnitude et d'un vecteur de valeur (1,1). Une fois *→F* obtenu, nous réarrangeons l'équation de l'accélération pour l'isoler ce qui nous donne :<br>
*→a = →F/Ma*<br>

#### Trouver la vitesse en fonction de l'accélération
La vitesse est obtenu en faisant de la cinétique :<br>
*V1 = V0 + →a * Δt*<br>
Comme Δt vaut toujours 1 (décidé ainsi car la mesure de la gravité est fait à des temps similaire et régulier pour tout les corps).
Se qui nous laisse avec : <br>
*V1 = V0 + →a* <br>
Cet équation nous informe que avec les conditions cité au dessus, la vitesse est égal à la vitesse précédente + l'accélération.

### Conséquence 
Si l'on reprends la formule *V1 = V0 + →a* l'on remarquera que si la vitesse est nulle au départ les corp ne pourront que s'écraser l'un dans l'autre. Il sera donc nécessaire pour obtenir une belle simulation de donner une vitesse initial à nos corps.
De plus si deux astres on exactement.
