# CondorDotNet
A simple .Net library for reading Polar files from the Condor 2 simulator

## Caveats
* This contains none of the data files contained in Condor 2. You need to have
[bought a copy of the sim](https://www.condorsoaring.com/order/)
(it's only €60, so there's no excuse for the hours of use you'll get from it!)
* This is a library, there's [currently] no front end for it. If you're feeling lazy, then modify the tests to extract the data you want!

## Rationale
Because I was _"too lazy"_ to enter polar values into the excellent [XCSoar Glide Computer](https://www.xcsoar.org/),
I decided to write this library. In reality it took about a hundred times longer than the _"hard"_ way, but in the
process I've learned a lot more about speed polars and the effects of mass, etc.

Yes, I know polars are published for most (if not all of the aircraft in Condor 2), but I wanted to XCSoar to use the
same data as the sim. Although in hindsight the dev team have probably used pretty realistic polars already.
