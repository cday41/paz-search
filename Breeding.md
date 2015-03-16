# Reproduction #

  * During the interdispersal period each resident female has a chance to breed
  * For each female, if the random number drawn is below the value for probability of pregnancy than the animal produces offspring.
  * If the random number drawn is greater than the value for probability of pregnancy than the animal is skipped but can reproduce in subsequent years if it survives
  * For pregnant females, the number of offspring will be drawn from a distribution based on the mean and standard deviation of offspring input by the user (This should be able to be 0 but not < 0)
  * Sex for each offspring will be decided based on the probability of young being female. If the random number is below the value the offspring will be female. Otherwise it will be male
  * Every offspring will start in the center of its mother’s HR