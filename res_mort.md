# Resident mortality #

  * Once animals establish HRs they become residents
  * During each timestep that a resident is alive each should be subjected to per timestep mortality risk
  * If the random number drawn is below the per timestep risk the animal dies
  * During the interdispersal period each resident should be subject to a single winter kill risk
  * If the random number drawn is below the winter kill risk the animal dies
  * Dead residents should have their HRs removed from the social map and should not reproduce
  * Dead residents should also have a line in the text file stating whether they died during a timestep or the interdispersal period and the time of death (date and time for timestep, year for winter kill)