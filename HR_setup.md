# Setting up a HR #

  * At the HR center, [Animals](Animals.md) cast out 30 points based on random drawing of direction with distance of h meters from the home-range center where h = j`*r*`s where j is a random number drawn from a normal distribution of mean 1.2 and standard deviation of 0.1, r is the radius of a circle with the size of the animal‟s minimum home range (r = √ (HRmin/π)) and s is the stretch factor of 1.0
  * These 30 points are connected to form a polygon
  * These points are calculated by
    1. generating a list of 30 angles by getting a random number then multiply by Math.PI multiplied by 2
    1. Then that list is sorted clockwise (requirement of ESRI to make a polygon)
    1. Then for each point the radius is calculated as follows
      * Calculate the square root of the animals required area / PI
      * Multiply that value by a random number between 1.0 and 1.2 (To allow for not being a perfect circle)
      * Then that value is multiplied by the [stretch factor](StretchFactor.md).
    1. The X value for the new point is calculated by the home range center point X + the radius just calculated times the Cos(Angle(N))
    1. The Y value for the new point is calculated by the home range center point Y + the radius just calculated times Sin (Angle(N))
    1. Advance N by one to get the next Angle in the list
  * This is repeated 30 times and a polygon is made from those 30 points.
  * All areas that are unsuitable or occupied by the same sex are removed
  * If the area of the polygon remains greater than the minimum HR area the animal establishes a HR
  * If the area is below the minimum HR area, the old polygon and points are discarded and the [stretch factor](StretchFactor.md) increases by 10% and steps 1 – 6 are repeated
  * If after 10 tries the animal fails to establish a HR it selects a new HR center and keeps trying
  * If a HR is created the polygon is added to the social map and the occupied status of that sex is filled in with the value of that animal
  * Once a disperser settles a line in the text file should say that the animal settled and the XY location of the HR center
  * If a disperser settles it then becomes a resident and is subject only to resident functions and no longer moves
  * If a disperser fails to establish a HR by the end of the dispersal season it will die of winter kill with a line in the text file indicating that. It should also stop moving and not reproduce.