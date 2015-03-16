# Activity #

  * Animal activity periods are set by the user
  * This can include up to 24 separate active and asleep periods (1 hr increments)
    * IS IT POSSIBLE TO HAVE PARTIAL HOURS FOR ACTIVITY IF TIMESTEPS ARE <1HR (I.E. 9.5 HOURS ACTIVE, 14.5 HOURS ASLEEP WITH ½ HR TIMESTEPS)?
  * Animals can begin either awake or asleep (with first activity = 0 for initial sleepers)
  * Time awake/asleep for each activity period will be drawn from distribution with the mean and standard deviation as input by user (there will not be a constraint with synchronization every 24 hours)
  * Animals that are asleep with not move or set up HRs but will be subject to energy gains and losses as well as mortality
  * Offspring are always born on the first  timestep of the first day of their birth year
  * Released animals begin activity cycle based on time of release