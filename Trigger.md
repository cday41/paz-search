# HR trigger #

  * The user inputs whether HR establishment will begin after a number of timesteps or a number of suitable sites visited by an animal
  * The user will also input the number of steps or sites needed to trigger this behavior
  * As animals move they should keep track of the end points of each time step as a possible home range center
  * If the trigger is based on steps, the animal should count the number of active steps it has taken
  * If the trigger is sites, the animal should count the number of suitable-unoccupied sites that it has visited (that were in that condition at time of most recent visit)
  * Once an animal has exceeded its trigger value it should choose a home range center
  * The animal should construct a list of all sites that were both suitable and unoccupied at the time of visiting them as well as being within a continuous area that was perceived by the animal that is greater than minimum HR size (i.e. take all sites and eliminate those that don’t qualify by clipping against memory map with only large enough suitable polygons included)
  * If no steps fit the criteria for a home range center (i.e. step vii results in empty list) the animal should continue searching and repeat steps iv – vii as necessary
  * The animal should then calculate the distance between its current location and each site
  * The animal will then assign a raw rank value for each site based on its [decision rule](decision.md)
  * All raw ranks will then be divided by the sum of all ranks so that they sum to 1
  * All corrected sums will be sequentially added so that each site gets an interval the size of its probability of being chosen between 0 and 1
  * The interval in which the random number falls will be the HR center
  * Once a HR center is chosen a line in the text file should indicate that a HR center has been chosen and the animal is moving towards it
  * After choosing a HR center, animals will move back towards that point using the directed mover
  * Once an animal is within its perceptual range of that point, it will have arrived at the HR center (and will beam to that xy during that timestep)
  * If, after attempting to set up a HR and at the chosen HR center, the animal fails to do so, it will repeat steps vii – xiv including any steps taken in the process of moving back to the HR center