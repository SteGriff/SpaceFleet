# Space Fleet

(Working title, considering "Hapless Galaxy", or "Unreasonable Space Conquest")

## Overview and history

This is a cheap, tongue-in-cheek console game, drawing on Galactic Civilizations II. It's a simplified 4X game of taking over the galaxy by force. It features/will feature ship and planet management, tech research, space battles, and more. The aim is to rid the galaxy of everyone but yourself... I suppose.

I started writing this on my breaks at KGV College when I was 17, and rediscovered it this year. So I've been working on it, on and off, from 2011. We wrote VB.Net at College, so... sorry about that. But switching languages often keeps you mentally nimble :)

### License

This is free work in the public domain, licensed under the [UNLICENSE](http://unlicense.org/unlicense)


## The game world

Space Fleet is set in a 1-dimensional universe. Your home planet has position 0 in this world, which stretches out like a nubmer line. This keeps co-ordinates and collisions simple!

I would like fleet management for allied units occupying the same spot (like fleet markers in GCII).

The player is able to rename their race, and will also be able to rename Earth and Mars.


## The science

The science and measurements of this game are completely fictional and non-scientific. I am considering changing the game's unit of distance from parsecs to a joke measurement... "space miles" maybe?


## Combat

Units which pass each other during their move step and are enemies will enter a space battle. Any allied units on the same spot will be involved. 

## Teams

The game is considered to be played in two teams; Human and Enemies (Computer). Teams are stored as an `Integer`.
	
	Human : 0
	Enemies : 1
	
This means that enemies don't bother each other, just the human player. We can build this out in future.
