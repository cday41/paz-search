# MSL when crossing boundary #

  * When step crosses a boundary (assuming crossing. See other section on crossing), the proportion of the first segment is dist. traveled/total step length where dist. traveled is from starting point to boundary
  * Second segment uses remaining proportion (1-first proportion) x step length of new polygon as remaining step length (orientation remains constant for all segments).
  * If second segment also encounters boundary, repeat.