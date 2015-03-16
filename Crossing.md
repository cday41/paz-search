# Crossing #

  * When animal encounters boundary (on movement map) it crosses or reflects back based on crossing values of both polygons
  * Crossing values are based on value on movement map
  * Probability of crossing is crossing value of new polygon/crossing value of old polygon
  * Random number between 0-1 drawn
  * If drawn number is < probability of crossing, animal crosses
  * If drawn number is > probability of crossing, animal reflects back
  * If an animal encounters a boundary that is on the edge of the map (moving off map) it will not stop at the boundary, but instead will move off the map and die from leaving the area and have a line in the text file indicating this as the cause of death