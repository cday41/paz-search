# Energy limits #

  * Energy levels should never exceed maximum energy level input to GUI or Xml
  * Energy levels that go below the minimum energy level should cause dispersers to die of [starvation](starvation.md)
  * Energy levels that go below the search/forage trigger should cause animals to switch from searching to foraging
  * Energy levels that exceed the search/forage trigger should cause animals to switch from foraging to searching