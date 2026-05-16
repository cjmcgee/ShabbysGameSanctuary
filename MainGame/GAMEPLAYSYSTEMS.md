# Gameplay Systems

## Emulated games
Emulated games have these traits:
- Arcarde, console, or computer
- Compatible systems
- Required add-ons
- Controller type
- Available from 
- Unlock criteria
	- Reputation impacts for unlock
	- Reputation impacts for unlock step completion
	- Price / resale price / trade value / price drops
- Release date (actual vs. in game)
- Box art
- Manual art
- ROM size
- ROM hashes 
- Hype level and Reaction level 
- Dialog
	- Hype dialog
	- Reaction dialog
	- Unlock hints
	- Unlock achiivement dialog

Owned or played games track:
- Aquisition date
- First played date 
- High score
- Completion percent 
- Total time played
- Play seession data
	- Date played
	- Score or completion achieved
	- Time played
- Unlock steps completed / Unlock steps not yet completed

### Art and ROMs
The player will have to provide these. We have a tool to locate missing data and let the user fill in missing. We may be able to download art at user request. But none of this can be included with the game unless we can somehow manage a miracle. 

### High score / completed percent
The highest score achieved in a game or completion percent is harvested from emulation sessions and saved on exit of the session. 

Hitting certain scores or completion percent can trigger completion of unlock completion step.

### Available from
Games can come from stores, be traded for, can be gifted, or can come from events. 

### Compatible systems
This can be a console system, arcade platform, or home computer. Arcade games will show up in specific map locations automatically on specified release date. Console and home comptuer games become available for unlock (or automatically unlock if they have no criteria) on their release date. 

Some games may be compatible with more than one console, arcade platform or home computer system.  Games may require a specific controller. Games may require an add-on like a disk drive, high res module, memory expansion, etc. 

Some systems may not be directly owned by the user but become "available" to the user by being unlocked. 

### Release date
As the release date approaches hype will build and NPC will because to have dialog hyping the game. This will direct the user which NPCs/organizations care about the game. They will also get an idea how hype the game is and thus how much impact the game will have on their reputation with these NPCs/orgs. 

Once the game is released it will have reaction and NPCs will comment about their experiences and feelings about the game. This will let the know if the gains for owning or playing this game will match the hype. But they user can try to hype their reactions for some games more. We will need something to limit how much they can do this. 

## Systems
Systems have 
- Specs
- Box art
- Manual
- Release date (actual vs. in game)
- Hype / reaction
- Backwards compatible with
- Controller compatibility
- Included controllers
- Add ons available
- Price / resale price / trade value / price drops
- Quirks / problems

### Art and manual
Again we cannot include these with the game, but we can probably let the user download this automatically when in the rom manager. 

### BIOS 
Systems will need zero or BIOS images. We will need to treat these basically as additional ROMs.

## Add-ons / controllers
Add ons are like disk drive, monitor, ccontroller, memory expansions, etc. 
Controllers are like Joy sticks, paddles, track ball, light gun, etc. 

These have:
- Compatability
- Price data
- Availability 
- Box art and manual?
- Hype / reaction
- Quirks / problems

## Reputations
Player can gain reputation with an organization. An organization might care about specific game systems, specific game genres, or specific games. 

Reputation can be affected by:
- Owning systems, add-ons, games
- Achieving specific high score limits or completion limits in specific games
- Achieving scores in minigames
- Unlocking relationships with members via member NPCs' dialog trees
- Attending and completing things in the organization's events

## Organizations
Organizations have one or more member NPCs. Organizations can put on events (by themselves or jointly with other organizations). 

Organizations can have:
- Meeting location or locations 
- Members
- Events
- Unlockable areas
- Unlockable vendors
- Unlockable owned systems 
- Unlockable minigames
- Unlockable NPCs

## Holidays / events
Holdays and events may limit the available map. They will change the map art. NPCs will have unique locations. NPCs will have unique dialog. Unique art/set pieces outside of the normal gameplay may show up here.   

Events are put on by an organization or cooperation between organizations. 

## Seasons
Seasons mean changes in weather and thus map art. NPC dialogs will change to reflect the season. Activities and jobs available on the map will change by season. Minigames can change in availablity or details by season. Events and holidays are tied to specific datesand thus to a season. Some map areas my become accessible/inacessible by season. 

## Age/time change in player, NPCs, map
Days pass. seasons pass, and years pass. Some things will be lost to time as time progresses. There will be some NPC changes like some moving out, some moving in, some dying and some being born. Child NPCs will start school or graduate to a new school, move away to college or graduate and get a job. NPCs may get fired, retire or quit; they may change jobs or get promoted. NPCs may get married or divorced. 

## Time progression triggers
I kind of need to play with this to get something that works well.
Goals:
- Not feel like you are on a time limit as you play
- Time should advancement should be player initiated; the play should not be surprised or annoyed that time is advancing
- So we probably need other limiting factor(s) they are very visible and only expended when the user choosed to; like stamina, max travel distance, etc. 
- The player should be able to play as much of their emulated games as they want without it using up time or other limiting factors
- We don't want the use to be able to unlock all the systems/games in one playthrough, but we want what is unlocked to be a very intentional decision
- We want decision to feel informed, or at least that they could be informed if the user is willing to take the time. Not like they have to make a random decision and hope for the best

## BBSes / modem / shareware / piracy?
If the user gets a home computer system and a modem they can access the local BBSes. 

BBSes provide:
- Unlock some online NPC interactions
- Access event information
- Access FAQs and cheat info
- Play BBS games
- Download shareware
	- Storage add-on required (tape or disk drive)
	- Available games will change over time
- Download pirated games
	- This option must be unlocked
	- Games available may need to be unlocked
	- Games available will change over time

## Minigames
Need to get further into the development to decide what makes sense. 
Some ideas:
- Home made ramp jump with bike or big-wheel
- Ice scating / roller scating
	- need locations for these...
- Bike race 
- BMX tricks
- Fishing / catch fish with hands
- Swing for distance
- Climb outside of play equipment
- Walk over thin ice

## Jobs
Can earn money and unlock NPC relationships. 
- Deliver papers
- Lemonade stand
- Find bottles/cans
- Mow lawns
- Baby sit
- Car wash
- Extra chores
- Garage sale
- Assisting NPCs
- Dumpster diving / scavanging

## Money / trade value / resale value
Vendors will sell things for fixed prices, though there can be sales and some systems, add-ons and games will drop in price over time. Some vendors will buy things for resale value.  Some NPCs will trade items for a trade value which is higher than the resale value.

Money can come from:
- Allowance
- Birthday/holiday gifts
- Selling items 
- Jobs

## Exploration unlocks
Exploring locations can unlock dialog options. Finding items can also unlock dialog options. 

## Time of day sections
This may need to change... 

School day:
- Breakfast in house
- Morning outside before school
- Lunch / recess
- After school
- Dinner 
- Evening in house

Saturday:
- Morning
- Lunch 
- Afternoon
- Dinner
- Evening in house

Sunday:
- Morning in house
- Church
- Lunch
- Afternoon
- Dinner
- Evening in house

Holidays and some events may have a special layout

## NPC Schedules
